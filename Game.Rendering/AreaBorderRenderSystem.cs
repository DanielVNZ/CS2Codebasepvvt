using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class AreaBorderRenderSystem : GameSystemBase
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

	[BurstCompile]
	private struct AreaBorderRenderJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Lot> m_LotType;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> m_MapTileType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Warning> m_WarningType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public RenderingSettingsData m_RenderingSettingsData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public bool m_EditorMode;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<Area>(ref m_AreaType);
				BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Node>(ref m_NodeType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if ((nativeArray[j].m_Flags & AreaFlags.Slave) == 0)
					{
						num += bufferAccessor[j].Length;
					}
				}
			}
			NativeParallelHashSet<Border> borderMap = default(NativeParallelHashSet<Border>);
			borderMap._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelHashSet<float3> nodeMap = default(NativeParallelHashSet<float3>);
			nodeMap._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			for (int k = 0; k < m_Chunks.Length; k++)
			{
				AddBorders(m_Chunks[k], borderMap);
			}
			for (int l = 0; l < m_Chunks.Length; l++)
			{
				DrawBorders(m_Chunks[l], borderMap, nodeMap);
			}
			borderMap.Dispose();
			nodeMap.Dispose();
		}

		private void AddBorders(ArchetypeChunk chunk, NativeParallelHashSet<Border> borderMap)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			bool flag = !((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType) && !((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (flag)
				{
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0 || (temp.m_Flags & TempFlags.Hidden) != 0)
					{
						continue;
					}
				}
				Area area = nativeArray[i];
				if ((area.m_Flags & AreaFlags.Slave) != 0)
				{
					continue;
				}
				Entity prefab = nativeArray3[i].m_Prefab;
				DynamicBuffer<Node> val = bufferAccessor[i];
				AreaGeometryData areaGeometryData = m_PrefabGeometryData[prefab];
				if (!m_EditorMode && (areaGeometryData.m_Flags & GeometryFlags.HiddenIngame) != 0)
				{
					continue;
				}
				float3 val2 = val[0].m_Position;
				for (int j = 1; j < val.Length; j++)
				{
					float3 position = val[j].m_Position;
					if ((area.m_Flags & AreaFlags.CounterClockwise) != 0)
					{
						borderMap.Add(new Border
						{
							m_StartPos = position,
							m_EndPos = val2
						});
					}
					else
					{
						borderMap.Add(new Border
						{
							m_StartPos = val2,
							m_EndPos = position
						});
					}
					val2 = position;
				}
				if ((area.m_Flags & AreaFlags.Complete) != 0)
				{
					float3 position2 = val[0].m_Position;
					if ((area.m_Flags & AreaFlags.CounterClockwise) != 0)
					{
						borderMap.Add(new Border
						{
							m_StartPos = position2,
							m_EndPos = val2
						});
					}
					else
					{
						borderMap.Add(new Border
						{
							m_StartPos = val2,
							m_EndPos = position2
						});
					}
				}
			}
		}

		private void DrawBorders(ArchetypeChunk chunk, NativeParallelHashSet<Border> borderMap, NativeParallelHashSet<float3> nodeMap)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			OverlayRenderSystem.StyleFlags styleFlags = (OverlayRenderSystem.StyleFlags)0;
			bool flag = (((ArchetypeChunk)(ref chunk)).Has<Lot>(ref m_LotType) ? true : false);
			bool flag2;
			bool dashedLines;
			if (((ArchetypeChunk)(ref chunk)).Has<MapTile>(ref m_MapTileType))
			{
				styleFlags |= OverlayRenderSystem.StyleFlags.Projected;
				if (m_EditorMode)
				{
					flag2 = true;
					dashedLines = false;
				}
				else
				{
					flag2 = false;
					dashedLines = true;
				}
			}
			else
			{
				flag2 = true;
				dashedLines = false;
			}
			bool flag3;
			Color val;
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType))
			{
				flag3 = false;
				val = m_RenderingSettingsData.m_ErrorColor;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Warning>(ref m_WarningType))
			{
				flag3 = false;
				val = m_RenderingSettingsData.m_WarningColor;
			}
			else
			{
				flag3 = true;
				dashedLines = false;
				val = m_RenderingSettingsData.m_HoveredColor;
			}
			Color color = default(Color);
			((Color)(ref color))._002Ector(1f, 1f, 1f, 0.5f);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Color val2 = val;
				bool flag4 = flag2;
				if (nativeArray2.Length != 0)
				{
					Temp temp = nativeArray2[i];
					if (flag3)
					{
						if ((temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0 || (temp.m_Flags & TempFlags.Hidden) != 0)
						{
							continue;
						}
						if ((temp.m_Flags & TempFlags.Parent) != 0)
						{
							val2 = m_RenderingSettingsData.m_OwnerColor;
						}
					}
					flag4 &= (temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) != 0;
				}
				else
				{
					flag4 = false;
				}
				Area area = nativeArray[i];
				if ((area.m_Flags & AreaFlags.Slave) != 0)
				{
					continue;
				}
				Entity prefab = nativeArray3[i].m_Prefab;
				DynamicBuffer<Node> val3 = bufferAccessor[i];
				AreaGeometryData geometryData = m_PrefabGeometryData[prefab];
				if (!m_EditorMode && (geometryData.m_Flags & GeometryFlags.HiddenIngame) != 0)
				{
					continue;
				}
				float3 val4 = val3[0].m_Position;
				if (val3.Length == 1)
				{
					if (flag4 && nodeMap.Add(val4))
					{
						DrawNode(color, val4, geometryData, styleFlags);
					}
					continue;
				}
				for (int j = 1; j < val3.Length; j++)
				{
					float3 position = val3[j].m_Position;
					Border border = (((area.m_Flags & AreaFlags.CounterClockwise) == 0) ? new Border
					{
						m_StartPos = position,
						m_EndPos = val4
					} : new Border
					{
						m_StartPos = val4,
						m_EndPos = position
					});
					if (flag4 && nodeMap.Add(val4))
					{
						DrawNode(color, val4, geometryData, styleFlags);
					}
					if (!borderMap.Contains(border))
					{
						if (flag && j == 1)
						{
							DrawEdge(val2 * 0.5f, val4, position, geometryData, dashedLines, styleFlags);
						}
						else
						{
							DrawEdge(val2, val4, position, geometryData, dashedLines, styleFlags);
						}
					}
					if (flag4 && nodeMap.Add(position))
					{
						DrawNode(color, position, geometryData, styleFlags);
					}
					val4 = position;
				}
				if ((area.m_Flags & AreaFlags.Complete) != 0)
				{
					float3 position2 = val3[0].m_Position;
					Border border2 = (((area.m_Flags & AreaFlags.CounterClockwise) == 0) ? new Border
					{
						m_StartPos = position2,
						m_EndPos = val4
					} : new Border
					{
						m_StartPos = val4,
						m_EndPos = position2
					});
					if (flag4 && nodeMap.Add(val4))
					{
						DrawNode(color, val4, geometryData, styleFlags);
					}
					if (!borderMap.Contains(border2))
					{
						DrawEdge(val2, val4, position2, geometryData, dashedLines, styleFlags);
					}
					if (flag4 && nodeMap.Add(position2))
					{
						DrawNode(color, position2, geometryData, styleFlags);
					}
				}
			}
		}

		private void DrawNode(Color color, float3 position, AreaGeometryData geometryData, OverlayRenderSystem.StyleFlags styleFlags)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			m_OverlayBuffer.DrawCircle(color, color, 0f, styleFlags, new float2(0f, 1f), position, geometryData.m_SnapDistance * 0.3f);
		}

		private void DrawEdge(Color color, float3 startPos, float3 endPos, AreaGeometryData geometryData, bool dashedLines, OverlayRenderSystem.StyleFlags styleFlags)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Segment line = default(Segment);
			((Segment)(ref line))._002Ector(startPos, endPos);
			if (dashedLines)
			{
				float num = math.distance(((float3)(ref startPos)).xz, ((float3)(ref endPos)).xz);
				num /= math.max(1f, math.round(num / (geometryData.m_SnapDistance * 1.25f)));
				m_OverlayBuffer.DrawDashedLine(color, color, 0f, styleFlags, line, geometryData.m_SnapDistance * 0.2f, num * 0.55f, num * 0.45f);
			}
			else
			{
				m_OverlayBuffer.DrawLine(color, color, 0f, styleFlags, line, geometryData.m_SnapDistance * 0.3f, float2.op_Implicit(1f));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lot> __Game_Areas_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> __Game_Areas_MapTile_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Warning> __Game_Tools_Warning_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

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
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Areas_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lot>(true);
			__Game_Areas_MapTile_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MapTile>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Warning_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Warning>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
		}
	}

	private OverlayRenderSystem m_OverlayRenderSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_AreaBorderQuery;

	private EntityQuery m_RenderingSettingsQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Area>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>(),
			ComponentType.ReadOnly<Warning>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hidden>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_AreaBorderQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RenderingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RenderingSettingsData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_AreaBorderQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		RenderingSettingsData renderingSettingsData = new RenderingSettingsData
		{
			m_HoveredColor = new Color(0.5f, 0.5f, 1f, 0.5f),
			m_ErrorColor = new Color(1f, 0.25f, 0.25f, 0.5f),
			m_WarningColor = new Color(1f, 1f, 0.25f, 0.5f),
			m_OwnerColor = new Color(0.5f, 1f, 0.5f, 0.5f)
		};
		if (!((EntityQuery)(ref m_RenderingSettingsQuery)).IsEmptyIgnoreFilter)
		{
			RenderingSettingsData singleton = ((EntityQuery)(ref m_RenderingSettingsQuery)).GetSingleton<RenderingSettingsData>();
			renderingSettingsData.m_HoveredColor = singleton.m_HoveredColor;
			renderingSettingsData.m_ErrorColor = singleton.m_ErrorColor;
			renderingSettingsData.m_WarningColor = singleton.m_WarningColor;
			renderingSettingsData.m_OwnerColor = singleton.m_OwnerColor;
		}
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_AreaBorderQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle dependencies;
		JobHandle val2 = IJobExtensions.Schedule<AreaBorderRenderJob>(new AreaBorderRenderJob
		{
			m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LotType = InternalCompilerInterface.GetComponentTypeHandle<Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MapTileType = InternalCompilerInterface.GetComponentTypeHandle<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WarningType = InternalCompilerInterface.GetComponentTypeHandle<Warning>(ref __TypeHandle.__Game_Tools_Warning_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenderingSettingsData = renderingSettingsData,
			m_Chunks = chunks,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies)
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
		chunks.Dispose(val2);
		m_OverlayRenderSystem.AddBufferWriter(val2);
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
	public AreaBorderRenderSystem()
	{
	}
}
