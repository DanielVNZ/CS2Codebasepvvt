using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class OverrideSystem : GameSystemBase
{
	private struct TreeAction
	{
		public Entity m_Entity;

		public BoundsMask m_Mask;
	}

	[BurstCompile]
	private struct UpdateOverriddenLayersJob : IJob
	{
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		public NativeQueue<TreeAction> m_Actions;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			TreeAction treeAction = default(TreeAction);
			QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
			while (m_Actions.TryDequeue(ref treeAction))
			{
				if (m_LaneSearchTree.TryGet(treeAction.m_Entity, ref quadTreeBoundsXZ))
				{
					quadTreeBoundsXZ.m_Mask = (quadTreeBoundsXZ.m_Mask & ~(BoundsMask.AllLayers | BoundsMask.NotOverridden)) | treeAction.m_Mask;
					m_LaneSearchTree.Update(treeAction.m_Entity, quadTreeBoundsXZ);
				}
			}
		}
	}

	[BurstCompile]
	private struct FindUpdatedLanesJob : IJobParallelForDefer
	{
		private struct Iterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ParallelWriter<Entity> m_ResultQueue;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity objectEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					m_ResultQueue.Enqueue(objectEntity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public ParallelWriter<Entity> m_ResultQueue;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Bounds = MathUtils.Expand(m_Bounds[index], float2.op_Implicit(1.6f)),
				m_ResultQueue = m_ResultQueue
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
		}
	}

	[BurstCompile]
	private struct CollectObjectsJob : IJob
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

		public NativeQueue<Entity> m_Queue;

		public NativeList<Entity> m_ResultList;

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
			int count = m_Queue.Count;
			m_ResultList.ResizeUninitialized(count);
			for (int i = 0; i < count; i++)
			{
				m_ResultList[i] = m_Queue.Dequeue();
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_ResultList, default(EntityComparer));
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_ResultList.Length)
			{
				Entity val2 = m_ResultList[num++];
				if (val2 != val)
				{
					m_ResultList[num2++] = val2;
					val = val2;
				}
			}
			if (num2 < m_ResultList.Length)
			{
				m_ResultList.RemoveRangeSwapBack(num2, m_ResultList.Length - num2);
			}
		}
	}

	[BurstCompile]
	private struct CheckLaneOverrideJob : IJobParallelForDefer
	{
		private struct LaneIteratorSubData
		{
			public Bounds3 m_Bounds1;

			public Bounds3 m_Bounds2;

			public Bezier4x3 m_Curve1;

			public Bezier4x3 m_Curve2;
		}

		private struct LaneIterator : INativeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, LaneIteratorSubData>, IUnsafeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, LaneIteratorSubData>
		{
			public float m_Range;

			public float m_SizeLimit;

			public float m_Priority;

			public Entity m_LaneEntity;

			public Curve m_LaneCurve;

			public NativeList<CutRange> m_CutRangeList;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

			public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

			public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

			public bool m_CutForTraffic;

			public bool m_FullOverride;

			public bool Intersect(QuadTreeBoundsXZ bounds, ref LaneIteratorSubData subData)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				if (m_FullOverride)
				{
					return false;
				}
				bool2 val = default(bool2);
				val.x = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds1);
				val.y = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds2);
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
					subData.m_Bounds1 = MathUtils.Expand(MathUtils.Bounds(subData.m_Curve1), float3.op_Implicit(m_Range));
					subData.m_Bounds2 = MathUtils.Expand(MathUtils.Bounds(subData.m_Curve2), float3.op_Implicit(m_Range));
					val.x = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds1);
					val.y = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds2);
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

			public void Iterate(QuadTreeBoundsXZ bounds, LaneIteratorSubData subData, Entity entity)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0310: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0392: Unknown result type (might be due to invalid IL or missing references)
				//IL_0352: Unknown result type (might be due to invalid IL or missing references)
				//IL_0361: Unknown result type (might be due to invalid IL or missing references)
				//IL_0373: Unknown result type (might be due to invalid IL or missing references)
				//IL_037d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				//IL_0189: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_0198: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0272: Unknown result type (might be due to invalid IL or missing references)
				//IL_0230: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				//IL_0242: Unknown result type (might be due to invalid IL or missing references)
				//IL_0247: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0200: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_0253: Unknown result type (might be due to invalid IL or missing references)
				//IL_0259: Unknown result type (might be due to invalid IL or missing references)
				//IL_025e: Unknown result type (might be due to invalid IL or missing references)
				//IL_025f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0263: Unknown result type (might be due to invalid IL or missing references)
				//IL_0268: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
				if (m_FullOverride || m_LaneEntity == entity)
				{
					return;
				}
				bool2 val = default(bool2);
				val.x = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds1);
				val.y = MathUtils.Intersect(bounds.m_Bounds, subData.m_Bounds2);
				if (!math.any(val))
				{
					return;
				}
				Bounds1 val2 = default(Bounds1);
				((Bounds1)(ref val2))._002Ector(1f, 0f);
				PrefabRef prefabRef = m_PrefabRefData[entity];
				UtilityLaneData utilityLaneData = default(UtilityLaneData);
				if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData))
				{
					if ((utilityLaneData.m_UtilityTypes & UtilityTypes.Fence) == 0 || utilityLaneData.m_VisualCapacity < m_Priority)
					{
						return;
					}
					Curve curve = m_CurveData[entity];
					NetLaneGeometryData netLaneGeometryData = m_PrefabLaneGeometryData[prefabRef.m_Prefab];
					float num = m_Range + netLaneGeometryData.m_Size.x * 0.5f;
					Bounds1 val3 = default(Bounds1);
					((Bounds1)(ref val3))._002Ector(1f, 0f);
					float num2 = default(float);
					if (MathUtils.Distance(m_LaneCurve.m_Bezier, curve.m_Bezier.a, ref num2) < num && IsParallel(MathUtils.Tangent(m_LaneCurve.m_Bezier, num2), MathUtils.StartTangent(curve.m_Bezier)))
					{
						val2 |= num2;
						val3 |= 0f;
					}
					float num3 = default(float);
					if (MathUtils.Distance(m_LaneCurve.m_Bezier, curve.m_Bezier.d, ref num3) < num && IsParallel(MathUtils.Tangent(m_LaneCurve.m_Bezier, num3), MathUtils.EndTangent(curve.m_Bezier)))
					{
						val2 |= num3;
						val3 |= 1f;
					}
					float num4 = default(float);
					if (MathUtils.Distance(curve.m_Bezier, m_LaneCurve.m_Bezier.a, ref num4) < num && IsParallel(MathUtils.Tangent(curve.m_Bezier, num4), MathUtils.StartTangent(m_LaneCurve.m_Bezier)))
					{
						val2 |= 0f;
						val3 |= num4;
					}
					float num5 = default(float);
					if (MathUtils.Distance(curve.m_Bezier, m_LaneCurve.m_Bezier.d, ref num5) < num && IsParallel(MathUtils.Tangent(curve.m_Bezier, num5), MathUtils.EndTangent(m_LaneCurve.m_Bezier)))
					{
						val2 |= 1f;
						val3 |= num5;
					}
					float num6 = MathUtils.Size(val2);
					float num7 = MathUtils.Size(val3);
					if (num6 <= 0f || num7 <= 0f || (m_Priority == utilityLaneData.m_VisualCapacity && (num7 > num6 || (num6 == num7 && m_LaneEntity.Index > entity.Index))))
					{
						return;
					}
				}
				else
				{
					NetLaneData netLaneData = default(NetLaneData);
					if (!m_CutForTraffic || !m_PrefabNetLaneData.TryGetComponent(prefabRef.m_Prefab, ref netLaneData) || (netLaneData.m_Flags & (LaneFlags.Road | LaneFlags.Pedestrian)) == 0)
					{
						return;
					}
					Curve curve2 = m_CurveData[entity];
					float2 val4 = default(float2);
					if (MathUtils.Intersect(((Bezier4x3)(ref m_LaneCurve.m_Bezier)).xz, ((Bezier4x3)(ref curve2.m_Bezier)).xz, ref val4, 3))
					{
						float num8 = netLaneData.m_Width * 0.5f / math.max(0.001f, m_LaneCurve.m_Length);
						val2.min = math.clamp(val4.x - num8, 0f, val2.min);
						val2.max = math.clamp(val4.x + num8, val2.max, 1f);
					}
					if (MathUtils.Size(val2) <= 0f)
					{
						return;
					}
				}
				AddCutRange(val2);
			}

			public void AddCutRange(Bounds1 range)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				if (range.min * m_LaneCurve.m_Length < m_Range)
				{
					range.min = 0f;
				}
				if ((1f - range.max) * m_LaneCurve.m_Length < m_Range)
				{
					range.max = 1f;
				}
				if (range.min == 0f && range.max == 1f)
				{
					m_FullOverride = true;
					return;
				}
				if (!m_CutRangeList.IsCreated)
				{
					m_CutRangeList = new NativeList<CutRange>(4, AllocatorHandle.op_Implicit((Allocator)2));
				}
				CutRange cutRange = new CutRange
				{
					m_CurveDelta = range
				};
				for (int i = 0; i < m_CutRangeList.Length; i++)
				{
					CutRange cutRange2 = m_CutRangeList[i];
					if (ShouldMerge(cutRange2, cutRange))
					{
						ref Bounds1 curveDelta = ref cutRange2.m_CurveDelta;
						curveDelta |= cutRange.m_CurveDelta;
						int num = 0;
						for (int j = i + 1; j < m_CutRangeList.Length; j++)
						{
							CutRange cutRange3 = m_CutRangeList[j];
							if (!ShouldMerge(cutRange2, cutRange3))
							{
								break;
							}
							ref Bounds1 curveDelta2 = ref cutRange2.m_CurveDelta;
							curveDelta2 |= cutRange3.m_CurveDelta;
							num++;
						}
						if (num != 0)
						{
							m_CutRangeList.RemoveRange(i + 1, num);
						}
						m_CutRangeList[i] = cutRange2;
						if (cutRange2.m_CurveDelta.min == 0f && cutRange2.m_CurveDelta.max == 1f)
						{
							m_FullOverride = true;
							m_CutRangeList.Clear();
						}
						return;
					}
					if (cutRange2.m_CurveDelta.min > cutRange.m_CurveDelta.min)
					{
						CollectionUtils.Insert<CutRange>(m_CutRangeList, i, cutRange);
						return;
					}
				}
				m_CutRangeList.Add(ref cutRange);
			}

			private bool IsParallel(float3 tangent1, float3 tangent2)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				return math.abs(math.dot(math.normalizesafe(((float3)(ref tangent1)).xz, default(float2)), math.normalizesafe(((float3)(ref tangent2)).xz, default(float2)))) > 0.95f;
			}

			private bool ShouldMerge(CutRange cutRange1, CutRange cutRange2)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				if ((cutRange1.m_CurveDelta.min - cutRange2.m_CurveDelta.max) * m_LaneCurve.m_Length < m_Range)
				{
					return (cutRange2.m_CurveDelta.min - cutRange1.m_CurveDelta.max) * m_LaneCurve.m_Length < m_Range;
				}
				return false;
			}
		}

		[ReadOnly]
		public ComponentLookup<Overridden> m_OverriddenData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<UtilityLane> m_UtilityLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabNetLaneData;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CutRange> m_CutRanges;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeArray<Entity> m_LaneArray;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<TreeAction> m_TreeActions;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_LaneArray[index];
			PrefabRef prefabRef = m_PrefabRefData[val];
			UtilityLaneData utilityLaneData = default(UtilityLaneData);
			if (!m_PrefabUtilityLaneData.TryGetComponent(prefabRef.m_Prefab, ref utilityLaneData) || (utilityLaneData.m_UtilityTypes & UtilityTypes.Fence) == 0)
			{
				return;
			}
			Curve curve = m_CurveData[val];
			UtilityLane utilityLane = m_UtilityLaneData[val];
			NetLaneGeometryData laneGeometryData = m_PrefabLaneGeometryData[prefabRef.m_Prefab];
			float num = laneGeometryData.m_Size.x * 0.5f + 1.6f;
			LaneIterator laneIterator = new LaneIterator
			{
				m_Range = num,
				m_SizeLimit = num * 4f,
				m_Priority = utilityLaneData.m_VisualCapacity,
				m_LaneEntity = val,
				m_LaneCurve = curve,
				m_CurveData = m_CurveData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabUtilityLaneData = m_PrefabUtilityLaneData,
				m_PrefabNetLaneData = m_PrefabNetLaneData,
				m_PrefabLaneGeometryData = m_PrefabLaneGeometryData,
				m_CutForTraffic = ((utilityLane.m_Flags & UtilityLaneFlags.CutForTraffic) != 0)
			};
			LaneIteratorSubData laneIteratorSubData = default(LaneIteratorSubData);
			MathUtils.Divide(curve.m_Bezier, ref laneIteratorSubData.m_Curve1, ref laneIteratorSubData.m_Curve2, 0.5f);
			laneIteratorSubData.m_Bounds1 = MathUtils.Expand(MathUtils.Bounds(laneIteratorSubData.m_Curve1), float3.op_Implicit(num));
			laneIteratorSubData.m_Bounds2 = MathUtils.Expand(MathUtils.Bounds(laneIteratorSubData.m_Curve2), float3.op_Implicit(num));
			m_LaneSearchTree.Iterate<LaneIterator, LaneIteratorSubData>(ref laneIterator, laneIteratorSubData, 0);
			if (laneIterator.m_CutForTraffic && !laneIterator.m_FullOverride && (!laneIterator.m_CutRangeList.IsCreated || laneIterator.m_CutRangeList.Length == 0))
			{
				float num2 = math.min(0.25f, 3f / math.max(0.001f, curve.m_Length));
				laneIterator.AddCutRange(new Bounds1(0.5f - num2, 0.5f + num2));
			}
			m_CurveData = laneIterator.m_CurveData;
			m_PrefabRefData = laneIterator.m_PrefabRefData;
			m_PrefabUtilityLaneData = laneIterator.m_PrefabUtilityLaneData;
			m_PrefabNetLaneData = laneIterator.m_PrefabNetLaneData;
			m_PrefabLaneGeometryData = laneIterator.m_PrefabLaneGeometryData;
			if (laneIterator.m_FullOverride)
			{
				if (!m_OverriddenData.HasComponent(val))
				{
					if (m_CutRanges.HasBuffer(val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CutRange>(index, val);
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Overridden>(index, val, default(Overridden));
					AddTreeAction(val, curve, utilityLane, laneGeometryData, isOverridden: true);
				}
			}
			else if (laneIterator.m_CutRangeList.IsCreated && laneIterator.m_CutRangeList.Length != 0)
			{
				DynamicBuffer<CutRange> cutRanges = default(DynamicBuffer<CutRange>);
				if (m_OverriddenData.HasComponent(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Overridden>(index, val);
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(index, val).AddRange(laneIterator.m_CutRangeList.AsArray());
					AddTreeAction(val, curve, utilityLane, laneGeometryData, isOverridden: false);
				}
				else if (m_CutRanges.TryGetBuffer(val, ref cutRanges))
				{
					if (!IsEqual(cutRanges, laneIterator.m_CutRangeList))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
						cutRanges.CopyFrom(laneIterator.m_CutRangeList.AsArray());
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<CutRange>(index, val).AddRange(laneIterator.m_CutRangeList.AsArray());
				}
			}
			else if (m_OverriddenData.HasComponent(val))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Overridden>(index, val);
				AddTreeAction(val, curve, utilityLane, laneGeometryData, isOverridden: false);
			}
			else if (m_CutRanges.HasBuffer(val))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CutRange>(index, val);
			}
			if (laneIterator.m_CutRangeList.IsCreated)
			{
				laneIterator.m_CutRangeList.Dispose();
			}
		}

		private void AddTreeAction(Entity entity, Curve curve, UtilityLane utilityLane, NetLaneGeometryData laneGeometryData, bool isOverridden)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			TreeAction treeAction = new TreeAction
			{
				m_Entity = entity
			};
			if (!isOverridden)
			{
				treeAction.m_Mask |= BoundsMask.NotOverridden;
				if (curve.m_Length > 0.1f)
				{
					MeshLayer defaultLayers = (m_EditorMode ? laneGeometryData.m_EditorLayers : laneGeometryData.m_GameLayers);
					Owner owner = default(Owner);
					m_OwnerData.TryGetComponent(entity, ref owner);
					treeAction.m_Mask |= CommonUtils.GetBoundsMask(SearchSystem.GetLayers(owner, utilityLane, defaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
				}
			}
			m_TreeActions.Enqueue(treeAction);
		}

		private bool IsEqual(DynamicBuffer<CutRange> cutRanges1, NativeList<CutRange> cutRanges2)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (cutRanges1.Length != cutRanges2.Length)
			{
				return false;
			}
			for (int i = 0; i < cutRanges1.Length; i++)
			{
				CutRange cutRange = cutRanges1[i];
				if (!((Bounds1)(ref cutRange.m_CurveDelta)).Equals(cutRanges2[i].m_CurveDelta))
				{
					return false;
				}
			}
			return true;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Overridden> __Game_Common_Overridden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLane> __Game_Net_UtilityLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> __Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		public BufferLookup<CutRange> __Game_Net_CutRange_RW_BufferLookup;

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
			__Game_Common_Overridden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Overridden>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_UtilityLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneGeometryData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_CutRange_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CutRange>(false);
		}
	}

	private const float MIN_PARALLEL_FENCE_DISTANCE = 1.6f;

	private UpdateCollectSystem m_NetUpdateCollectSystem;

	private SearchSystem m_NetSearchSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private ToolSystem m_ToolSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateCollectSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		if (m_NetUpdateCollectSystem.lanesUpdated)
		{
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<TreeAction> actions = default(NativeQueue<TreeAction>);
			actions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, CollectUpdatedLanes(val));
			JobHandle dependencies;
			CheckLaneOverrideJob checkLaneOverrideJob = new CheckLaneOverrideJob
			{
				m_OverriddenData = InternalCompilerInterface.GetComponentLookup<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLaneGeometryData = InternalCompilerInterface.GetComponentLookup<NetLaneGeometryData>(ref __TypeHandle.__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CutRanges = InternalCompilerInterface.GetBufferLookup<CutRange>(ref __TypeHandle.__Game_Net_CutRange_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_LaneArray = val.AsDeferredJobArray(),
				m_LaneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies)
			};
			EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
			checkLaneOverrideJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkLaneOverrideJob.m_TreeActions = actions.AsParallelWriter();
			CheckLaneOverrideJob checkLaneOverrideJob2 = checkLaneOverrideJob;
			JobHandle dependencies2;
			UpdateOverriddenLayersJob obj = new UpdateOverriddenLayersJob
			{
				m_LaneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: false, out dependencies2),
				m_Actions = actions
			};
			JobHandle val3 = IJobParallelForDeferExtensions.Schedule<CheckLaneOverrideJob, Entity>(checkLaneOverrideJob2, val, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			JobHandle val4 = IJobExtensions.Schedule<UpdateOverriddenLayersJob>(obj, JobHandle.CombineDependencies(val3, dependencies2));
			val.Dispose(val3);
			actions.Dispose(val4);
			m_NetSearchSystem.AddLaneSearchTreeWriter(val4);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
		}
	}

	private JobHandle CollectUpdatedLanes(NativeList<Entity> updateLanesList)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> queue = default(NativeQueue<Entity>);
		queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeQuadTree<Entity, QuadTreeBoundsXZ> laneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies);
		JobHandle val = default(JobHandle);
		if (m_NetUpdateCollectSystem.lanesUpdated)
		{
			JobHandle dependencies2;
			NativeList<Bounds2> updatedLaneBounds = m_NetUpdateCollectSystem.GetUpdatedLaneBounds(out dependencies2);
			JobHandle val2 = IJobParallelForDeferExtensions.Schedule<FindUpdatedLanesJob, Bounds2>(new FindUpdatedLanesJob
			{
				m_Bounds = updatedLaneBounds.AsDeferredJobArray(),
				m_SearchTree = laneSearchTree,
				m_ResultQueue = queue.AsParallelWriter()
			}, updatedLaneBounds, 1, JobHandle.CombineDependencies(dependencies2, dependencies));
			m_NetUpdateCollectSystem.AddLaneBoundsReader(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		JobHandle val3 = IJobExtensions.Schedule<CollectObjectsJob>(new CollectObjectsJob
		{
			m_Queue = queue,
			m_ResultList = updateLanesList
		}, val);
		queue.Dispose(val3);
		m_NetSearchSystem.AddLaneSearchTreeReader(val);
		return val3;
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
	public OverrideSystem()
	{
	}
}
