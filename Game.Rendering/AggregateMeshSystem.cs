using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Serialization;
using Game.Tools;
using Game.UI;
using TMPro;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class AggregateMeshSystem : GameSystemBase, IPreDeserialize
{
	private struct MaterialData
	{
		public Material m_Material;

		public bool m_IsUnderground;

		public bool m_HasMesh;
	}

	private class MeshData
	{
		public JobHandle m_DataDependencies;

		public Material m_OriginalMaterial;

		public List<MaterialData> m_Materials;

		public Mesh m_Mesh;

		public MeshDataArray m_MeshData;

		public bool m_MeshDirty;

		public bool m_HasMeshData;

		public bool m_HasMesh;
	}

	private struct MaterialUpdate
	{
		public Entity m_Entity;

		public int m_Material;
	}

	[BurstCompile]
	private struct UpdateLabelPositionsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<LabelExtents> m_LabelExtentsType;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> m_AggregateElementType;

		public BufferTypeHandle<LabelPosition> m_LabelPositionType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<LabelExtents> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LabelExtents>(ref m_LabelExtentsType);
			BufferAccessor<AggregateElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AggregateElement>(ref m_AggregateElementType);
			BufferAccessor<LabelPosition> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LabelPosition>(ref m_LabelPositionType);
			NativeList<float> redundancyBuffer = default(NativeList<float>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				LabelExtents labelExtents = nativeArray[i];
				DynamicBuffer<AggregateElement> aggregateElements = bufferAccessor[i];
				DynamicBuffer<LabelPosition> labelPositions = bufferAccessor2[i];
				labelPositions.Clear();
				if (aggregateElements.Length <= 0)
				{
					continue;
				}
				if (!redundancyBuffer.IsCreated)
				{
					redundancyBuffer._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				}
				int startIndex = 0;
				int j = 1;
				Edge edge = m_EdgeData[aggregateElements[0].m_Edge];
				for (; j < aggregateElements.Length; j++)
				{
					Edge edge2 = m_EdgeData[aggregateElements[j].m_Edge];
					if (edge2.m_Start == edge.m_End || edge2.m_Start == edge.m_Start)
					{
						if (m_ConnectedEdges[edge2.m_Start].Length > 2)
						{
							AddLabels(startIndex, j, labelExtents, redundancyBuffer, aggregateElements, labelPositions);
							startIndex = j;
						}
					}
					else if (edge2.m_End == edge.m_End || edge2.m_End == edge.m_Start)
					{
						if (m_ConnectedEdges[edge2.m_End].Length > 2)
						{
							AddLabels(startIndex, j, labelExtents, redundancyBuffer, aggregateElements, labelPositions);
							startIndex = j;
						}
					}
					else
					{
						AddLabels(startIndex, j, labelExtents, redundancyBuffer, aggregateElements, labelPositions);
						startIndex = j;
					}
					edge = edge2;
				}
				AddLabels(startIndex, j, labelExtents, redundancyBuffer, aggregateElements, labelPositions);
				float num = 0f;
				for (int k = 0; k < redundancyBuffer.Length; k++)
				{
					num += redundancyBuffer[k];
				}
				float num2 = (math.max(1f, math.round(num)) - num) * 0.5f;
				float num3 = 0f;
				for (int l = 0; l < redundancyBuffer.Length; l++)
				{
					float num4 = redundancyBuffer[l];
					num2 += num4;
					if (num2 < 0.5f)
					{
						LabelPosition labelPosition = labelPositions[l];
						num3 += num4;
						if (num3 < 0.25f)
						{
							labelPosition.m_MaxScale *= 0.25f;
						}
						else
						{
							labelPosition.m_MaxScale *= 0.5f;
							num3 = ((!(num4 < 0.25f)) ? 0f : (num3 - 0.5f));
						}
						labelPositions[l] = labelPosition;
					}
					else
					{
						num2 -= 1f;
						num3 = 0f;
					}
				}
				redundancyBuffer.Clear();
			}
			if (redundancyBuffer.IsCreated)
			{
				redundancyBuffer.Dispose();
			}
		}

		private void AddLabels(int startIndex, int endIndex, LabelExtents labelExtents, NativeList<float> redundancyBuffer, DynamicBuffer<AggregateElement> aggregateElements, DynamicBuffer<LabelPosition> labelPositions)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = startIndex; i < endIndex; i++)
			{
				Entity edge = aggregateElements[i].m_Edge;
				Curve curve = m_CurveData[edge];
				Composition composition = m_CompositionData[edge];
				float num2 = math.sqrt(math.max(1f, m_NetCompositionData[composition.m_Edge].m_Width));
				num += curve.m_Length / num2;
			}
			float num3 = 100f;
			int num4 = math.clamp(Mathf.RoundToInt(num / num3), 1, endIndex - startIndex);
			num3 = num / (float)num4;
			float num5 = num3 / 100f;
			float num6 = 0f;
			float num7 = 0f;
			int num8 = -1;
			LabelPosition labelPosition = default(LabelPosition);
			for (int j = startIndex; j < endIndex; j++)
			{
				Entity edge2 = aggregateElements[j].m_Edge;
				Curve curve2 = m_CurveData[edge2];
				Composition composition2 = m_CompositionData[edge2];
				float num9 = math.sqrt(math.max(1f, m_NetCompositionData[composition2.m_Edge].m_Width));
				float num10 = num6 + curve2.m_Length / num9;
				if (num8 != -1 && num10 - num3 > num3 - num6)
				{
					Entity edge3 = aggregateElements[num8].m_Edge;
					Curve curve3 = m_CurveData[edge3];
					Composition composition3 = m_CompositionData[edge3];
					NetCompositionData netCompositionData = m_NetCompositionData[composition3.m_Edge];
					labelPosition.m_Curve = curve3.m_Bezier;
					labelPosition.m_ElementIndex = j;
					labelPosition.m_HalfLength = curve3.m_Length * 0.5f;
					labelPosition.m_MaxScale = netCompositionData.m_Width * 0.5f / math.max(1f, math.max(0f - labelExtents.m_Bounds.min.y, labelExtents.m_Bounds.max.y));
					labelPosition.m_IsUnderground = (netCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
					labelPositions.Add(labelPosition);
					redundancyBuffer.Add(ref num5);
					num6 -= num3;
					num10 -= num3;
					num7 = 0f;
					num8 = -1;
				}
				float num11 = math.lerp(num6, num10, 0.5f);
				float num12 = curve2.m_Length * num9 / math.max(1f, num3 + math.abs(num11 - num3 * 0.5f));
				num6 = num10;
				if (num12 > num7)
				{
					num7 = num12;
					num8 = j;
				}
			}
			if (num8 != -1)
			{
				Entity edge4 = aggregateElements[num8].m_Edge;
				Curve curve4 = m_CurveData[edge4];
				Composition composition4 = m_CompositionData[edge4];
				NetCompositionData netCompositionData2 = m_NetCompositionData[composition4.m_Edge];
				LabelPosition labelPosition2 = default(LabelPosition);
				labelPosition2.m_Curve = curve4.m_Bezier;
				labelPosition2.m_ElementIndex = num8;
				labelPosition2.m_HalfLength = curve4.m_Length * 0.5f;
				labelPosition2.m_MaxScale = netCompositionData2.m_Width * 0.5f / math.max(1f, math.max(0f - labelExtents.m_Bounds.min.y, labelExtents.m_Bounds.max.y));
				labelPosition2.m_IsUnderground = (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
				labelPositions.Add(labelPosition2);
				redundancyBuffer.Add(ref num5);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillTempMapJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Aggregated> m_AggregatedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		public NativeParallelMultiHashMap<Entity, TempValue> m_TempMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Aggregated> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Aggregated>(ref m_AggregatedType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Aggregated aggregated = nativeArray2[i];
				Temp temp = nativeArray3[i];
				if (aggregated.m_Aggregate != Entity.Null && temp.m_Original != Entity.Null && (temp.m_Flags & (TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0)
				{
					m_TempMap.Add(aggregated.m_Aggregate, new TempValue
					{
						m_TempEntity = nativeArray[i],
						m_OriginalEntity = temp.m_Original
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TempValue
	{
		public Entity m_TempEntity;

		public Entity m_OriginalEntity;
	}

	[BurstCompile]
	private struct UpdateArrowPositionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> m_AggregateElementType;

		public BufferTypeHandle<ArrowPosition> m_ArrowPositionType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<NetCompositionCarriageway> m_NetCompositionCarriageways;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public NativeParallelMultiHashMap<Entity, TempValue> m_TempMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<AggregateElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AggregateElement>(ref m_AggregateElementType);
			BufferAccessor<ArrowPosition> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ArrowPosition>(ref m_ArrowPositionType);
			NativeParallelHashMap<Entity, Entity> edgeMap = default(NativeParallelHashMap<Entity, Entity>);
			NativeList<AggregateElement> val = default(NativeList<AggregateElement>);
			Entity edge = default(Entity);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<AggregateElement> val2 = bufferAccessor[i];
				DynamicBuffer<ArrowPosition> arrowPositions = bufferAccessor2[i];
				arrowPositions.Clear();
				if (val2.Length <= 0)
				{
					continue;
				}
				UpdateEdgeMap(nativeArray[i], ref edgeMap);
				ProcessElements(val2.AsNativeArray(), arrowPositions, edgeMap);
				if (!edgeMap.IsCreated || edgeMap.IsEmpty)
				{
					continue;
				}
				if (val.IsCreated)
				{
					val.Clear();
				}
				else
				{
					val._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				}
				for (int j = 0; j < val2.Length; j++)
				{
					if (edgeMap.TryGetValue(val2[j].m_Edge, ref edge))
					{
						AggregateElement aggregateElement = new AggregateElement(edge);
						val.Add(ref aggregateElement);
					}
					else if (!val.IsEmpty)
					{
						ProcessElements(val.AsArray(), arrowPositions, default(NativeParallelHashMap<Entity, Entity>));
						val.Clear();
					}
				}
				if (!val.IsEmpty)
				{
					ProcessElements(val.AsArray(), arrowPositions, default(NativeParallelHashMap<Entity, Entity>));
				}
			}
			if (edgeMap.IsCreated)
			{
				edgeMap.Dispose();
			}
			if (val.IsCreated)
			{
				val.Dispose();
			}
		}

		private void UpdateEdgeMap(Entity aggregate, ref NativeParallelHashMap<Entity, Entity> edgeMap)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			TempValue tempValue = default(TempValue);
			NativeParallelMultiHashMapIterator<Entity> val = default(NativeParallelMultiHashMapIterator<Entity>);
			if (m_TempMap.TryGetFirstValue(aggregate, ref tempValue, ref val))
			{
				if (edgeMap.IsCreated)
				{
					edgeMap.Clear();
				}
				else
				{
					edgeMap = new NativeParallelHashMap<Entity, Entity>(32, AllocatorHandle.op_Implicit((Allocator)2));
				}
				do
				{
					edgeMap.TryAdd(tempValue.m_OriginalEntity, tempValue.m_TempEntity);
				}
				while (m_TempMap.TryGetNextValue(ref tempValue, ref val));
			}
			else if (edgeMap.IsCreated)
			{
				edgeMap.Clear();
			}
		}

		private void ProcessElements(NativeArray<AggregateElement> aggregateElements, DynamicBuffer<ArrowPosition> arrowPositions, NativeParallelHashMap<Entity, Entity> edgeMap)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			int startIndex = 0;
			int i = 1;
			Edge edge = m_EdgeData[aggregateElements[0].m_Edge];
			for (; i < aggregateElements.Length; i++)
			{
				Edge edge2 = m_EdgeData[aggregateElements[i].m_Edge];
				if (edge2.m_Start == edge.m_End || edge2.m_Start == edge.m_Start)
				{
					DynamicBuffer<ConnectedEdge> connectedEdges = m_ConnectedEdges[edge2.m_Start];
					if (CompositionChange(connectedEdges, edge2.m_Start == edge.m_Start))
					{
						AddArrows(startIndex, i, aggregateElements, arrowPositions, edgeMap);
						startIndex = i;
					}
				}
				else if (edge2.m_End == edge.m_End || edge2.m_End == edge.m_Start)
				{
					DynamicBuffer<ConnectedEdge> connectedEdges2 = m_ConnectedEdges[edge2.m_End];
					if (CompositionChange(connectedEdges2, edge2.m_End == edge.m_End))
					{
						AddArrows(startIndex, i, aggregateElements, arrowPositions, edgeMap);
						startIndex = i;
					}
				}
				else
				{
					AddArrows(startIndex, i, aggregateElements, arrowPositions, edgeMap);
					startIndex = i;
				}
				edge = edge2;
			}
			AddArrows(startIndex, i, aggregateElements, arrowPositions, edgeMap);
		}

		private bool CompositionChange(DynamicBuffer<ConnectedEdge> connectedEdges, bool invert)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (connectedEdges.Length != 2)
			{
				return true;
			}
			Entity edge = connectedEdges[0].m_Edge;
			Entity edge2 = connectedEdges[1].m_Edge;
			Composition composition = m_CompositionData[edge];
			Composition composition2 = m_CompositionData[edge2];
			DynamicBuffer<NetCompositionCarriageway> val = m_NetCompositionCarriageways[composition.m_Edge];
			DynamicBuffer<NetCompositionCarriageway> val2 = m_NetCompositionCarriageways[composition2.m_Edge];
			if (val.Length != val2.Length)
			{
				return true;
			}
			for (int i = 0; i < val.Length; i++)
			{
				NetCompositionCarriageway netCompositionCarriageway = val[i];
				NetCompositionCarriageway netCompositionCarriageway2;
				if (invert)
				{
					netCompositionCarriageway2 = val2[val2.Length - i - 1];
					if ((netCompositionCarriageway2.m_Flags & LaneFlags.Twoway) == 0)
					{
						netCompositionCarriageway2.m_Flags ^= LaneFlags.Invert;
					}
				}
				else
				{
					netCompositionCarriageway2 = val2[i];
				}
				if (((netCompositionCarriageway.m_Flags ^ netCompositionCarriageway2.m_Flags) & (LaneFlags.Invert | LaneFlags.Road | LaneFlags.Track | LaneFlags.Twoway)) != 0)
				{
					return true;
				}
			}
			return false;
		}

		private void AddArrows(int startIndex, int endIndex, NativeArray<AggregateElement> aggregateElements, DynamicBuffer<ArrowPosition> arrowPositions, NativeParallelHashMap<Entity, Entity> edgeMap)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = startIndex; i < endIndex; i++)
			{
				Entity edge = aggregateElements[i].m_Edge;
				Curve curve = m_CurveData[edge];
				Composition composition = m_CompositionData[edge];
				NetCompositionData netCompositionData = m_NetCompositionData[composition.m_Edge];
				DynamicBuffer<NetCompositionCarriageway> val = m_NetCompositionCarriageways[composition.m_Edge];
				float num2 = 0f;
				int num3 = 0;
				if ((netCompositionData.m_State & CompositionState.Marker) == 0 || m_EditorMode)
				{
					for (int j = 0; j < val.Length; j++)
					{
						NetCompositionCarriageway netCompositionCarriageway = val[j];
						if ((netCompositionCarriageway.m_Flags & LaneFlags.Twoway) == 0)
						{
							num2 = math.min(num2, netCompositionCarriageway.m_Width + 4f);
							num3++;
						}
					}
				}
				num2 = math.max(num2, netCompositionData.m_Width / (float)math.max(1, num3));
				num2 = math.sqrt(math.max(1f, num2));
				num += curve.m_Length / num2;
			}
			float num4 = 25f;
			int num5 = math.max(Mathf.RoundToInt(num / num4), 1);
			num4 = math.max(1f, num / (float)num5);
			float num6 = 20f;
			float num7 = num6 * 0.5f;
			num = num4 * -0.5f;
			ArrowPosition arrowPosition = default(ArrowPosition);
			for (int k = startIndex; k < endIndex; k++)
			{
				Entity edge2 = aggregateElements[k].m_Edge;
				Curve curve2 = m_CurveData[edge2];
				Composition composition2 = m_CompositionData[edge2];
				NetCompositionData netCompositionData2 = m_NetCompositionData[composition2.m_Edge];
				DynamicBuffer<NetCompositionCarriageway> val2 = m_NetCompositionCarriageways[composition2.m_Edge];
				float num8 = 0f;
				int num9 = 0;
				if ((netCompositionData2.m_State & CompositionState.Marker) == 0 || m_EditorMode)
				{
					for (int l = 0; l < val2.Length; l++)
					{
						NetCompositionCarriageway netCompositionCarriageway2 = val2[l];
						if ((netCompositionCarriageway2.m_Flags & LaneFlags.Twoway) == 0)
						{
							num8 = math.min(num8, netCompositionCarriageway2.m_Width + 4f);
							num9++;
						}
					}
				}
				num8 = math.max(num8, netCompositionData2.m_Width / (float)math.max(1, num9));
				num8 = math.sqrt(math.max(1f, num8));
				float num10 = num + curve2.m_Length / num8;
				bool flag = IsInverted(edge2, k, aggregateElements);
				float num11 = math.min(0.25f, num6 / math.max(1f, curve2.m_Length));
				float num12 = 1f - num11;
				if (k > startIndex)
				{
					Curve curve3 = m_CurveData[aggregateElements[k - 1].m_Edge];
					num11 = math.select(num11, 0f, IsContinuous(curve3, curve2));
				}
				if (k < endIndex - 1)
				{
					Curve curve4 = m_CurveData[aggregateElements[k + 1].m_Edge];
					num12 = math.select(num12, 1f, IsContinuous(curve2, curve4));
				}
				bool flag2 = true;
				if (edgeMap.IsCreated && edgeMap.ContainsKey(edge2))
				{
					flag2 = false;
				}
				while (num10 >= 0f)
				{
					if (flag2)
					{
						float num13 = (0f - num) / math.max(1f, num10 - num);
						num13 = math.clamp(num13, num11, num12);
						num13 = math.select(num13, 1f - num13, flag);
						float3 val3 = MathUtils.Position(curve2.m_Bezier, num13);
						float3 val4 = math.normalizesafe(MathUtils.Tangent(curve2.m_Bezier, num13), default(float3));
						float3 val5 = math.normalizesafe(new float3(val4.z, 0f, 0f - val4.x), default(float3));
						if ((netCompositionData2.m_State & CompositionState.Marker) == 0 || m_EditorMode)
						{
							for (int m = 0; m < val2.Length; m++)
							{
								NetCompositionCarriageway netCompositionCarriageway3 = val2[m];
								if ((netCompositionCarriageway3.m_Flags & LaneFlags.Twoway) == 0)
								{
									num8 = math.max(netCompositionCarriageway3.m_Width + 4f, netCompositionData2.m_Width / (float)math.max(1, num9));
									arrowPosition.m_Direction = math.select(val4, -val4, (netCompositionCarriageway3.m_Flags & LaneFlags.Invert) != 0);
									arrowPosition.m_Position = val3 + val5 * netCompositionCarriageway3.m_Position.x + arrowPosition.m_Direction;
									arrowPosition.m_Position.y += netCompositionCarriageway3.m_Position.y;
									arrowPosition.m_MaxScale = num8 * 0.5f / num7;
									arrowPosition.m_IsTrack = (netCompositionCarriageway3.m_Flags & LaneFlags.Road) == 0;
									arrowPosition.m_IsUnderground = (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
									arrowPositions.Add(arrowPosition);
								}
							}
						}
					}
					num -= num4;
					num10 -= num4;
				}
				num = num10;
			}
		}

		private bool IsContinuous(Curve curve1, Curve curve2)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			val.x = math.abs(math.distancesq(curve1.m_Bezier.a, curve2.m_Bezier.a));
			val.y = math.abs(math.distancesq(curve1.m_Bezier.a, curve2.m_Bezier.d));
			val.z = math.abs(math.distancesq(curve1.m_Bezier.d, curve2.m_Bezier.a));
			val.w = math.abs(math.distancesq(curve1.m_Bezier.d, curve2.m_Bezier.d));
			if (math.all(val > 1f))
			{
				return false;
			}
			float3 val2 = ((!math.any((((float4)(ref val)).xy < ((float4)(ref val)).zw) & (((float4)(ref val)).xy < ((float4)(ref val)).wz))) ? MathUtils.EndTangent(curve1.m_Bezier) : (-MathUtils.StartTangent(curve1.m_Bezier)));
			float3 val3 = ((!math.any((((float4)(ref val)).xz < ((float4)(ref val)).yw) & (((float4)(ref val)).xz < ((float4)(ref val)).wy))) ? MathUtils.EndTangent(curve2.m_Bezier) : (-MathUtils.StartTangent(curve2.m_Bezier)));
			return math.dot(math.normalizesafe(val2, default(float3)), math.normalizesafe(val3, default(float3))) < -0.99f;
		}

		private bool IsInverted(Entity edge, int index, NativeArray<AggregateElement> aggregateElements)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			if (index > 0)
			{
				Edge edge2 = m_EdgeData[aggregateElements[index - 1].m_Edge];
				Edge edge3 = m_EdgeData[edge];
				if (!(edge3.m_End == edge2.m_Start))
				{
					return edge3.m_End == edge2.m_End;
				}
				return true;
			}
			if (index < aggregateElements.Length - 1)
			{
				Edge edge4 = m_EdgeData[edge];
				Edge edge5 = m_EdgeData[aggregateElements[index + 1].m_Edge];
				if (!(edge4.m_Start == edge5.m_Start))
				{
					return edge4.m_Start == edge5.m_End;
				}
				return true;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct LabelVertexData
	{
		public Vector3 m_Position;

		public Vector3 m_Normal;

		public Color32 m_Color;

		public Vector4 m_UV0;

		public Vector4 m_UV1;

		public Vector4 m_UV2;

		public Vector4 m_UV3;
	}

	private struct SubMeshData
	{
		public int m_VertexCount;

		public Bounds3 m_Bounds;
	}

	[BurstCompile]
	private struct FillNameDataJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<LabelExtents> m_LabelExtentsType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public BufferTypeHandle<LabelPosition> m_LabelPositionType;

		[ReadOnly]
		public BufferTypeHandle<LabelVertex> m_LabelVertexType;

		[ReadOnly]
		public ComponentLookup<NetNameData> m_NetNameData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public int m_SubMeshCount;

		public MeshDataArray m_NameMeshData;

		public void Execute()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<SubMeshData> val = default(NativeArray<SubMeshData>);
			val._002Ector(m_SubMeshCount, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				if (((ArchetypeChunk)(ref val2)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				BufferAccessor<LabelPosition> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<LabelPosition>(ref m_LabelPositionType);
				BufferAccessor<LabelVertex> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<LabelVertex>(ref m_LabelVertexType);
				for (int j = 0; j < bufferAccessor2.Length; j++)
				{
					DynamicBuffer<LabelPosition> val3 = bufferAccessor[j];
					DynamicBuffer<LabelVertex> val4 = bufferAccessor2[j];
					for (int k = 0; k < val3.Length; k++)
					{
						LabelPosition labelPosition = val3[k];
						for (int l = 0; l < val4.Length; l += 4)
						{
							int2 material = val4[l].m_Material;
							int num = math.select(material.x, material.y, labelPosition.m_IsUnderground);
							CollectionUtils.ElementAt<SubMeshData>(val, num).m_VertexCount += 4;
						}
					}
				}
			}
			int num2 = 0;
			for (int m = 0; m < m_SubMeshCount; m++)
			{
				num2 += val[m].m_VertexCount;
			}
			MeshData val5 = ((MeshDataArray)(ref m_NameMeshData))[0];
			NativeArray<VertexAttributeDescriptor> val6 = default(NativeArray<VertexAttributeDescriptor>);
			val6._002Ector(7, (Allocator)2, (NativeArrayOptions)0);
			val6[0] = new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 3, 0);
			val6[1] = new VertexAttributeDescriptor((VertexAttribute)1, (VertexAttributeFormat)0, 3, 0);
			val6[2] = new VertexAttributeDescriptor((VertexAttribute)3, (VertexAttributeFormat)2, 4, 0);
			val6[3] = new VertexAttributeDescriptor((VertexAttribute)4, (VertexAttributeFormat)0, 4, 0);
			val6[4] = new VertexAttributeDescriptor((VertexAttribute)5, (VertexAttributeFormat)0, 4, 0);
			val6[5] = new VertexAttributeDescriptor((VertexAttribute)6, (VertexAttributeFormat)0, 4, 0);
			val6[6] = new VertexAttributeDescriptor((VertexAttribute)7, (VertexAttributeFormat)0, 4, 0);
			((MeshData)(ref val5)).SetVertexBufferParams(num2, val6);
			((MeshData)(ref val5)).SetIndexBufferParams((num2 >> 2) * 6, (IndexFormat)1);
			val6.Dispose();
			num2 = 0;
			((MeshData)(ref val5)).subMeshCount = m_SubMeshCount;
			for (int n = 0; n < m_SubMeshCount; n++)
			{
				ref SubMeshData reference = ref CollectionUtils.ElementAt<SubMeshData>(val, n);
				int num3 = n;
				SubMeshDescriptor val7 = default(SubMeshDescriptor);
				((SubMeshDescriptor)(ref val7)).firstVertex = num2;
				((SubMeshDescriptor)(ref val7)).indexStart = (num2 >> 2) * 6;
				((SubMeshDescriptor)(ref val7)).vertexCount = reference.m_VertexCount;
				((SubMeshDescriptor)(ref val7)).indexCount = (reference.m_VertexCount >> 2) * 6;
				((SubMeshDescriptor)(ref val7)).topology = (MeshTopology)0;
				((MeshData)(ref val5)).SetSubMesh(num3, val7, (MeshUpdateFlags)13);
				num2 += reference.m_VertexCount;
				reference.m_VertexCount = 0;
				reference.m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			}
			NativeArray<LabelVertexData> vertexData = ((MeshData)(ref val5)).GetVertexData<LabelVertexData>(0);
			NativeArray<uint> indexData = ((MeshData)(ref val5)).GetIndexData<uint>();
			float3 val12 = default(float3);
			float4 val13 = default(float4);
			float4 val14 = default(float4);
			Bounds3 val16 = default(Bounds3);
			LabelVertexData labelVertexData = default(LabelVertexData);
			for (int num4 = 0; num4 < m_Chunks.Length; num4++)
			{
				ArchetypeChunk val8 = m_Chunks[num4];
				if (((ArchetypeChunk)(ref val8)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				NativeArray<LabelExtents> nativeArray = ((ArchetypeChunk)(ref val8)).GetNativeArray<LabelExtents>(ref m_LabelExtentsType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val8)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref val8)).GetNativeArray<Temp>(ref m_TempType);
				BufferAccessor<LabelPosition> bufferAccessor3 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<LabelPosition>(ref m_LabelPositionType);
				BufferAccessor<LabelVertex> bufferAccessor4 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<LabelVertex>(ref m_LabelVertexType);
				for (int num5 = 0; num5 < bufferAccessor4.Length; num5++)
				{
					LabelExtents labelExtents = nativeArray[num5];
					PrefabRef prefabRef = nativeArray2[num5];
					DynamicBuffer<LabelPosition> val9 = bufferAccessor3[num5];
					DynamicBuffer<LabelVertex> val10 = bufferAccessor4[num5];
					NetNameData netNameData = m_NetNameData[prefabRef.m_Prefab];
					Color32 val11 = netNameData.m_Color;
					if (nativeArray3.Length != 0 && (nativeArray3[num5].m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						val11 = netNameData.m_SelectedColor;
					}
					float num6 = math.length(math.max(-labelExtents.m_Bounds.min, labelExtents.m_Bounds.max));
					for (int num7 = 0; num7 < val9.Length; num7++)
					{
						LabelPosition labelPosition2 = val9[num7];
						((float3)(ref val12))._002Ector(labelPosition2.m_HalfLength, 0f, 0f);
						float2 xy = ((float3)(ref labelPosition2.m_Curve.a)).xy;
						float2 xy2 = ((float3)(ref labelPosition2.m_Curve.b)).xy;
						((float4)(ref val13))._002Ector(labelPosition2.m_Curve.c, labelPosition2.m_Curve.a.z);
						((float4)(ref val14))._002Ector(labelPosition2.m_Curve.d, labelPosition2.m_Curve.b.z);
						float3 val15 = MathUtils.Position(labelPosition2.m_Curve, 0.5f);
						float num8 = num6 * labelPosition2.m_MaxScale;
						((Bounds3)(ref val16))._002Ector(val15 - num8, val15 + num8);
						SubMeshDescriptor val17 = default(SubMeshDescriptor);
						int num9 = -1;
						for (int num10 = 0; num10 < val10.Length; num10 += 4)
						{
							int2 material2 = val10[num10].m_Material;
							int num11 = math.select(material2.x, material2.y, labelPosition2.m_IsUnderground);
							ref SubMeshData reference2 = ref CollectionUtils.ElementAt<SubMeshData>(val, num11);
							if (num11 != num9)
							{
								val17 = ((MeshData)(ref val5)).GetSubMesh(num11);
								ref Bounds3 reference3 = ref reference2.m_Bounds;
								reference3 |= val16;
								num9 = num11;
							}
							int num12 = ((SubMeshDescriptor)(ref val17)).firstVertex + reference2.m_VertexCount;
							int num13 = ((SubMeshDescriptor)(ref val17)).indexStart + (reference2.m_VertexCount >> 2) * 6;
							reference2.m_VertexCount += 4;
							indexData[num13] = (uint)num12;
							indexData[num13 + 1] = (uint)(num12 + 1);
							indexData[num13 + 2] = (uint)(num12 + 2);
							indexData[num13 + 3] = (uint)(num12 + 2);
							indexData[num13 + 4] = (uint)(num12 + 3);
							indexData[num13 + 5] = (uint)num12;
							for (int num14 = 0; num14 < 4; num14++)
							{
								LabelVertex labelVertex = val10[num10 + num14];
								labelVertexData.m_Position = float3.op_Implicit(labelVertex.m_Position);
								labelVertexData.m_Normal = float3.op_Implicit(val12);
								labelVertexData.m_Color = new Color32((byte)(labelVertex.m_Color.r * val11.r >> 8), (byte)(labelVertex.m_Color.g * val11.g >> 8), (byte)(labelVertex.m_Color.b * val11.b >> 8), (byte)(labelVertex.m_Color.a * val11.a >> 8));
								labelVertexData.m_UV0 = float4.op_Implicit(new float4(labelVertex.m_UV0, xy));
								labelVertexData.m_UV1 = float4.op_Implicit(new float4(labelPosition2.m_MaxScale, labelVertex.m_UV1.y, xy2));
								labelVertexData.m_UV2 = float4.op_Implicit(val13);
								labelVertexData.m_UV3 = float4.op_Implicit(val14);
								vertexData[num12 + num14] = labelVertexData;
							}
						}
					}
				}
			}
			for (int num15 = 0; num15 < m_SubMeshCount; num15++)
			{
				SubMeshDescriptor subMesh = ((MeshData)(ref val5)).GetSubMesh(num15);
				((SubMeshDescriptor)(ref subMesh)).bounds = RenderingUtils.ToBounds(val[num15].m_Bounds);
				((MeshData)(ref val5)).SetSubMesh(num15, subMesh, (MeshUpdateFlags)13);
			}
			val.Dispose();
		}
	}

	private struct ArrowVertexData
	{
		public Vector3 m_Position;

		public Color32 m_Color;

		public Vector2 m_UV0;

		public Vector4 m_UV1;
	}

	[BurstCompile]
	private struct FillArrowDataJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Hidden> m_HiddenType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<ArrowPosition> m_ArrowPositionType;

		[ReadOnly]
		public ComponentLookup<NetArrowData> m_NetArrowData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public MeshDataArray m_ArrowMeshData;

		public void Execute()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				BufferAccessor<ArrowPosition> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ArrowPosition>(ref m_ArrowPositionType);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<ArrowPosition> val2 = bufferAccessor[j];
					for (int k = 0; k < val2.Length; k++)
					{
						if (val2[k].m_IsUnderground)
						{
							num3 += 4;
							num4 += 6;
						}
						else
						{
							num += 4;
							num2 += 6;
						}
					}
				}
			}
			MeshData val3 = ((MeshDataArray)(ref m_ArrowMeshData))[0];
			NativeArray<VertexAttributeDescriptor> val4 = default(NativeArray<VertexAttributeDescriptor>);
			val4._002Ector(4, (Allocator)2, (NativeArrayOptions)0);
			val4[0] = new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 3, 0);
			val4[1] = new VertexAttributeDescriptor((VertexAttribute)3, (VertexAttributeFormat)2, 4, 0);
			val4[2] = new VertexAttributeDescriptor((VertexAttribute)4, (VertexAttributeFormat)0, 2, 0);
			val4[3] = new VertexAttributeDescriptor((VertexAttribute)5, (VertexAttributeFormat)0, 4, 0);
			((MeshData)(ref val3)).SetVertexBufferParams(num + num3, val4);
			((MeshData)(ref val3)).SetIndexBufferParams(num2 + num4, (IndexFormat)1);
			val4.Dispose();
			((MeshData)(ref val3)).subMeshCount = 2;
			SubMeshDescriptor val5 = default(SubMeshDescriptor);
			((SubMeshDescriptor)(ref val5)).vertexCount = num;
			((SubMeshDescriptor)(ref val5)).indexCount = num2;
			((SubMeshDescriptor)(ref val5)).topology = (MeshTopology)0;
			((MeshData)(ref val3)).SetSubMesh(0, val5, (MeshUpdateFlags)13);
			val5 = default(SubMeshDescriptor);
			((SubMeshDescriptor)(ref val5)).firstVertex = num;
			((SubMeshDescriptor)(ref val5)).indexStart = num2;
			((SubMeshDescriptor)(ref val5)).vertexCount = num3;
			((SubMeshDescriptor)(ref val5)).indexCount = num4;
			((SubMeshDescriptor)(ref val5)).topology = (MeshTopology)0;
			((MeshData)(ref val3)).SetSubMesh(1, val5, (MeshUpdateFlags)13);
			NativeArray<ArrowVertexData> vertexData = ((MeshData)(ref val3)).GetVertexData<ArrowVertexData>(0);
			NativeArray<uint> indexData = ((MeshData)(ref val3)).GetIndexData<uint>();
			SubMeshDescriptor subMesh = ((MeshData)(ref val3)).GetSubMesh(0);
			SubMeshDescriptor subMesh2 = ((MeshData)(ref val3)).GetSubMesh(1);
			Bounds3 val6 = default(Bounds3);
			((Bounds3)(ref val6))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			Bounds3 val7 = default(Bounds3);
			((Bounds3)(ref val7))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			int vertexIndex = 0;
			int indexIndex = 0;
			int vertexIndex2 = ((SubMeshDescriptor)(ref subMesh2)).firstVertex;
			int indexIndex2 = ((SubMeshDescriptor)(ref subMesh2)).indexStart;
			float4 uv = default(float4);
			for (int l = 0; l < m_Chunks.Length; l++)
			{
				ArchetypeChunk val8 = m_Chunks[l];
				if (((ArchetypeChunk)(ref val8)).Has<Hidden>(ref m_HiddenType))
				{
					continue;
				}
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val8)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<ArrowPosition> bufferAccessor2 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<ArrowPosition>(ref m_ArrowPositionType);
				for (int m = 0; m < bufferAccessor2.Length; m++)
				{
					PrefabRef prefabRef = nativeArray[m];
					DynamicBuffer<ArrowPosition> val9 = bufferAccessor2[m];
					NetArrowData netArrowData = m_NetArrowData[prefabRef.m_Prefab];
					float num5 = 20f;
					float num6 = num5 * 0.5f;
					for (int n = 0; n < val9.Length; n++)
					{
						ArrowPosition arrowPosition = val9[n];
						Color32 color = (arrowPosition.m_IsTrack ? netArrowData.m_TrackColor : netArrowData.m_RoadColor);
						((float4)(ref uv))._002Ector(arrowPosition.m_Position, arrowPosition.m_MaxScale);
						float3 z = arrowPosition.m_Direction * num5;
						float3 x = math.normalizesafe(new float3(0f - arrowPosition.m_Direction.z, 0f, arrowPosition.m_Direction.x), math.right()) * num6;
						float num7 = num5 * arrowPosition.m_MaxScale;
						if (arrowPosition.m_IsUnderground)
						{
							val7 |= new Bounds3(arrowPosition.m_Position - num7, arrowPosition.m_Position + num7);
							AddArrow(vertexData, indexData, color, uv, z, x, ref vertexIndex2, ref indexIndex2);
						}
						else
						{
							val6 |= new Bounds3(arrowPosition.m_Position - num7, arrowPosition.m_Position + num7);
							AddArrow(vertexData, indexData, color, uv, z, x, ref vertexIndex, ref indexIndex);
						}
					}
				}
			}
			((SubMeshDescriptor)(ref subMesh)).bounds = RenderingUtils.ToBounds(val6);
			((SubMeshDescriptor)(ref subMesh2)).bounds = RenderingUtils.ToBounds(val7);
			((MeshData)(ref val3)).SetSubMesh(0, subMesh, (MeshUpdateFlags)13);
			((MeshData)(ref val3)).SetSubMesh(1, subMesh2, (MeshUpdateFlags)13);
		}

		private void AddArrow(NativeArray<ArrowVertexData> vertices, NativeArray<uint> indices, Color32 color, float4 uv1, float3 z, float3 x, ref int vertexIndex, ref int indexIndex)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			indices[indexIndex++] = (uint)vertexIndex;
			indices[indexIndex++] = (uint)(vertexIndex + 1);
			indices[indexIndex++] = (uint)(vertexIndex + 2);
			indices[indexIndex++] = (uint)(vertexIndex + 2);
			indices[indexIndex++] = (uint)(vertexIndex + 3);
			indices[indexIndex++] = (uint)vertexIndex;
			ArrowVertexData arrowVertexData = default(ArrowVertexData);
			arrowVertexData.m_Position = float3.op_Implicit(-x - z);
			arrowVertexData.m_Color = color;
			arrowVertexData.m_UV0 = float2.op_Implicit(new float2(0f, 0f));
			arrowVertexData.m_UV1 = float4.op_Implicit(uv1);
			vertices[vertexIndex++] = arrowVertexData;
			arrowVertexData.m_Position = float3.op_Implicit(x - z);
			arrowVertexData.m_Color = color;
			arrowVertexData.m_UV0 = float2.op_Implicit(new float2(1f, 0f));
			arrowVertexData.m_UV1 = float4.op_Implicit(uv1);
			vertices[vertexIndex++] = arrowVertexData;
			arrowVertexData.m_Position = float3.op_Implicit(x + z);
			arrowVertexData.m_Color = color;
			arrowVertexData.m_UV0 = float2.op_Implicit(new float2(1f, 1f));
			arrowVertexData.m_UV1 = float4.op_Implicit(uv1);
			vertices[vertexIndex++] = arrowVertexData;
			arrowVertexData.m_Position = float3.op_Implicit(z - x);
			arrowVertexData.m_Color = color;
			arrowVertexData.m_UV0 = float2.op_Implicit(new float2(0f, 1f));
			arrowVertexData.m_UV1 = float4.op_Implicit(uv1);
			vertices[vertexIndex++] = arrowVertexData;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<NetNameData> __Game_Prefabs_NetNameData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetArrowData> __Game_Prefabs_NetArrowData_RW_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Updated> __Game_Common_Updated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatchesUpdated> __Game_Common_BatchesUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<LabelExtents> __Game_Net_LabelExtents_RW_ComponentTypeHandle;

		public SharedComponentTypeHandle<LabelMaterial> __Game_Net_LabelMaterial_SharedComponentTypeHandle;

		public BufferTypeHandle<LabelVertex> __Game_Net_LabelVertex_RW_BufferTypeHandle;

		public SharedComponentTypeHandle<ArrowMaterial> __Game_Net_ArrowMaterial_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LabelExtents> __Game_Net_LabelExtents_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> __Game_Net_AggregateElement_RO_BufferTypeHandle;

		public BufferTypeHandle<LabelPosition> __Game_Net_LabelPosition_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Aggregated> __Game_Net_Aggregated_RO_ComponentTypeHandle;

		public BufferTypeHandle<ArrowPosition> __Game_Net_ArrowPosition_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<NetCompositionCarriageway> __Game_Prefabs_NetCompositionCarriageway_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Hidden> __Game_Tools_Hidden_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LabelPosition> __Game_Net_LabelPosition_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LabelVertex> __Game_Net_LabelVertex_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetNameData> __Game_Prefabs_NetNameData_RO_ComponentLookup;

		[ReadOnly]
		public BufferTypeHandle<ArrowPosition> __Game_Net_ArrowPosition_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetArrowData> __Game_Prefabs_NetArrowData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_NetNameData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetNameData>(false);
			__Game_Prefabs_NetArrowData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetArrowData>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Updated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Updated>(true);
			__Game_Common_BatchesUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatchesUpdated>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_LabelExtents_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LabelExtents>(false);
			__Game_Net_LabelMaterial_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<LabelMaterial>();
			__Game_Net_LabelVertex_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelVertex>(false);
			__Game_Net_ArrowMaterial_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<ArrowMaterial>();
			__Game_Net_LabelExtents_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LabelExtents>(true);
			__Game_Net_AggregateElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AggregateElement>(true);
			__Game_Net_LabelPosition_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelPosition>(false);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_Aggregated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Aggregated>(true);
			__Game_Net_ArrowPosition_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ArrowPosition>(false);
			__Game_Prefabs_NetCompositionCarriageway_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionCarriageway>(true);
			__Game_Tools_Hidden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Hidden>(true);
			__Game_Net_LabelPosition_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelPosition>(true);
			__Game_Net_LabelVertex_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LabelVertex>(true);
			__Game_Prefabs_NetNameData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetNameData>(true);
			__Game_Net_ArrowPosition_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ArrowPosition>(true);
			__Game_Prefabs_NetArrowData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetArrowData>(true);
		}
	}

	private EntityQuery m_CreatedPrefabQuery;

	private EntityQuery m_UpdatedLabelQuery;

	private EntityQuery m_LabelQuery;

	private EntityQuery m_UpdatedArrowQuery;

	private EntityQuery m_ArrowQuery;

	private EntityQuery m_TempAggregatedQuery;

	private OverlayRenderSystem m_OverlayRenderSystem;

	private UndergroundViewSystem m_UndergroundViewSystem;

	private PrefabSystem m_PrefabSystem;

	private NameSystem m_NameSystem;

	private ToolSystem m_ToolSystem;

	private List<MeshData> m_LabelData;

	private List<MeshData> m_ArrowData;

	private Dictionary<Entity, string> m_CachedLabels;

	private int m_FaceColor;

	private bool m_TunnelSelectOn;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		m_UndergroundViewSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UndergroundViewSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<AggregateNetData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NetNameData>(),
			ComponentType.ReadOnly<NetArrowData>()
		};
		array[0] = val;
		m_CreatedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Aggregate>(),
			ComponentType.ReadOnly<LabelMaterial>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_UpdatedLabelQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_LabelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Aggregate>(),
			ComponentType.ReadOnly<LabelMaterial>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Aggregate>(),
			ComponentType.ReadOnly<ArrowMaterial>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array3[0] = val;
		m_UpdatedArrowQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		m_ArrowQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Aggregate>(),
			ComponentType.ReadOnly<ArrowMaterial>(),
			ComponentType.Exclude<Deleted>()
		});
		m_TempAggregatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Aggregated>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_FaceColor = Shader.PropertyToID("_FaceColor");
		GameManager.instance.localizationManager.onActiveDictionaryChanged += OnDictionaryChanged;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		DestroyMeshData(m_LabelData);
		DestroyMeshData(m_ArrowData);
		GameManager.instance.localizationManager.onActiveDictionaryChanged -= OnDictionaryChanged;
		base.OnDestroy();
	}

	private void DestroyMeshData(List<MeshData> meshData)
	{
		if (meshData == null)
		{
			return;
		}
		for (int i = 0; i < meshData.Count; i++)
		{
			MeshData meshData2 = meshData[i];
			if (meshData2.m_Materials != null)
			{
				for (int j = 0; j < meshData2.m_Materials.Count; j++)
				{
					MaterialData materialData = meshData2.m_Materials[j];
					if ((Object)(object)materialData.m_Material != (Object)null)
					{
						Object.Destroy((Object)(object)materialData.m_Material);
					}
				}
			}
			if ((Object)(object)meshData2.m_Mesh != (Object)null)
			{
				Object.Destroy((Object)(object)meshData2.m_Mesh);
			}
			if (meshData2.m_HasMeshData)
			{
				((MeshDataArray)(ref meshData2.m_MeshData)).Dispose();
			}
		}
		meshData.Clear();
	}

	public void PreDeserialize(Context context)
	{
		ClearMeshData(m_LabelData);
		ClearMeshData(m_ArrowData);
		if (m_CachedLabels != null)
		{
			m_CachedLabels.Clear();
		}
		m_Loaded = true;
	}

	private void UpdateUndergroundState(List<MeshData> meshData, bool undergroundOn)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (meshData == null)
		{
			return;
		}
		for (int i = 0; i < meshData.Count; i++)
		{
			MeshData meshData2 = meshData[i];
			if (meshData2.m_Materials == null)
			{
				continue;
			}
			for (int j = 0; j < meshData2.m_Materials.Count; j++)
			{
				MaterialData materialData = meshData2.m_Materials[j];
				if (!materialData.m_IsUnderground && (Object)(object)materialData.m_Material != (Object)null)
				{
					materialData.m_Material.SetColor(m_FaceColor, new Color(1f, 1f, 1f, undergroundOn ? 0.25f : 1f));
				}
			}
		}
	}

	private void OnDictionaryChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Updated>(m_LabelQuery);
	}

	private void ClearMeshData(List<MeshData> meshData)
	{
		if (meshData == null)
		{
			return;
		}
		for (int i = 0; i < meshData.Count; i++)
		{
			MeshData meshData2 = meshData[i];
			if (meshData2.m_Materials != null)
			{
				for (int j = 0; j < meshData2.m_Materials.Count; j++)
				{
					MaterialData value = meshData2.m_Materials[j];
					value.m_HasMesh = false;
					meshData2.m_Materials[j] = value;
				}
			}
			if ((Object)(object)meshData2.m_Mesh != (Object)null)
			{
				Object.Destroy((Object)(object)meshData2.m_Mesh);
				meshData2.m_Mesh = null;
			}
			if (meshData2.m_HasMeshData)
			{
				((MeshDataArray)(ref meshData2.m_MeshData)).Dispose();
				meshData2.m_HasMeshData = false;
			}
			meshData2.m_HasMesh = false;
		}
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		if (!((EntityQuery)(ref m_CreatedPrefabQuery)).IsEmptyIgnoreFilter)
		{
			InitializePrefabs();
		}
		EntityQuery val = (loaded ? m_LabelQuery : m_UpdatedLabelQuery);
		EntityQuery val2 = (loaded ? m_ArrowQuery : m_UpdatedArrowQuery);
		bool flag = m_UndergroundViewSystem.undergroundOn && m_UndergroundViewSystem.tunnelsOn;
		bool flag2 = !((EntityQuery)(ref val)).IsEmptyIgnoreFilter;
		bool flag3 = !((EntityQuery)(ref val2)).IsEmptyIgnoreFilter;
		if (flag != m_TunnelSelectOn)
		{
			UpdateUndergroundState(m_LabelData, flag);
			UpdateUndergroundState(m_ArrowData, flag);
			m_TunnelSelectOn = flag;
		}
		if (flag2 || flag3)
		{
			JobHandle dependency = ((SystemBase)this).Dependency;
			JobHandle val3 = default(JobHandle);
			if (flag2)
			{
				UpdateLabelVertices(loaded);
				JobHandle inputDeps = UpdateLabelPositions(dependency, loaded);
				val3 = JobHandle.CombineDependencies(val3, FillNameMeshData(inputDeps));
			}
			if (flag3)
			{
				UpdateArrowMaterials(loaded);
				JobHandle inputDeps2 = UpdateArrowPositions(dependency, loaded);
				val3 = JobHandle.CombineDependencies(val3, FillArrowMeshData(inputDeps2));
			}
			((SystemBase)this).Dependency = val3;
		}
	}

	private void InitializePrefabs()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_CreatedPrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			bool flag = m_UndergroundViewSystem.undergroundOn && m_UndergroundViewSystem.tunnelsOn;
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetNameData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<NetNameData>(ref __TypeHandle.__Game_Prefabs_NetNameData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetArrowData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<NetArrowData>(ref __TypeHandle.__Game_Prefabs_NetArrowData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				NativeArray<NetNameData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<NetNameData>(ref componentTypeHandle2);
				NativeArray<NetArrowData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<NetArrowData>(ref componentTypeHandle3);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					AggregateNetPrefab prefab = m_PrefabSystem.GetPrefab<AggregateNetPrefab>(nativeArray[j]);
					NetLabel component = prefab.GetComponent<NetLabel>();
					NetArrow component2 = prefab.GetComponent<NetArrow>();
					NetNameData netNameData;
					int num;
					if ((Object)(object)component != (Object)null && (Object)(object)component.m_NameMaterial != (Object)null)
					{
						netNameData = nativeArray2[j];
						if (m_LabelData != null)
						{
							num = 0;
							while (num < m_LabelData.Count)
							{
								if (!((Object)(object)m_LabelData[num].m_OriginalMaterial == (Object)(object)component.m_NameMaterial))
								{
									num++;
									continue;
								}
								goto IL_012f;
							}
						}
						MeshData meshData = new MeshData();
						meshData.m_OriginalMaterial = component.m_NameMaterial;
						meshData.m_Materials = new List<MaterialData>(2);
						if (m_LabelData == null)
						{
							m_LabelData = new List<MeshData>();
						}
						netNameData.m_MaterialIndex = m_LabelData.Count;
						nativeArray2[j] = netNameData;
						m_LabelData.Add(meshData);
					}
					goto IL_01b9;
					IL_01b9:
					if (!((Object)(object)component2 != (Object)null) || !((Object)(object)component2.m_ArrowMaterial != (Object)null))
					{
						continue;
					}
					NetArrowData netArrowData = nativeArray3[j];
					int num2;
					if (m_ArrowData != null)
					{
						num2 = 0;
						while (num2 < m_ArrowData.Count)
						{
							if (!((Object)(object)m_ArrowData[num2].m_OriginalMaterial == (Object)(object)component2.m_ArrowMaterial))
							{
								num2++;
								continue;
							}
							goto IL_0210;
						}
					}
					MeshData meshData2 = new MeshData();
					meshData2.m_OriginalMaterial = component2.m_ArrowMaterial;
					meshData2.m_Materials = new List<MaterialData>(2);
					MaterialData item = new MaterialData
					{
						m_Material = new Material(component2.m_ArrowMaterial)
					};
					((Object)item.m_Material).name = "Aggregate arrows (" + ((Object)prefab).name + ")";
					item.m_Material.SetColor(m_FaceColor, new Color(1f, 1f, 1f, flag ? 0.25f : 1f));
					meshData2.m_Materials.Add(item);
					MaterialData item2 = new MaterialData
					{
						m_Material = new Material(component2.m_ArrowMaterial)
					};
					((Object)item2.m_Material).name = "Aggregate underground arrows (" + ((Object)prefab).name + ")";
					item2.m_Material.SetColor(m_FaceColor, new Color(1f, 1f, 1f, 1f));
					item2.m_IsUnderground = true;
					meshData2.m_Materials.Add(item2);
					if (m_ArrowData == null)
					{
						m_ArrowData = new List<MeshData>();
					}
					netArrowData.m_MaterialIndex = m_ArrowData.Count;
					nativeArray3[j] = netArrowData;
					m_ArrowData.Add(meshData2);
					continue;
					IL_012f:
					netNameData.m_MaterialIndex = num;
					nativeArray2[j] = netNameData;
					goto IL_01b9;
					IL_0210:
					netArrowData.m_MaterialIndex = num2;
					nativeArray3[j] = netArrowData;
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void UpdateLabelVertices(bool isLoaded)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Expected O, but got Unknown
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Expected O, but got Unknown
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		List<MaterialUpdate> list = null;
		EntityQuery val = (isLoaded ? m_LabelQuery : m_UpdatedLabelQuery);
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityManager entityManager;
		try
		{
			TextMeshPro textMesh = m_OverlayRenderSystem.GetTextMesh();
			((TMP_Text)textMesh).rectTransform.sizeDelta = new Vector2(250f, 100f);
			((TMP_Text)textMesh).fontSize = 200f;
			((TMP_Text)textMesh).alignment = (TextAlignmentOptions)514;
			bool flag = m_UndergroundViewSystem.undergroundOn && m_UndergroundViewSystem.tunnelsOn;
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Updated> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BatchesUpdated> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<BatchesUpdated>(ref __TypeHandle.__Game_Common_BatchesUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Temp> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<LabelExtents> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<LabelExtents>(ref __TypeHandle.__Game_Net_LabelExtents_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			SharedComponentTypeHandle<LabelMaterial> sharedComponentTypeHandle = InternalCompilerInterface.GetSharedComponentTypeHandle<LabelMaterial>(ref __TypeHandle.__Game_Net_LabelMaterial_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<LabelVertex> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<LabelVertex>(ref __TypeHandle.__Game_Net_LabelVertex_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			LabelVertex labelVertex = default(LabelVertex);
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				LabelMaterial sharedComponent = ((ArchetypeChunk)(ref val3)).GetSharedComponent<LabelMaterial>(sharedComponentTypeHandle, ((ComponentSystemBase)this).EntityManager);
				if (isLoaded || ((ArchetypeChunk)(ref val3)).Has<Updated>(ref componentTypeHandle) || ((ArchetypeChunk)(ref val3)).Has<BatchesUpdated>(ref componentTypeHandle2))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref componentTypeHandle3);
					NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref componentTypeHandle4);
					NativeArray<LabelExtents> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<LabelExtents>(ref componentTypeHandle5);
					BufferAccessor<LabelVertex> bufferAccessor = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<LabelVertex>(ref bufferTypeHandle);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val4 = nativeArray[j];
						PrefabRef prefabRef = nativeArray3[j];
						DynamicBuffer<LabelVertex> val5 = bufferAccessor[j];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						NetNameData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetNameData>(prefabRef.m_Prefab);
						MeshData meshData = m_LabelData[componentData.m_MaterialIndex];
						meshData.m_MeshDirty = true;
						if (componentData.m_MaterialIndex != sharedComponent.m_Index)
						{
							if (list == null)
							{
								list = new List<MaterialUpdate>();
							}
							list.Add(new MaterialUpdate
							{
								m_Entity = val4,
								m_Material = componentData.m_MaterialIndex
							});
						}
						string renderedLabelName;
						if (nativeArray2.Length != 0)
						{
							Temp temp = nativeArray2[j];
							if (!(temp.m_Original != Entity.Null))
							{
								if (m_CachedLabels != null && m_CachedLabels.ContainsKey(val4))
								{
									m_CachedLabels.Remove(val4);
								}
								val5.Clear();
								continue;
							}
							renderedLabelName = m_NameSystem.GetRenderedLabelName(temp.m_Original);
						}
						else
						{
							renderedLabelName = m_NameSystem.GetRenderedLabelName(val4);
						}
						if (m_CachedLabels != null)
						{
							if (m_CachedLabels.TryGetValue(val4, out var value))
							{
								if (value == renderedLabelName)
								{
									continue;
								}
								m_CachedLabels[val4] = renderedLabelName;
							}
							else
							{
								m_CachedLabels.Add(val4, renderedLabelName);
							}
						}
						else
						{
							m_CachedLabels = new Dictionary<Entity, string>();
							m_CachedLabels.Add(val4, renderedLabelName);
						}
						TMP_TextInfo textInfo = ((TMP_Text)textMesh).GetTextInfo(renderedLabelName);
						int num = 0;
						for (int k = 0; k < textInfo.meshInfo.Length; k++)
						{
							TMP_MeshInfo val6 = textInfo.meshInfo[k];
							num += val6.vertexCount;
						}
						val5.ResizeUninitialized(num);
						num = 0;
						for (int l = 0; l < textInfo.meshInfo.Length; l++)
						{
							TMP_MeshInfo val7 = textInfo.meshInfo[l];
							if (val7.vertexCount == 0)
							{
								continue;
							}
							Texture mainTexture = val7.material.mainTexture;
							int2 val8 = int2.op_Implicit(-1);
							for (int m = 0; m < meshData.m_Materials.Count; m++)
							{
								MaterialData materialData = meshData.m_Materials[m];
								if ((Object)(object)materialData.m_Material.mainTexture == (Object)(object)mainTexture)
								{
									if (materialData.m_IsUnderground)
									{
										val8.y = m;
									}
									else
									{
										val8.x = m;
									}
								}
							}
							if (val8.x == -1)
							{
								MaterialData item = new MaterialData
								{
									m_Material = new Material(meshData.m_OriginalMaterial)
								};
								item.m_Material.SetColor(m_FaceColor, new Color(1f, 1f, 1f, flag ? 0.25f : 1f));
								m_OverlayRenderSystem.CopyFontAtlasParameters(val7.material, item.m_Material);
								val8.x = meshData.m_Materials.Count;
								meshData.m_Materials.Add(item);
								((Object)item.m_Material).name = $"Aggregate names {val8.x} ({((Object)meshData.m_OriginalMaterial).name})";
							}
							if (val8.y == -1)
							{
								MaterialData item2 = new MaterialData
								{
									m_Material = new Material(meshData.m_OriginalMaterial)
								};
								item2.m_Material.SetColor(m_FaceColor, new Color(1f, 1f, 1f, 1f));
								m_OverlayRenderSystem.CopyFontAtlasParameters(val7.material, item2.m_Material);
								item2.m_IsUnderground = true;
								val8.y = meshData.m_Materials.Count;
								meshData.m_Materials.Add(item2);
								((Object)item2.m_Material).name = $"Aggregate underground names {val8.y} ({((Object)meshData.m_OriginalMaterial).name})";
							}
							Vector3[] vertices = val7.vertices;
							Vector2[] uvs = val7.uvs0;
							Vector2[] uvs2 = val7.uvs2;
							Color32[] colors = val7.colors32;
							for (int n = 0; n < val7.vertexCount; n++)
							{
								labelVertex.m_Position = float3.op_Implicit(vertices[n]);
								labelVertex.m_Color = colors[n];
								labelVertex.m_UV0 = float2.op_Implicit(uvs[n]);
								labelVertex.m_UV1 = float2.op_Implicit(uvs2[n]);
								labelVertex.m_Material = val8;
								val5[num + n] = labelVertex;
							}
							num += val7.vertexCount;
						}
						LabelExtents labelExtents = default(LabelExtents);
						for (int num2 = 0; num2 < textInfo.lineCount; num2++)
						{
							Extents lineExtents = textInfo.lineInfo[num2].lineExtents;
							ref Bounds2 bounds = ref labelExtents.m_Bounds;
							bounds |= new Bounds2(float2.op_Implicit(lineExtents.min), float2.op_Implicit(lineExtents.max));
						}
						nativeArray4[j] = labelExtents;
					}
					continue;
				}
				NativeArray<Entity> nativeArray5 = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabRef> nativeArray6 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref componentTypeHandle4);
				for (int num3 = 0; num3 < nativeArray5.Length; num3++)
				{
					Entity key = nativeArray5[num3];
					PrefabRef prefabRef2 = nativeArray6[num3];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					NetNameData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<NetNameData>(prefabRef2.m_Prefab);
					m_LabelData[componentData2.m_MaterialIndex].m_MeshDirty = true;
					if (m_CachedLabels != null)
					{
						m_CachedLabels.Remove(key);
					}
				}
			}
		}
		finally
		{
			val2.Dispose();
		}
		if (list != null)
		{
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				MaterialUpdate materialUpdate = list[num4];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetSharedComponent<LabelMaterial>(materialUpdate.m_Entity, new LabelMaterial
				{
					m_Index = materialUpdate.m_Material
				});
			}
		}
	}

	private void UpdateArrowMaterials(bool isLoaded)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		List<MaterialUpdate> list = null;
		EntityQuery val = (isLoaded ? m_ArrowQuery : m_UpdatedArrowQuery);
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityManager entityManager;
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Updated> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			SharedComponentTypeHandle<ArrowMaterial> sharedComponentTypeHandle = InternalCompilerInterface.GetSharedComponentTypeHandle<ArrowMaterial>(ref __TypeHandle.__Game_Net_ArrowMaterial_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				ArrowMaterial sharedComponent = ((ArchetypeChunk)(ref val3)).GetSharedComponent<ArrowMaterial>(sharedComponentTypeHandle, ((ComponentSystemBase)this).EntityManager);
				if (isLoaded || ((ArchetypeChunk)(ref val3)).Has<Updated>(ref componentTypeHandle))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity entity = nativeArray[j];
						PrefabRef prefabRef = nativeArray2[j];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						NetArrowData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetArrowData>(prefabRef.m_Prefab);
						m_ArrowData[componentData.m_MaterialIndex].m_MeshDirty = true;
						if (componentData.m_MaterialIndex != sharedComponent.m_Index)
						{
							if (list == null)
							{
								list = new List<MaterialUpdate>();
							}
							list.Add(new MaterialUpdate
							{
								m_Entity = entity,
								m_Material = componentData.m_MaterialIndex
							});
						}
					}
				}
				else
				{
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
					NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					for (int k = 0; k < nativeArray3.Length; k++)
					{
						_ = nativeArray3[k];
						PrefabRef prefabRef2 = nativeArray4[k];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						NetArrowData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<NetArrowData>(prefabRef2.m_Prefab);
						m_ArrowData[componentData2.m_MaterialIndex].m_MeshDirty = true;
					}
				}
			}
		}
		finally
		{
			val2.Dispose();
		}
		if (list != null)
		{
			for (int l = 0; l < list.Count; l++)
			{
				MaterialUpdate materialUpdate = list[l];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetSharedComponent<ArrowMaterial>(materialUpdate.m_Entity, new ArrowMaterial
				{
					m_Index = materialUpdate.m_Material
				});
			}
		}
	}

	private JobHandle UpdateLabelPositions(JobHandle inputDeps, bool isLoaded)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (isLoaded ? m_LabelQuery : m_UpdatedLabelQuery);
		return JobChunkExtensions.ScheduleParallel<UpdateLabelPositionsJob>(new UpdateLabelPositionsJob
		{
			m_LabelExtentsType = InternalCompilerInterface.GetComponentTypeHandle<LabelExtents>(ref __TypeHandle.__Game_Net_LabelExtents_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElementType = InternalCompilerInterface.GetBufferTypeHandle<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LabelPositionType = InternalCompilerInterface.GetBufferTypeHandle<LabelPosition>(ref __TypeHandle.__Game_Net_LabelPosition_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		}, val, inputDeps);
	}

	private JobHandle UpdateArrowPositions(JobHandle inputDeps, bool isLoaded)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (isLoaded ? m_ArrowQuery : m_UpdatedArrowQuery);
		NativeParallelMultiHashMap<Entity, TempValue> tempMap = default(NativeParallelMultiHashMap<Entity, TempValue>);
		tempMap._002Ector(32, AllocatorHandle.op_Implicit((Allocator)3));
		FillTempMapJob fillTempMapJob = new FillTempMapJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregatedType = InternalCompilerInterface.GetComponentTypeHandle<Aggregated>(ref __TypeHandle.__Game_Net_Aggregated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempMap = tempMap
		};
		UpdateArrowPositionsJob obj = new UpdateArrowPositionsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElementType = InternalCompilerInterface.GetBufferTypeHandle<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ArrowPositionType = InternalCompilerInterface.GetBufferTypeHandle<ArrowPosition>(ref __TypeHandle.__Game_Net_ArrowPosition_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionCarriageways = InternalCompilerInterface.GetBufferLookup<NetCompositionCarriageway>(ref __TypeHandle.__Game_Prefabs_NetCompositionCarriageway_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_TempMap = tempMap
		};
		JobHandle val2 = JobChunkExtensions.Schedule<FillTempMapJob>(fillTempMapJob, m_TempAggregatedQuery, inputDeps);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<UpdateArrowPositionsJob>(obj, val, val2);
		tempMap.Dispose(val3);
		return val3;
	}

	private JobHandle FillNameMeshData(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = inputDeps;
		if (m_LabelData != null)
		{
			JobHandle val2 = default(JobHandle);
			for (int i = 0; i < m_LabelData.Count; i++)
			{
				MeshData meshData = m_LabelData[i];
				if (!meshData.m_MeshDirty)
				{
					continue;
				}
				meshData.m_MeshDirty = false;
				((EntityQuery)(ref m_LabelQuery)).ResetFilter();
				((EntityQuery)(ref m_LabelQuery)).SetSharedComponentFilter<LabelMaterial>(new LabelMaterial
				{
					m_Index = i
				});
				if (((EntityQuery)(ref m_LabelQuery)).IsEmptyIgnoreFilter)
				{
					if (meshData.m_Materials != null)
					{
						for (int j = 0; j < meshData.m_Materials.Count; j++)
						{
							MaterialData value = meshData.m_Materials[j];
							value.m_HasMesh = false;
							meshData.m_Materials[j] = value;
						}
					}
					if ((Object)(object)meshData.m_Mesh != (Object)null)
					{
						Object.Destroy((Object)(object)meshData.m_Mesh);
						meshData.m_Mesh = null;
					}
					if (meshData.m_HasMeshData)
					{
						meshData.m_HasMeshData = false;
						((MeshDataArray)(ref meshData.m_MeshData)).Dispose();
					}
					meshData.m_HasMesh = false;
					continue;
				}
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_LabelQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
				if (!meshData.m_HasMeshData)
				{
					meshData.m_HasMeshData = true;
					meshData.m_MeshData = Mesh.AllocateWritableMeshData(1);
				}
				JobHandle val3 = IJobExtensions.Schedule<FillNameDataJob>(new FillNameDataJob
				{
					m_LabelExtentsType = InternalCompilerInterface.GetComponentTypeHandle<LabelExtents>(ref __TypeHandle.__Game_Net_LabelExtents_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LabelPositionType = InternalCompilerInterface.GetBufferTypeHandle<LabelPosition>(ref __TypeHandle.__Game_Net_LabelPosition_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LabelVertexType = InternalCompilerInterface.GetBufferTypeHandle<LabelVertex>(ref __TypeHandle.__Game_Net_LabelVertex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NetNameData = InternalCompilerInterface.GetComponentLookup<NetNameData>(ref __TypeHandle.__Game_Prefabs_NetNameData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Chunks = chunks,
					m_SubMeshCount = meshData.m_Materials.Count,
					m_NameMeshData = meshData.m_MeshData
				}, JobHandle.CombineDependencies(val2, inputDeps));
				chunks.Dispose(val3);
				meshData.m_DataDependencies = val3;
				val = JobHandle.CombineDependencies(val, val3);
			}
		}
		return val;
	}

	private JobHandle FillArrowMeshData(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = inputDeps;
		if (m_ArrowData != null)
		{
			JobHandle val2 = default(JobHandle);
			for (int i = 0; i < m_ArrowData.Count; i++)
			{
				MeshData meshData = m_ArrowData[i];
				if (!meshData.m_MeshDirty)
				{
					continue;
				}
				meshData.m_MeshDirty = false;
				((EntityQuery)(ref m_ArrowQuery)).ResetFilter();
				((EntityQuery)(ref m_ArrowQuery)).SetSharedComponentFilter<ArrowMaterial>(new ArrowMaterial
				{
					m_Index = i
				});
				if (((EntityQuery)(ref m_ArrowQuery)).IsEmptyIgnoreFilter)
				{
					if (meshData.m_Materials != null)
					{
						for (int j = 0; j < meshData.m_Materials.Count; j++)
						{
							MaterialData value = meshData.m_Materials[j];
							value.m_HasMesh = false;
							meshData.m_Materials[j] = value;
						}
					}
					if ((Object)(object)meshData.m_Mesh != (Object)null)
					{
						Object.Destroy((Object)(object)meshData.m_Mesh);
						meshData.m_Mesh = null;
					}
					if (meshData.m_HasMeshData)
					{
						meshData.m_HasMeshData = false;
						((MeshDataArray)(ref meshData.m_MeshData)).Dispose();
					}
					meshData.m_HasMesh = false;
				}
				else
				{
					NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_ArrowQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
					if (!meshData.m_HasMeshData)
					{
						meshData.m_HasMeshData = true;
						meshData.m_MeshData = Mesh.AllocateWritableMeshData(1);
					}
					JobHandle val3 = IJobExtensions.Schedule<FillArrowDataJob>(new FillArrowDataJob
					{
						m_HiddenType = InternalCompilerInterface.GetComponentTypeHandle<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_ArrowPositionType = InternalCompilerInterface.GetBufferTypeHandle<ArrowPosition>(ref __TypeHandle.__Game_Net_ArrowPosition_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
						m_NetArrowData = InternalCompilerInterface.GetComponentLookup<NetArrowData>(ref __TypeHandle.__Game_Prefabs_NetArrowData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
						m_Chunks = chunks,
						m_ArrowMeshData = meshData.m_MeshData
					}, JobHandle.CombineDependencies(val2, inputDeps));
					chunks.Dispose(val3);
					meshData.m_DataDependencies = val3;
					val = JobHandle.CombineDependencies(val, val3);
				}
			}
		}
		return val;
	}

	public int GetNameMaterialCount()
	{
		if (m_LabelData != null)
		{
			return m_LabelData.Count;
		}
		return 0;
	}

	public int GetArrowMaterialCount()
	{
		if (m_ArrowData != null)
		{
			return m_ArrowData.Count;
		}
		return 0;
	}

	public bool GetNameMesh(int index, out Mesh mesh, out int subMeshCount)
	{
		return GetMeshData(m_LabelData, index, out mesh, out subMeshCount);
	}

	public bool GetNameMaterial(int index, int subMeshIndex, out Material material)
	{
		return GetMaterialData(m_LabelData, index, subMeshIndex, out material);
	}

	public bool GetArrowMesh(int index, out Mesh mesh, out int subMeshCount)
	{
		return GetMeshData(m_ArrowData, index, out mesh, out subMeshCount);
	}

	public bool GetArrowMaterial(int index, int subMeshIndex, out Material material)
	{
		return GetMaterialData(m_ArrowData, index, subMeshIndex, out material);
	}

	private bool GetMeshData(List<MeshData> meshData, int index, out Mesh mesh, out int subMeshCount)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		MeshData meshData2 = meshData[index];
		subMeshCount = meshData2.m_Materials.Count;
		if (meshData2.m_HasMeshData)
		{
			meshData2.m_HasMeshData = false;
			((JobHandle)(ref meshData2.m_DataDependencies)).Complete();
			meshData2.m_DataDependencies = default(JobHandle);
			if ((Object)(object)meshData2.m_Mesh == (Object)null)
			{
				meshData2.m_Mesh = new Mesh();
				if ((Object)(object)meshData2.m_OriginalMaterial != (Object)null)
				{
					((Object)meshData2.m_Mesh).name = $"Aggregates ({meshData2.m_OriginalMaterial})";
				}
			}
			Mesh.ApplyAndDisposeWritableMeshData(meshData2.m_MeshData, meshData2.m_Mesh, (MeshUpdateFlags)9);
			Bounds bounds = default(Bounds);
			meshData2.m_HasMesh = false;
			for (int i = 0; i < subMeshCount; i++)
			{
				MaterialData value = meshData2.m_Materials[i];
				SubMeshDescriptor subMesh = meshData2.m_Mesh.GetSubMesh(i);
				value.m_HasMesh = ((SubMeshDescriptor)(ref subMesh)).vertexCount > 0;
				if (value.m_HasMesh)
				{
					if (meshData2.m_HasMesh)
					{
						((Bounds)(ref bounds)).Encapsulate(((SubMeshDescriptor)(ref subMesh)).bounds);
					}
					else
					{
						bounds = ((SubMeshDescriptor)(ref subMesh)).bounds;
						meshData2.m_HasMesh = true;
					}
				}
				meshData2.m_Materials[i] = value;
			}
			meshData2.m_Mesh.bounds = bounds;
		}
		mesh = meshData2.m_Mesh;
		return meshData2.m_HasMesh;
	}

	private bool GetMaterialData(List<MeshData> meshData, int index, int subMeshIndex, out Material material)
	{
		MaterialData materialData = meshData[index].m_Materials[subMeshIndex];
		material = materialData.m_Material;
		return materialData.m_HasMesh;
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
	public AggregateMeshSystem()
	{
	}
}
