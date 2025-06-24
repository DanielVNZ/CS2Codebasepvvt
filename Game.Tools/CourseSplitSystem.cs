using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class CourseSplitSystem : GameSystemBase
{
	private struct IntersectPos : IComparable<IntersectPos>
	{
		public CoursePos m_CoursePos;

		public Bounds1 m_CourseIntersection;

		public Bounds1 m_IntersectionHeightMin;

		public Bounds1 m_IntersectionHeightMax;

		public Bounds1 m_EdgeIntersection;

		public Bounds1 m_EdgeHeightRangeMin;

		public Bounds1 m_EdgeHeightRangeMax;

		public Bounds1 m_CanMove;

		public float m_Priority;

		public int m_CourseIndex;

		public int m_AuxIndex;

		public bool m_IsNode;

		public bool m_IsOptional;

		public bool m_IsStartEnd;

		public bool m_IsTunnel;

		public int CompareTo(IntersectPos other)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return (int)math.sign(m_CourseIntersection.min - other.m_CourseIntersection.min);
		}

		public override int GetHashCode()
		{
			return m_CourseIndex;
		}
	}

	private struct Course
	{
		public CreationDefinition m_CreationDefinition;

		public OwnerDefinition m_OwnerDefinition;

		public NetCourse m_CourseData;

		public Upgraded m_UpgradedData;

		public Entity m_CourseEntity;
	}

	private struct Overlap
	{
		public Entity m_OverlapEntity;

		public int m_CourseIndex;
	}

	[BurstCompile]
	private struct CheckCoursesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> m_OwnerDefinitionType;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> m_UpgradedType;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		public NativeHashMap<Entity, bool> m_DeletedEntities;

		public NativeList<Course> m_CourseList;

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
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CreationDefinition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			NativeArray<OwnerDefinition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<OwnerDefinition>(ref m_OwnerDefinitionType);
			NativeArray<NetCourse> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCourse>(ref m_NetCourseType);
			NativeArray<Upgraded> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Course course = new Course
				{
					m_CourseEntity = nativeArray[i],
					m_CreationDefinition = nativeArray2[i],
					m_CourseData = nativeArray4[i]
				};
				if (course.m_CreationDefinition.m_Original != Entity.Null)
				{
					m_DeletedEntities.Add(course.m_CreationDefinition.m_Original, (course.m_CreationDefinition.m_Flags & CreationFlags.Delete) != 0);
				}
				else if (m_NetGeometryData.HasComponent(course.m_CreationDefinition.m_Prefab))
				{
					CollectionUtils.TryGet<Upgraded>(nativeArray5, i, ref course.m_UpgradedData);
					CollectionUtils.TryGet<OwnerDefinition>(nativeArray3, i, ref course.m_OwnerDefinition);
					m_CourseList.Add(ref course);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FindOverlapsJob : IJobParallelForDefer
	{
		private struct OverlapIteratorSubData
		{
			public Bounds2 m_Bounds1;

			public Bounds2 m_Bounds2;

			public Bezier4x2 m_Curve1;

			public Bezier4x2 m_Curve2;
		}

		private struct OverlapIterator : INativeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, OverlapIteratorSubData>, IUnsafeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, OverlapIteratorSubData>
		{
			public float m_Range;

			public float m_SizeLimit;

			public int m_CourseIndex;

			public ParallelWriter<Overlap> m_OverlapQueue;

			public ComponentLookup<Deleted> m_DeletedData;

			public bool Intersect(QuadTreeBoundsXZ bounds, ref OverlapIteratorSubData subData)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				bool2 val = default(bool2);
				val.x = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds1);
				val.y = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds2);
				if (!math.any(val))
				{
					return false;
				}
				if (math.all(val))
				{
					return true;
				}
				while (math.any(MathUtils.Size(subData.m_Bounds1) > m_SizeLimit))
				{
					if (val.x)
					{
						MathUtils.Divide(subData.m_Curve1, ref subData.m_Curve1, ref subData.m_Curve2, 0.5f);
					}
					else
					{
						MathUtils.Divide(subData.m_Curve2, ref subData.m_Curve1, ref subData.m_Curve2, 0.5f);
					}
					subData.m_Bounds1 = MathUtils.Expand(MathUtils.Bounds(subData.m_Curve1), float2.op_Implicit(m_Range));
					subData.m_Bounds2 = MathUtils.Expand(MathUtils.Bounds(subData.m_Curve2), float2.op_Implicit(m_Range));
					val.x = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds1);
					val.y = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds2);
					if (!math.any(val))
					{
						return false;
					}
					if (math.all(val))
					{
						return true;
					}
				}
				return true;
			}

			public void Iterate(QuadTreeBoundsXZ bounds, OverlapIteratorSubData subData, Entity overlapEntity)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				bool2 val = default(bool2);
				val.x = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds1);
				val.y = MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, subData.m_Bounds2);
				if (math.any(val) && !m_DeletedData.HasComponent(overlapEntity))
				{
					m_OverlapQueue.Enqueue(new Overlap
					{
						m_CourseIndex = m_CourseIndex,
						m_OverlapEntity = overlapEntity
					});
				}
			}
		}

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public BufferLookup<AuxiliaryNet> m_AuxiliaryNets;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		[ReadOnly]
		public NativeList<Course> m_CourseList;

		public ParallelWriter<Overlap> m_OverlapQueue;

		public void Execute(int index)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			Course course = m_CourseList[index];
			if ((course.m_CourseData.m_StartPosition.m_Flags & course.m_CourseData.m_EndPosition.m_Flags & CoursePosFlags.DisableMerge) != 0)
			{
				return;
			}
			float num = m_NetData[course.m_CreationDefinition.m_Prefab].m_DefaultWidth * 0.5f;
			DynamicBuffer<AuxiliaryNet> val = default(DynamicBuffer<AuxiliaryNet>);
			if (m_AuxiliaryNets.TryGetBuffer(course.m_CreationDefinition.m_Prefab, ref val))
			{
				NetGeometryData netGeometryData = default(NetGeometryData);
				for (int i = 0; i < val.Length; i++)
				{
					AuxiliaryNet auxiliaryNet = val[i];
					m_NetData.TryGetComponent(auxiliaryNet.m_Prefab, ref netGeometryData);
					num = math.max(num, math.abs(auxiliaryNet.m_Position.x) + netGeometryData.m_DefaultWidth * 0.5f);
				}
			}
			OverlapIterator overlapIterator = new OverlapIterator
			{
				m_Range = num,
				m_SizeLimit = num * 4f,
				m_CourseIndex = index,
				m_OverlapQueue = m_OverlapQueue,
				m_DeletedData = m_DeletedData
			};
			OverlapIteratorSubData overlapIteratorSubData = default(OverlapIteratorSubData);
			MathUtils.Divide(((Bezier4x3)(ref course.m_CourseData.m_Curve)).xz, ref overlapIteratorSubData.m_Curve1, ref overlapIteratorSubData.m_Curve2, 0.5f);
			overlapIteratorSubData.m_Bounds1 = MathUtils.Expand(MathUtils.Bounds(overlapIteratorSubData.m_Curve1), float2.op_Implicit(num));
			overlapIteratorSubData.m_Bounds2 = MathUtils.Expand(MathUtils.Bounds(overlapIteratorSubData.m_Curve2), float2.op_Implicit(num));
			m_SearchTree.Iterate<OverlapIterator, OverlapIteratorSubData>(ref overlapIterator, overlapIteratorSubData, 0);
		}
	}

	[BurstCompile]
	private struct DequeueOverlapsJob : IJob
	{
		public NativeQueue<Overlap> m_OverlapQueue;

		public NativeList<Overlap> m_OverlapList;

		public void Execute()
		{
			Overlap overlap = default(Overlap);
			while (m_OverlapQueue.TryDequeue(ref overlap))
			{
				m_OverlapList.Add(ref overlap);
			}
		}
	}

	[BurstCompile]
	private struct CheckCourseIntersectionsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<AuxiliaryNet> m_PrefabAuxiliaryNets;

		[ReadOnly]
		public NativeList<Course> m_CourseList;

		[ReadOnly]
		public NativeList<Overlap> m_OverlapList;

		[ReadOnly]
		public NativeHashMap<Entity, bool> m_DeletedEntities;

		public Writer<IntersectPos> m_Results;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Overlap overlap = m_OverlapList[index];
			if (!m_EdgeGeometryData.HasComponent(overlap.m_OverlapEntity))
			{
				return;
			}
			Entity val = overlap.m_OverlapEntity;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val, ref owner) && !m_BuildingData.HasComponent(val))
			{
				val = owner.m_Owner;
				if (m_DeletedData.HasComponent(val))
				{
					return;
				}
			}
			PrefabRef prefabRef = m_PrefabRefData[val];
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) && (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden)) == (Game.Objects.GeometryFlags.Overridable | Game.Objects.GeometryFlags.DeleteOverridden))
			{
				return;
			}
			Course course = m_CourseList[overlap.m_CourseIndex];
			CheckCourseIntersections(overlap, course, course, -1);
			DynamicBuffer<AuxiliaryNet> val2 = default(DynamicBuffer<AuxiliaryNet>);
			if (!m_PrefabAuxiliaryNets.TryGetBuffer(course.m_CreationDefinition.m_Prefab, ref val2))
			{
				return;
			}
			for (int i = 0; i < val2.Length; i++)
			{
				AuxiliaryNet auxiliaryNet = val2[i];
				Course course2 = course;
				course2.m_CreationDefinition = GetAuxDefinition(course.m_CreationDefinition, auxiliaryNet);
				course2.m_CourseData = course.m_CourseData;
				if (GetAuxCourse(ref course2.m_CourseData, auxiliaryNet, invert: false))
				{
					CheckCourseIntersections(overlap, course, course2, i);
				}
			}
		}

		public void CheckCourseIntersections(Overlap overlap, Course mainCourse, Course course, int auxIndex)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			Edge edgeData = m_EdgeData[overlap.m_OverlapEntity];
			bool3 val = bool3.op_Implicit(true);
			bool flag = default(bool);
			if (m_DeletedEntities.TryGetValue(overlap.m_OverlapEntity, ref flag) && flag)
			{
				val.x = !WillBeOrphan(edgeData.m_Start);
				val.y = false;
				val.z = !WillBeOrphan(edgeData.m_End);
				if (!math.any(((bool3)(ref val)).xz))
				{
					return;
				}
			}
			Curve curve = m_CurveData[overlap.m_OverlapEntity];
			Composition composition = m_CompositionData[overlap.m_OverlapEntity];
			EdgeGeometry geometry = m_EdgeGeometryData[overlap.m_OverlapEntity];
			EdgeNodeGeometry geometry2 = m_StartNodeGeometryData[overlap.m_OverlapEntity].m_Geometry;
			EdgeNodeGeometry geometry3 = m_EndNodeGeometryData[overlap.m_OverlapEntity].m_Geometry;
			NetGeometryData prefabGeometryData = m_PrefabGeometryData[course.m_CreationDefinition.m_Prefab];
			int num = math.max(4, (int)math.ceil(math.log(course.m_CourseData.m_Length * 16f / prefabGeometryData.m_EdgeLengthRange.max) * 1.442695f));
			num = math.select(num, 0, course.m_CourseData.m_Length < prefabGeometryData.m_DefaultWidth * 0.01f);
			float2 courseOffset = default(float2);
			((float2)(ref courseOffset))._002Ector(course.m_CourseData.m_StartPosition.m_CourseDelta, course.m_CourseData.m_EndPosition.m_CourseDelta);
			IntersectPos currentIntersectPos = default(IntersectPos);
			IntersectPos result = default(IntersectPos);
			IntersectPos result2 = default(IntersectPos);
			currentIntersectPos.m_Priority = -1f;
			result.m_Priority = -1f;
			result2.m_Priority = -1f;
			PrefabRef prefabRef = m_PrefabRefData[overlap.m_OverlapEntity];
			NetGeometryData prefabGeometryData2 = m_PrefabGeometryData[prefabRef.m_Prefab];
			NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
			NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
			NetCompositionData prefabCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
			if (m_PrefabAuxiliaryNets.HasBuffer(prefabRef.m_Prefab))
			{
				NetData netData = m_PrefabNetData[course.m_CreationDefinition.m_Prefab];
				if ((m_PrefabNetData[prefabRef.m_Prefab].m_ConnectLayers & Layer.Waterway) == 0 || (netData.m_RequiredLayers & Layer.Waterway) == 0)
				{
					prefabCompositionData.m_HeightRange = prefabGeometryData2.m_DefaultHeightRange;
					prefabCompositionData2.m_HeightRange = prefabGeometryData2.m_DefaultHeightRange;
					prefabCompositionData3.m_HeightRange = prefabGeometryData2.m_DefaultHeightRange;
				}
			}
			if ((prefabGeometryData.m_MergeLayers & prefabGeometryData2.m_MergeLayers) == 0)
			{
				NetData netData2 = m_PrefabNetData[course.m_CreationDefinition.m_Prefab];
				NetData netData3 = m_PrefabNetData[prefabRef.m_Prefab];
				Owner owner = default(Owner);
				PrefabRef prefabRef2 = default(PrefabRef);
				NetData netData4 = default(NetData);
				if (!NetUtils.CanConnect(netData2, netData3) && (((prefabGeometryData.m_Flags | prefabGeometryData2.m_Flags) & Game.Net.GeometryFlags.Marker) != 0 || ((netData3.m_RequiredLayers & Layer.Waterway) != Layer.None && auxIndex != -1 && (m_PrefabNetData[mainCourse.m_CreationDefinition.m_Prefab].m_ConnectLayers & Layer.Waterway) != Layer.None) || ((netData2.m_RequiredLayers & Layer.Waterway) != Layer.None && m_OwnerData.TryGetComponent(overlap.m_OverlapEntity, ref owner) && m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2) && m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData4) && (netData4.m_ConnectLayers & Layer.Waterway) != Layer.None)))
				{
					return;
				}
			}
			if (prefabGeometryData2.m_MergeLayers == Layer.None || !val.y)
			{
				if (val.x)
				{
					CheckNodeGeometry(course.m_CourseData, overlap.m_CourseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, overlap.m_OverlapEntity, edgeData.m_Start, curve.m_Bezier.a, courseOffset, 0f);
				}
				if (val.z)
				{
					CheckNodeGeometry(course.m_CourseData, overlap.m_CourseIndex, auxIndex, ref result2, prefabGeometryData, prefabCompositionData3, overlap.m_OverlapEntity, edgeData.m_End, curve.m_Bezier.d, courseOffset, 1f);
				}
			}
			else
			{
				if (val.x)
				{
					CheckNodeGeometry(course.m_CourseData, overlap.m_CourseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, overlap.m_OverlapEntity, edgeData.m_Start, geometry2, courseOffset, 0f, num);
				}
				if (val.z)
				{
					CheckNodeGeometry(course.m_CourseData, overlap.m_CourseIndex, auxIndex, ref result2, prefabGeometryData, prefabCompositionData3, overlap.m_OverlapEntity, edgeData.m_End, geometry3, courseOffset, 1f, num);
				}
			}
			if (val.y)
			{
				CheckEdgeGeometry(course.m_CourseData, overlap.m_CourseIndex, auxIndex, ref result, ref result2, ref currentIntersectPos, prefabGeometryData, prefabGeometryData2, prefabCompositionData, overlap.m_OverlapEntity, edgeData, geometry, curve.m_Bezier, courseOffset, num);
			}
			if (result.m_Priority != -1f)
			{
				m_Results.Enqueue(result);
			}
			if (result2.m_Priority != -1f)
			{
				m_Results.Enqueue(result2);
			}
			if (currentIntersectPos.m_Priority != -1f)
			{
				m_Results.Enqueue(currentIntersectPos);
			}
		}

		private bool WillBeOrphan(Entity node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			bool flag = default(bool);
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if ((edge2.m_Start == node || edge2.m_End == node) && (!m_DeletedEntities.TryGetValue(edge, ref flag) || !flag))
				{
					return false;
				}
			}
			return true;
		}

		private void CheckEdgeGeometry(NetCourse courseData, int courseIndex, int auxIndex, ref IntersectPos startIntersectPos, ref IntersectPos endIntersectPos, ref IntersectPos currentIntersectPos, NetGeometryData prefabGeometryData, NetGeometryData prefabGeometryData2, NetCompositionData prefabCompositionData2, Entity edge, Edge edgeData, EdgeGeometry geometry, Bezier4x3 curve, float2 courseOffset, int iterations)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 val = MathUtils.Bounds(MathUtils.Cut(((Bezier4x3)(ref courseData.m_Curve)).xz, courseOffset));
			ref float2 min = ref val.min;
			min -= prefabGeometryData.m_DefaultWidth * 0.5f;
			ref float2 max = ref val.max;
			max += prefabGeometryData.m_DefaultWidth * 0.5f;
			if (!MathUtils.Intersect(val, ((Bounds3)(ref geometry.m_Bounds)).xz))
			{
				return;
			}
			if (iterations <= 0)
			{
				IntersectPos lastIntersectPos = new IntersectPos
				{
					m_Priority = -1f
				};
				CheckEdgeSegment(courseData, courseIndex, auxIndex, ref startIntersectPos, ref endIntersectPos, ref currentIntersectPos, ref lastIntersectPos, prefabGeometryData, prefabGeometryData2, prefabCompositionData2, edge, edgeData, geometry.m_Start, curve, courseOffset, new float2(0f, 0.5f));
				CheckEdgeSegment(courseData, courseIndex, auxIndex, ref startIntersectPos, ref endIntersectPos, ref currentIntersectPos, ref lastIntersectPos, prefabGeometryData, prefabGeometryData2, prefabCompositionData2, edge, edgeData, geometry.m_End, curve, courseOffset, new float2(0.5f, 1f));
				if (lastIntersectPos.m_Priority != -1f)
				{
					Add(ref currentIntersectPos, lastIntersectPos);
				}
			}
			else
			{
				float3 val2 = default(float3);
				((float3)(ref val2))._002Ector(courseOffset.x, math.lerp(courseOffset.x, courseOffset.y, 0.5f), courseOffset.y);
				CheckEdgeGeometry(courseData, courseIndex, auxIndex, ref startIntersectPos, ref endIntersectPos, ref currentIntersectPos, prefabGeometryData, prefabGeometryData2, prefabCompositionData2, edge, edgeData, geometry, curve, ((float3)(ref val2)).xy, iterations - 1);
				CheckEdgeGeometry(courseData, courseIndex, auxIndex, ref startIntersectPos, ref endIntersectPos, ref currentIntersectPos, prefabGeometryData, prefabGeometryData2, prefabCompositionData2, edge, edgeData, geometry, curve, ((float3)(ref val2)).yz, iterations - 1);
			}
		}

		private void CheckNodeGeometry(NetCourse courseData, int courseIndex, int auxIndex, ref IntersectPos result, NetGeometryData prefabGeometryData, NetCompositionData prefabCompositionData2, Entity edge, Entity node, EdgeNodeGeometry geometry, float2 courseOffset, float edgeOffset, int iterations)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			Bounds2 val = MathUtils.Bounds(MathUtils.Cut(((Bezier4x3)(ref courseData.m_Curve)).xz, courseOffset));
			ref float2 min = ref val.min;
			min -= prefabGeometryData.m_DefaultWidth * 0.5f;
			ref float2 max = ref val.max;
			max += prefabGeometryData.m_DefaultWidth * 0.5f;
			if (!MathUtils.Intersect(val, ((Bounds3)(ref geometry.m_Bounds)).xz))
			{
				return;
			}
			if (iterations <= 0)
			{
				if (geometry.m_MiddleRadius > 0f)
				{
					Segment right = geometry.m_Right;
					Segment right2 = geometry.m_Right;
					right.m_Right = MathUtils.Lerp(geometry.m_Right.m_Left, geometry.m_Right.m_Right, 0.5f);
					right.m_Right.d = geometry.m_Middle.d;
					right2.m_Left = right.m_Right;
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, geometry.m_Left, courseOffset, edgeOffset, 0.5f);
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, right, courseOffset, edgeOffset, 1f);
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, right2, courseOffset, edgeOffset, 0f);
				}
				else
				{
					Segment left = geometry.m_Left;
					Segment right3 = geometry.m_Right;
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, left, courseOffset, edgeOffset, 1f);
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, right3, courseOffset, edgeOffset, 0f);
					left.m_Right = geometry.m_Middle;
					right3.m_Left = geometry.m_Middle;
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, left, courseOffset, edgeOffset, 1f);
					CheckNodeSegment(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, right3, courseOffset, edgeOffset, 0f);
				}
			}
			else
			{
				float3 val2 = default(float3);
				((float3)(ref val2))._002Ector(courseOffset.x, math.lerp(courseOffset.x, courseOffset.y, 0.5f), courseOffset.y);
				CheckNodeGeometry(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, geometry, ((float3)(ref val2)).xy, edgeOffset, iterations - 1);
				CheckNodeGeometry(courseData, courseIndex, auxIndex, ref result, prefabGeometryData, prefabCompositionData2, edge, node, geometry, ((float3)(ref val2)).yz, edgeOffset, iterations - 1);
			}
		}

		private void CheckNodeGeometry(NetCourse courseData, int courseIndex, int auxIndex, ref IntersectPos result, NetGeometryData prefabGeometryData, NetCompositionData prefabCompositionData2, Entity edge, Entity node, float3 nodePos, float2 courseOffset, float edgeOffset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x2 val = MathUtils.Cut(((Bezier4x3)(ref courseData.m_Curve)).xz, courseOffset);
			float num = prefabGeometryData.m_DefaultWidth * 0.5f;
			Bounds2 val2 = MathUtils.Expand(MathUtils.Bounds(val), float2.op_Implicit(num));
			Circle2 val3 = default(Circle2);
			((Circle2)(ref val3))._002Ector(prefabCompositionData2.m_Width * 0.5f, ((float3)(ref nodePos)).xz);
			Bounds2 val4 = MathUtils.Bounds(val3);
			if (!MathUtils.Intersect(val2, val4))
			{
				return;
			}
			float num3 = default(float);
			float num2 = MathUtils.Distance(val, ((float3)(ref nodePos)).xz, ref num3);
			if (num2 <= num + val3.radius)
			{
				float num4 = math.lerp(courseOffset.x, courseOffset.y, num3);
				float3 val5 = MathUtils.Position(courseData.m_Curve, num4);
				Bounds1 val6 = default(Bounds1);
				val6.min = math.lerp(courseOffset.x, courseOffset.y, num3 - 0.01f);
				val6.max = math.lerp(courseOffset.x, courseOffset.y, num3 + 0.01f);
				float num5 = math.max(0f, (num2 - num) / val3.radius);
				num5 = math.sqrt(1f - num5 * num5) * val3.radius;
				if (val6.max < 1f)
				{
					Bounds1 val7 = default(Bounds1);
					((Bounds1)(ref val7))._002Ector(val6.max, 1f);
					MathUtils.ClampLength(courseData.m_Curve, ref val7, num5);
					val6.max = val7.max;
				}
				if (val6.min > 0f)
				{
					Bounds1 val8 = default(Bounds1);
					((Bounds1)(ref val8))._002Ector(0f, val6.min);
					MathUtils.ClampLengthInverse(courseData.m_Curve, ref val8, num5);
					val6.min = val8.min;
				}
				int parentMesh = -1;
				LocalTransformCache localTransformCache = default(LocalTransformCache);
				if (m_LocalTransformCacheData.TryGetComponent(node, ref localTransformCache))
				{
					parentMesh = localTransformCache.m_ParentMesh;
				}
				result = default(IntersectPos);
				result.m_CoursePos.m_Entity = node;
				result.m_CoursePos.m_Position = val5;
				result.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, num4));
				result.m_CoursePos.m_CourseDelta = num4;
				result.m_CoursePos.m_SplitPosition = edgeOffset;
				result.m_CoursePos.m_Flags = courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.IsGrid);
				result.m_CoursePos.m_Flags |= CoursePosFlags.FreeHeight;
				result.m_CoursePos.m_ParentMesh = parentMesh;
				result.m_CourseIntersection = val6;
				result.m_IntersectionHeightMin = new Bounds1(float2.op_Implicit(val5.y));
				result.m_IntersectionHeightMax = new Bounds1(float2.op_Implicit(val5.y));
				result.m_EdgeIntersection = new Bounds1(edgeOffset, edgeOffset);
				result.m_EdgeHeightRangeMin = nodePos.y + prefabCompositionData2.m_HeightRange;
				result.m_EdgeHeightRangeMax = nodePos.y + prefabCompositionData2.m_HeightRange;
				result.m_Priority = num2;
				result.m_AuxIndex = auxIndex;
				result.m_CourseIndex = courseIndex;
				result.m_IsNode = true;
				result.m_IsTunnel = (prefabCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0 && ((prefabCompositionData2.m_Flags.m_Left | prefabCompositionData2.m_Flags.m_Right) & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) == 0;
			}
		}

		private void CheckEdgeSegment(NetCourse courseData, int courseIndex, int auxIndex, ref IntersectPos startIntersectPos, ref IntersectPos endIntersectPos, ref IntersectPos currentIntersectPos, ref IntersectPos lastIntersectPos, NetGeometryData prefabGeometryData, NetGeometryData prefabGeometryData2, NetCompositionData prefabCompositionData2, Entity edge, Edge edgeData, Segment segment, Bezier4x3 curve, float2 courseOffset, float2 edgeOffset)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_081f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(prefabGeometryData.m_DefaultWidth * 0.5f, prefabGeometryData.m_DefaultWidth * -0.5f);
			float3 val2 = MathUtils.Position(courseData.m_Curve, courseOffset.x);
			float3 val3 = MathUtils.Position(courseData.m_Curve, courseOffset.y);
			float3 val4 = MathUtils.Tangent(courseData.m_Curve, courseOffset.x);
			float3 val5 = MathUtils.Tangent(courseData.m_Curve, courseOffset.y);
			MathUtils.TryNormalize(ref val4);
			MathUtils.TryNormalize(ref val5);
			float2 startLeft = ((float3)(ref val2)).xz - ((float3)(ref val4)).zx * val;
			float2 startRight = ((float3)(ref val2)).xz + ((float3)(ref val4)).zx * val;
			float2 endLeft = ((float3)(ref val3)).xz - ((float3)(ref val5)).zx * val;
			float2 endRight = ((float3)(ref val3)).xz + ((float3)(ref val5)).zx * val;
			float num = 0f;
			float2 val6 = ((float3)(ref segment.m_Left.a)).xz;
			float2 val7 = ((float3)(ref segment.m_Right.a)).xz;
			Segment val9 = default(Segment);
			float num3 = default(float);
			Segment val10 = default(Segment);
			float num4 = default(float);
			Segment val12 = default(Segment);
			float2 val13 = default(float2);
			Bounds1 val14 = default(Bounds1);
			Bounds1 val15 = default(Bounds1);
			Bounds1 val16 = default(Bounds1);
			Bounds1 val17 = default(Bounds1);
			LocalTransformCache localTransformCache = default(LocalTransformCache);
			LocalTransformCache localTransformCache2 = default(LocalTransformCache);
			float num7 = default(float);
			float num8 = default(float);
			for (int i = 1; i <= 8; i++)
			{
				float num2 = (float)i / 8f;
				float3 val8 = MathUtils.Position(segment.m_Left, num2);
				float2 xz = ((float3)(ref val8)).xz;
				val8 = MathUtils.Position(segment.m_Right, num2);
				float2 xz2 = ((float3)(ref val8)).xz;
				Bounds1 intersectRange;
				Bounds1 intersectRange2;
				bool flag = QuadIntersect(startLeft, startRight, endLeft, endRight, val6, val7, xz, xz2, out intersectRange, out intersectRange2);
				if (courseOffset.x == 0f)
				{
					((Segment)(ref val9))._002Ector(math.lerp(val6, val7, 0.5f), math.lerp(xz, xz2, 0.5f));
					if (MathUtils.Distance(val9, ((float3)(ref val2)).xz, ref num3) <= MathUtils.Distance(val9, ((float3)(ref val3)).xz, ref num3) && CircleIntersect(new Circle2(val.x, ((float3)(ref val2)).xz), val6, val7, xz, xz2, out var intersectRange3))
					{
						intersectRange |= 0f;
						intersectRange2 |= intersectRange3;
						flag = true;
					}
				}
				if (courseOffset.y == 1f)
				{
					((Segment)(ref val10))._002Ector(math.lerp(val6, val7, 0.5f), math.lerp(xz, xz2, 0.5f));
					if (MathUtils.Distance(val10, ((float3)(ref val2)).xz, ref num4) >= MathUtils.Distance(val10, ((float3)(ref val3)).xz, ref num4) && CircleIntersect(new Circle2(val.x, ((float3)(ref val3)).xz), val6, val7, xz, xz2, out var intersectRange4))
					{
						intersectRange |= 1f;
						intersectRange2 |= intersectRange4;
						flag = true;
					}
				}
				if (flag)
				{
					Segment val11 = new Segment(((float3)(ref val2)).xz, ((float3)(ref val3)).xz);
					((Segment)(ref val12))._002Ector(math.lerp(val6, val7, 0.5f), math.lerp(xz, xz2, 0.5f));
					float priority = MathUtils.Distance(val11, val12, ref val13);
					float num5 = math.lerp(courseOffset.x, courseOffset.y, val13.x);
					float3 position = MathUtils.Position(courseData.m_Curve, num5);
					MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref position)).xz, ref val13.y);
					if ((prefabGeometryData2.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
					{
						float num6 = MathUtils.Length(((Bezier4x3)(ref curve)).xz, new Bounds1(0f, val13.y));
						num6 = MathUtils.Snap(num6, 4f);
						((Bounds1)(ref val14))._002Ector(0f, 1f);
						if (MathUtils.ClampLength(((Bezier4x3)(ref curve)).xz, ref val14, num6))
						{
							val13.y = val14.max;
						}
					}
					intersectRange.min = math.lerp(courseOffset.x, courseOffset.y, intersectRange.min - 0.01f);
					intersectRange.max = math.lerp(courseOffset.x, courseOffset.y, intersectRange.max + 0.01f);
					val15.min = math.lerp(num, num2, intersectRange2.min - 0.01f);
					val15.max = math.lerp(num, num2, intersectRange2.max + 0.01f);
					intersectRange2.min = math.lerp(edgeOffset.x, edgeOffset.y, val15.min);
					intersectRange2.max = math.lerp(edgeOffset.x, edgeOffset.y, val15.max);
					if (prefabCompositionData2.m_NodeOffset > 0f)
					{
						if (intersectRange2.min > 0f)
						{
							((Bounds1)(ref val16))._002Ector(0f, intersectRange2.min);
							MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve)).xz, ref val16, prefabCompositionData2.m_NodeOffset);
							intersectRange2.min = val16.min;
						}
						if (intersectRange2.max > 0f)
						{
							((Bounds1)(ref val17))._002Ector(intersectRange2.max, 1f);
							MathUtils.ClampLength(((Bezier4x3)(ref curve)).xz, ref val17, prefabCompositionData2.m_NodeOffset);
							intersectRange2.max = val17.max;
						}
					}
					int parentMesh = -1;
					if (m_LocalTransformCacheData.TryGetComponent(edgeData.m_Start, ref localTransformCache) && m_LocalTransformCacheData.TryGetComponent(edgeData.m_End, ref localTransformCache2) && localTransformCache.m_ParentMesh == localTransformCache2.m_ParentMesh)
					{
						parentMesh = localTransformCache.m_ParentMesh;
					}
					MathUtils.Distance(((Bezier4x3)(ref curve)).xz, MathUtils.Position(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectRange.min), ref num7);
					MathUtils.Distance(((Bezier4x3)(ref curve)).xz, MathUtils.Position(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectRange.max), ref num8);
					if (num8 < num7)
					{
						CommonUtils.Swap(ref val15.min, ref val15.max);
					}
					IntersectPos target = new IntersectPos
					{
						m_CoursePos = 
						{
							m_Entity = edge,
							m_Position = position,
							m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, num5)),
							m_CourseDelta = num5,
							m_SplitPosition = val13.y,
							m_Flags = (courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.IsGrid))
						}
					};
					target.m_CoursePos.m_Flags |= CoursePosFlags.FreeHeight;
					target.m_CoursePos.m_ParentMesh = parentMesh;
					target.m_CourseIntersection = intersectRange;
					target.m_IntersectionHeightMin = new Bounds1(float2.op_Implicit(MathUtils.Position(curve, num7).y));
					target.m_IntersectionHeightMax = new Bounds1(float2.op_Implicit(MathUtils.Position(curve, num8).y));
					target.m_EdgeIntersection = intersectRange2;
					target.m_EdgeHeightRangeMin = GetHeightRange(segment, val15.min, prefabCompositionData2.m_HeightRange);
					target.m_EdgeHeightRangeMax = GetHeightRange(segment, val15.max, prefabCompositionData2.m_HeightRange);
					target.m_Priority = priority;
					target.m_CourseIndex = courseIndex;
					target.m_AuxIndex = auxIndex;
					target.m_IsTunnel = (prefabCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
					if (intersectRange2.min <= 0f)
					{
						target.m_CoursePos.m_Entity = edgeData.m_Start;
						target.m_IsNode = true;
					}
					else if (intersectRange2.max >= 1f)
					{
						target.m_CoursePos.m_Entity = edgeData.m_End;
						target.m_IsNode = true;
					}
					if (startIntersectPos.m_Priority != -1f && (MathUtils.Intersect(startIntersectPos.m_CourseIntersection, target.m_CourseIntersection) || MathUtils.Intersect(startIntersectPos.m_EdgeIntersection, target.m_EdgeIntersection)) && Merge(ref target, startIntersectPos))
					{
						startIntersectPos.m_Priority = -1f;
					}
					if (endIntersectPos.m_Priority != -1f && (MathUtils.Intersect(endIntersectPos.m_CourseIntersection, target.m_CourseIntersection) || MathUtils.Intersect(endIntersectPos.m_EdgeIntersection, target.m_EdgeIntersection)) && Merge(ref target, endIntersectPos))
					{
						endIntersectPos.m_Priority = -1f;
					}
					if (lastIntersectPos.m_Priority != -1f)
					{
						if (!MathUtils.Intersect(lastIntersectPos.m_CourseIntersection, target.m_CourseIntersection) && !MathUtils.Intersect(lastIntersectPos.m_EdgeIntersection, target.m_EdgeIntersection))
						{
							Add(ref currentIntersectPos, lastIntersectPos);
							lastIntersectPos = target;
						}
						else if (!Merge(ref lastIntersectPos, target))
						{
							Add(ref currentIntersectPos, lastIntersectPos);
							lastIntersectPos = target;
						}
					}
					else
					{
						lastIntersectPos = target;
					}
				}
				num = num2;
				val6 = xz;
				val7 = xz2;
			}
		}

		private bool Merge(ref IntersectPos target, IntersectPos other)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			if (target.m_IsNode && other.m_IsNode && target.m_CoursePos.m_Entity != other.m_CoursePos.m_Entity)
			{
				return false;
			}
			if (target.m_IsNode && !other.m_IsNode)
			{
				other.m_CoursePos.m_Entity = target.m_CoursePos.m_Entity;
				other.m_IsNode = true;
			}
			else if (!target.m_IsNode && other.m_IsNode)
			{
				target.m_CoursePos.m_Entity = other.m_CoursePos.m_Entity;
				target.m_IsNode = true;
			}
			if (other.m_CourseIntersection.min < target.m_CourseIntersection.min)
			{
				target.m_CourseIntersection.min = other.m_CourseIntersection.min;
				target.m_IntersectionHeightMin = other.m_IntersectionHeightMin;
				target.m_EdgeHeightRangeMin = other.m_EdgeHeightRangeMin;
			}
			else if (other.m_CourseIntersection.min == target.m_CourseIntersection.min)
			{
				ref Bounds1 reference = ref target.m_IntersectionHeightMin;
				reference |= other.m_IntersectionHeightMin;
				ref Bounds1 reference2 = ref target.m_EdgeHeightRangeMin;
				reference2 |= other.m_EdgeHeightRangeMin;
			}
			if (other.m_CourseIntersection.max > target.m_CourseIntersection.max)
			{
				target.m_CourseIntersection.max = other.m_CourseIntersection.max;
				target.m_IntersectionHeightMax = other.m_IntersectionHeightMax;
				target.m_EdgeHeightRangeMax = other.m_EdgeHeightRangeMax;
			}
			else if (other.m_CourseIntersection.max == target.m_CourseIntersection.max)
			{
				ref Bounds1 reference3 = ref target.m_IntersectionHeightMax;
				reference3 |= other.m_IntersectionHeightMax;
				ref Bounds1 reference4 = ref target.m_EdgeHeightRangeMax;
				reference4 |= other.m_EdgeHeightRangeMax;
			}
			ref Bounds1 reference5 = ref target.m_EdgeIntersection;
			reference5 |= other.m_EdgeIntersection;
			target.m_IsTunnel &= other.m_IsTunnel;
			if (other.m_Priority < target.m_Priority)
			{
				target.m_CoursePos = other.m_CoursePos;
				target.m_Priority = other.m_Priority;
			}
			return true;
		}

		private void Add(ref IntersectPos current, IntersectPos other)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (current.m_Priority != -1f)
			{
				if (!MathUtils.Intersect(current.m_CourseIntersection, other.m_CourseIntersection) && !MathUtils.Intersect(current.m_EdgeIntersection, other.m_EdgeIntersection))
				{
					m_Results.Enqueue(current);
					current = other;
				}
				else if (!Merge(ref current, other))
				{
					m_Results.Enqueue(current);
					current = other;
				}
			}
			else
			{
				current = other;
			}
		}

		private bool QuadIntersect(float2 startLeft1, float2 startRight1, float2 endLeft1, float2 endRight1, float2 startLeft2, float2 startRight2, float2 endLeft2, float2 endRight2, out Bounds1 intersectRange1, out Bounds1 intersectRange2)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0693: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_072c: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0823: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0853: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_088b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0958: Unknown result type (might be due to invalid IL or missing references)
			//IL_095f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_097a: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_098b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			intersectRange1.min = 1f;
			intersectRange1.max = 0f;
			intersectRange2.min = 1f;
			intersectRange2.max = 0f;
			Bounds2 val = default(Bounds2);
			val.min = math.min(math.min(startLeft1, startRight1), math.min(endLeft1, endRight1));
			val.max = math.max(math.max(startLeft1, startRight1), math.max(endLeft1, endRight1));
			Bounds2 val2 = default(Bounds2);
			val2.min = math.min(math.min(startLeft2, startRight2), math.min(endLeft2, endRight2));
			val2.max = math.max(math.max(startLeft2, startRight2), math.max(endLeft2, endRight2));
			if (!MathUtils.Intersect(val, val2))
			{
				return false;
			}
			Triangle2 val3 = new Triangle2(startLeft1, endLeft1, endRight1);
			Triangle2 val4 = default(Triangle2);
			((Triangle2)(ref val4))._002Ector(endRight1, startRight1, startLeft1);
			Triangle2 val5 = default(Triangle2);
			((Triangle2)(ref val5))._002Ector(startLeft2, endLeft2, endRight2);
			Triangle2 val6 = default(Triangle2);
			((Triangle2)(ref val6))._002Ector(endRight2, startRight2, startLeft2);
			Segment val7 = default(Segment);
			((Segment)(ref val7))._002Ector(startLeft1, startRight1);
			Segment val8 = default(Segment);
			((Segment)(ref val8))._002Ector(endLeft1, endRight1);
			Segment val9 = default(Segment);
			((Segment)(ref val9))._002Ector(startLeft1, endLeft1);
			Segment val10 = default(Segment);
			((Segment)(ref val10))._002Ector(startRight1, endRight1);
			Segment val11 = default(Segment);
			((Segment)(ref val11))._002Ector(startLeft2, startRight2);
			Segment val12 = default(Segment);
			((Segment)(ref val12))._002Ector(endLeft2, endRight2);
			Segment val13 = default(Segment);
			((Segment)(ref val13))._002Ector(startLeft2, endLeft2);
			Segment val14 = default(Segment);
			((Segment)(ref val14))._002Ector(startRight2, endRight2);
			float2 val15 = default(float2);
			if (MathUtils.Intersect(val3, startLeft2, ref val15))
			{
				intersectRange1 |= val15.x + val15.y;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val3, startRight2, ref val15))
			{
				intersectRange1 |= val15.x + val15.y;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val3, endLeft2, ref val15))
			{
				intersectRange1 |= val15.x + val15.y;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val3, endRight2, ref val15))
			{
				intersectRange1 |= val15.x + val15.y;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val4, startLeft2, ref val15))
			{
				intersectRange1 |= 1f - val15.x - val15.y;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val4, startRight2, ref val15))
			{
				intersectRange1 |= 1f - val15.x - val15.y;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val4, endLeft2, ref val15))
			{
				intersectRange1 |= 1f - val15.x - val15.y;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val4, endRight2, ref val15))
			{
				intersectRange1 |= 1f - val15.x - val15.y;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val5, startLeft1, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= val15.x + val15.y;
			}
			if (MathUtils.Intersect(val5, startRight1, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= val15.x + val15.y;
			}
			if (MathUtils.Intersect(val5, endLeft1, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= val15.x + val15.y;
			}
			if (MathUtils.Intersect(val5, endRight1, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= val15.x + val15.y;
			}
			if (MathUtils.Intersect(val6, startLeft1, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= 1f - val15.x - val15.y;
			}
			if (MathUtils.Intersect(val6, startRight1, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= 1f - val15.x - val15.y;
			}
			if (MathUtils.Intersect(val6, endLeft1, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= 1f - val15.x - val15.y;
			}
			if (MathUtils.Intersect(val6, endRight1, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= 1f - val15.x - val15.y;
			}
			if (MathUtils.Intersect(val7, val11, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val7, val12, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val7, val13, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val7, val14, ref val15))
			{
				intersectRange1 |= 0f;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val8, val11, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val8, val12, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val8, val13, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val8, val14, ref val15))
			{
				intersectRange1 |= 1f;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val9, val11, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val9, val12, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val9, val13, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val9, val14, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val10, val11, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(val10, val12, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(val10, val13, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= val15.y;
			}
			if (MathUtils.Intersect(val10, val14, ref val15))
			{
				intersectRange1 |= val15.x;
				intersectRange2 |= val15.y;
			}
			return intersectRange1.min <= intersectRange1.max;
		}

		private bool CircleIntersect(Circle2 circle1, float2 startLeft2, float2 startRight2, float2 endLeft2, float2 endRight2, out Bounds1 intersectRange2)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			intersectRange2.min = 1f;
			intersectRange2.max = 0f;
			Bounds2 val = MathUtils.Bounds(circle1);
			Bounds2 val2 = default(Bounds2);
			val2.min = math.min(math.min(startLeft2, startRight2), math.min(endLeft2, endRight2));
			val2.max = math.max(math.max(startLeft2, startRight2), math.max(endLeft2, endRight2));
			if (!MathUtils.Intersect(val, val2))
			{
				return false;
			}
			Triangle2 val3 = default(Triangle2);
			((Triangle2)(ref val3))._002Ector(startLeft2, endLeft2, endRight2);
			Triangle2 val4 = default(Triangle2);
			((Triangle2)(ref val4))._002Ector(endRight2, startRight2, startLeft2);
			Segment val5 = default(Segment);
			((Segment)(ref val5))._002Ector(startLeft2, startRight2);
			Segment val6 = default(Segment);
			((Segment)(ref val6))._002Ector(endLeft2, endRight2);
			Segment val7 = default(Segment);
			((Segment)(ref val7))._002Ector(startLeft2, endLeft2);
			Segment val8 = default(Segment);
			((Segment)(ref val8))._002Ector(startRight2, endRight2);
			float2 val9 = default(float2);
			if (MathUtils.Intersect(val3, circle1.position, ref val9))
			{
				float2 val10 = default(float2);
				((float2)(ref val10))._002Ector(math.distance(val3.a, val3.b), math.distance(val3.a, val3.c));
				float2 val11 = circle1.radius * val9 / (val10 * (val9.x + val9.y));
				float2 val12 = math.max(float2.op_Implicit(0f), val9 - val11);
				float2 val13 = math.min(float2.op_Implicit(1f), val9 + val11);
				intersectRange2 |= val12.x + val13.y;
				intersectRange2 |= val13.x + val13.y;
			}
			if (MathUtils.Intersect(val4, circle1.position, ref val9))
			{
				float2 val14 = default(float2);
				((float2)(ref val14))._002Ector(math.distance(val4.a, val4.b), math.distance(val4.a, val4.c));
				float2 val15 = circle1.radius * val9 / (val14 * (val9.x + val9.y));
				float2 val16 = math.max(float2.op_Implicit(0f), val9 - val15);
				float2 val17 = math.min(float2.op_Implicit(1f), val9 + val15);
				intersectRange2 |= 1f - val16.x - val17.y;
				intersectRange2 |= 1f - val17.x - val17.y;
			}
			if (MathUtils.Intersect(circle1, val5, ref val9))
			{
				intersectRange2 |= 0f;
			}
			if (MathUtils.Intersect(circle1, val6, ref val9))
			{
				intersectRange2 |= 1f;
			}
			if (MathUtils.Intersect(circle1, val7, ref val9))
			{
				intersectRange2 |= new Bounds1(val9.x, val9.y);
			}
			if (MathUtils.Intersect(circle1, val8, ref val9))
			{
				intersectRange2 |= new Bounds1(val9.x, val9.y);
			}
			return intersectRange2.min <= intersectRange2.max;
		}

		private Bounds1 GetHeightRange(Bezier4x3 curve, float curvePos, Bounds1 heightRange)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Position(curve, curvePos).y + heightRange;
		}

		private Bounds1 GetHeightRange(Segment segment, float curvePos, Bounds1 heightRange)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			return GetHeightRange(segment.m_Left, curvePos, heightRange) | GetHeightRange(segment.m_Right, curvePos, heightRange);
		}

		private void CheckNodeSegment(NetCourse courseData, int courseIndex, int auxIndex, ref IntersectPos currentIntersectPos, NetGeometryData prefabGeometryData, NetCompositionData prefabCompositionData2, Entity edge, Entity node, Segment segment, float2 courseOffset, float edgeOffset, float centerOffset)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(prefabGeometryData.m_DefaultWidth * 0.5f, prefabGeometryData.m_DefaultWidth * -0.5f);
			float3 val2 = MathUtils.Position(courseData.m_Curve, courseOffset.x);
			float3 val3 = MathUtils.Position(courseData.m_Curve, courseOffset.y);
			float3 val4 = MathUtils.Tangent(courseData.m_Curve, courseOffset.x);
			float3 val5 = MathUtils.Tangent(courseData.m_Curve, courseOffset.y);
			MathUtils.TryNormalize(ref val4);
			MathUtils.TryNormalize(ref val5);
			float2 startLeft = ((float3)(ref val2)).xz - ((float3)(ref val4)).zx * val;
			float2 startRight = ((float3)(ref val2)).xz + ((float3)(ref val4)).zx * val;
			float2 endLeft = ((float3)(ref val3)).xz - ((float3)(ref val5)).zx * val;
			float2 endRight = ((float3)(ref val3)).xz + ((float3)(ref val5)).zx * val;
			float num = 0f;
			float2 val6 = ((float3)(ref segment.m_Left.a)).xz;
			float2 val7 = ((float3)(ref segment.m_Right.a)).xz;
			IntersectPos target = new IntersectPos
			{
				m_Priority = -1f
			};
			float num3 = default(float);
			float num4 = default(float);
			Segment val9 = default(Segment);
			float num5 = default(float);
			Segment val10 = default(Segment);
			float num6 = default(float);
			Segment val12 = default(Segment);
			float2 val13 = default(float2);
			Bounds1 val14 = default(Bounds1);
			LocalTransformCache localTransformCache = default(LocalTransformCache);
			float num8 = default(float);
			float num9 = default(float);
			for (int i = 1; i <= 8; i++)
			{
				float num2 = (float)i / 8f;
				float3 val8 = MathUtils.Position(segment.m_Left, num2);
				float2 xz = ((float3)(ref val8)).xz;
				val8 = MathUtils.Position(segment.m_Right, num2);
				float2 xz2 = ((float3)(ref val8)).xz;
				Bounds1 intersectRange;
				Bounds1 intersectRange2;
				bool flag = QuadIntersect(startLeft, startRight, endLeft, endRight, val6, val7, xz, xz2, out intersectRange, out intersectRange2);
				if (flag)
				{
					if ((prefabCompositionData2.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.Roundabout)) == CompositionFlags.General.DeadEnd)
					{
						MathUtils.Distance(((Bezier4x3)(ref courseData.m_Curve)).xz, ((float3)(ref segment.m_Left.a)).xz, ref num3);
						MathUtils.Distance(((Bezier4x3)(ref courseData.m_Curve)).xz, ((float3)(ref segment.m_Right.a)).xz, ref num4);
						intersectRange = MathUtils.Bounds(num3, num4);
						intersectRange.min = math.select(intersectRange.min, 0f, courseOffset.x == 0f && intersectRange.min <= 0.01f);
						intersectRange.max = math.select(intersectRange.max, 1f, courseOffset.y == 1f && intersectRange.max >= 0.99f);
					}
					else
					{
						intersectRange.min = math.lerp(courseOffset.x, courseOffset.y, intersectRange.min - 0.01f);
						intersectRange.max = math.lerp(courseOffset.x, courseOffset.y, intersectRange.max + 0.01f);
					}
				}
				if (courseOffset.x == 0f)
				{
					((Segment)(ref val9))._002Ector(math.lerp(val6, val7, centerOffset), math.lerp(xz, xz2, centerOffset));
					if (MathUtils.Distance(val9, ((float3)(ref val2)).xz, ref num5) <= MathUtils.Distance(val9, ((float3)(ref val3)).xz, ref num5) && CircleIntersect(new Circle2(val.x, ((float3)(ref val2)).xz), val6, val7, xz, xz2, out var intersectRange3))
					{
						intersectRange |= 0f;
						intersectRange2 |= intersectRange3;
						flag = true;
					}
				}
				if (courseOffset.y == 1f)
				{
					((Segment)(ref val10))._002Ector(math.lerp(val6, val7, centerOffset), math.lerp(xz, xz2, centerOffset));
					if (MathUtils.Distance(val10, ((float3)(ref val2)).xz, ref num6) >= MathUtils.Distance(val10, ((float3)(ref val3)).xz, ref num6) && CircleIntersect(new Circle2(val.x, ((float3)(ref val3)).xz), val6, val7, xz, xz2, out var intersectRange4))
					{
						intersectRange |= 1f;
						intersectRange2 |= intersectRange4;
						flag = true;
					}
				}
				if (flag)
				{
					Segment val11 = new Segment(((float3)(ref val2)).xz, ((float3)(ref val3)).xz);
					((Segment)(ref val12))._002Ector(math.lerp(val6, val7, centerOffset), math.lerp(xz, xz2, centerOffset));
					float priority = MathUtils.Distance(val11, val12, ref val13);
					float num7 = math.lerp(courseOffset.x, courseOffset.y, val13.x);
					float3 position = MathUtils.Position(courseData.m_Curve, num7);
					val14.min = math.lerp(num, num2, intersectRange2.min - 0.01f);
					val14.max = math.lerp(num, num2, intersectRange2.max + 0.01f);
					((Bounds1)(ref intersectRange2))._002Ector(edgeOffset, edgeOffset);
					int parentMesh = -1;
					if (m_LocalTransformCacheData.TryGetComponent(node, ref localTransformCache))
					{
						parentMesh = localTransformCache.m_ParentMesh;
					}
					Bezier4x3 val15 = MathUtils.Lerp(segment.m_Left, segment.m_Right, 0.5f);
					MathUtils.Distance(((Bezier4x3)(ref val15)).xz, MathUtils.Position(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectRange.min), ref num8);
					MathUtils.Distance(((Bezier4x3)(ref val15)).xz, MathUtils.Position(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectRange.max), ref num9);
					if (num9 < num8)
					{
						CommonUtils.Swap(ref val14.min, ref val14.max);
					}
					IntersectPos intersectPos = new IntersectPos
					{
						m_CoursePos = 
						{
							m_Entity = node,
							m_Position = position,
							m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, num7)),
							m_CourseDelta = num7,
							m_SplitPosition = edgeOffset,
							m_Flags = (courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.IsGrid))
						}
					};
					intersectPos.m_CoursePos.m_Flags |= CoursePosFlags.FreeHeight;
					intersectPos.m_CoursePos.m_ParentMesh = parentMesh;
					intersectPos.m_CourseIntersection = intersectRange;
					intersectPos.m_IntersectionHeightMin = new Bounds1(float2.op_Implicit(MathUtils.Position(val15, num8).y));
					intersectPos.m_IntersectionHeightMax = new Bounds1(float2.op_Implicit(MathUtils.Position(val15, num9).y));
					intersectPos.m_EdgeIntersection = intersectRange2;
					intersectPos.m_EdgeHeightRangeMin = GetHeightRange(segment, val14.min, prefabCompositionData2.m_HeightRange);
					intersectPos.m_EdgeHeightRangeMax = GetHeightRange(segment, val14.max, prefabCompositionData2.m_HeightRange);
					intersectPos.m_Priority = priority;
					intersectPos.m_AuxIndex = auxIndex;
					intersectPos.m_CourseIndex = courseIndex;
					intersectPos.m_IsNode = true;
					intersectPos.m_IsTunnel = (prefabCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0 && ((prefabCompositionData2.m_Flags.m_Left | prefabCompositionData2.m_Flags.m_Right) & (CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition)) == 0;
					if (target.m_Priority != -1f)
					{
						if (!MathUtils.Intersect(target.m_CourseIntersection, intersectPos.m_CourseIntersection) && !MathUtils.Intersect(target.m_EdgeIntersection, intersectPos.m_EdgeIntersection))
						{
							Add(ref currentIntersectPos, target);
							target = intersectPos;
						}
						else if (!Merge(ref target, intersectPos))
						{
							Add(ref currentIntersectPos, target);
							target = intersectPos;
						}
					}
					else
					{
						target = intersectPos;
					}
				}
				num = num2;
				val6 = xz;
				val7 = xz2;
			}
			if (target.m_Priority != -1f)
			{
				Add(ref currentIntersectPos, target);
			}
		}
	}

	private struct CourseHeightItem
	{
		public float m_TerrainHeight;

		public float m_TerrainBuildHeight;

		public float m_WaterHeight;

		public float m_CourseHeight;

		public float m_DistanceOffset;

		public Bounds1 m_LimitRange;

		public float2 m_LimitDistance;

		public bool m_ForceElevated;
	}

	private struct CourseHeightData
	{
		private NativeArray<CourseHeightItem> m_Buffer;

		private float2 m_SampleRange;

		private float m_SampleFactor;

		public CourseHeightData(Allocator allocator, NetCourse course, NetGeometryData netGeometryData, bool sampleTerrain, ref TerrainHeightData terrainHeightData, ref WaterSurfaceData waterSurfaceData)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			m_SampleRange = new float2(course.m_StartPosition.m_CourseDelta, course.m_EndPosition.m_CourseDelta);
			Bezier4x3 val = MathUtils.Cut(course.m_Curve, m_SampleRange);
			float num = MathUtils.Length(((Bezier4x3)(ref val)).xz);
			int num2 = 1 + Mathf.CeilToInt(num / 4f);
			if (m_SampleRange.y > m_SampleRange.x)
			{
				m_SampleFactor = (float)(num2 - 1) / (m_SampleRange.y - m_SampleRange.x);
			}
			else
			{
				m_SampleFactor = 0f;
			}
			m_Buffer = new NativeArray<CourseHeightItem>(num2, allocator, (NativeArrayOptions)1);
			float3 val2 = course.m_StartPosition.m_Position;
			float num3 = 1f / (float)math.max(1, num2 - 1);
			float num4 = 0f;
			float num5 = math.max(course.m_StartPosition.m_Elevation.x, course.m_EndPosition.m_Elevation.x);
			for (int i = 0; i < num2; i++)
			{
				float3 val3;
				float num6;
				if (i == 0)
				{
					val3 = course.m_StartPosition.m_Position;
					num6 = course.m_StartPosition.m_Elevation.x;
				}
				else if (i == num2 - 1)
				{
					val3 = course.m_EndPosition.m_Position;
					num6 = course.m_EndPosition.m_Elevation.x;
				}
				else
				{
					val3 = MathUtils.Position(val, (float)i * num3);
					num6 = math.lerp(course.m_StartPosition.m_Elevation.x, course.m_EndPosition.m_Elevation.x, (float)i * num3);
				}
				CourseHeightItem courseHeightItem = default(CourseHeightItem);
				if (sampleTerrain)
				{
					WaterUtils.SampleHeight(ref waterSurfaceData, ref terrainHeightData, val3, out courseHeightItem.m_TerrainHeight, out courseHeightItem.m_WaterHeight, out var waterDepth);
					courseHeightItem.m_TerrainBuildHeight = math.select(courseHeightItem.m_TerrainHeight, courseHeightItem.m_TerrainHeight + num5, num5 < -1f);
					if (waterDepth < 0.2f)
					{
						courseHeightItem.m_WaterHeight = courseHeightItem.m_TerrainHeight;
					}
					else
					{
						courseHeightItem.m_WaterHeight += netGeometryData.m_ElevationLimit * 2f;
					}
				}
				else
				{
					courseHeightItem.m_TerrainHeight = val3.y - num6;
					courseHeightItem.m_TerrainBuildHeight = math.select(courseHeightItem.m_TerrainHeight, courseHeightItem.m_TerrainHeight + num5, num5 < -1f);
					courseHeightItem.m_WaterHeight = -1000000f;
					courseHeightItem.m_CourseHeight = val3.y;
				}
				courseHeightItem.m_DistanceOffset = math.distance(((float3)(ref val2)).xz, ((float3)(ref val3)).xz);
				courseHeightItem.m_LimitRange = new Bounds1(-1000000f, 1000000f);
				courseHeightItem.m_LimitDistance = float2.op_Implicit(1000000f);
				m_Buffer[i] = courseHeightItem;
				val2 = val3;
				num4 += courseHeightItem.m_DistanceOffset;
			}
			if (!sampleTerrain)
			{
				return;
			}
			InitializeCoursePos(ref course.m_StartPosition);
			InitializeCoursePos(ref course.m_EndPosition);
			Bounds1 val4 = default(Bounds1);
			((Bounds1)(ref val4))._002Ector(-1000000f, 1000000f);
			Bounds1 val5 = default(Bounds1);
			((Bounds1)(ref val5))._002Ector(-1000000f, 1000000f);
			if (course.m_StartPosition.m_Elevation.x >= netGeometryData.m_ElevationLimit || course.m_EndPosition.m_Elevation.x >= netGeometryData.m_ElevationLimit || (netGeometryData.m_Flags & Game.Net.GeometryFlags.RequireElevated) != 0)
			{
				val4.min = course.m_StartPosition.m_Position.y;
				val5.min = course.m_EndPosition.m_Position.y;
			}
			else
			{
				if (course.m_StartPosition.m_Elevation.x > 1f)
				{
					val4.min = course.m_StartPosition.m_Position.y;
					val5.min = math.max(val5.min, course.m_EndPosition.m_Position.y - num4 * netGeometryData.m_MaxSlopeSteepness * 0.5f);
				}
				if (course.m_EndPosition.m_Elevation.x > 1f)
				{
					val4.min = math.max(val4.min, course.m_StartPosition.m_Position.y - num4 * netGeometryData.m_MaxSlopeSteepness * 0.5f);
					val5.min = course.m_EndPosition.m_Position.y;
				}
			}
			if (course.m_StartPosition.m_Elevation.x <= 0f - netGeometryData.m_ElevationLimit || course.m_EndPosition.m_Elevation.x <= 0f - netGeometryData.m_ElevationLimit)
			{
				val4.max = course.m_StartPosition.m_Position.y;
				val5.max = course.m_EndPosition.m_Position.y;
			}
			else
			{
				if (course.m_StartPosition.m_Elevation.x < -1f)
				{
					val4.max = course.m_StartPosition.m_Position.y;
					val5.max = math.min(val5.max, course.m_EndPosition.m_Position.y + num4 * netGeometryData.m_MaxSlopeSteepness * 0.5f);
				}
				if (course.m_EndPosition.m_Elevation.x < -1f)
				{
					val4.max = math.min(val4.max, course.m_StartPosition.m_Position.y + num4 * netGeometryData.m_MaxSlopeSteepness * 0.5f);
					val5.max = course.m_EndPosition.m_Position.y;
				}
			}
			float num7 = -1000000f;
			float num8 = 0f;
			for (int j = 0; j < num2; j++)
			{
				CourseHeightItem courseHeightItem2 = m_Buffer[j];
				Bounds1 val6;
				if (j == 0)
				{
					val6 = val4;
				}
				else if (j == num2 - 1)
				{
					val6 = val5;
				}
				else
				{
					num8 += courseHeightItem2.m_DistanceOffset;
					val6 = MathUtils.Lerp(val4, val5, num8 / num4);
				}
				num7 -= courseHeightItem2.m_DistanceOffset * netGeometryData.m_MaxSlopeSteepness;
				courseHeightItem2.m_CourseHeight = math.select(math.max(courseHeightItem2.m_TerrainHeight, courseHeightItem2.m_WaterHeight), courseHeightItem2.m_TerrainBuildHeight, num5 < -1f);
				courseHeightItem2.m_CourseHeight = MathUtils.Clamp(courseHeightItem2.m_CourseHeight, val6);
				courseHeightItem2.m_CourseHeight = math.max(courseHeightItem2.m_CourseHeight, num7);
				num7 = courseHeightItem2.m_CourseHeight;
				m_Buffer[j] = courseHeightItem2;
			}
			num7 = -1000000f;
			for (int num9 = num2 - 1; num9 >= 0; num9--)
			{
				CourseHeightItem courseHeightItem3 = m_Buffer[num9];
				courseHeightItem3.m_CourseHeight = math.max(courseHeightItem3.m_CourseHeight, num7);
				num7 = courseHeightItem3.m_CourseHeight - courseHeightItem3.m_DistanceOffset * netGeometryData.m_MaxSlopeSteepness;
				m_Buffer[num9] = courseHeightItem3;
			}
		}

		public void InitializeCoursePos(ref CoursePos coursePos)
		{
			if ((coursePos.m_Flags & CoursePosFlags.FreeHeight) != 0)
			{
				float num;
				if (coursePos.m_CourseDelta == m_SampleRange.x)
				{
					CourseHeightItem courseHeightItem = m_Buffer[0];
					num = math.select(math.max(courseHeightItem.m_TerrainHeight, courseHeightItem.m_WaterHeight), courseHeightItem.m_TerrainHeight, coursePos.m_Elevation.x < -1f);
				}
				else if (coursePos.m_CourseDelta == m_SampleRange.y)
				{
					CourseHeightItem courseHeightItem2 = m_Buffer[m_Buffer.Length - 1];
					num = math.select(math.max(courseHeightItem2.m_TerrainHeight, courseHeightItem2.m_WaterHeight), courseHeightItem2.m_TerrainHeight, coursePos.m_Elevation.x < -1f);
				}
				else
				{
					float num2 = (coursePos.m_CourseDelta - m_SampleRange.x) * m_SampleFactor;
					int num3 = math.clamp(Mathf.FloorToInt(num2), 0, m_Buffer.Length - 1);
					CourseHeightItem courseHeightItem3 = m_Buffer[num3];
					CourseHeightItem courseHeightItem4 = m_Buffer[math.min(num3 + 1, m_Buffer.Length - 1)];
					float num4 = math.saturate(num2 - (float)num3);
					float num5 = math.lerp(courseHeightItem3.m_TerrainHeight, courseHeightItem4.m_TerrainHeight, num4);
					float num6 = math.lerp(courseHeightItem3.m_WaterHeight, courseHeightItem4.m_WaterHeight, num4);
					num = math.select(math.max(num5, num6), num5, coursePos.m_Elevation.x < -1f);
				}
				coursePos.m_Position.y = num + coursePos.m_Elevation.x;
			}
		}

		public void ApplyLimitRange(IntersectPos intersectPos, NetGeometryData netGeometryData, Bounds1 limitRangeMin, Bounds1 limitRangeMax, bool shrink = false, bool forceElevated = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (new float2(intersectPos.m_CourseIntersection.min, intersectPos.m_CourseIntersection.max) - m_SampleRange.x) * m_SampleFactor;
			int num;
			int num2;
			if (shrink)
			{
				num = math.max(Mathf.RoundToInt(val.x), 0);
				num2 = math.min(Mathf.RoundToInt(val.y), m_Buffer.Length - 1);
			}
			else
			{
				num = math.max(Mathf.FloorToInt(val.x), 0);
				num2 = math.min(Mathf.CeilToInt(val.y), m_Buffer.Length - 1);
			}
			num = math.min(num, m_Buffer.Length);
			num2 = math.max(num2, -1);
			float num3 = 1f / (float)math.max(1, num2 - num);
			if (forceElevated)
			{
				for (int i = num; i <= num2; i++)
				{
					CourseHeightItem item = m_Buffer[i];
					Bounds1 limitRange = MathUtils.Lerp(limitRangeMin, limitRangeMax, (float)(i - num) * num3);
					if (AddLimit(ref item, limitRange, 0f) || !item.m_ForceElevated)
					{
						item.m_CourseHeight = MathUtils.Clamp(item.m_CourseHeight, item.m_LimitRange);
						item.m_ForceElevated |= forceElevated;
						m_Buffer[i] = item;
					}
				}
			}
			else
			{
				for (int j = num; j <= num2; j++)
				{
					CourseHeightItem item2 = m_Buffer[j];
					Bounds1 limitRange2 = MathUtils.Lerp(limitRangeMin, limitRangeMax, (float)(j - num) * num3);
					if (AddLimit(ref item2, limitRange2, 0f))
					{
						item2.m_CourseHeight = MathUtils.Clamp(item2.m_CourseHeight, item2.m_LimitRange);
						m_Buffer[j] = item2;
					}
				}
			}
			if (num > 0)
			{
				float num4 = 0f;
				if (num < m_Buffer.Length)
				{
					num4 += m_Buffer[num].m_DistanceOffset;
				}
				for (int num5 = num - 1; num5 >= 0; num5--)
				{
					CourseHeightItem item3 = m_Buffer[num5];
					Bounds1 limitRange3 = MathUtils.Expand(limitRangeMin, num4 * netGeometryData.m_MaxSlopeSteepness);
					if (!AddLimit(ref item3, limitRange3, num4))
					{
						break;
					}
					item3.m_CourseHeight = MathUtils.Clamp(item3.m_CourseHeight, item3.m_LimitRange);
					num4 += item3.m_DistanceOffset;
					m_Buffer[num5] = item3;
				}
			}
			if (num2 >= m_Buffer.Length - 1)
			{
				return;
			}
			float num6 = 0f;
			for (int k = num2 + 1; k < m_Buffer.Length; k++)
			{
				CourseHeightItem item4 = m_Buffer[k];
				num6 += item4.m_DistanceOffset;
				Bounds1 limitRange4 = MathUtils.Expand(limitRangeMax, num6 * netGeometryData.m_MaxSlopeSteepness);
				if (AddLimit(ref item4, limitRange4, num6))
				{
					item4.m_CourseHeight = MathUtils.Clamp(item4.m_CourseHeight, item4.m_LimitRange);
					m_Buffer[k] = item4;
					continue;
				}
				break;
			}
		}

		private bool AddLimit(ref CourseHeightItem item, Bounds1 limitRange, float distance)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			if (limitRange.min > item.m_LimitRange.min)
			{
				item.m_LimitDistance.x = distance;
				if (limitRange.min > item.m_LimitRange.max)
				{
					float num = math.select(distance / math.csum(item.m_LimitDistance), 0.5f, math.all(item.m_LimitDistance == 0f));
					((Bounds1)(ref limitRange))._002Ector(float2.op_Implicit(math.lerp(limitRange.min, item.m_LimitRange.max, num)));
				}
			}
			else
			{
				limitRange.min = item.m_LimitRange.min;
			}
			if (limitRange.max < item.m_LimitRange.max)
			{
				item.m_LimitDistance.y = distance;
				if (limitRange.max < item.m_LimitRange.min)
				{
					float num2 = math.select(distance / math.csum(item.m_LimitDistance), 0.5f, math.all(item.m_LimitDistance == 0f));
					((Bounds1)(ref limitRange))._002Ector(float2.op_Implicit(math.lerp(limitRange.max, item.m_LimitRange.min, num2)));
				}
			}
			else
			{
				limitRange.max = item.m_LimitRange.max;
			}
			bool result = !((Bounds1)(ref limitRange)).Equals(item.m_LimitRange);
			item.m_LimitRange = limitRange;
			return result;
		}

		public void StraightenElevation(IntersectPos firstPos, IntersectPos lastPos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (new float2(firstPos.m_CourseIntersection.max, lastPos.m_CourseIntersection.min) - m_SampleRange.x) * m_SampleFactor;
			int num = math.max(Mathf.FloorToInt(val.x), 0);
			int num2 = math.min(Mathf.CeilToInt(val.y), m_Buffer.Length - 1);
			int num3 = num + 1;
			while (num3 < num2)
			{
				CourseHeightItem courseHeightItem = m_Buffer[num3];
				if (courseHeightItem.m_CourseHeight != courseHeightItem.m_TerrainBuildHeight && courseHeightItem.m_LimitRange.min < courseHeightItem.m_LimitRange.max)
				{
					float num4 = courseHeightItem.m_DistanceOffset;
					int i = num3;
					CourseHeightItem courseHeightItem2 = m_Buffer[i];
					for (; i < num2; i++)
					{
						courseHeightItem2 = m_Buffer[i + 1];
						num4 += courseHeightItem2.m_DistanceOffset;
						if (courseHeightItem2.m_CourseHeight == courseHeightItem2.m_TerrainBuildHeight || courseHeightItem2.m_LimitRange.min >= courseHeightItem2.m_LimitRange.max)
						{
							break;
						}
					}
					CourseHeightItem courseHeightItem3 = m_Buffer[num3 - 1];
					float num5 = 0f;
					for (int j = num3; j <= i; j++)
					{
						CourseHeightItem courseHeightItem4 = m_Buffer[j];
						num5 += courseHeightItem4.m_DistanceOffset;
						courseHeightItem4.m_CourseHeight = math.lerp(courseHeightItem3.m_CourseHeight, courseHeightItem2.m_CourseHeight, num5 / num4);
						courseHeightItem4.m_CourseHeight = MathUtils.Clamp(courseHeightItem4.m_CourseHeight, courseHeightItem4.m_LimitRange);
						m_Buffer[j] = courseHeightItem4;
					}
					num3 = i + 1;
				}
				else
				{
					num3++;
				}
			}
		}

		public void SampleCourseHeight(ref NetCourse course, NetGeometryData netGeometryData)
		{
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			if ((course.m_StartPosition.m_Flags & CoursePosFlags.FreeHeight) != 0)
			{
				course.m_StartPosition.m_Position.y = SampleHeight(course.m_StartPosition.m_CourseDelta, out var forceElevated);
				if (forceElevated)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
			}
			else
			{
				SampleHeight(course.m_StartPosition.m_CourseDelta, out var forceElevated2);
				if (forceElevated2)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
			}
			if ((course.m_EndPosition.m_Flags & CoursePosFlags.FreeHeight) != 0)
			{
				course.m_EndPosition.m_Position.y = SampleHeight(course.m_EndPosition.m_CourseDelta, out var forceElevated3);
				if (forceElevated3)
				{
					course.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
			}
			else
			{
				SampleHeight(course.m_EndPosition.m_CourseDelta, out var forceElevated4);
				if (forceElevated4)
				{
					course.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
			}
			if (((float3)(ref course.m_StartPosition.m_Position)).Equals(course.m_EndPosition.m_Position))
			{
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
				{
					course.m_Curve.a = course.m_StartPosition.m_Position;
				}
				else
				{
					course.m_Curve.a.y = course.m_StartPosition.m_Position.y;
				}
				course.m_Curve.b = course.m_Curve.a;
				course.m_Curve.c = course.m_Curve.a;
				course.m_Curve.d = course.m_Curve.a;
			}
			else if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StraightEdges) != 0)
			{
				float2 val = default(float2);
				((float2)(ref val))._002Ector(course.m_StartPosition.m_CourseDelta, course.m_EndPosition.m_CourseDelta);
				float2 val2 = math.lerp(float2.op_Implicit(val.x), float2.op_Implicit(val.y), new float2(1f / 3f, 2f / 3f));
				SampleHeight(val2.x, out var forceElevated5);
				SampleHeight(val2.y, out var forceElevated6);
				if (forceElevated5 || forceElevated6)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
					course.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
				}
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
				{
					course.m_Curve = NetUtils.StraightCurve(course.m_StartPosition.m_Position, course.m_EndPosition.m_Position, netGeometryData.m_Hanging);
				}
				else
				{
					float3 startPos = MathUtils.Position(course.m_Curve, course.m_StartPosition.m_CourseDelta);
					float3 endPos = MathUtils.Position(course.m_Curve, course.m_EndPosition.m_CourseDelta);
					startPos.y = course.m_StartPosition.m_Position.y;
					endPos.y = course.m_EndPosition.m_Position.y;
					course.m_Curve = NetUtils.StraightCurve(startPos, endPos, netGeometryData.m_Hanging);
				}
			}
			else
			{
				float2 val3 = default(float2);
				((float2)(ref val3))._002Ector(course.m_StartPosition.m_CourseDelta, course.m_EndPosition.m_CourseDelta);
				float2 val4 = math.lerp(float2.op_Implicit(val3.x), float2.op_Implicit(val3.y), new float2(1f / 3f, 2f / 3f));
				course.m_Curve = MathUtils.Cut(course.m_Curve, val3);
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
				{
					course.m_Curve.a = course.m_StartPosition.m_Position;
					course.m_Curve.d = course.m_EndPosition.m_Position;
				}
				else
				{
					course.m_Curve.a.y = course.m_StartPosition.m_Position.y;
					course.m_Curve.d.y = course.m_EndPosition.m_Position.y;
				}
				course.m_Curve.b.y = SampleHeight(val4.x, out var forceElevated7);
				course.m_Curve.c.y = SampleHeight(val4.y, out var forceElevated8);
				if (forceElevated7 || forceElevated8)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
					course.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
				}
				float num = course.m_Curve.b.y - MathUtils.Position(course.m_Curve, 1f / 3f).y;
				float num2 = course.m_Curve.c.y - MathUtils.Position(course.m_Curve, 2f / 3f).y;
				course.m_Curve.b.y += num * 3f - num2 * 1.5f;
				course.m_Curve.c.y += num2 * 3f - num * 1.5f;
			}
			course.m_StartPosition.m_CourseDelta = 0f;
			course.m_EndPosition.m_CourseDelta = 1f;
			course.m_Length = MathUtils.Length(course.m_Curve);
		}

		public float SampleHeight(float courseDelta, out bool forceElevated)
		{
			if (courseDelta == m_SampleRange.x)
			{
				CourseHeightItem courseHeightItem = m_Buffer[0];
				forceElevated = courseHeightItem.m_ForceElevated;
				return courseHeightItem.m_CourseHeight;
			}
			if (courseDelta == m_SampleRange.y)
			{
				CourseHeightItem courseHeightItem2 = m_Buffer[m_Buffer.Length - 1];
				forceElevated = courseHeightItem2.m_ForceElevated;
				return courseHeightItem2.m_CourseHeight;
			}
			float num = (courseDelta - m_SampleRange.x) * m_SampleFactor;
			int num2 = math.clamp(Mathf.FloorToInt(num), 0, m_Buffer.Length - 1);
			CourseHeightItem courseHeightItem3 = m_Buffer[num2];
			CourseHeightItem courseHeightItem4 = m_Buffer[math.min(num2 + 1, m_Buffer.Length - 1)];
			forceElevated = courseHeightItem3.m_ForceElevated | courseHeightItem4.m_ForceElevated;
			return math.lerp(courseHeightItem3.m_CourseHeight, courseHeightItem4.m_CourseHeight, math.saturate(num - (float)num2));
		}

		public void GetHeightRange(IntersectPos intersectPos, NetGeometryData netGeometryData, bool canUseElevatedHeight, out Bounds1 minBounds, out Bounds1 maxBounds, out bool elevated)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (new float2(intersectPos.m_CourseIntersection.min, intersectPos.m_CourseIntersection.max) - m_SampleRange.x) * m_SampleFactor;
			int num = math.max(Mathf.FloorToInt(val.x), 0);
			int num2 = math.min(Mathf.CeilToInt(val.y), m_Buffer.Length - 1);
			minBounds = new Bounds1(1000000f, -1000000f);
			maxBounds = new Bounds1(1000000f, -1000000f);
			elevated = false;
			if (num < m_Buffer.Length)
			{
				CourseHeightItem courseHeightItem = m_Buffer[num];
				CourseHeightItem courseHeightItem2 = m_Buffer[math.min(num + 1, m_Buffer.Length - 1)];
				minBounds = new Bounds1(float2.op_Implicit(math.lerp(courseHeightItem.m_CourseHeight, courseHeightItem2.m_CourseHeight, math.saturate(val.x - (float)num))));
				elevated |= (courseHeightItem.m_CourseHeight > courseHeightItem.m_TerrainHeight) | (courseHeightItem2.m_CourseHeight > courseHeightItem2.m_TerrainHeight);
			}
			if (num2 >= 0)
			{
				CourseHeightItem courseHeightItem3 = m_Buffer[math.max(num2 - 1, 0)];
				CourseHeightItem courseHeightItem4 = m_Buffer[num2];
				maxBounds = new Bounds1(float2.op_Implicit(math.lerp(courseHeightItem3.m_CourseHeight, courseHeightItem4.m_CourseHeight, math.saturate(val.y - (float)(num2 - 1)))));
				elevated |= (courseHeightItem3.m_CourseHeight > courseHeightItem3.m_TerrainHeight) | (courseHeightItem4.m_CourseHeight > courseHeightItem4.m_TerrainHeight);
			}
			Bounds1 val2 = minBounds | maxBounds;
			for (int i = num + 1; i < num2; i++)
			{
				CourseHeightItem courseHeightItem5 = m_Buffer[i];
				if (courseHeightItem5.m_CourseHeight < val2.min || courseHeightItem5.m_CourseHeight > val2.max)
				{
					val2 |= courseHeightItem5.m_CourseHeight;
					float num3 = ((float)i - val.x) / math.max(1f, val.y - val.x);
					float2 val3 = math.saturate(new float2(2f - 2f * num3, 2f * num3));
					float4 val4 = math.max(float4.op_Implicit(0f), new float4(minBounds.min - courseHeightItem5.m_CourseHeight, courseHeightItem5.m_CourseHeight - minBounds.max, maxBounds.min - courseHeightItem5.m_CourseHeight, courseHeightItem5.m_CourseHeight - maxBounds.max)) * ((float2)(ref val3)).xxyy;
					minBounds.min -= val4.x;
					minBounds.max += val4.y;
					maxBounds.min -= val4.z;
					maxBounds.max += val4.w;
				}
				elevated |= courseHeightItem5.m_CourseHeight > courseHeightItem5.m_TerrainHeight;
			}
			float num4 = math.select(netGeometryData.m_DefaultHeightRange.min, netGeometryData.m_ElevatedHeightRange.min, canUseElevatedHeight & elevated);
			float num5 = math.select(netGeometryData.m_DefaultHeightRange.max, netGeometryData.m_ElevatedHeightRange.max, canUseElevatedHeight & elevated);
			minBounds.min += num4;
			minBounds.max += num5;
			maxBounds.min += num4;
			maxBounds.max += num5;
		}

		public void Dispose()
		{
			m_Buffer.Dispose();
		}
	}

	[BurstCompile]
	private struct CheckCourseIntersectionResultsJob : IJobParallelForDefer
	{
		private struct AuxIntersectionEntity
		{
			public Entity m_Entity;

			public float m_SplitPosition;
		}

		private struct ElevationSegment
		{
			public Bounds1 m_CourseRange;

			public Bounds1 m_DistanceOffset;

			public int2 m_ElevationType;

			public bool m_CanRemove;
		}

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<LocalCurveCache> m_LocalCurveCacheData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PrefabPlaceableData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_PrefabBuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> m_PrefabServiceUpgradeData;

		[ReadOnly]
		public BufferLookup<FixedNetElement> m_PrefabFixedNetElements;

		[ReadOnly]
		public BufferLookup<AuxiliaryNet> m_PrefabAuxiliaryNets;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public NativeList<Course> m_CourseList;

		[ReadOnly]
		public NativeHashMap<Entity, bool> m_DeletedEntities;

		[ReadOnly]
		public Reader<IntersectPos> m_IntersectionQueue;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_098a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_092f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_076a: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_078f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			Course course = m_CourseList[index];
			NetData prefabNetData = m_PrefabNetData[course.m_CreationDefinition.m_Prefab];
			NetGeometryData netGeometryData = m_PrefabGeometryData[course.m_CreationDefinition.m_Prefab];
			IntersectPos intersectPos = default(IntersectPos);
			intersectPos.m_CoursePos = course.m_CourseData.m_StartPosition;
			intersectPos.m_CourseIntersection = new Bounds1(course.m_CourseData.m_StartPosition.m_CourseDelta, course.m_CourseData.m_StartPosition.m_CourseDelta);
			intersectPos.m_IntersectionHeightMin = new Bounds1(float.MaxValue, float.MinValue);
			intersectPos.m_IntersectionHeightMax = new Bounds1(float.MaxValue, float.MinValue);
			intersectPos.m_EdgeIntersection = new Bounds1(course.m_CourseData.m_StartPosition.m_SplitPosition, course.m_CourseData.m_StartPosition.m_SplitPosition);
			intersectPos.m_EdgeHeightRangeMin = new Bounds1(1000000f, -1000000f);
			intersectPos.m_EdgeHeightRangeMax = new Bounds1(1000000f, -1000000f);
			intersectPos.m_Priority = -1f;
			intersectPos.m_AuxIndex = -1;
			intersectPos.m_IsNode = m_NodeData.HasComponent(intersectPos.m_CoursePos.m_Entity);
			intersectPos.m_IsStartEnd = true;
			IntersectPos intersectPos2 = default(IntersectPos);
			intersectPos2.m_CoursePos = course.m_CourseData.m_EndPosition;
			intersectPos2.m_CourseIntersection = new Bounds1(course.m_CourseData.m_EndPosition.m_CourseDelta, course.m_CourseData.m_EndPosition.m_CourseDelta);
			intersectPos2.m_IntersectionHeightMin = new Bounds1(float.MaxValue, float.MinValue);
			intersectPos2.m_IntersectionHeightMax = new Bounds1(float.MaxValue, float.MinValue);
			intersectPos2.m_EdgeIntersection = new Bounds1(course.m_CourseData.m_EndPosition.m_SplitPosition, course.m_CourseData.m_EndPosition.m_SplitPosition);
			intersectPos2.m_EdgeHeightRangeMin = new Bounds1(1000000f, -1000000f);
			intersectPos2.m_EdgeHeightRangeMax = new Bounds1(1000000f, -1000000f);
			intersectPos2.m_Priority = -1f;
			intersectPos2.m_AuxIndex = -1;
			intersectPos2.m_IsNode = m_NodeData.HasComponent(intersectPos2.m_CoursePos.m_Entity);
			intersectPos2.m_IsStartEnd = true;
			bool flag = !m_EditorMode && (course.m_CreationDefinition.m_Flags & CreationFlags.SubElevation) != 0 && m_PrefabServiceUpgradeData.HasComponent(course.m_CreationDefinition.m_Prefab);
			bool flag2 = (course.m_CreationDefinition.m_Owner == Entity.Null && course.m_OwnerDefinition.m_Prefab == Entity.Null) || flag;
			CourseHeightData courseHeightData = new CourseHeightData((Allocator)2, course.m_CourseData, netGeometryData, flag2, ref m_TerrainHeightData, ref m_WaterSurfaceData);
			if (flag2)
			{
				courseHeightData.InitializeCoursePos(ref intersectPos.m_CoursePos);
				courseHeightData.InitializeCoursePos(ref intersectPos2.m_CoursePos);
			}
			courseHeightData.ApplyLimitRange(intersectPos, netGeometryData, new Bounds1(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y)), new Bounds1(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y)));
			courseHeightData.ApplyLimitRange(intersectPos2, netGeometryData, new Bounds1(float2.op_Implicit(intersectPos2.m_CoursePos.m_Position.y)), new Bounds1(float2.op_Implicit(intersectPos2.m_CoursePos.m_Position.y)));
			courseHeightData.StraightenElevation(intersectPos, intersectPos2);
			bool flag3 = m_PrefabBuildingExtensionData.HasComponent(course.m_OwnerDefinition.m_Prefab);
			bool canBeSplitted = flag3 || (course.m_CreationDefinition.m_Owner == Entity.Null && course.m_OwnerDefinition.m_Prefab == Entity.Null) || (course.m_CreationDefinition.m_Flags & CreationFlags.SubElevation) != 0;
			NativeList<IntersectPos> val = default(NativeList<IntersectPos>);
			val._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<IntersectPos> val2 = default(NativeList<IntersectPos>);
			val2._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<IntersectPos> val3 = default(NativeList<IntersectPos>);
			NativeList<IntersectPos> val4 = default(NativeList<IntersectPos>);
			NativeList<AuxIntersectionEntity> auxIntersectionEntities = default(NativeList<AuxIntersectionEntity>);
			int intersectionAuxIndex = 0;
			DynamicBuffer<AuxiliaryNet> auxiliaryNets = default(DynamicBuffer<AuxiliaryNet>);
			if (m_PrefabAuxiliaryNets.TryGetBuffer(course.m_CreationDefinition.m_Prefab, ref auxiliaryNets))
			{
				auxIntersectionEntities._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				auxIntersectionEntities.Resize(16, (NativeArrayOptions)1);
				val3._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
				val4._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			}
			val.Add(ref intersectPos);
			Enumerator<IntersectPos> enumerator = m_IntersectionQueue.GetEnumerator(index % m_IntersectionQueue.HashRange);
			while (enumerator.MoveNext())
			{
				IntersectPos current = enumerator.Current;
				if (current.m_CourseIndex == index)
				{
					if (current.m_AuxIndex == -1)
					{
						val.Add(ref current);
					}
					else
					{
						val3.Add(ref current);
					}
				}
			}
			enumerator.Dispose();
			val.Add(ref intersectPos2);
			if (val.Length >= 4)
			{
				NativeSortExtension.Sort<IntersectPos>(val.AsArray().GetSubArray(1, val.Length - 2));
			}
			bool canAdjustHeight = flag2 && (prefabNetData.m_RequiredLayers & Layer.Waterway) == 0;
			bool flag4 = (prefabNetData.m_ConnectLayers & Layer.Waterway) != 0;
			if (!flag4 && auxiliaryNets.IsCreated)
			{
				for (int i = 0; i < auxiliaryNets.Length; i++)
				{
					AuxiliaryNet auxiliaryNet = auxiliaryNets[i];
					if ((m_PrefabNetData[auxiliaryNet.m_Prefab].m_RequiredLayers & Layer.Waterway) != Layer.None)
					{
						flag4 = true;
						break;
					}
				}
			}
			MergePositions(course.m_CourseData, course.m_CreationDefinition, course.m_OwnerDefinition, prefabNetData, netGeometryData, ref courseHeightData, val, val2, 0f, canAdjustHeight, flag4, canBeSplitted, flag3, auxiliaryNets.IsCreated && auxiliaryNets.Length != 0);
			if (auxiliaryNets.IsCreated)
			{
				intersectPos.m_CoursePos.m_Entity = Entity.Null;
				intersectPos.m_IsNode = false;
				intersectPos2.m_CoursePos.m_Entity = Entity.Null;
				intersectPos2.m_IsNode = false;
				for (int j = 0; j < auxiliaryNets.Length; j++)
				{
					AuxiliaryNet auxiliaryNet2 = auxiliaryNets[j];
					NetData prefabNetData2 = m_PrefabNetData[auxiliaryNet2.m_Prefab];
					NetGeometryData prefabGeometryData = m_PrefabGeometryData[auxiliaryNet2.m_Prefab];
					val.Clear();
					val4.Clear();
					intersectPos.m_CoursePos.m_Position.y = course.m_CourseData.m_StartPosition.m_Position.y + auxiliaryNet2.m_Position.y;
					intersectPos.m_AuxIndex = j;
					intersectPos2.m_CoursePos.m_Position.y = course.m_CourseData.m_EndPosition.m_Position.y + auxiliaryNet2.m_Position.y;
					intersectPos2.m_AuxIndex = j;
					val.Add(ref intersectPos);
					for (int k = 0; k < val3.Length; k++)
					{
						IntersectPos intersectPos3 = val3[k];
						if (intersectPos3.m_AuxIndex == j)
						{
							val.Add(ref intersectPos3);
						}
					}
					val.Add(ref intersectPos2);
					if (val.Length >= 4)
					{
						NativeSortExtension.Sort<IntersectPos>(val.AsArray().GetSubArray(1, val.Length - 2));
					}
					CreationDefinition creationDefinition = course.m_CreationDefinition;
					creationDefinition.m_Prefab = auxiliaryNet2.m_Prefab;
					canAdjustHeight = flag2 && (prefabNetData2.m_RequiredLayers & Layer.Waterway) == 0;
					MergePositions(course.m_CourseData, creationDefinition, course.m_OwnerDefinition, prefabNetData2, prefabGeometryData, ref courseHeightData, val, val4, auxiliaryNet2.m_Position.y, canAdjustHeight, flag4, canBeSplitted, flag3, hasSubNets: false);
					MergeAuxPositions(course.m_CourseData, val4, val2, auxiliaryNets, auxIntersectionEntities, ref intersectionAuxIndex);
				}
			}
			courseHeightData.StraightenElevation(intersectPos, intersectPos2);
			val.Clear();
			IntersectPos intersectPos4;
			if (m_PrefabFixedNetElements.HasBuffer(course.m_CreationDefinition.m_Prefab))
			{
				intersectPos4 = val2[0];
				val.Add(ref intersectPos4);
				if (val2.Length >= 2)
				{
					intersectPos4 = val2[val2.Length - 1];
					val.Add(ref intersectPos4);
				}
			}
			else
			{
				PlaceableNetData placeableNetData = default(PlaceableNetData);
				m_PrefabPlaceableData.TryGetComponent(course.m_CreationDefinition.m_Prefab, ref placeableNetData);
				CheckHeightRange(course.m_CourseData, netGeometryData, placeableNetData, ref courseHeightData, flag2, val2, val);
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
				{
					SnapCoursePositions(course.m_CourseData, val);
				}
			}
			if (val.Length >= 3)
			{
				float num = (((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0) ? 36f : 1f);
				if (val[1].m_IsOptional)
				{
					intersectPos4 = val[0];
					float2 xz = ((float3)(ref intersectPos4.m_CoursePos.m_Position)).xz;
					intersectPos4 = val[1];
					if (math.distancesq(xz, ((float3)(ref intersectPos4.m_CoursePos.m_Position)).xz) < num)
					{
						val.RemoveAt(1);
					}
				}
			}
			if (val.Length >= 3)
			{
				float num2 = (((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0) ? 36f : 1f);
				if (val[val.Length - 2].m_IsOptional)
				{
					intersectPos4 = val[val.Length - 1];
					float2 xz2 = ((float3)(ref intersectPos4.m_CoursePos.m_Position)).xz;
					intersectPos4 = val[val.Length - 2];
					if (math.distancesq(xz2, ((float3)(ref intersectPos4.m_CoursePos.m_Position)).xz) < num2)
					{
						val.RemoveAt(val.Length - 2);
					}
				}
			}
			UpdateCourses(course.m_CourseData, course.m_CreationDefinition, course.m_OwnerDefinition, course.m_UpgradedData, course.m_CourseEntity, index, val, auxIntersectionEntities, ref courseHeightData);
			if (auxIntersectionEntities.IsCreated)
			{
				auxIntersectionEntities.Dispose();
			}
			val.Dispose();
			val2.Dispose();
			courseHeightData.Dispose();
		}

		private void SnapCoursePositions(NetCourse courseData, NativeList<IntersectPos> intersectionList)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			for (int i = 1; i < intersectionList.Length - 1; i++)
			{
				IntersectPos intersectPos = intersectionList[i - 1];
				IntersectPos intersectPos2 = intersectionList[i];
				IntersectPos intersectPos3 = intersectionList[i + 1];
				float num = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(intersectPos.m_CoursePos.m_CourseDelta, intersectPos2.m_CoursePos.m_CourseDelta));
				num = MathUtils.Snap(num, 4f);
				((Bounds1)(ref val))._002Ector(intersectPos.m_CoursePos.m_CourseDelta, intersectPos3.m_CoursePos.m_CourseDelta);
				MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val, num);
				intersectPos2.m_CoursePos.m_CourseDelta = val.max;
				if (((int)math.round(num / 4f) & 1) != 0 != ((intersectPos.m_CoursePos.m_Flags & CoursePosFlags.HalfAlign) != 0))
				{
					intersectPos2.m_CoursePos.m_Flags |= CoursePosFlags.HalfAlign;
				}
				else
				{
					intersectPos2.m_CoursePos.m_Flags &= ~CoursePosFlags.HalfAlign;
				}
				intersectionList[i] = intersectPos2;
			}
		}

		private bool IsUnder(NetCourse courseData, Bounds1 heightRangeMin, Bounds1 heightRangeMax, IntersectPos intersectPos, float courseHeightOffset, bool canAdjustHeight)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (intersectPos.m_IsStartEnd)
			{
				return false;
			}
			float num = math.max(heightRangeMin.max - intersectPos.m_EdgeHeightRangeMin.min, heightRangeMax.max - intersectPos.m_EdgeHeightRangeMax.min);
			if (num <= 0f)
			{
				return true;
			}
			if (!canAdjustHeight)
			{
				return false;
			}
			float num2 = MathUtils.Position(courseData.m_Curve, intersectPos.m_CourseIntersection.min).y + courseHeightOffset;
			float num3 = MathUtils.Position(courseData.m_Curve, intersectPos.m_CourseIntersection.max).y + courseHeightOffset;
			Bounds1 val = intersectPos.m_IntersectionHeightMin;
			Bounds1 val2 = intersectPos.m_IntersectionHeightMax;
			if (val.max < val.min)
			{
				((Bounds1)(ref val))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
			}
			if (val2.max < val2.min)
			{
				((Bounds1)(ref val2))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
			}
			if (num < val.min - num2)
			{
				return num < val2.min - num3;
			}
			return false;
		}

		private bool IsOver(NetCourse courseData, Bounds1 heightRangeMin, Bounds1 heightRangeMax, IntersectPos intersectPos, float courseHeightOffset, bool canAdjustHeight)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			if (intersectPos.m_IsStartEnd)
			{
				return false;
			}
			float num = math.max(intersectPos.m_EdgeHeightRangeMin.max - heightRangeMin.min, intersectPos.m_EdgeHeightRangeMax.max - heightRangeMax.min);
			if (num <= 0f)
			{
				return true;
			}
			if (!canAdjustHeight)
			{
				return false;
			}
			float num2 = MathUtils.Position(courseData.m_Curve, intersectPos.m_CourseIntersection.min).y + courseHeightOffset;
			float num3 = MathUtils.Position(courseData.m_Curve, intersectPos.m_CourseIntersection.max).y + courseHeightOffset;
			Bounds1 val = intersectPos.m_IntersectionHeightMin;
			Bounds1 val2 = intersectPos.m_IntersectionHeightMax;
			if (val.max < val.min)
			{
				((Bounds1)(ref val))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
			}
			if (val2.max < val2.min)
			{
				((Bounds1)(ref val2))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
			}
			if (num < num2 - val.max)
			{
				return num < num3 - val2.max;
			}
			return false;
		}

		private void MergeAuxPositions(NetCourse courseData, NativeList<IntersectPos> source, NativeList<IntersectPos> target, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, ref int intersectionAuxIndex)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (target.Length == 0)
			{
				return;
			}
			ref IntersectPos reference = ref target.ElementAt(num++);
			for (int i = 0; i < source.Length; i++)
			{
				IntersectPos intersectPos = source[i];
				if (intersectPos.m_CoursePos.m_Entity == Entity.Null)
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				if (i == 0)
				{
					flag = true;
				}
				else if (i == source.Length - 1)
				{
					reference = ref target.ElementAt(target.Length - 1);
					flag = true;
				}
				else
				{
					while (reference.m_CourseIntersection.max < intersectPos.m_CourseIntersection.min && num < target.Length)
					{
						reference = ref target.ElementAt(num++);
					}
					if (MathUtils.Intersect(intersectPos.m_CourseIntersection, reference.m_CourseIntersection))
					{
						if (num < target.Length)
						{
							IntersectPos intersectPos2 = target[num];
							if (!MathUtils.Intersect(intersectPos.m_CourseIntersection, intersectPos2.m_CourseIntersection))
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
					}
					else if (num > 1)
					{
						flag2 = true;
					}
				}
				if (flag)
				{
					if (reference.m_AuxIndex == -1)
					{
						reference.m_AuxIndex = intersectionAuxIndex++;
					}
					if (auxIntersectionEntities[reference.m_AuxIndex * auxiliaryNets.Length + intersectPos.m_AuxIndex].m_Entity == Entity.Null)
					{
						ref Bounds1 reference2 = ref reference.m_CourseIntersection;
						reference2 |= intersectPos.m_CourseIntersection;
						reference.m_CanMove = default(Bounds1);
						reference.m_IsOptional = false;
						auxIntersectionEntities[reference.m_AuxIndex * auxiliaryNets.Length + intersectPos.m_AuxIndex] = new AuxIntersectionEntity
						{
							m_Entity = intersectPos.m_CoursePos.m_Entity,
							m_SplitPosition = intersectPos.m_CoursePos.m_SplitPosition
						};
					}
				}
				else if (flag2)
				{
					auxIntersectionEntities[intersectionAuxIndex * auxiliaryNets.Length + intersectPos.m_AuxIndex] = new AuxIntersectionEntity
					{
						m_Entity = intersectPos.m_CoursePos.m_Entity,
						m_SplitPosition = intersectPos.m_CoursePos.m_SplitPosition
					};
					intersectPos.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta);
					intersectPos.m_CoursePos.m_Entity = Entity.Null;
					intersectPos.m_CoursePos.m_SplitPosition = 0f;
					intersectPos.m_AuxIndex = intersectionAuxIndex++;
					CollectionUtils.Insert<IntersectPos>(target, num, intersectPos);
					reference = ref target.ElementAt(num++);
				}
			}
		}

		private void MergePositions(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, NetData prefabNetData, NetGeometryData prefabGeometryData, ref CourseHeightData courseHeightData, NativeList<IntersectPos> source, NativeList<IntersectPos> target, float courseHeightOffset, bool canAdjustHeight, bool canConnectWaterway, bool canBeSplitted, bool canSplitOwnedEdges, bool hasSubNets)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0880: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05be: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			PrefabRef prefabRef = default(PrefabRef);
			NetData netData = default(NetData);
			PrefabRef prefabRef2 = default(PrefabRef);
			NetData netData2 = default(NetData);
			Bounds1 val = default(Bounds1);
			while (num < source.Length)
			{
				IntersectPos intersectPos = source[num++];
				bool flag = canAdjustHeight;
				bool flag2 = !hasSubNets;
				if (canConnectWaterway && m_PrefabRefData.TryGetComponent(intersectPos.m_CoursePos.m_Entity, ref prefabRef) && m_PrefabNetData.TryGetComponent(prefabRef.m_Prefab, ref netData))
				{
					flag &= (netData.m_RequiredLayers & Layer.Waterway) == 0;
					flag2 = (netData.m_RequiredLayers & Layer.Waterway) != 0;
				}
				courseHeightData.GetHeightRange(intersectPos, prefabGeometryData, flag2, out var minBounds, out var maxBounds, out var elevated);
				minBounds += courseHeightOffset;
				maxBounds += courseHeightOffset;
				if (IsUnder(courseData, minBounds, maxBounds, intersectPos, courseHeightOffset, flag))
				{
					if (flag)
					{
						float num3 = courseHeightOffset + math.select(prefabGeometryData.m_DefaultHeightRange.max, prefabGeometryData.m_ElevatedHeightRange.max, flag2 && elevated) + 0.5f;
						courseHeightData.ApplyLimitRange(intersectPos, prefabGeometryData, new Bounds1(-1000000f, intersectPos.m_EdgeHeightRangeMin.min - num3), new Bounds1(-1000000f, intersectPos.m_EdgeHeightRangeMax.min - num3));
					}
					continue;
				}
				bool flag3 = !IsOver(courseData, minBounds, maxBounds, intersectPos, courseHeightOffset, flag);
				bool ignoreHeight = false;
				bool flag4 = flag3 && CanConnect(creationDefinition, ownerDefinition, prefabNetData, intersectPos.m_IsNode, canSplitOwnedEdges, intersectPos.m_CoursePos.m_Entity, out ignoreHeight);
				int num4 = num;
				num2 += math.select(0, 1, intersectPos.m_IsStartEnd);
				while (num < source.Length)
				{
					IntersectPos intersectPos2 = source[num];
					bool flag5 = canAdjustHeight;
					bool flag6 = !hasSubNets;
					if (canConnectWaterway && m_PrefabRefData.TryGetComponent(intersectPos2.m_CoursePos.m_Entity, ref prefabRef2) && m_PrefabNetData.TryGetComponent(prefabRef2.m_Prefab, ref netData2))
					{
						flag5 &= (netData2.m_RequiredLayers & Layer.Waterway) == 0;
						flag6 = (netData2.m_RequiredLayers & Layer.Waterway) != 0;
					}
					courseHeightData.GetHeightRange(intersectPos2, prefabGeometryData, flag6, out var minBounds2, out var maxBounds2, out elevated);
					minBounds2 += courseHeightOffset;
					maxBounds2 += courseHeightOffset;
					bool flag7 = flag4;
					if (IsUnder(courseData, minBounds2, maxBounds2, intersectPos2, courseHeightOffset, flag5))
					{
						if (flag5)
						{
							float num5 = courseHeightOffset + math.select(prefabGeometryData.m_DefaultHeightRange.max, prefabGeometryData.m_ElevatedHeightRange.max, flag6 && elevated) + 0.5f;
							courseHeightData.ApplyLimitRange(intersectPos2, prefabGeometryData, new Bounds1(-1000000f, intersectPos2.m_EdgeHeightRangeMin.min - num5), new Bounds1(-1000000f, intersectPos2.m_EdgeHeightRangeMax.min - num5));
						}
						if (num > num4)
						{
							source.RemoveAt(num);
							continue;
						}
						num++;
						num4++;
						continue;
					}
					if ((intersectPos.m_IsStartEnd && intersectPos2.m_IsStartEnd && (creationDefinition.m_Flags & CreationFlags.SubElevation) == 0 && (intersectPos.m_CoursePos.m_Flags & intersectPos2.m_CoursePos.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) || !MathUtils.Intersect(intersectPos2.m_CourseIntersection, intersectPos.m_CourseIntersection, ref val))
					{
						break;
					}
					if (intersectPos2.m_CoursePos.m_Entity != intersectPos.m_CoursePos.m_Entity)
					{
						bool flag8 = !IsOver(courseData, minBounds2, maxBounds2, intersectPos2, courseHeightOffset, flag5);
						if (flag3 || flag8)
						{
							bool ignoreHeight2;
							bool flag9 = flag8 && CanConnect(creationDefinition, ownerDefinition, prefabNetData, intersectPos.m_IsNode | intersectPos2.m_IsNode, canSplitOwnedEdges, intersectPos2.m_CoursePos.m_Entity, out ignoreHeight2);
							if (!flag4 && flag9 && intersectPos2.m_IsNode && !intersectPos.m_IsNode)
							{
								flag7 = flag3 && CanConnect(creationDefinition, ownerDefinition, prefabNetData, isNode: true, canSplitOwnedEdges, intersectPos.m_CoursePos.m_Entity, out ignoreHeight2);
							}
							if (flag7 != flag9)
							{
								num++;
								continue;
							}
							if (flag7 && val.min > 0f && val.max < 1f)
							{
								num++;
								continue;
							}
						}
					}
					flag4 = flag7;
					if (intersectPos.m_IsNode && !intersectPos2.m_IsNode)
					{
						intersectPos2.m_CoursePos.m_Entity = intersectPos.m_CoursePos.m_Entity;
						intersectPos2.m_IsNode = true;
					}
					else if (!intersectPos.m_IsNode && intersectPos2.m_IsNode)
					{
						intersectPos.m_CoursePos.m_Entity = intersectPos2.m_CoursePos.m_Entity;
						intersectPos.m_IsNode = true;
					}
					if (intersectPos2.m_CoursePos.m_Entity == Entity.Null)
					{
						intersectPos2.m_CoursePos.m_Entity = intersectPos.m_CoursePos.m_Entity;
						intersectPos2.m_CoursePos.m_SplitPosition = intersectPos.m_CoursePos.m_SplitPosition;
					}
					else if (intersectPos.m_CoursePos.m_Entity == Entity.Null)
					{
						intersectPos.m_CoursePos.m_Entity = intersectPos2.m_CoursePos.m_Entity;
						intersectPos.m_CoursePos.m_SplitPosition = intersectPos2.m_CoursePos.m_SplitPosition;
					}
					if (intersectPos2.m_CourseIntersection.min < intersectPos.m_CourseIntersection.min)
					{
						intersectPos.m_CourseIntersection.min = intersectPos2.m_CourseIntersection.min;
						intersectPos.m_IntersectionHeightMin = intersectPos2.m_IntersectionHeightMin;
						intersectPos.m_EdgeHeightRangeMin = intersectPos2.m_EdgeHeightRangeMin;
					}
					else if (intersectPos2.m_CourseIntersection.min == intersectPos.m_CourseIntersection.min)
					{
						ref Bounds1 reference = ref intersectPos.m_IntersectionHeightMin;
						reference |= intersectPos2.m_IntersectionHeightMin;
						ref Bounds1 reference2 = ref intersectPos.m_EdgeHeightRangeMin;
						reference2 |= intersectPos2.m_EdgeHeightRangeMin;
					}
					if (intersectPos2.m_CourseIntersection.max > intersectPos.m_CourseIntersection.max)
					{
						intersectPos.m_CourseIntersection.max = intersectPos2.m_CourseIntersection.max;
						intersectPos.m_IntersectionHeightMax = intersectPos2.m_IntersectionHeightMax;
						intersectPos.m_EdgeHeightRangeMax = intersectPos2.m_EdgeHeightRangeMax;
					}
					else if (intersectPos2.m_CourseIntersection.max == intersectPos.m_CourseIntersection.max)
					{
						ref Bounds1 reference3 = ref intersectPos.m_IntersectionHeightMax;
						reference3 |= intersectPos2.m_IntersectionHeightMax;
						ref Bounds1 reference4 = ref intersectPos.m_EdgeHeightRangeMax;
						reference4 |= intersectPos2.m_EdgeHeightRangeMax;
					}
					num2 += math.select(0, 1, intersectPos2.m_IsStartEnd);
					intersectPos.m_IsStartEnd |= intersectPos2.m_IsStartEnd;
					ref Bounds1 reference5 = ref intersectPos.m_EdgeIntersection;
					reference5 |= intersectPos2.m_EdgeIntersection;
					intersectPos.m_IsTunnel &= intersectPos2.m_IsTunnel;
					CoursePosFlags flags = intersectPos.m_CoursePos.m_Flags;
					CoursePosFlags flags2 = intersectPos2.m_CoursePos.m_Flags;
					intersectPos.m_CoursePos.m_Flags |= flags2 & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast);
					intersectPos.m_CoursePos.m_Flags &= (CoursePosFlags)((uint)flags2 | 0xFFFFFF7Fu);
					intersectPos2.m_CoursePos.m_Flags |= flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast);
					intersectPos2.m_CoursePos.m_Flags &= (CoursePosFlags)((uint)flags | 0xFFFFFF7Fu);
					if (intersectPos2.m_Priority < intersectPos.m_Priority)
					{
						intersectPos.m_CoursePos = intersectPos2.m_CoursePos;
						intersectPos.m_Priority = intersectPos2.m_Priority;
					}
					if (num > num4)
					{
						source.RemoveAt(num);
						continue;
					}
					num++;
					num4++;
				}
				if (flag4)
				{
					if (!ignoreHeight)
					{
						Bounds1 val2 = intersectPos.m_IntersectionHeightMin;
						Bounds1 val3 = intersectPos.m_IntersectionHeightMax;
						if (val2.max < val2.min)
						{
							((Bounds1)(ref val2))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
						}
						if (val3.max < val3.min)
						{
							((Bounds1)(ref val3))._002Ector(float2.op_Implicit(intersectPos.m_CoursePos.m_Position.y));
						}
						courseHeightData.ApplyLimitRange(intersectPos, prefabGeometryData, val2 - courseHeightOffset, val3 - courseHeightOffset, shrink: true);
					}
				}
				else
				{
					intersectPos.m_CoursePos.m_Entity = Entity.Null;
					elevated |= !intersectPos.m_IsTunnel;
					if (flag)
					{
						float num6 = courseHeightOffset + math.select(prefabGeometryData.m_DefaultHeightRange.min, prefabGeometryData.m_ElevatedHeightRange.min, flag2 && elevated) - 0.5f;
						bool forceElevated = !intersectPos.m_IsTunnel && (prefabGeometryData.m_Flags & Game.Net.GeometryFlags.ExclusiveGround) != 0;
						courseHeightData.ApplyLimitRange(intersectPos, prefabGeometryData, new Bounds1(intersectPos.m_EdgeHeightRangeMin.max - num6, 1000000f), new Bounds1(intersectPos.m_EdgeHeightRangeMax.max - num6, 1000000f), shrink: false, forceElevated);
					}
					if ((intersectPos.m_IsTunnel || intersectPos.m_AuxIndex != -1) && !intersectPos.m_IsStartEnd)
					{
						num = num4;
						continue;
					}
					intersectPos.m_IsOptional = !intersectPos.m_IsStartEnd;
				}
				if (intersectPos.m_IsStartEnd)
				{
					target.Add(ref intersectPos);
				}
				else if (canBeSplitted)
				{
					if (num2 >= 2)
					{
						if (target.Length >= 2)
						{
							CollectionUtils.Insert<IntersectPos>(target, target.Length - 1, intersectPos);
						}
					}
					else
					{
						target.Add(ref intersectPos);
					}
				}
				num = num4;
			}
		}

		private bool CanConnect(CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, NetData prefabNetData1, bool isNode, bool canSplitOwnedEdges, Entity entity2, out bool ignoreHeight)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			ignoreHeight = false;
			if (entity2 == Entity.Null)
			{
				return true;
			}
			PrefabRef prefabRef = m_PrefabRefData[entity2];
			NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
			bool flag = !m_EditorMode && m_PrefabServiceUpgradeData.HasComponent(creationDefinition.m_Prefab);
			bool flag2 = !m_EditorMode && m_PrefabServiceUpgradeData.HasComponent(prefabRef.m_Prefab);
			Owner owner = default(Owner);
			if (((!canSplitOwnedEdges && !isNode) || ((prefabNetData1.m_RequiredLayers & (Layer.MarkerPathway | Layer.MarkerTaxiway)) == 0 && (netData.m_RequiredLayers & (Layer.MarkerPathway | Layer.MarkerTaxiway)) != Layer.None && (creationDefinition.m_Flags & CreationFlags.SubElevation) != 0)) && m_OwnerData.TryGetComponent(entity2, ref owner))
			{
				Transform transform = default(Transform);
				PrefabRef prefabRef2 = default(PrefabRef);
				if (creationDefinition.m_Owner != owner.m_Owner && (!m_TransformData.TryGetComponent(owner.m_Owner, ref transform) || !m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef2) || !((float3)(ref ownerDefinition.m_Position)).Equals(transform.m_Position) || !((quaternion)(ref ownerDefinition.m_Rotation)).Equals(transform.m_Rotation) || ownerDefinition.m_Prefab != prefabRef2.m_Prefab))
				{
					return false;
				}
				if (!canSplitOwnedEdges && !isNode && flag && !m_ServiceUpgradeData.HasComponent(entity2))
				{
					return false;
				}
			}
			if (!isNode && !m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab))
			{
				return false;
			}
			if (!NetUtils.CanConnect(prefabNetData1, netData))
			{
				return false;
			}
			if (flag || flag2)
			{
				Entity val = entity2;
				while (m_OwnerData.TryGetComponent(val, ref owner))
				{
					val = owner.m_Owner;
				}
				Transform transform2 = default(Transform);
				PrefabRef prefabRef3 = default(PrefabRef);
				if (creationDefinition.m_Owner != val && (!m_TransformData.TryGetComponent(val, ref transform2) || !m_PrefabRefData.TryGetComponent(val, ref prefabRef3) || !((float3)(ref ownerDefinition.m_Position)).Equals(transform2.m_Position) || !((quaternion)(ref ownerDefinition.m_Rotation)).Equals(transform2.m_Rotation) || ownerDefinition.m_Prefab != prefabRef3.m_Prefab))
				{
					return false;
				}
			}
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (((prefabNetData1.m_RequiredLayers ^ netData.m_RequiredLayers) & Layer.Waterway) != Layer.None)
			{
				ignoreHeight = true;
			}
			else if (((prefabNetData1.m_RequiredLayers ^ netData.m_RequiredLayers) & Layer.TrainTrack) != Layer.None && m_ElevationData.TryGetComponent(entity2, ref elevation) && m_PrefabGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData) && math.any(math.abs(elevation.m_Elevation) >= netGeometryData.m_ElevationLimit))
			{
				return false;
			}
			return true;
		}

		private void GetElevationRanges(NetCourse courseData, NetGeometryData prefabGeometryData, ref CourseHeightData courseHeightData, Bounds1 courseDelta, NativeList<ElevationSegment> leftSegments, NativeList<ElevationSegment> rightSegments)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, courseDelta);
			float num2 = prefabGeometryData.m_DefaultWidth * 0.5f;
			int num3 = Mathf.RoundToInt(num / 4f);
			leftSegments.Clear();
			rightSegments.Clear();
			if (num3 < 1)
			{
				return;
			}
			float3 val = MathUtils.Position(courseData.m_Curve, courseDelta.min);
			float2 val2 = float2.op_Implicit(courseDelta.min);
			float2 val3 = float2.op_Implicit(0f);
			int2 val4 = int2.op_Implicit(0);
			bool2 val5 = bool2.op_Implicit(true);
			float num4 = courseDelta.min;
			float num5 = 0f;
			float num6 = 1f / (float)num3;
			int2 val9 = default(int2);
			ElevationSegment elevationSegment;
			for (int i = 1; i <= num3; i++)
			{
				float num7 = math.lerp(courseDelta.min, courseDelta.max, (float)i * num6);
				float num8 = math.lerp(courseDelta.min, courseDelta.max, ((float)i - 0.5f) * num6);
				float3 val6 = MathUtils.Position(courseData.m_Curve, num7);
				float3 val7 = MathUtils.Position(courseData.m_Curve, num8);
				val7.y = courseHeightData.SampleHeight(num8, out var forceElevated);
				float3 val8 = default(float3);
				((float3)(ref val8)).xz = math.normalizesafe(MathUtils.Right(MathUtils.Tangent(((Bezier4x3)(ref courseData.m_Curve)).xz, num8)), default(float2)) * num2;
				bool flag;
				if (forceElevated)
				{
					val9 = int2.op_Implicit(2);
					flag = false;
				}
				else
				{
					val9.x = GetElevationType(prefabGeometryData, val7 - val8);
					val9.y = GetElevationType(prefabGeometryData, val7 + val8);
					flag = true;
				}
				if (i != 1)
				{
					if (val9.x != val4.x)
					{
						elevationSegment = new ElevationSegment
						{
							m_CourseRange = new Bounds1(val2.x, num4),
							m_DistanceOffset = new Bounds1(val3.x, num5),
							m_ElevationType = int2.op_Implicit(val4.x),
							m_CanRemove = val5.x
						};
						leftSegments.Add(ref elevationSegment);
						val5.x = true;
						val2.x = num4;
						val3.x = num5;
					}
					if (val9.y != val4.y)
					{
						elevationSegment = new ElevationSegment
						{
							m_CourseRange = new Bounds1(val2.y, num4),
							m_DistanceOffset = new Bounds1(val3.y, num5),
							m_ElevationType = int2.op_Implicit(val4.y),
							m_CanRemove = val5.y
						};
						rightSegments.Add(ref elevationSegment);
						val5.y = true;
						val2.y = num4;
						val3.y = num5;
					}
				}
				num4 = num7;
				num5 += math.distance(((float3)(ref val)).xz, ((float3)(ref val6)).xz);
				val4 = val9;
				val5 &= flag;
				val = val6;
			}
			elevationSegment = new ElevationSegment
			{
				m_CourseRange = new Bounds1(val2.x, num4),
				m_DistanceOffset = new Bounds1(val3.x, num5),
				m_ElevationType = int2.op_Implicit(val4.x),
				m_CanRemove = val5.x
			};
			leftSegments.Add(ref elevationSegment);
			elevationSegment = new ElevationSegment
			{
				m_CourseRange = new Bounds1(val2.y, num4),
				m_DistanceOffset = new Bounds1(val3.y, num5),
				m_ElevationType = int2.op_Implicit(val4.y),
				m_CanRemove = val5.y
			};
			rightSegments.Add(ref elevationSegment);
		}

		private int GetElevationType(NetGeometryData prefabGeometryData, float3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			float num = position.y - TerrainUtils.SampleHeight(ref m_TerrainHeightData, position);
			float3 val = default(float3);
			((float3)(ref val))._002Ector(prefabGeometryData.m_ElevationLimit * 0.5f, prefabGeometryData.m_ElevationLimit, prefabGeometryData.m_ElevationLimit * 3f);
			int2 val2 = math.select(int2.op_Implicit(0), new int2(1), num >= ((float3)(ref val)).xy);
			int3 val3 = math.select(int3.op_Implicit(0), new int3(-1), num <= -val);
			return math.csum(val2) + math.csum(val3);
		}

		private void ExpandMajorElevationSegments(NetCourse courseData, NativeList<ElevationSegment> elevationSegments)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			float num = 8f;
			for (int i = 0; i < elevationSegments.Length; i++)
			{
				ElevationSegment elevationSegment = elevationSegments[i];
				if (!math.all(elevationSegment.m_ElevationType == 1) && !math.all(elevationSegment.m_ElevationType == -1))
				{
					continue;
				}
				float num2 = math.min(num, MathUtils.Size(elevationSegment.m_DistanceOffset) * 0.5f);
				int num3 = math.select(2, -2, elevationSegment.m_ElevationType.x == -1);
				if (i > 0)
				{
					ElevationSegment elevationSegment2 = elevationSegments[i - 1];
					if (math.all(elevationSegment2.m_ElevationType == num3))
					{
						Bounds1 val = elevationSegment.m_CourseRange;
						MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val, num2);
						elevationSegment.m_CourseRange.min = val.max;
						elevationSegment.m_DistanceOffset.min += num2;
						elevationSegment2.m_CourseRange.max = val.max;
						elevationSegment2.m_DistanceOffset.max += num2;
						elevationSegments[i - 1] = elevationSegment2;
					}
				}
				if (i < elevationSegments.Length - 1)
				{
					ElevationSegment elevationSegment3 = elevationSegments[i + 1];
					if (math.all(elevationSegment3.m_ElevationType == num3))
					{
						Bounds1 val2 = elevationSegment.m_CourseRange;
						MathUtils.ClampLengthInverse(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val2, num2);
						elevationSegment.m_CourseRange.max = val2.min;
						elevationSegment.m_DistanceOffset.max -= num2;
						elevationSegment3.m_CourseRange.min = val2.min;
						elevationSegment3.m_DistanceOffset.min -= num2;
						elevationSegments[i + 1] = elevationSegment3;
					}
				}
				elevationSegment.m_ElevationType = int2.op_Implicit(0);
				elevationSegments[i] = elevationSegment;
			}
		}

		private void MergeSimilarElevationSegments(NativeList<ElevationSegment> elevationSegments)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			int num = 0;
			while (i < elevationSegments.Length)
			{
				ElevationSegment elevationSegment = elevationSegments[i++];
				for (; i < elevationSegments.Length; i++)
				{
					ElevationSegment elevationSegment2 = elevationSegments[i];
					if (math.any(elevationSegment2.m_ElevationType != elevationSegment.m_ElevationType))
					{
						break;
					}
					elevationSegment.m_CourseRange.max = elevationSegment2.m_CourseRange.max;
					elevationSegment.m_DistanceOffset.max = elevationSegment2.m_DistanceOffset.max;
					elevationSegment.m_CanRemove &= elevationSegment2.m_CanRemove;
				}
				elevationSegments[num++] = elevationSegment;
			}
			if (num < i)
			{
				elevationSegments.RemoveRange(num, i - num);
			}
		}

		private void RemoveShortElevationSegments(NetGeometryData prefabGeometryData, NativeList<ElevationSegment> elevationSegments)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			float defaultWidth = prefabGeometryData.m_DefaultWidth;
			for (int i = 0; i < elevationSegments.Length; i++)
			{
				ElevationSegment elevationSegment = elevationSegments[i];
				float num = MathUtils.Size(elevationSegment.m_DistanceOffset);
				if (!(num < defaultWidth) || !elevationSegment.m_CanRemove)
				{
					continue;
				}
				int num2 = i--;
				for (int j = num2 + 1; j < elevationSegments.Length; j++)
				{
					ElevationSegment elevationSegment2 = elevationSegments[j];
					float num3 = MathUtils.Size(elevationSegment2.m_DistanceOffset);
					if (num3 >= num || !elevationSegment2.m_CanRemove)
					{
						break;
					}
					num = num3;
					num2 = j;
				}
				elevationSegment = elevationSegments[num2];
				ElevationSegment elevationSegment3 = new ElevationSegment
				{
					m_ElevationType = int2.op_Implicit(-1000000)
				};
				if (num2 > 0)
				{
					elevationSegment3 = elevationSegments[num2 - 1];
					elevationSegment3.m_CourseRange.max = MathUtils.Center(elevationSegment.m_CourseRange);
					elevationSegment3.m_DistanceOffset.max = MathUtils.Center(elevationSegment.m_DistanceOffset);
					elevationSegments[num2 - 1] = elevationSegment3;
				}
				if (num2 < elevationSegments.Length - 1)
				{
					ElevationSegment elevationSegment4 = elevationSegments[num2 + 1];
					if (math.all(elevationSegment4.m_ElevationType == elevationSegment3.m_ElevationType))
					{
						elevationSegment3.m_CourseRange.max = elevationSegment4.m_CourseRange.max;
						elevationSegment3.m_DistanceOffset.max = elevationSegment4.m_DistanceOffset.max;
						elevationSegments[num2 - 1] = elevationSegment3;
						elevationSegments.RemoveAt(num2 + 1);
					}
					else
					{
						elevationSegment4.m_CourseRange.min = MathUtils.Center(elevationSegment.m_CourseRange);
						elevationSegment4.m_DistanceOffset.min = MathUtils.Center(elevationSegment.m_DistanceOffset);
						elevationSegments[num2 + 1] = elevationSegment4;
					}
				}
				elevationSegments.RemoveAt(num2);
			}
		}

		private void MergeSideElevationSegments(NativeList<ElevationSegment> leftSegments, NativeList<ElevationSegment> rightSegments)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			int length = rightSegments.Length;
			rightSegments.AddRange(leftSegments.AsArray());
			int length2 = rightSegments.Length;
			int num = length;
			int num2 = 0;
			leftSegments.Clear();
			ElevationSegment elevationSegment = new ElevationSegment
			{
				m_ElevationType = int2.op_Implicit(-1000000)
			};
			bool2 val = bool2.op_Implicit(true);
			while (true)
			{
				ElevationSegment elevationSegment2 = new ElevationSegment
				{
					m_ElevationType = int2.op_Implicit(-1000000)
				};
				ElevationSegment elevationSegment3 = new ElevationSegment
				{
					m_ElevationType = int2.op_Implicit(-1000000)
				};
				if (num < length2 && num2 < length)
				{
					ElevationSegment elevationSegment4 = rightSegments[num];
					ElevationSegment elevationSegment5 = rightSegments[num2];
					if (elevationSegment2.m_CourseRange.min <= elevationSegment3.m_CourseRange.min)
					{
						elevationSegment2 = elevationSegment4;
						num++;
					}
					else
					{
						elevationSegment3 = elevationSegment5;
						num2++;
					}
				}
				else if (num < length2)
				{
					elevationSegment2 = rightSegments[num++];
				}
				else
				{
					if (num2 >= length)
					{
						break;
					}
					elevationSegment3 = rightSegments[num2++];
				}
				if (elevationSegment2.m_ElevationType.x != -1000000)
				{
					if (math.all(elevationSegment.m_ElevationType != -1000000))
					{
						elevationSegment.m_CourseRange.max = elevationSegment2.m_CourseRange.min;
						elevationSegment.m_DistanceOffset.max = elevationSegment2.m_DistanceOffset.min;
						leftSegments.Add(ref elevationSegment);
						val.x = true;
					}
					elevationSegment.m_CourseRange = elevationSegment2.m_CourseRange;
					elevationSegment.m_DistanceOffset = elevationSegment2.m_DistanceOffset;
					elevationSegment.m_ElevationType.x = elevationSegment2.m_ElevationType.x;
					val &= elevationSegment2.m_CanRemove;
				}
				if (elevationSegment3.m_ElevationType.y != -1000000)
				{
					if (math.all(elevationSegment.m_ElevationType != -1000000))
					{
						elevationSegment.m_CourseRange.max = elevationSegment3.m_CourseRange.min;
						elevationSegment.m_DistanceOffset.max = elevationSegment3.m_DistanceOffset.min;
						leftSegments.Add(ref elevationSegment);
						val.y = true;
					}
					elevationSegment.m_CourseRange = elevationSegment3.m_CourseRange;
					elevationSegment.m_DistanceOffset = elevationSegment3.m_DistanceOffset;
					elevationSegment.m_ElevationType.y = elevationSegment3.m_ElevationType.y;
					val &= elevationSegment3.m_CanRemove;
				}
			}
			if (math.all(elevationSegment.m_ElevationType != -1000000))
			{
				leftSegments.Add(ref elevationSegment);
			}
		}

		private void CheckHeightRange(NetCourse courseData, NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, ref CourseHeightData courseHeightData, bool sampleTerrain, NativeList<IntersectPos> source, NativeList<IntersectPos> target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0701: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0965: Unknown result type (might be due to invalid IL or missing references)
			//IL_0976: Unknown result type (might be due to invalid IL or missing references)
			//IL_097b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0988: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f86: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_101b: Unknown result type (might be due to invalid IL or missing references)
			//IL_102c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1031: Unknown result type (might be due to invalid IL or missing references)
			//IL_103e: Unknown result type (might be due to invalid IL or missing references)
			//IL_104f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1054: Unknown result type (might be due to invalid IL or missing references)
			//IL_1059: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			NativeList<ElevationSegment> val = default(NativeList<ElevationSegment>);
			val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<ElevationSegment> val2 = default(NativeList<ElevationSegment>);
			val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			Bounds1 courseDelta = default(Bounds1);
			Bounds1 val3 = default(Bounds1);
			Bounds1 val4 = default(Bounds1);
			Bounds1 val5 = default(Bounds1);
			Bounds1 val6 = default(Bounds1);
			Bounds1 val7 = default(Bounds1);
			Bounds1 val8 = default(Bounds1);
			Bounds1 val9 = default(Bounds1);
			for (int i = 0; i < source.Length; i++)
			{
				IntersectPos intersectPos = source[i];
				if (sampleTerrain && i != 0 && (prefabGeometryData.m_Flags & Game.Net.GeometryFlags.RequireElevated) == 0 && (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.ShoreLine) == 0)
				{
					IntersectPos intersectPos2 = source[i - 1];
					((Bounds1)(ref courseDelta))._002Ector(intersectPos2.m_CourseIntersection.max, intersectPos.m_CourseIntersection.min);
					GetElevationRanges(courseData, prefabGeometryData, ref courseHeightData, courseDelta, val, val2);
					ExpandMajorElevationSegments(courseData, val);
					ExpandMajorElevationSegments(courseData, val2);
					MergeSimilarElevationSegments(val);
					MergeSimilarElevationSegments(val2);
					RemoveShortElevationSegments(prefabGeometryData, val);
					RemoveShortElevationSegments(prefabGeometryData, val2);
					MergeSideElevationSegments(val, val2);
					RemoveShortElevationSegments(prefabGeometryData, val);
					for (int j = 1; j < val.Length; j++)
					{
						ElevationSegment elevationSegment = val[j - 1];
						ElevationSegment elevationSegment2 = val[j];
						IntersectPos intersectPos3 = new IntersectPos
						{
							m_CoursePos = 
							{
								m_CourseDelta = elevationSegment2.m_CourseRange.min,
								m_Flags = (intersectPos.m_CoursePos.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.IsGrid))
							}
						};
						intersectPos3.m_CoursePos.m_Flags |= CoursePosFlags.FreeHeight;
						intersectPos3.m_CoursePos.m_ParentMesh = math.select(intersectPos.m_CoursePos.m_ParentMesh, -1, intersectPos.m_CoursePos.m_ParentMesh != intersectPos2.m_CoursePos.m_ParentMesh);
						intersectPos3.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos3.m_CoursePos.m_CourseDelta);
						intersectPos3.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos3.m_CoursePos.m_CourseDelta));
						intersectPos3.m_AuxIndex = -1;
						if (elevationSegment2.m_ElevationType.x != elevationSegment.m_ElevationType.x)
						{
							intersectPos3.m_CoursePos.m_Flags |= CoursePosFlags.LeftTransition;
						}
						if (elevationSegment2.m_ElevationType.y != elevationSegment.m_ElevationType.y)
						{
							intersectPos3.m_CoursePos.m_Flags |= CoursePosFlags.RightTransition;
						}
						if (flag)
						{
							int num = target.Length;
							for (int num2 = target.Length - 1; num2 >= 0; num2--)
							{
								IntersectPos intersectPos4 = target[num2];
								if (!intersectPos4.m_IsOptional || intersectPos4.m_CoursePos.m_CourseDelta <= intersectPos3.m_CoursePos.m_CourseDelta)
								{
									break;
								}
								((Bounds1)(ref val3))._002Ector(intersectPos3.m_CoursePos.m_CourseDelta, intersectPos4.m_CoursePos.m_CourseDelta);
								float num3 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val3);
								intersectPos4.m_CanMove.max = math.max(intersectPos4.m_CanMove.max, prefabGeometryData.m_ElevatedLength * 0.95f - num3);
								target[num2] = intersectPos4;
								num = num2;
							}
							CollectionUtils.Insert<IntersectPos>(target, num, intersectPos3);
						}
						else
						{
							target.Add(ref intersectPos3);
						}
					}
				}
				if (!intersectPos.m_IsOptional && !m_FixedData.HasComponent(intersectPos.m_CoursePos.m_Entity))
				{
					if (flag && !intersectPos.m_IsStartEnd)
					{
						int num4 = target.Length;
						for (int num5 = target.Length - 1; num5 >= 0; num5--)
						{
							IntersectPos intersectPos5 = target[num5];
							if (!intersectPos5.m_IsOptional || intersectPos5.m_CoursePos.m_CourseDelta <= intersectPos.m_CoursePos.m_CourseDelta)
							{
								break;
							}
							((Bounds1)(ref val4))._002Ector(intersectPos.m_CoursePos.m_CourseDelta, intersectPos5.m_CoursePos.m_CourseDelta);
							float num6 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val4);
							intersectPos5.m_CanMove.max = math.max(intersectPos5.m_CanMove.max, prefabGeometryData.m_ElevatedLength * 0.95f - num6);
							target[num5] = intersectPos5;
							num4 = num5;
						}
						CollectionUtils.Insert<IntersectPos>(target, num4, intersectPos);
					}
					else
					{
						target.Add(ref intersectPos);
					}
					continue;
				}
				float num7 = prefabGeometryData.m_DefaultWidth * 0.5f;
				if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.MiddlePillars) != 0)
				{
					float num8 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectPos.m_CourseIntersection);
					((Bounds1)(ref val5))._002Ector(intersectPos.m_CourseIntersection.min, intersectPos.m_CourseIntersection.max);
					MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val5, num8 * 0.5f);
					num8 += num7 * 2f;
					float num9 = (prefabGeometryData.m_ElevatedLength * 0.95f - num8) * 0.5f;
					num9 = math.max(0f, num9);
					intersectPos.m_CoursePos.m_Entity = Entity.Null;
					intersectPos.m_CoursePos.m_CourseDelta = math.min(1f, val5.max);
					intersectPos.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta);
					intersectPos.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta));
					intersectPos.m_CanMove = new Bounds1(0f - num9, num9);
					intersectPos.m_IsOptional = true;
					intersectPos.m_AuxIndex = -1;
					float num10 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(0f, intersectPos.m_CoursePos.m_CourseDelta));
					float num11 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(intersectPos.m_CoursePos.m_CourseDelta, 1f));
					if (!(math.min(num10, num11) > num9))
					{
						continue;
					}
					int num12 = target.Length;
					for (int num13 = target.Length - 1; num13 >= 0; num13--)
					{
						IntersectPos intersectPos6 = target[num13];
						if (intersectPos6.m_IsOptional || intersectPos6.m_IsStartEnd || intersectPos6.m_CoursePos.m_CourseDelta <= intersectPos.m_CoursePos.m_CourseDelta)
						{
							break;
						}
						num12 = num13;
					}
					if (num12 < target.Length)
					{
						((Bounds1)(ref val6))._002Ector(intersectPos.m_CoursePos.m_CourseDelta, target[num12].m_CoursePos.m_CourseDelta);
						float num14 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val6);
						intersectPos.m_CanMove.min = math.min(intersectPos.m_CanMove.min, num14 - prefabGeometryData.m_ElevatedLength * 0.95f);
					}
					CollectionUtils.Insert<IntersectPos>(target, num12, intersectPos);
					flag = true;
					continue;
				}
				((Bounds1)(ref val7))._002Ector(0f, intersectPos.m_CourseIntersection.min);
				((Bounds1)(ref val8))._002Ector(intersectPos.m_CourseIntersection.max, 1f);
				float num15 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, intersectPos.m_CourseIntersection) + num7 * 2f;
				float num16 = (prefabGeometryData.m_ElevatedLength * 0.95f - num15) * 0.5f;
				num7 = math.max(0f, num7 + math.min(0f, num16));
				num16 = math.max(0f, num16);
				MathUtils.ClampLengthInverse(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val7, num7);
				MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val8, num7);
				intersectPos.m_CoursePos.m_Entity = Entity.Null;
				intersectPos.m_CoursePos.m_CourseDelta = math.max(0f, val7.min);
				intersectPos.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta);
				intersectPos.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta));
				intersectPos.m_CanMove = new Bounds1(0f - num16, 0f);
				intersectPos.m_IsOptional = true;
				intersectPos.m_AuxIndex = -1;
				if (MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(intersectPos.m_CoursePos.m_CourseDelta, 1f)) > num16)
				{
					int num17 = target.Length;
					for (int num18 = target.Length - 1; num18 >= 0; num18--)
					{
						IntersectPos intersectPos7 = target[num18];
						if (intersectPos7.m_IsOptional || intersectPos7.m_IsStartEnd || intersectPos7.m_CoursePos.m_CourseDelta <= intersectPos.m_CoursePos.m_CourseDelta)
						{
							break;
						}
						num17 = num18;
					}
					if (num17 < target.Length)
					{
						((Bounds1)(ref val9))._002Ector(intersectPos.m_CoursePos.m_CourseDelta, target[num17].m_CoursePos.m_CourseDelta);
						float num19 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val9);
						intersectPos.m_CanMove.min = math.min(intersectPos.m_CanMove.min, num19 - prefabGeometryData.m_ElevatedLength * 0.95f);
					}
					CollectionUtils.Insert<IntersectPos>(target, num17, intersectPos);
					flag = true;
				}
				intersectPos.m_CoursePos.m_Entity = Entity.Null;
				intersectPos.m_CoursePos.m_CourseDelta = math.min(1f, val8.max);
				intersectPos.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta);
				intersectPos.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos.m_CoursePos.m_CourseDelta));
				intersectPos.m_CanMove = new Bounds1(0f, num16);
				intersectPos.m_IsOptional = true;
				intersectPos.m_AuxIndex = -1;
				if (MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(0f, intersectPos.m_CoursePos.m_CourseDelta)) > num16)
				{
					target.Add(ref intersectPos);
					flag = true;
				}
			}
			val.Dispose();
			val2.Dispose();
			if (!flag)
			{
				return;
			}
			source.Clear();
			Bounds1 val10 = default(Bounds1);
			for (int k = 0; k < target.Length; k++)
			{
				IntersectPos intersectPos8 = target[k];
				if (intersectPos8.m_IsOptional)
				{
					for (; k + 1 < target.Length; k++)
					{
						IntersectPos intersectPos9 = target[k + 1];
						if (!intersectPos9.m_IsOptional)
						{
							break;
						}
						if (intersectPos9.m_CoursePos.m_CourseDelta <= intersectPos8.m_CoursePos.m_CourseDelta)
						{
							intersectPos8.m_CoursePos.m_CourseDelta = math.lerp(intersectPos8.m_CoursePos.m_CourseDelta, intersectPos9.m_CoursePos.m_CourseDelta, 0.5f);
							intersectPos8.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos8.m_CoursePos.m_CourseDelta);
							intersectPos8.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos8.m_CoursePos.m_CourseDelta));
							intersectPos8.m_CanMove = new Bounds1(0f, 0f);
							continue;
						}
						((Bounds1)(ref val10))._002Ector(intersectPos8.m_CoursePos.m_CourseDelta, intersectPos9.m_CoursePos.m_CourseDelta);
						float num20 = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val10);
						if (!(intersectPos8.m_CanMove.max - intersectPos9.m_CanMove.min >= num20))
						{
							break;
						}
						float num21 = math.min(intersectPos8.m_CanMove.max, math.max(num20 * 0.5f, num20 + intersectPos9.m_CanMove.min));
						MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val10, num21);
						intersectPos8.m_CoursePos.m_CourseDelta = val10.max;
						intersectPos8.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos8.m_CoursePos.m_CourseDelta);
						intersectPos8.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos8.m_CoursePos.m_CourseDelta));
						intersectPos8.m_CanMove.min = math.min(0f, math.max(intersectPos8.m_CanMove.min - num21, intersectPos9.m_CanMove.min + num20 - num21));
						intersectPos8.m_CanMove.max = math.max(0f, math.min(intersectPos9.m_CanMove.max + num20 - num21, intersectPos8.m_CanMove.max - num21));
					}
				}
				source.Add(ref intersectPos8);
			}
			target.Clear();
			Bounds1 val11 = default(Bounds1);
			for (int l = 1; l < source.Length; l++)
			{
				IntersectPos intersectPos10 = source[l - 1];
				IntersectPos intersectPos11 = source[l];
				((Bounds1)(ref val11))._002Ector(intersectPos10.m_CoursePos.m_CourseDelta, intersectPos11.m_CoursePos.m_CourseDelta);
				intersectPos10.m_Priority = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, val11);
				source[l - 1] = intersectPos10;
			}
			for (int m = 0; m < source.Length; m++)
			{
				if (FindOptionalRange(source, m, out var minIndex, out var maxIndex))
				{
					m = maxIndex;
					int bestIndex;
					while (FindBestIntersectionToRemove(prefabGeometryData, source, minIndex, maxIndex, out bestIndex))
					{
						IntersectPos intersectPos12 = source[bestIndex - 1];
						IntersectPos intersectPos13 = source[bestIndex];
						intersectPos12.m_Priority += intersectPos13.m_Priority;
						source[bestIndex - 1] = intersectPos12;
						if (bestIndex == minIndex)
						{
							intersectPos12 = target[target.Length - 1];
							intersectPos12.m_Priority += intersectPos13.m_Priority;
							target[target.Length - 1] = intersectPos12;
						}
						for (int n = bestIndex; n < maxIndex; n++)
						{
							source[n] = source[n + 1];
						}
						maxIndex--;
					}
					for (int num22 = minIndex; num22 <= maxIndex; num22++)
					{
						IntersectPos intersectPos14 = source[num22];
						target.Add(ref intersectPos14);
					}
				}
				else
				{
					IntersectPos intersectPos14 = source[m];
					target.Add(ref intersectPos14);
				}
			}
			Bounds1 val12 = default(Bounds1);
			Bounds1 val13 = default(Bounds1);
			for (int num23 = 1; num23 + 1 < target.Length; num23++)
			{
				IntersectPos intersectPos15 = target[num23];
				if (!intersectPos15.m_IsOptional)
				{
					continue;
				}
				IntersectPos intersectPos16 = target[num23 - 1];
				intersectPos15.m_CanMove.min = math.min(0f, math.max(intersectPos15.m_CanMove.min, intersectPos15.m_Priority - prefabGeometryData.m_ElevatedLength * 0.95f));
				intersectPos15.m_CanMove.max = math.max(0f, math.min(intersectPos15.m_CanMove.max, prefabGeometryData.m_ElevatedLength * 0.95f - intersectPos16.m_Priority));
				float num24 = MathUtils.Clamp((intersectPos15.m_Priority - intersectPos16.m_Priority) * 0.5f, intersectPos15.m_CanMove);
				if (num24 != 0f)
				{
					if (num24 > 0f)
					{
						((Bounds1)(ref val12))._002Ector(intersectPos15.m_CoursePos.m_CourseDelta, 1f);
						MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val12, num24);
						intersectPos15.m_CoursePos.m_CourseDelta = val12.max;
						intersectPos15.m_CanMove.max = math.max(0f, intersectPos15.m_CanMove.max - num24);
					}
					else
					{
						((Bounds1)(ref val13))._002Ector(0f, intersectPos15.m_CoursePos.m_CourseDelta);
						MathUtils.ClampLengthInverse(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val13, 0f - num24);
						intersectPos15.m_CoursePos.m_CourseDelta = val13.min;
						intersectPos15.m_CanMove.min = math.min(0f, intersectPos15.m_CanMove.min - num24);
					}
					intersectPos15.m_Priority -= num24;
					intersectPos15.m_CoursePos.m_Position = MathUtils.Position(courseData.m_Curve, intersectPos15.m_CoursePos.m_CourseDelta);
					intersectPos15.m_CoursePos.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, intersectPos15.m_CoursePos.m_CourseDelta));
					target[num23] = intersectPos15;
					intersectPos16.m_Priority += num24;
					target[num23 - 1] = intersectPos16;
				}
			}
		}

		private bool FindOptionalRange(NativeList<IntersectPos> source, int index, out int minIndex, out int maxIndex)
		{
			minIndex = index;
			maxIndex = index - 1;
			if (index == 0)
			{
				return false;
			}
			for (int i = minIndex; i + 1 < source.Length; i++)
			{
				if (source[i].m_IsOptional)
				{
					maxIndex = i;
					continue;
				}
				return maxIndex >= minIndex;
			}
			return maxIndex >= minIndex;
		}

		private bool FindBestIntersectionToRemove(NetGeometryData netGeometryData, NativeList<IntersectPos> source, int minIndex, int maxIndex, out int bestIndex)
		{
			bestIndex = minIndex;
			float num = netGeometryData.m_ElevatedLength;
			for (int i = minIndex; i <= maxIndex; i++)
			{
				IntersectPos intersectPos = source[i - 1];
				IntersectPos intersectPos2 = source[i];
				float num2 = intersectPos.m_Priority + intersectPos2.m_Priority;
				if (num2 < num)
				{
					bestIndex = i;
					num = num2;
				}
			}
			return num <= netGeometryData.m_ElevatedLength * 0.95f;
		}

		private void UpdateCourses(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NativeList<IntersectPos> intersectionList, NativeList<AuxIntersectionEntity> auxIntersectionEntities, ref CourseHeightData courseHeightData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			NetData netData = m_PrefabNetData[creationDefinition.m_Prefab];
			NetGeometryData netGeometryData = m_PrefabGeometryData[creationDefinition.m_Prefab];
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			m_PrefabPlaceableData.TryGetComponent(creationDefinition.m_Prefab, ref placeableNetData);
			DynamicBuffer<FixedNetElement> fixedNetElements = default(DynamicBuffer<FixedNetElement>);
			m_PrefabFixedNetElements.TryGetBuffer(creationDefinition.m_Prefab, ref fixedNetElements);
			DynamicBuffer<AuxiliaryNet> auxiliaryNets = default(DynamicBuffer<AuxiliaryNet>);
			m_PrefabAuxiliaryNets.TryGetBuffer(creationDefinition.m_Prefab, ref auxiliaryNets);
			int courseIndex = 0;
			if (intersectionList.Length != 0)
			{
				courseData.m_StartPosition = intersectionList[0].m_CoursePos;
				int2 val = default(int2);
				((int2)(ref val))._002Ector(intersectionList[0].m_AuxIndex, -1);
				int num = 0;
				for (int i = 1; i < intersectionList.Length; i++)
				{
					courseData.m_EndPosition = intersectionList[i].m_CoursePos;
					val.y = intersectionList[i].m_AuxIndex;
					if (courseData.m_EndPosition.m_Entity == Entity.Null || courseData.m_EndPosition.m_Entity != courseData.m_StartPosition.m_Entity)
					{
						TryAddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, fixedNetElements, auxiliaryNets, auxIntersectionEntities, val, ref courseHeightData, ref courseIndex);
						num++;
					}
					courseData.m_StartPosition = courseData.m_EndPosition;
					val.x = val.y;
				}
				if (num == 0)
				{
					courseData.m_StartPosition = intersectionList[0].m_CoursePos;
					courseData.m_EndPosition = intersectionList[intersectionList.Length - 1].m_CoursePos;
					((int2)(ref val))._002Ector(intersectionList[0].m_AuxIndex, intersectionList[intersectionList.Length - 1].m_AuxIndex);
					TryAddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, fixedNetElements, auxiliaryNets, auxIntersectionEntities, val, ref courseHeightData, ref courseIndex);
				}
			}
			if (courseIndex == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(jobIndex, courseEntity);
			}
		}

		private void TryAddCourse(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NetData netData, NetGeometryData netGeometryData, PlaceableNetData placeableNetData, DynamicBuffer<FixedNetElement> fixedNetElements, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, int2 auxIndex, ref CourseHeightData courseHeightData, ref int courseIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			if (creationDefinition.m_Original != Entity.Null || creationDefinition.m_Prefab == Entity.Null)
			{
				courseHeightData.SampleCourseHeight(ref courseData, netGeometryData);
				AddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, auxiliaryNets, auxIntersectionEntities, auxIndex, ref courseIndex);
				return;
			}
			float3 val = MathUtils.Tangent(courseData.m_Curve, courseData.m_StartPosition.m_CourseDelta);
			float2 xz = ((float3)(ref val)).xz;
			val = MathUtils.Tangent(courseData.m_Curve, courseData.m_EndPosition.m_CourseDelta);
			float2 xz2 = ((float3)(ref val)).xz;
			if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
			{
				courseHeightData.SampleCourseHeight(ref courseData, netGeometryData);
				AddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, auxiliaryNets, auxIntersectionEntities, auxIndex, ref courseIndex);
			}
			else if (math.dot(xz, xz2) < -0.001f && (netGeometryData.m_Flags & Game.Net.GeometryFlags.NoCurveSplit) == 0)
			{
				float num = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref courseData.m_Curve)).xz, new float2(courseData.m_StartPosition.m_CourseDelta, courseData.m_EndPosition.m_CourseDelta));
				CoursePos coursePos = new CoursePos
				{
					m_CourseDelta = num,
					m_Position = MathUtils.Position(courseData.m_Curve, num),
					m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(courseData.m_Curve, num)),
					m_Flags = (courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.IsGrid))
				};
				coursePos.m_Flags |= CoursePosFlags.FreeHeight;
				coursePos.m_ParentMesh = math.select(courseData.m_StartPosition.m_ParentMesh, -1, courseData.m_StartPosition.m_ParentMesh != courseData.m_EndPosition.m_ParentMesh);
				NetCourse courseData2 = courseData;
				NetCourse courseData3 = courseData;
				courseData2.m_EndPosition = coursePos;
				courseData3.m_StartPosition = coursePos;
				courseData2.m_Length = MathUtils.Length(courseData2.m_Curve, new Bounds1(courseData2.m_StartPosition.m_CourseDelta, courseData2.m_EndPosition.m_CourseDelta));
				courseData3.m_Length = MathUtils.Length(courseData3.m_Curve, new Bounds1(courseData3.m_StartPosition.m_CourseDelta, courseData3.m_EndPosition.m_CourseDelta));
				TryAddCoursePhase2(courseData2, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, fixedNetElements, auxiliaryNets, auxIntersectionEntities, new int2(auxIndex.x, -1), ref courseHeightData, ref courseIndex);
				TryAddCoursePhase2(courseData3, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, fixedNetElements, auxiliaryNets, auxIntersectionEntities, new int2(-1, auxIndex.y), ref courseHeightData, ref courseIndex);
			}
			else
			{
				TryAddCoursePhase2(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, fixedNetElements, auxiliaryNets, auxIntersectionEntities, auxIndex, ref courseHeightData, ref courseIndex);
			}
		}

		private void TryAddCoursePhase2(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NetData netData, NetGeometryData netGeometryData, PlaceableNetData placeableNetData, DynamicBuffer<FixedNetElement> fixedNetElements, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, int2 auxIndex, ref CourseHeightData courseHeightData, ref int courseIndex)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_087f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0890: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06af: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_093d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0810: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0823: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0993: Unknown result type (might be due to invalid IL or missing references)
			//IL_099d: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref courseData.m_Curve)).xz, new Bounds1(courseData.m_StartPosition.m_CourseDelta, courseData.m_EndPosition.m_CourseDelta));
			if (fixedNetElements.IsCreated)
			{
				float2 val = default(float2);
				((float2)(ref val))._002Ector(0f, 0f);
				float num2 = 0f;
				for (int i = 0; i < fixedNetElements.Length; i++)
				{
					FixedNetElement fixedNetElement = fixedNetElements[i];
					float2 val2 = new float2(fixedNetElement.m_LengthRange.min, fixedNetElement.m_LengthRange.max);
					int2 val3 = math.select(fixedNetElement.m_CountRange, new int2(0, 10000), fixedNetElement.m_CountRange == 0);
					float2 val4 = val2 * float2.op_Implicit(val3);
					float num3 = math.select(0f, val4.y - val4.x, fixedNetElement.m_LengthRange.max != fixedNetElement.m_LengthRange.min);
					val += val4;
					num2 += num3;
				}
				float2 val5 = (num - 0.16f) / math.max(float2.op_Implicit(1f), val);
				int num4;
				if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.NoCurveSplit) != 0)
				{
					num4 = Mathf.CeilToInt(val5.y);
				}
				else
				{
					float3 val6 = MathUtils.Tangent(courseData.m_Curve, courseData.m_StartPosition.m_CourseDelta);
					float2 val7 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
					val6 = MathUtils.Tangent(courseData.m_Curve, courseData.m_EndPosition.m_CourseDelta);
					float2 val8 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
					float num5 = math.acos(math.clamp(math.dot(val7, val8), -1f, 1f));
					float num6 = math.ceil(val5.y);
					float num7 = math.max(num6, math.floor(val5.x));
					num4 = Mathf.RoundToInt(math.lerp(num6, num7, math.saturate(num5 * (2f / (float)Math.PI))));
				}
				NetCourse netCourse = courseData;
				int2 val9 = auxIndex;
				NativeArray<NetCourse> val10 = default(NativeArray<NetCourse>);
				float2 val18 = default(float2);
				float2 val19 = default(float2);
				for (int j = 1; j <= num4; j++)
				{
					if (j == num4)
					{
						netCourse.m_EndPosition = courseData.m_EndPosition;
						val9.y = auxIndex.y;
					}
					else
					{
						netCourse.m_EndPosition = CutCourse(courseData, netGeometryData, num * (float)j / (float)num4);
						val9.y = -1;
					}
					NetCourse course = netCourse;
					if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StraightEdges) != 0)
					{
						courseHeightData.SampleCourseHeight(ref course, netGeometryData);
					}
					else
					{
						course.m_Length = MathUtils.Length(((Bezier4x3)(ref course.m_Curve)).xz, new Bounds1(course.m_StartPosition.m_CourseDelta, course.m_EndPosition.m_CourseDelta));
					}
					float num8 = course.m_Length - val.x;
					float num9 = math.max(0f, num8);
					float num10 = 0f;
					num8 -= num9;
					val10._002Ector(fixedNetElements.Length, (Allocator)2, (NativeArrayOptions)1);
					NetCourse netCourse2 = course;
					for (int k = 0; k < fixedNetElements.Length; k++)
					{
						FixedNetElement fixedNetElement2 = fixedNetElements[k];
						float2 val11 = new float2(fixedNetElement2.m_LengthRange.min, fixedNetElement2.m_LengthRange.max);
						int2 val12 = math.select(fixedNetElement2.m_CountRange, new int2(0, 10000), fixedNetElement2.m_CountRange == 0);
						float2 val13 = val11 * float2.op_Implicit(val12);
						float num11 = math.select(0f, val13.y - val13.x, fixedNetElement2.m_LengthRange.max != fixedNetElement2.m_LengthRange.min);
						netCourse2.m_Length = val13.x + num11 * num9 / math.max(1f, num2);
						netCourse2.m_Length += val13.x * num8 / math.max(1f, val.x);
						netCourse2.m_FixedIndex = k;
						if (k == fixedNetElements.Length - 1)
						{
							netCourse2.m_EndPosition = course.m_EndPosition;
						}
						else
						{
							netCourse2.m_EndPosition = CutCourse(course, netGeometryData, num10 + netCourse2.m_Length);
							netCourse2.m_EndPosition.m_Flags |= CoursePosFlags.IsFixed;
							num10 += netCourse2.m_Length;
						}
						val10[k] = netCourse2;
						netCourse2.m_StartPosition = netCourse2.m_EndPosition;
					}
					if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StraightEdges) == 0)
					{
						int num12 = 0;
						int num13 = fixedNetElements.Length - 1;
						float3 position = course.m_StartPosition.m_Position;
						float3 position2 = course.m_EndPosition.m_Position;
						float3 val14 = MathUtils.Tangent(course.m_Curve, course.m_StartPosition.m_CourseDelta);
						float3 val15 = MathUtils.Tangent(course.m_Curve, course.m_EndPosition.m_CourseDelta);
						val14 = MathUtils.Normalize(val14, ((float3)(ref val14)).xz);
						val15 = MathUtils.Normalize(val15, ((float3)(ref val15)).xz);
						Bezier4x3 val16;
						for (int l = 0; l < fixedNetElements.Length && (fixedNetElements[l].m_Flags & FixedNetFlags.Straight) != 0; l++)
						{
							netCourse2 = val10[l];
							ref Bezier4x3 curve = ref netCourse2.m_Curve;
							val16 = NetUtils.StraightCurve(position, position + val14 * netCourse2.m_Length);
							((Bezier4x3)(ref curve)).xz = ((Bezier4x3)(ref val16)).xz;
							netCourse2.m_StartPosition.m_CourseDelta = 0f;
							netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
							netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(val14);
							netCourse2.m_EndPosition.m_CourseDelta = 1f;
							netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
							netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(val14);
							val10[l] = netCourse2;
							position = netCourse2.m_EndPosition.m_Position;
							num12++;
						}
						int num14 = fixedNetElements.Length - 1;
						while (num14 >= 0 && (fixedNetElements[num14].m_Flags & FixedNetFlags.Straight) != 0)
						{
							netCourse2 = val10[num14];
							ref Bezier4x3 curve2 = ref netCourse2.m_Curve;
							val16 = NetUtils.StraightCurve(position2 - val15 * netCourse2.m_Length, position2 + val14);
							((Bezier4x3)(ref curve2)).xz = ((Bezier4x3)(ref val16)).xz;
							netCourse2.m_StartPosition.m_CourseDelta = 0f;
							netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
							netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(-val15);
							netCourse2.m_EndPosition.m_CourseDelta = 1f;
							netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
							netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(-val15);
							val10[num14] = netCourse2;
							position2 = netCourse2.m_StartPosition.m_Position;
							num13--;
							num14--;
						}
						if (num13 >= num12)
						{
							Bezier4x3 val17 = NetUtils.FitCurve(position, val14, val15, position2);
							float courseDelta = val10[num12].m_StartPosition.m_CourseDelta;
							float courseDelta2 = val10[num13].m_EndPosition.m_CourseDelta;
							for (int m = num12; m <= num13; m++)
							{
								netCourse2 = val10[m];
								((float2)(ref val18))._002Ector(netCourse2.m_StartPosition.m_CourseDelta, netCourse2.m_EndPosition.m_CourseDelta);
								val18 = (val18 - courseDelta) / math.max(0.001f, courseDelta2 - courseDelta);
								ref Bezier4x3 curve3 = ref netCourse2.m_Curve;
								val16 = MathUtils.Cut(val17, val18);
								((Bezier4x3)(ref curve3)).xz = ((Bezier4x3)(ref val16)).xz;
								netCourse2.m_StartPosition.m_CourseDelta = 0f;
								netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
								netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse2.m_Curve));
								netCourse2.m_EndPosition.m_CourseDelta = 1f;
								netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
								netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse2.m_Curve));
								val10[m] = netCourse2;
							}
						}
					}
					for (int n = 0; n < fixedNetElements.Length; n++)
					{
						netCourse2 = val10[n];
						FixedNetElement fixedNetElement3 = fixedNetElements[n];
						((float2)(ref val19))._002Ector(fixedNetElement3.m_LengthRange.min, fixedNetElement3.m_LengthRange.max);
						int2 val20 = math.select(fixedNetElement3.m_CountRange, new int2(0, 10000), fixedNetElement3.m_CountRange == 0);
						int num15 = (int)math.ceil((netCourse2.m_Length - 0.16f) / val19.y);
						num15 = math.clamp(num15, val20.x, val20.y);
						NetCourse netCourse3 = netCourse2;
						for (int num16 = 1; num16 <= num15; num16++)
						{
							if (num16 == num15)
							{
								netCourse3.m_EndPosition = netCourse2.m_EndPosition;
							}
							else
							{
								netCourse3.m_EndPosition = CutCourse(netCourse2, netGeometryData, netCourse2.m_Length * (float)num16 / (float)num15);
								netCourse3.m_EndPosition.m_Flags |= CoursePosFlags.IsFixed;
							}
							int2 auxIndex2 = math.select(int2.op_Implicit(-1), val9, new bool2(n == 0 && num16 == 1, n == fixedNetElements.Length - 1 && num16 == num15));
							if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.StraightEdges) == 0)
							{
								NetCourse course2 = netCourse3;
								courseHeightData.SampleCourseHeight(ref course2, netGeometryData);
								AddCourse(course2, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, auxiliaryNets, auxIntersectionEntities, auxIndex2, ref courseIndex);
							}
							else
							{
								netCourse3.m_Length = MathUtils.Length(((Bezier4x3)(ref netCourse3.m_Curve)).xz, new Bounds1(netCourse3.m_StartPosition.m_CourseDelta, netCourse3.m_EndPosition.m_CourseDelta));
								AddCourse(netCourse3, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, auxiliaryNets, auxIntersectionEntities, auxIndex2, ref courseIndex);
							}
							netCourse3.m_StartPosition = netCourse3.m_EndPosition;
						}
					}
					val10.Dispose();
					netCourse.m_StartPosition = netCourse.m_EndPosition;
					val9.x = val9.y;
				}
				return;
			}
			NetCourse course3 = courseData;
			courseHeightData.SampleCourseHeight(ref course3, netGeometryData);
			CalculateElevation(creationDefinition, ownerDefinition, ref course3, ref upgraded, netGeometryData, placeableNetData);
			float num17 = netGeometryData.m_EdgeLengthRange.max;
			if ((creationDefinition.m_Flags & CreationFlags.SubElevation) != 0)
			{
				CompositionFlags elevationFlags = NetCompositionHelpers.GetElevationFlags(new Game.Net.Elevation(course3.m_StartPosition.m_Elevation), new Game.Net.Elevation(course3.m_Elevation), new Game.Net.Elevation(course3.m_EndPosition.m_Elevation), netGeometryData);
				num17 = math.select(num17, netGeometryData.m_ElevatedLength, (elevationFlags.m_General & CompositionFlags.General.Elevated) != 0);
			}
			int num18 = (int)math.ceil((num - 0.16f) / num17);
			if (num18 > 1)
			{
				course3 = courseData;
				int2 val21 = auxIndex;
				for (int num19 = 1; num19 <= num18; num19++)
				{
					if (num19 == num18)
					{
						course3.m_EndPosition = courseData.m_EndPosition;
						val21.y = auxIndex.y;
					}
					else
					{
						course3.m_EndPosition = CutCourse(courseData, netGeometryData, num * (float)num19 / (float)num18);
						val21.y = -1;
					}
					NetCourse course4 = course3;
					courseHeightData.SampleCourseHeight(ref course4, netGeometryData);
					AddCourse(course4, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, placeableNetData, auxiliaryNets, auxIntersectionEntities, val21, ref courseIndex);
					course3.m_StartPosition = course3.m_EndPosition;
					val21.x = val21.y;
				}
			}
			else
			{
				AddCourse(course3, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, auxiliaryNets, auxIntersectionEntities, auxIndex, ref courseIndex);
			}
		}

		private CoursePos CutCourse(NetCourse course, NetGeometryData netGeometryData, float cutLength)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			CoursePos result = default(CoursePos);
			if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
			{
				cutLength = MathUtils.Snap(cutLength + 0.16f, 4f);
				if (((int)math.round(cutLength / 4f) & 1) != 0 != ((course.m_StartPosition.m_Flags & CoursePosFlags.HalfAlign) != 0))
				{
					result.m_Flags |= CoursePosFlags.HalfAlign;
				}
			}
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(course.m_StartPosition.m_CourseDelta, 1f);
			MathUtils.ClampLength(((Bezier4x3)(ref course.m_Curve)).xz, ref val, cutLength);
			result.m_CourseDelta = val.max;
			result.m_Position = MathUtils.Position(course.m_Curve, result.m_CourseDelta);
			result.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(course.m_Curve, result.m_CourseDelta));
			result.m_Flags |= course.m_StartPosition.m_Flags & (CoursePosFlags.IsParallel | CoursePosFlags.IsRight | CoursePosFlags.IsLeft | CoursePosFlags.ForceElevatedEdge | CoursePosFlags.IsGrid);
			result.m_Flags |= CoursePosFlags.FreeHeight;
			result.m_ParentMesh = math.select(course.m_StartPosition.m_ParentMesh, -1, course.m_StartPosition.m_ParentMesh != course.m_EndPosition.m_ParentMesh);
			if ((course.m_StartPosition.m_Flags & CoursePosFlags.ForceElevatedEdge) != 0)
			{
				result.m_Flags |= CoursePosFlags.ForceElevatedNode;
			}
			return result;
		}

		private float2 CalculateElevation(CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, NetGeometryData netGeometryData, PlaceableNetData placeableNetData, Bezier4x3 curve, float delta, float offset, bool serviceUpgrade)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.Position(curve, delta);
			bool flag = (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) == 0 && !serviceUpgrade;
			if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.OnWater) != 0)
			{
				return float2.op_Implicit(0f);
			}
			if (flag && ownerDefinition.m_Prefab != Entity.Null)
			{
				return float2.op_Implicit(val.y - ownerDefinition.m_Position.y);
			}
			Transform transform = default(Transform);
			if (flag && creationDefinition.m_Owner != Entity.Null && m_TransformData.TryGetComponent(creationDefinition.m_Owner, ref transform))
			{
				return float2.op_Implicit(val.y - transform.m_Position.y);
			}
			float3 val2 = MathUtils.Tangent(curve, delta);
			float3 val3 = default(float3);
			((float3)(ref val3)).xz = math.normalizesafe(MathUtils.Right(((float3)(ref val2)).xz), default(float2)) * offset;
			float2 val4 = default(float2);
			val4.x = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val - val3);
			val4.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val + val3);
			val4 = val.y - val4;
			if ((placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.ShoreLine) != Game.Net.PlacementFlags.None)
			{
				bool2 val5 = default(bool2);
				val5.x = WaterUtils.SampleDepth(ref m_WaterSurfaceData, val - val3) >= netGeometryData.m_ElevationLimit;
				val5.y = WaterUtils.SampleDepth(ref m_WaterSurfaceData, val + val3) >= netGeometryData.m_ElevationLimit;
				if (math.all(val5))
				{
					val4 = math.max(float2.op_Implicit(netGeometryData.m_ElevationLimit), val4);
				}
				else if (val5.x || (val4.x - val4.y >= netGeometryData.m_ElevationLimit * 0.1f && !val5.y))
				{
					val4.x = math.max(netGeometryData.m_ElevationLimit, val4.x);
					val4.y = 0f;
				}
				else if (val5.y || (val4.y - val4.x >= netGeometryData.m_ElevationLimit * 0.1f && !val5.x))
				{
					val4.x = 0f;
					val4.y = math.max(netGeometryData.m_ElevationLimit, val4.y);
				}
			}
			return val4;
		}

		private void CalculateElevation(CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, ref NetCourse courseData, ref Upgraded upgraded, NetGeometryData netGeometryData, PlaceableNetData placeableNetData)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			float delta = math.lerp(courseData.m_StartPosition.m_CourseDelta, courseData.m_EndPosition.m_CourseDelta, 0.5f);
			float offset = netGeometryData.m_DefaultWidth * 0.5f;
			bool serviceUpgrade = !m_EditorMode && (creationDefinition.m_Flags & CreationFlags.SubElevation) != 0 && m_PrefabServiceUpgradeData.HasComponent(creationDefinition.m_Prefab);
			float2 val = CalculateElevation(creationDefinition, ownerDefinition, netGeometryData, placeableNetData, courseData.m_Curve, courseData.m_StartPosition.m_CourseDelta, offset, serviceUpgrade);
			float2 val2 = CalculateElevation(creationDefinition, ownerDefinition, netGeometryData, placeableNetData, courseData.m_Curve, delta, offset, serviceUpgrade);
			float2 val3 = CalculateElevation(creationDefinition, ownerDefinition, netGeometryData, placeableNetData, courseData.m_Curve, courseData.m_EndPosition.m_CourseDelta, offset, serviceUpgrade);
			bool flag = (upgraded.m_Flags.m_General & CompositionFlags.General.Elevated) != 0;
			if (flag)
			{
				courseData.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
				courseData.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedEdge;
				upgraded.m_Flags.m_General &= ~CompositionFlags.General.Elevated;
				float num = MathUtils.Max(((Bezier4x3)(ref courseData.m_Curve)).y);
				if (courseData.m_Curve.a.y > num - 0.1f)
				{
					courseData.m_StartPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
				if (courseData.m_Curve.d.y > num - 0.1f)
				{
					courseData.m_EndPosition.m_Flags |= CoursePosFlags.ForceElevatedNode;
				}
			}
			val = math.select(val, float2.op_Implicit(netGeometryData.m_ElevationLimit * 2f), ((courseData.m_StartPosition.m_Flags & CoursePosFlags.ForceElevatedNode) != 0) & (val < netGeometryData.m_ElevationLimit * 2f));
			val2 = math.select(val2, float2.op_Implicit(netGeometryData.m_ElevationLimit * 2f), ((courseData.m_StartPosition.m_Flags & courseData.m_EndPosition.m_Flags & CoursePosFlags.ForceElevatedEdge) != 0) & (val2 < netGeometryData.m_ElevationLimit * 2f));
			val3 = math.select(val3, float2.op_Implicit(netGeometryData.m_ElevationLimit * 2f), ((courseData.m_EndPosition.m_Flags & CoursePosFlags.ForceElevatedNode) != 0) & (val3 < netGeometryData.m_ElevationLimit * 2f));
			val = math.select(val, float2.op_Implicit(0f), new bool2((courseData.m_StartPosition.m_Flags & CoursePosFlags.LeftTransition) != 0, (courseData.m_StartPosition.m_Flags & CoursePosFlags.RightTransition) != 0));
			val3 = math.select(val3, float2.op_Implicit(0f), new bool2((courseData.m_EndPosition.m_Flags & CoursePosFlags.LeftTransition) != 0, (courseData.m_EndPosition.m_Flags & CoursePosFlags.RightTransition) != 0));
			courseData.m_StartPosition.m_Elevation = math.select(default(float2), val, (val >= netGeometryData.m_ElevationLimit) | (val <= 0f - netGeometryData.m_ElevationLimit));
			courseData.m_Elevation = math.select(default(float2), val2, (val2 >= netGeometryData.m_ElevationLimit) | (val2 <= 0f - netGeometryData.m_ElevationLimit));
			courseData.m_EndPosition.m_Elevation = math.select(default(float2), val3, (val3 >= netGeometryData.m_ElevationLimit) | (val3 <= 0f - netGeometryData.m_ElevationLimit));
			if ((creationDefinition.m_Owner != Entity.Null || ownerDefinition.m_Prefab != Entity.Null) && !flag && (creationDefinition.m_Flags & CreationFlags.SubElevation) == 0)
			{
				if (courseData.m_StartPosition.m_ParentMesh < 0)
				{
					courseData.m_StartPosition.m_Elevation = default(float2);
				}
				if (courseData.m_StartPosition.m_ParentMesh < 0 && courseData.m_EndPosition.m_ParentMesh < 0)
				{
					courseData.m_Elevation = default(float2);
				}
				if (courseData.m_EndPosition.m_ParentMesh < 0)
				{
					courseData.m_EndPosition.m_Elevation = default(float2);
				}
			}
			LimitElevation(ref courseData.m_StartPosition.m_Elevation, placeableNetData);
			LimitElevation(ref courseData.m_Elevation, placeableNetData);
			LimitElevation(ref courseData.m_EndPosition.m_Elevation, placeableNetData);
		}

		private void AddCourse(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NetData netData, NetGeometryData netGeometryData, PlaceableNetData placeableNetData, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, int2 auxIndex, ref int courseIndex)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			CalculateElevation(creationDefinition, ownerDefinition, ref courseData, ref upgraded, netGeometryData, placeableNetData);
			AddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, netGeometryData, auxiliaryNets, auxIntersectionEntities, auxIndex, ref courseIndex);
		}

		private void AddCourse(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NetData netData, NetGeometryData netGeometryData, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, int2 auxIndex, ref int courseIndex)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			AddCourse(courseData, creationDefinition, ownerDefinition, upgraded, courseEntity, jobIndex, netData, ref courseIndex);
			if (!auxiliaryNets.IsCreated)
			{
				return;
			}
			OwnerDefinition ownerDefinition2 = new OwnerDefinition
			{
				m_Prefab = creationDefinition.m_Prefab,
				m_Position = MathUtils.Position(courseData.m_Curve, courseData.m_StartPosition.m_CourseDelta),
				m_Rotation = quaternion.op_Implicit(new float4(MathUtils.Position(courseData.m_Curve, courseData.m_EndPosition.m_CourseDelta), 0f))
			};
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			for (int i = 0; i < auxiliaryNets.Length; i++)
			{
				AuxiliaryNet auxiliaryNet = auxiliaryNets[i];
				NetData netData2 = m_PrefabNetData[auxiliaryNet.m_Prefab];
				NetGeometryData netGeometryData2 = m_PrefabGeometryData[auxiliaryNet.m_Prefab];
				m_PrefabPlaceableData.TryGetComponent(auxiliaryNet.m_Prefab, ref placeableNetData);
				bool flag = NetUtils.ShouldInvert(auxiliaryNet.m_InvertMode, m_LefthandTraffic);
				CreationDefinition auxDefinition = GetAuxDefinition(creationDefinition, auxiliaryNet);
				NetCourse courseData2 = courseData;
				if (GetAuxCourse(ref courseData2, auxiliaryNet, flag))
				{
					int2 val = math.select(auxIndex, ((int2)(ref auxIndex)).yx, flag);
					FixAuxCoursePos(ref courseData2.m_StartPosition, auxiliaryNets, auxIntersectionEntities, val.x, i);
					FixAuxCoursePos(ref courseData2.m_EndPosition, auxiliaryNets, auxIntersectionEntities, val.y, i);
					Upgraded upgraded2 = upgraded;
					if (flag)
					{
						upgraded2.m_Flags = NetCompositionHelpers.InvertCompositionFlags(upgraded2.m_Flags);
					}
					CalculateElevation(creationDefinition, ownerDefinition, ref courseData2, ref upgraded2, netGeometryData2, placeableNetData);
					AddCourse(courseData2, auxDefinition, ownerDefinition2, upgraded2, courseEntity, jobIndex, netData2, ref courseIndex);
				}
			}
		}

		private void FixAuxCoursePos(ref CoursePos coursePos, DynamicBuffer<AuxiliaryNet> auxiliaryNets, NativeList<AuxIntersectionEntity> auxIntersectionEntities, int intersectionAuxIndex, int auxNetIndex)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (intersectionAuxIndex != -1)
			{
				AuxIntersectionEntity auxIntersectionEntity = auxIntersectionEntities[intersectionAuxIndex * auxiliaryNets.Length + auxNetIndex];
				coursePos.m_Entity = auxIntersectionEntity.m_Entity;
				coursePos.m_SplitPosition = auxIntersectionEntity.m_SplitPosition;
				coursePos.m_Flags &= ~CoursePosFlags.ForceElevatedNode;
			}
			else
			{
				coursePos.m_Entity = Entity.Null;
				coursePos.m_SplitPosition = 0f;
			}
		}

		private bool IgnoreOverlappingEdge(CreationDefinition creationDefinition, NetCourse courseData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			if (creationDefinition.m_Original != Entity.Null)
			{
				return false;
			}
			if (courseData.m_StartPosition.m_Entity == Entity.Null)
			{
				return false;
			}
			if (courseData.m_EndPosition.m_Entity == Entity.Null)
			{
				return false;
			}
			if (((courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsGrid)) == CoursePosFlags.IsFirst && (courseData.m_EndPosition.m_Flags & (CoursePosFlags.IsLast | CoursePosFlags.IsGrid)) == CoursePosFlags.IsLast) || ((courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsLast | CoursePosFlags.IsGrid)) == CoursePosFlags.IsLast && (courseData.m_EndPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsGrid)) == CoursePosFlags.IsFirst))
			{
				return false;
			}
			if (courseData.m_EndPosition.m_Entity == courseData.m_StartPosition.m_Entity)
			{
				return true;
			}
			Edge edge = default(Edge);
			if (m_EdgeData.TryGetComponent(courseData.m_StartPosition.m_Entity, ref edge) && (courseData.m_EndPosition.m_Entity == edge.m_Start || courseData.m_EndPosition.m_Entity == edge.m_End))
			{
				return true;
			}
			Edge edge2 = default(Edge);
			if (m_EdgeData.TryGetComponent(courseData.m_EndPosition.m_Entity, ref edge2) && (courseData.m_StartPosition.m_Entity == edge2.m_Start || courseData.m_StartPosition.m_Entity == edge2.m_End))
			{
				return true;
			}
			return false;
		}

		private void AddCourse(NetCourse courseData, CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Upgraded upgraded, Entity courseEntity, int jobIndex, NetData netData, ref int courseIndex)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			creationDefinition.m_RandomSeed += courseIndex;
			FindOriginalEdge(ref creationDefinition, ownerDefinition, netData, courseData);
			if (IgnoreOverlappingEdge(creationDefinition, courseData))
			{
				courseData.m_StartPosition.m_Flags |= CoursePosFlags.DontCreate;
				courseData.m_EndPosition.m_Flags |= CoursePosFlags.DontCreate;
			}
			bool flag = m_LocalCurveCacheData.HasComponent(courseEntity);
			LocalCurveCache localCurveCache = default(LocalCurveCache);
			if (flag)
			{
				Transform inverseParentTransform = ObjectUtils.InverseTransform(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation));
				localCurveCache.m_Curve.a = ObjectUtils.WorldToLocal(inverseParentTransform, courseData.m_Curve.a);
				localCurveCache.m_Curve.b = ObjectUtils.WorldToLocal(inverseParentTransform, courseData.m_Curve.b);
				localCurveCache.m_Curve.c = ObjectUtils.WorldToLocal(inverseParentTransform, courseData.m_Curve.c);
				localCurveCache.m_Curve.d = ObjectUtils.WorldToLocal(inverseParentTransform, courseData.m_Curve.d);
			}
			if (courseIndex++ == 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CreationDefinition>(jobIndex, courseEntity, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<NetCourse>(jobIndex, courseEntity, courseData);
				if (flag)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<LocalCurveCache>(jobIndex, courseEntity, localCurveCache);
				}
				if (upgraded.m_Flags != default(CompositionFlags))
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Upgraded>(jobIndex, courseEntity, upgraded);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Upgraded>(jobIndex, courseEntity);
				}
				return;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(jobIndex, val, creationDefinition);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<NetCourse>(jobIndex, val, courseData);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val, default(Updated));
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(jobIndex, val, ownerDefinition);
			}
			if (flag)
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(jobIndex, val, localCurveCache);
			}
			if (upgraded.m_Flags != default(CompositionFlags))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Upgraded>(jobIndex, val, upgraded);
			}
		}

		private void LimitElevation(ref float2 elevation, PlaceableNetData placeableNetData)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			elevation = math.select(elevation, float2.op_Implicit(placeableNetData.m_ElevationRange.min), (elevation < placeableNetData.m_ElevationRange.min) & (placeableNetData.m_ElevationRange.min >= 0f));
			elevation = math.select(elevation, float2.op_Implicit(placeableNetData.m_ElevationRange.max), (elevation > placeableNetData.m_ElevationRange.max) & (placeableNetData.m_ElevationRange.max < 0f));
		}

		private bool MatchingOwner(CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(entity, ref owner))
			{
				Transform transform = default(Transform);
				PrefabRef prefabRef = default(PrefabRef);
				if (creationDefinition.m_Owner != owner.m_Owner && (!m_TransformData.TryGetComponent(owner.m_Owner, ref transform) || !m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef) || !((float3)(ref ownerDefinition.m_Position)).Equals(transform.m_Position) || !((quaternion)(ref ownerDefinition.m_Rotation)).Equals(transform.m_Rotation) || ownerDefinition.m_Prefab != prefabRef.m_Prefab))
				{
					return false;
				}
			}
			else if (ownerDefinition.m_Prefab != Entity.Null)
			{
				return false;
			}
			return true;
		}

		private void FindOriginalEdge(ref CreationDefinition creationDefinition, OwnerDefinition ownerDefinition, NetData netData, NetCourse netCourse)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			if (creationDefinition.m_Original != Entity.Null || (creationDefinition.m_Flags & CreationFlags.Permanent) != 0 || !m_ConnectedEdges.HasBuffer(netCourse.m_StartPosition.m_Entity) || !m_ConnectedEdges.HasBuffer(netCourse.m_EndPosition.m_Entity))
			{
				return;
			}
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[netCourse.m_StartPosition.m_Entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				if (m_DeletedEntities.ContainsKey(edge))
				{
					continue;
				}
				Edge edge2 = m_EdgeData[edge];
				if (edge2.m_Start == netCourse.m_StartPosition.m_Entity && edge2.m_End == netCourse.m_EndPosition.m_Entity)
				{
					if (CanReplace(netData, edge))
					{
						creationDefinition.m_Original = edge;
					}
					break;
				}
				if (edge2.m_Start == netCourse.m_EndPosition.m_Entity && edge2.m_End == netCourse.m_StartPosition.m_Entity)
				{
					if (CanReplace(netData, edge))
					{
						creationDefinition.m_Original = edge;
						creationDefinition.m_Flags |= CreationFlags.Invert;
					}
					break;
				}
			}
			if (creationDefinition.m_Original != Entity.Null && !MatchingOwner(creationDefinition, ownerDefinition, creationDefinition.m_Original))
			{
				creationDefinition.m_Original = Entity.Null;
				creationDefinition.m_Flags &= ~CreationFlags.Invert;
			}
		}

		private bool CanReplace(NetData netData, Entity original)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[original];
			NetData netData2 = m_PrefabNetData[prefabRef.m_Prefab];
			return (netData.m_RequiredLayers & netData2.m_RequiredLayers) != 0;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> __Game_Net_Upgraded_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AuxiliaryNet> __Game_Prefabs_AuxiliaryNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalCurveCache> __Game_Tools_LocalCurveCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<FixedNetElement> __Game_Prefabs_FixedNetElement_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<OwnerDefinition>(true);
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Net_Upgraded_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_AuxiliaryNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AuxiliaryNet>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Tools_LocalCurveCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalCurveCache>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUpgradeData>(true);
			__Game_Prefabs_FixedNetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FixedNetElement>(true);
		}
	}

	private ToolSystem m_ToolSystem;

	private ToolReadyBarrier m_ToolReadyBarrier;

	private Game.Net.SearchSystem m_SearchSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_CourseQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ToolReadyBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolReadyBarrier>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_CourseQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<NetCourse>(),
			ComponentType.ReadOnly<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CourseQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		NativeHashMap<Entity, bool> deletedEntities = default(NativeHashMap<Entity, bool>);
		deletedEntities._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Course> val = default(NativeList<Course>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Overlap> overlapQueue = default(NativeQueue<Overlap>);
		overlapQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Overlap> val2 = default(NativeList<Overlap>);
		val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelQueue<IntersectPos> val3 = default(NativeParallelQueue<IntersectPos>);
		val3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		CheckCoursesJob checkCoursesJob = new CheckCoursesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedEntities = deletedEntities,
			m_CourseList = val
		};
		JobHandle dependencies;
		FindOverlapsJob findOverlapsJob = new FindOverlapsJob
		{
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AuxiliaryNets = InternalCompilerInterface.GetBufferLookup<AuxiliaryNet>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = m_SearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_CourseList = val,
			m_OverlapQueue = overlapQueue.AsParallelWriter()
		};
		DequeueOverlapsJob dequeueOverlapsJob = new DequeueOverlapsJob
		{
			m_OverlapQueue = overlapQueue,
			m_OverlapList = val2
		};
		CheckCourseIntersectionsJob checkCourseIntersectionsJob = new CheckCourseIntersectionsJob
		{
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAuxiliaryNets = InternalCompilerInterface.GetBufferLookup<AuxiliaryNet>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CourseList = val,
			m_OverlapList = val2,
			m_DeletedEntities = deletedEntities,
			m_Results = val3.AsWriter()
		};
		JobHandle deps;
		CheckCourseIntersectionResultsJob checkCourseIntersectionResultsJob = new CheckCourseIntersectionResultsJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalCurveCacheData = InternalCompilerInterface.GetComponentLookup<LocalCurveCache>(ref __TypeHandle.__Game_Tools_LocalCurveCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceableData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabFixedNetElements = InternalCompilerInterface.GetBufferLookup<FixedNetElement>(ref __TypeHandle.__Game_Prefabs_FixedNetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAuxiliaryNets = InternalCompilerInterface.GetBufferLookup<AuxiliaryNet>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_CourseList = val,
			m_DeletedEntities = deletedEntities,
			m_IntersectionQueue = val3.AsReader()
		};
		EntityCommandBuffer val4 = m_ToolReadyBarrier.CreateCommandBuffer();
		checkCourseIntersectionResultsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
		CheckCourseIntersectionResultsJob checkCourseIntersectionResultsJob2 = checkCourseIntersectionResultsJob;
		JobHandle val5 = JobChunkExtensions.Schedule<CheckCoursesJob>(checkCoursesJob, m_CourseQuery, ((SystemBase)this).Dependency);
		JobHandle val6 = IJobParallelForDeferExtensions.Schedule<FindOverlapsJob, Course>(findOverlapsJob, val, 1, JobHandle.CombineDependencies(val5, dependencies));
		JobHandle val7 = IJobExtensions.Schedule<DequeueOverlapsJob>(dequeueOverlapsJob, val6);
		JobHandle val8 = IJobParallelForDeferExtensions.Schedule<CheckCourseIntersectionsJob, Overlap>(checkCourseIntersectionsJob, val2, 1, val7);
		JobHandle val9 = IJobParallelForDeferExtensions.Schedule<CheckCourseIntersectionResultsJob, Course>(checkCourseIntersectionResultsJob2, val, 1, JobHandle.CombineDependencies(val8, deps));
		deletedEntities.Dispose(val9);
		val.Dispose(val9);
		overlapQueue.Dispose(val7);
		val2.Dispose(val8);
		val3.Dispose(val9);
		m_SearchSystem.AddNetSearchTreeReader(val6);
		m_TerrainSystem.AddCPUHeightReader(val9);
		m_WaterSystem.AddSurfaceReader(val9);
		((EntityCommandBufferSystem)m_ToolReadyBarrier).AddJobHandleForProducer(val9);
		((SystemBase)this).Dependency = val9;
	}

	private static CreationDefinition GetAuxDefinition(CreationDefinition creationDefinition, AuxiliaryNet auxiliaryNet)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new CreationDefinition
		{
			m_Prefab = auxiliaryNet.m_Prefab,
			m_Flags = creationDefinition.m_Flags,
			m_RandomSeed = creationDefinition.m_RandomSeed
		};
	}

	private static bool GetAuxCourse(ref NetCourse courseData, AuxiliaryNet auxiliaryNet, bool invert)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		courseData.m_StartPosition.m_Flags |= CoursePosFlags.IsParallel;
		courseData.m_EndPosition.m_Flags |= CoursePosFlags.IsParallel;
		courseData.m_FixedIndex = -1;
		if (auxiliaryNet.m_Position.x != 0f)
		{
			if ((courseData.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) == (CoursePosFlags.IsFirst | CoursePosFlags.IsLast))
			{
				return false;
			}
			courseData.m_Curve = MathUtils.Cut(courseData.m_Curve, new Bounds1(courseData.m_StartPosition.m_CourseDelta, courseData.m_EndPosition.m_CourseDelta));
			courseData.m_Curve = NetUtils.OffsetCurveLeftSmooth(courseData.m_Curve, float2.op_Implicit(0f - auxiliaryNet.m_Position.x));
			courseData.m_Length = MathUtils.Length(courseData.m_Curve);
			courseData.m_StartPosition.m_CourseDelta = 0f;
			courseData.m_EndPosition.m_CourseDelta = 1f;
			courseData.m_StartPosition.m_Position.x += auxiliaryNet.m_Position.x * 0.01f;
			courseData.m_EndPosition.m_Position.x += auxiliaryNet.m_Position.x * 0.01f;
		}
		if (auxiliaryNet.m_Position.z != 0f)
		{
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(courseData.m_StartPosition.m_CourseDelta, courseData.m_EndPosition.m_CourseDelta);
			float num = math.abs(auxiliaryNet.m_Position.z);
			float num2 = courseData.m_Length - num * 2f;
			if (!(num2 > math.max(0.9f, num - 0.5f)) || !MathUtils.ClampLength(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val, courseData.m_Length - num) || !MathUtils.ClampLengthInverse(((Bezier4x3)(ref courseData.m_Curve)).xz, ref val, num2))
			{
				return false;
			}
			courseData.m_Curve = MathUtils.Cut(courseData.m_Curve, val);
			courseData.m_Length = num2;
			courseData.m_StartPosition.m_CourseDelta = 0f;
			courseData.m_EndPosition.m_CourseDelta = 1f;
		}
		if (auxiliaryNet.m_Position.y != 0f)
		{
			ref Bezier4x3 curve = ref courseData.m_Curve;
			((Bezier4x3)(ref curve)).y = ((Bezier4x3)(ref curve)).y + auxiliaryNet.m_Position.y;
			courseData.m_StartPosition.m_Position.y += auxiliaryNet.m_Position.y;
			courseData.m_EndPosition.m_Position.y += auxiliaryNet.m_Position.y;
		}
		if (invert)
		{
			courseData.m_Curve = MathUtils.Invert(courseData.m_Curve);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_Entity, ref courseData.m_EndPosition.m_Entity);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_CourseDelta, ref courseData.m_EndPosition.m_CourseDelta);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_SplitPosition, ref courseData.m_EndPosition.m_SplitPosition);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_Position, ref courseData.m_EndPosition.m_Position);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_Rotation, ref courseData.m_EndPosition.m_Rotation);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_Elevation, ref courseData.m_EndPosition.m_Elevation);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_Flags, ref courseData.m_EndPosition.m_Flags);
			CommonUtils.Swap(ref courseData.m_StartPosition.m_ParentMesh, ref courseData.m_EndPosition.m_ParentMesh);
			quaternion val2 = quaternion.RotateY((float)Math.PI);
			courseData.m_StartPosition.m_Rotation = math.mul(val2, courseData.m_StartPosition.m_Rotation);
			courseData.m_EndPosition.m_Rotation = math.mul(val2, courseData.m_EndPosition.m_Rotation);
			courseData.m_StartPosition.m_CourseDelta = 1f - courseData.m_StartPosition.m_CourseDelta;
			courseData.m_EndPosition.m_CourseDelta = 1f - courseData.m_EndPosition.m_CourseDelta;
		}
		return true;
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
	public CourseSplitSystem()
	{
	}
}
