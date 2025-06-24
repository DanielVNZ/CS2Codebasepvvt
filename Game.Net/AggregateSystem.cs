using System.Runtime.CompilerServices;
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
public class AggregateSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateAgregatesJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<AggregateNetData> m_PrefabAggregateData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public ComponentLookup<Aggregated> m_AggregatedData;

		public BufferLookup<AggregateElement> m_AggregateElements;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				num += ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashSet<Entity> edgeSet = default(NativeParallelHashSet<Entity>);
			edgeSet._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashSet<Entity> emptySet = default(NativeParallelHashSet<Entity>);
			emptySet._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashMap<Entity, Entity> updateMap = default(NativeParallelHashMap<Entity, Entity>);
			updateMap._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val2 = m_Chunks[j];
				bool flag = ((ArchetypeChunk)(ref val2)).Has<Temp>(ref m_TempType);
				if (((ArchetypeChunk)(ref val2)).Has<Created>(ref m_CreatedType))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
					for (int k = 0; k < nativeArray.Length; k++)
					{
						Entity val3 = nativeArray[k];
						Aggregated aggregated = m_AggregatedData[val3];
						if (aggregated.m_Aggregate != Entity.Null)
						{
							if (flag && !m_TempData.HasComponent(aggregated.m_Aggregate))
							{
								updateMap.TryAdd(aggregated.m_Aggregate, aggregated.m_Aggregate);
								continue;
							}
							m_AggregateElements[aggregated.m_Aggregate].Add(new AggregateElement(val3));
							edgeSet.Add(aggregated.m_Aggregate);
						}
						else
						{
							emptySet.Add(val3);
						}
					}
					continue;
				}
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				for (int l = 0; l < nativeArray2.Length; l++)
				{
					Entity val4 = nativeArray2[l];
					Aggregated aggregated2 = m_AggregatedData[val4];
					if (aggregated2.m_Aggregate != Entity.Null)
					{
						if (flag && !m_TempData.HasComponent(aggregated2.m_Aggregate))
						{
							updateMap.TryAdd(aggregated2.m_Aggregate, aggregated2.m_Aggregate);
						}
						else
						{
							edgeSet.Add(aggregated2.m_Aggregate);
						}
					}
				}
			}
			if (!edgeSet.IsEmpty)
			{
				NativeArray<Entity> val5 = edgeSet.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
				edgeSet.Clear();
				for (int m = 0; m < val5.Length; m++)
				{
					ValidateAggregate(val5[m], edgeSet, emptySet, updateMap);
				}
				for (int n = 0; n < val5.Length; n++)
				{
					CombineAggregate(val5[n], updateMap);
				}
				val5.Dispose();
			}
			if (!emptySet.IsEmpty)
			{
				NativeArray<Entity> val6 = emptySet.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
				NativeList<AggregateElement> edgeList = default(NativeList<AggregateElement>);
				edgeList._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				for (int num2 = 0; num2 < val6.Length; num2++)
				{
					Entity val7 = val6[num2];
					if (emptySet.Contains(val7))
					{
						emptySet.Remove(val7);
						CreateAggregate(val7, emptySet, edgeList, updateMap);
					}
				}
				edgeList.Dispose();
				val6.Dispose();
			}
			if (!updateMap.IsEmpty)
			{
				Entity val10 = default(Entity);
				for (int num3 = 0; num3 < m_Chunks.Length; num3++)
				{
					ArchetypeChunk val8 = m_Chunks[num3];
					if (!((ArchetypeChunk)(ref val8)).Has<Temp>(ref m_TempType))
					{
						continue;
					}
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val8)).GetNativeArray(m_EntityType);
					for (int num4 = 0; num4 < nativeArray3.Length; num4++)
					{
						Entity val9 = nativeArray3[num4];
						Aggregated aggregated3 = m_AggregatedData[val9];
						if (aggregated3.m_Aggregate != Entity.Null && updateMap.TryGetValue(aggregated3.m_Aggregate, ref val10) && val10 != aggregated3.m_Aggregate)
						{
							aggregated3.m_Aggregate = val10;
							m_AggregatedData[val9] = aggregated3;
						}
					}
				}
				Enumerator<Entity, Entity> enumerator = updateMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Entity value = enumerator.Current.Value;
					if (m_AggregateElements.HasBuffer(value))
					{
						if (m_DeletedData.HasComponent(value))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(value);
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(value, default(Updated));
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(value, default(BatchesUpdated));
						}
					}
				}
			}
			edgeSet.Dispose();
			emptySet.Dispose();
			updateMap.Dispose();
		}

		private void CreateAggregate(Entity startEdge, NativeParallelHashSet<Entity> emptySet, NativeList<AggregateElement> edgeList, NativeParallelHashMap<Entity, Entity> updateMap)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			Entity aggregateType = GetAggregateType(startEdge);
			if (aggregateType == Entity.Null)
			{
				return;
			}
			Edge edge = m_EdgeData[startEdge];
			AddElements(startEdge, edge.m_Start, isStartNode: true, aggregateType, emptySet, edgeList);
			CollectionUtils.Reverse<AggregateElement>(edgeList.AsArray());
			AggregateElement aggregateElement = new AggregateElement(startEdge);
			edgeList.Add(ref aggregateElement);
			AddElements(startEdge, edge.m_End, isStartNode: false, aggregateType, emptySet, edgeList);
			bool flag = m_TempData.HasComponent(startEdge);
			if (!TryCombine(aggregateType, edgeList, flag, updateMap))
			{
				AggregateNetData aggregateNetData = m_PrefabAggregateData[aggregateType];
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(aggregateNetData.m_Archetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef(aggregateType));
				DynamicBuffer<AggregateElement> val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<AggregateElement>(val);
				if (flag)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val, new Temp(Entity.Null, TempFlags.Create));
				}
				for (int i = 0; i < edgeList.Length; i++)
				{
					AggregateElement aggregateElement2 = edgeList[i];
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Aggregated>(aggregateElement2.m_Edge, new Aggregated
					{
						m_Aggregate = val
					});
					val2.Add(aggregateElement2);
				}
			}
			edgeList.Clear();
		}

		private bool TryCombine(Entity prefab, NativeList<AggregateElement> edgeList, bool isTemp, NativeParallelHashMap<Entity, Entity> updateMap)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			if (GetStart(edgeList.AsArray(), out var edge, out var node, out var isStart) && ShouldCombine(edge, node, isStart, prefab, Entity.Null, isTemp, out var otherAggregate, out var otherIsStart))
			{
				DynamicBuffer<AggregateElement> val = m_AggregateElements[otherAggregate];
				int length = val.Length;
				val.ResizeUninitialized(val.Length + edgeList.Length);
				if (otherIsStart)
				{
					for (int num = length - 1; num >= 0; num--)
					{
						val[edgeList.Length + num] = val[num];
					}
				}
				for (int i = 0; i < edgeList.Length; i++)
				{
					AggregateElement aggregateElement = edgeList[i];
					m_AggregatedData[aggregateElement.m_Edge] = new Aggregated
					{
						m_Aggregate = otherAggregate
					};
					val[math.select(length, 0, otherIsStart) + edgeList.Length - i - 1] = aggregateElement;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(otherAggregate);
				if (GetEnd(edgeList.AsArray(), out var edge2, out var node2, out var isStart2) && ShouldCombine(edge2, node2, isStart2, prefab, otherAggregate, isTemp, out var otherAggregate2, out var otherIsStart2))
				{
					DynamicBuffer<AggregateElement> val2 = m_AggregateElements[otherAggregate2];
					length = val.Length;
					val.ResizeUninitialized(val2.Length + val.Length);
					if (otherIsStart)
					{
						for (int num2 = length - 1; num2 >= 0; num2--)
						{
							val[val2.Length + num2] = val[num2];
						}
					}
					for (int j = 0; j < val2.Length; j++)
					{
						AggregateElement aggregateElement2 = val2[j];
						m_AggregatedData[aggregateElement2.m_Edge] = new Aggregated
						{
							m_Aggregate = otherAggregate
						};
						val[math.select(length, 0, otherIsStart) + math.select(j, val2.Length - j - 1, otherIsStart2 == otherIsStart)] = aggregateElement2;
					}
					val2.Clear();
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(otherAggregate2);
					if (updateMap.ContainsKey(otherAggregate2))
					{
						updateMap[otherAggregate2] = otherAggregate;
					}
				}
				return true;
			}
			if (GetEnd(edgeList.AsArray(), out var edge3, out var node3, out var isStart3) && ShouldCombine(edge3, node3, isStart3, prefab, Entity.Null, isTemp, out var otherAggregate3, out var otherIsStart3))
			{
				DynamicBuffer<AggregateElement> val3 = m_AggregateElements[otherAggregate3];
				int length2 = val3.Length;
				val3.ResizeUninitialized(val3.Length + edgeList.Length);
				if (otherIsStart3)
				{
					for (int num3 = length2 - 1; num3 >= 0; num3--)
					{
						val3[edgeList.Length + num3] = val3[num3];
					}
				}
				for (int k = 0; k < edgeList.Length; k++)
				{
					AggregateElement aggregateElement3 = edgeList[k];
					m_AggregatedData[aggregateElement3.m_Edge] = new Aggregated
					{
						m_Aggregate = otherAggregate3
					};
					val3[math.select(length2, 0, otherIsStart3) + k] = aggregateElement3;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(otherAggregate3);
				return true;
			}
			return false;
		}

		private bool GetBestConnectionEdge(Entity prefab, Entity prevEdge, Entity prevNode, bool prevIsStart, out Entity nextEdge, out Entity nextNode, out bool nextIsStart)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			Curve curve = m_CurveData[prevEdge];
			float3 val;
			float2 val2;
			float2 val3;
			float2 xz;
			if (prevIsStart)
			{
				val = MathUtils.StartTangent(curve.m_Bezier);
				val2 = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
				val3 = math.normalizesafe(((float3)(ref curve.m_Bezier.a)).xz - ((float3)(ref curve.m_Bezier.d)).xz, default(float2));
				xz = ((float3)(ref curve.m_Bezier.a)).xz;
			}
			else
			{
				val = MathUtils.EndTangent(curve.m_Bezier);
				val2 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
				val3 = math.normalizesafe(((float3)(ref curve.m_Bezier.d)).xz - ((float3)(ref curve.m_Bezier.a)).xz, default(float2));
				xz = ((float3)(ref curve.m_Bezier.d)).xz;
			}
			DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[prevNode];
			float num = 2f;
			nextEdge = Entity.Null;
			nextNode = Entity.Null;
			nextIsStart = false;
			for (int i = 0; i < val4.Length; i++)
			{
				ConnectedEdge connectedEdge = val4[i];
				if (connectedEdge.m_Edge == prevEdge)
				{
					continue;
				}
				Edge edge = m_EdgeData[connectedEdge.m_Edge];
				if (edge.m_Start == prevNode)
				{
					if (GetAggregateType(connectedEdge.m_Edge) == prefab)
					{
						Curve curve2 = m_CurveData[connectedEdge.m_Edge];
						val = MathUtils.StartTangent(curve2.m_Bezier);
						float2 val5 = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
						float2 val6 = math.normalizesafe(((float3)(ref curve2.m_Bezier.a)).xz - ((float3)(ref curve2.m_Bezier.d)).xz, default(float2));
						float2 val7 = ((float3)(ref curve2.m_Bezier.a)).xz - xz;
						float num2 = math.abs(math.dot(val7, MathUtils.Right(val2))) + math.abs(math.dot(val7, MathUtils.Right(val5)));
						num2 = 0.5f - 0.5f / (1f + num2 * 0.1f);
						float num3 = math.dot(val2, val5) + math.dot(val3, val6) * 0.5f + num2;
						if (num3 < num)
						{
							num = num3;
							nextEdge = connectedEdge.m_Edge;
							nextNode = edge.m_End;
							nextIsStart = false;
						}
					}
				}
				else if (edge.m_End == prevNode && GetAggregateType(connectedEdge.m_Edge) == prefab)
				{
					Curve curve3 = m_CurveData[connectedEdge.m_Edge];
					val = MathUtils.EndTangent(curve3.m_Bezier);
					float2 val8 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
					float2 val9 = math.normalizesafe(((float3)(ref curve3.m_Bezier.d)).xz - ((float3)(ref curve3.m_Bezier.a)).xz, default(float2));
					float2 val10 = ((float3)(ref curve3.m_Bezier.d)).xz - xz;
					float num4 = math.abs(math.dot(val10, MathUtils.Right(val2))) + math.abs(math.dot(val10, MathUtils.Right(val8)));
					num4 = 0.5f - 0.5f / (1f + num4 * 0.1f);
					float num5 = math.dot(val2, val8) + math.dot(val3, val9) * 0.5f + num4;
					if (num5 < num)
					{
						num = num5;
						nextEdge = connectedEdge.m_Edge;
						nextNode = edge.m_Start;
						nextIsStart = true;
					}
				}
			}
			return nextEdge != Entity.Null;
		}

		private void AddElements(Entity startEdge, Entity startNode, bool isStartNode, Entity prefab, NativeParallelHashSet<Entity> emptySet, NativeList<AggregateElement> elements)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			Entity nextEdge;
			Entity nextNode;
			bool nextIsStart;
			Entity nextEdge2;
			Entity nextNode2;
			bool nextIsStart2;
			while (GetBestConnectionEdge(prefab, startEdge, startNode, isStartNode, out nextEdge, out nextNode, out nextIsStart) && GetBestConnectionEdge(prefab, nextEdge, startNode, !nextIsStart, out nextEdge2, out nextNode2, out nextIsStart2) && nextEdge2 == startEdge && emptySet.Contains(nextEdge))
			{
				AggregateElement aggregateElement = new AggregateElement(nextEdge);
				elements.Add(ref aggregateElement);
				emptySet.Remove(nextEdge);
				startEdge = nextEdge;
				startNode = nextNode;
				isStartNode = nextIsStart;
			}
		}

		private void CombineAggregate(Entity aggregate, NativeParallelHashMap<Entity, Entity> updateMap)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<AggregateElement> val = m_AggregateElements[aggregate];
			Entity prefab = m_PrefabRefData[aggregate].m_Prefab;
			bool isTemp = m_TempData.HasComponent(aggregate);
			Entity edge;
			Entity node;
			bool isStart;
			Entity otherAggregate;
			bool otherIsStart;
			while (GetStart(val.AsNativeArray(), out edge, out node, out isStart) && ShouldCombine(edge, node, isStart, prefab, aggregate, isTemp, out otherAggregate, out otherIsStart))
			{
				DynamicBuffer<AggregateElement> val2 = m_AggregateElements[otherAggregate];
				int length = val.Length;
				val.ResizeUninitialized(val2.Length + val.Length);
				for (int num = length - 1; num >= 0; num--)
				{
					val[val2.Length + num] = val[num];
				}
				for (int i = 0; i < val2.Length; i++)
				{
					AggregateElement aggregateElement = val2[i];
					m_AggregatedData[aggregateElement.m_Edge] = new Aggregated
					{
						m_Aggregate = aggregate
					};
					val[math.select(i, val2.Length - i - 1, otherIsStart)] = aggregateElement;
				}
				val2.Clear();
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(otherAggregate);
				if (updateMap.ContainsKey(otherAggregate))
				{
					updateMap[otherAggregate] = aggregate;
				}
			}
			Entity edge2;
			Entity node2;
			bool isStart2;
			Entity otherAggregate2;
			bool otherIsStart2;
			while (GetEnd(val.AsNativeArray(), out edge2, out node2, out isStart2) && ShouldCombine(edge2, node2, isStart2, prefab, aggregate, isTemp, out otherAggregate2, out otherIsStart2))
			{
				DynamicBuffer<AggregateElement> val3 = m_AggregateElements[otherAggregate2];
				int length2 = val.Length;
				val.ResizeUninitialized(val3.Length + val.Length);
				for (int j = 0; j < val3.Length; j++)
				{
					AggregateElement aggregateElement2 = val3[j];
					m_AggregatedData[aggregateElement2.m_Edge] = new Aggregated
					{
						m_Aggregate = aggregate
					};
					val[length2 + math.select(j, val3.Length - j - 1, otherIsStart2)] = aggregateElement2;
				}
				val3.Clear();
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(otherAggregate2);
				if (updateMap.ContainsKey(otherAggregate2))
				{
					updateMap[otherAggregate2] = aggregate;
				}
			}
		}

		private bool GetStart(NativeArray<AggregateElement> elements, out Entity edge, out Entity node, out bool isStart)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			if (elements.Length == 0)
			{
				edge = Entity.Null;
				node = Entity.Null;
				isStart = false;
				return false;
			}
			if (elements.Length == 1)
			{
				edge = elements[0].m_Edge;
				node = m_EdgeData[edge].m_Start;
				isStart = true;
				return true;
			}
			edge = elements[0].m_Edge;
			Entity edge2 = elements[1].m_Edge;
			Edge edge3 = m_EdgeData[edge];
			Edge edge4 = m_EdgeData[edge2];
			if (edge3.m_End == edge4.m_Start || edge3.m_End == edge4.m_End)
			{
				node = edge3.m_Start;
				isStart = true;
			}
			else
			{
				node = edge3.m_End;
				isStart = false;
			}
			return true;
		}

		private bool GetEnd(NativeArray<AggregateElement> elements, out Entity edge, out Entity node, out bool isStart)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (elements.Length == 0)
			{
				edge = Entity.Null;
				node = Entity.Null;
				isStart = false;
				return false;
			}
			if (elements.Length == 1)
			{
				edge = elements[0].m_Edge;
				node = m_EdgeData[edge].m_End;
				isStart = false;
				return true;
			}
			edge = elements[elements.Length - 1].m_Edge;
			Entity edge2 = elements[elements.Length - 2].m_Edge;
			Edge edge3 = m_EdgeData[edge];
			Edge edge4 = m_EdgeData[edge2];
			if (edge3.m_End == edge4.m_Start || edge3.m_End == edge4.m_End)
			{
				node = edge3.m_Start;
				isStart = true;
			}
			else
			{
				node = edge3.m_End;
				isStart = false;
			}
			return true;
		}

		private void ValidateAggregate(Entity aggregate, NativeParallelHashSet<Entity> edgeSet, NativeParallelHashSet<Entity> emptySet, NativeParallelHashMap<Entity, Entity> updateMap)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<AggregateElement> elements = m_AggregateElements[aggregate];
			Entity val = Entity.Null;
			Entity prefab = m_PrefabRefData[aggregate].m_Prefab;
			for (int i = 0; i < elements.Length; i++)
			{
				AggregateElement aggregateElement = elements[i];
				if (!m_DeletedData.HasComponent(aggregateElement.m_Edge))
				{
					if (GetAggregateType(aggregateElement.m_Edge) != prefab)
					{
						emptySet.Add(aggregateElement.m_Edge);
						m_AggregatedData[aggregateElement.m_Edge] = default(Aggregated);
					}
					else if (val == Entity.Null)
					{
						val = aggregateElement.m_Edge;
					}
					else
					{
						edgeSet.Add(aggregateElement.m_Edge);
					}
				}
			}
			elements.Clear();
			if (val == Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(aggregate);
				if (updateMap.ContainsKey(aggregate))
				{
					updateMap[aggregate] = Entity.Null;
				}
			}
			else
			{
				Edge edge = m_EdgeData[val];
				AddElements(val, edge.m_Start, isStartNode: true, prefab, edgeSet, elements);
				CollectionUtils.Reverse<AggregateElement>(elements.AsNativeArray());
				int length = elements.Length;
				elements.Add(new AggregateElement(val));
				AddElements(val, edge.m_End, isStartNode: false, prefab, edgeSet, elements);
				if (length > elements.Length - length - 1)
				{
					CollectionUtils.Reverse<AggregateElement>(elements.AsNativeArray());
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(aggregate);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(aggregate);
			}
			if (!edgeSet.IsEmpty)
			{
				NativeArray<Entity> val2 = edgeSet.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
				for (int j = 0; j < val2.Length; j++)
				{
					Entity val3 = val2[j];
					emptySet.Add(val3);
					m_AggregatedData[val3] = default(Aggregated);
				}
				val2.Dispose();
				edgeSet.Clear();
			}
		}

		private bool ShouldCombine(Entity startEdge, Entity startNode, bool isStartNode, Entity prefab, Entity aggregate, bool isTemp, out Entity otherAggregate, out bool otherIsStart)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			if (GetBestConnectionEdge(prefab, startEdge, startNode, isStartNode, out var nextEdge, out var nextNode, out var nextIsStart) && GetBestConnectionEdge(prefab, nextEdge, startNode, !nextIsStart, out var nextEdge2, out nextNode, out var _) && nextEdge2 == startEdge && m_AggregatedData.HasComponent(nextEdge))
			{
				Aggregated aggregated = m_AggregatedData[nextEdge];
				if (aggregated.m_Aggregate != aggregate && m_AggregateElements.HasBuffer(aggregated.m_Aggregate) && m_TempData.HasComponent(aggregated.m_Aggregate) == isTemp)
				{
					DynamicBuffer<AggregateElement> val = m_AggregateElements[aggregated.m_Aggregate];
					if (val[0].m_Edge == nextEdge)
					{
						otherAggregate = aggregated.m_Aggregate;
						otherIsStart = true;
						return true;
					}
					if (val[val.Length - 1].m_Edge == nextEdge)
					{
						otherAggregate = aggregated.m_Aggregate;
						otherIsStart = false;
						return true;
					}
				}
			}
			otherAggregate = Entity.Null;
			otherIsStart = false;
			return false;
		}

		private void AddElements(Entity startEdge, Entity startNode, bool isStartNode, Entity prefab, NativeParallelHashSet<Entity> edgeSet, DynamicBuffer<AggregateElement> elements)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			Entity nextEdge;
			Entity nextNode;
			bool nextIsStart;
			Entity nextEdge2;
			Entity nextNode2;
			bool nextIsStart2;
			while (GetBestConnectionEdge(prefab, startEdge, startNode, isStartNode, out nextEdge, out nextNode, out nextIsStart) && GetBestConnectionEdge(prefab, nextEdge, startNode, !nextIsStart, out nextEdge2, out nextNode2, out nextIsStart2) && nextEdge2 == startEdge && edgeSet.Contains(nextEdge))
			{
				elements.Add(new AggregateElement(nextEdge));
				edgeSet.Remove(nextEdge);
				startEdge = nextEdge;
				startNode = nextNode;
				isStartNode = nextIsStart;
			}
		}

		private Entity GetAggregateType(Entity edge)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[edge];
			return m_PrefabGeometryData[prefabRef.m_Prefab].m_AggregateType;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AggregateNetData> __Game_Prefabs_AggregateNetData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		public ComponentLookup<Aggregated> __Game_Net_Aggregated_RW_ComponentLookup;

		public BufferLookup<AggregateElement> __Game_Net_AggregateElement_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_AggregateNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AggregateNetData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_Aggregated_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aggregated>(false);
			__Game_Net_AggregateElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AggregateElement>(false);
		}
	}

	private EntityQuery m_ModifiedQuery;

	private ModificationBarrier2B m_ModificationBarrier;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2B>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Aggregated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_ModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_ModifiedQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_ModifiedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<UpdateAgregatesJob>(new UpdateAgregatesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAggregateData = InternalCompilerInterface.GetComponentLookup<AggregateNetData>(ref __TypeHandle.__Game_Prefabs_AggregateNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AggregatedData = InternalCompilerInterface.GetComponentLookup<Aggregated>(ref __TypeHandle.__Game_Net_Aggregated_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElements = InternalCompilerInterface.GetBufferLookup<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = chunks,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
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
	public AggregateSystem()
	{
	}
}
