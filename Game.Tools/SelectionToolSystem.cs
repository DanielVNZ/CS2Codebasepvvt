using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Audio;
using Game.Common;
using Game.Input;
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
public class SelectionToolSystem : ToolBaseSystem
{
	public enum State
	{
		Default,
		Selecting,
		Deselecting
	}

	[BurstCompile]
	private struct FindEntitiesJob : IJob
	{
		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Quad2 m_Quad;

			public AreaType m_AreaType;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_AreaGeometryData;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public NativeList<Entity> m_Entities;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Quad);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Quad))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[areaItem.m_Area];
				if (m_AreaGeometryData[prefabRef.m_Prefab].m_Type == m_AreaType)
				{
					Triangle2 triangle = AreaUtils.GetTriangle2(m_Nodes[areaItem.m_Area], m_Triangles[areaItem.m_Area][areaItem.m_Triangle]);
					if (MathUtils.Intersect(m_Quad, triangle))
					{
						m_Entities.Add(ref areaItem.m_Area);
					}
				}
			}
		}

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

		[ReadOnly]
		public ControlPoint m_StartPoint;

		[ReadOnly]
		public ControlPoint m_EndPoint;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public Quad2 m_SelectionQuad;

		[ReadOnly]
		public AreaType m_AreaType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_AreaGeometryData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		public NativeList<Entity> m_Entities;

		public void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			if (m_StartPoint.m_OriginalEntity != Entity.Null && m_AreaType != AreaType.None && m_Nodes.HasBuffer(m_StartPoint.m_OriginalEntity))
			{
				m_Entities.Add(ref m_StartPoint.m_OriginalEntity);
			}
			if (m_EndPoint.m_OriginalEntity != Entity.Null && m_AreaType != AreaType.None && m_Nodes.HasBuffer(m_EndPoint.m_OriginalEntity))
			{
				m_Entities.Add(ref m_EndPoint.m_OriginalEntity);
			}
			if (!m_StartPoint.Equals(default(ControlPoint)) && !m_EndPoint.Equals(default(ControlPoint)) && m_AreaType != AreaType.None)
			{
				AreaIterator areaIterator = new AreaIterator
				{
					m_Quad = m_SelectionQuad,
					m_AreaType = m_AreaType,
					m_PrefabRefData = m_PrefabRefData,
					m_AreaGeometryData = m_AreaGeometryData,
					m_Nodes = m_Nodes,
					m_Triangles = m_Triangles,
					m_Entities = m_Entities
				};
				m_AreaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_Entities, default(EntityComparer));
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_Entities.Length)
			{
				Entity val2 = m_Entities[num++];
				if (val2 != val)
				{
					m_Entities[num2++] = val2;
					val = val2;
				}
			}
			if (num2 < m_Entities.Length)
			{
				m_Entities.RemoveRange(num2, m_Entities.Length - num2);
			}
		}
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJobParallelForDefer
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public ComponentLookup<MapTile> m_MapTileData;

		[ReadOnly]
		public BufferLookup<Node> m_AreaNodes;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			if (m_AreaNodes.HasBuffer(val) && (m_EditorMode || !m_MapTileData.HasComponent(val) || m_NativeData.HasComponent(val)))
			{
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(index);
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = val
				};
				creationDefinition.m_Flags |= CreationFlags.Select;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(index, val2, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(index, val2, default(Updated));
				DynamicBuffer<Node> val3 = m_AreaNodes[val];
				DynamicBuffer<Node> val4 = ((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Node>(index, val2);
				val4.ResizeUninitialized(val3.Length);
				val4.CopyFrom(val3.AsNativeArray());
			}
		}
	}

	[BurstCompile]
	private struct ToggleEntityJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public bool m_Select;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Native> m_NativeType;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> m_MapTileType;

		public BufferLookup<SelectionElement> m_SelectionElements;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			if (!m_EditorMode && ((ArchetypeChunk)(ref chunk)).Has<MapTile>(ref m_MapTileType) && !((ArchetypeChunk)(ref chunk)).Has<Native>(ref m_NativeType))
			{
				return;
			}
			NativeArray<Temp> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Temp temp = nativeArray[i];
				if (!(temp.m_Original != Entity.Null) || !m_SelectionElements.HasBuffer(m_SelectionEntity))
				{
					continue;
				}
				DynamicBuffer<SelectionElement> val = m_SelectionElements[m_SelectionEntity];
				int num = 0;
				while (true)
				{
					if (num < val.Length)
					{
						SelectionElement selectionElement = val[num];
						if (((Entity)(ref selectionElement.m_Entity)).Equals(temp.m_Original))
						{
							if (!m_Select)
							{
								val.RemoveAt(num);
							}
							break;
						}
						num++;
						continue;
					}
					if (m_Select)
					{
						val.Add(new SelectionElement(temp.m_Original));
					}
					break;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CopyStartTilesJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public NativeList<Entity> m_StartTiles;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public BufferLookup<SelectionElement> m_SelectionElements;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SelectionElements.HasBuffer(m_SelectionEntity))
			{
				return;
			}
			DynamicBuffer<SelectionElement> val = m_SelectionElements[m_SelectionEntity];
			val.Clear();
			val.EnsureCapacity(m_StartTiles.Length);
			for (int i = 0; i < m_StartTiles.Length; i++)
			{
				Entity val2 = m_StartTiles[i];
				if (m_PrefabRefData.HasComponent(val2))
				{
					val.Add(new SelectionElement(val2));
				}
			}
		}
	}

	[BurstCompile]
	private struct UpdateStartTilesJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<SelectionElement> m_SelectionElements;

		public NativeList<Entity> m_StartTiles;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SelectionElements.HasBuffer(m_SelectionEntity))
			{
				return;
			}
			DynamicBuffer<SelectionElement> val = m_SelectionElements[m_SelectionEntity];
			for (int i = 0; i < m_StartTiles.Length; i++)
			{
				Entity val2 = m_StartTiles[i];
				if (m_PrefabRefData.HasComponent(val2))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
				}
			}
			m_StartTiles.ResizeUninitialized(val.Length);
			for (int j = 0; j < val.Length; j++)
			{
				Entity entity = val[j].m_Entity;
				m_StartTiles[j] = entity;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
			}
		}
	}

	[BurstCompile]
	private struct CopyServiceDistrictsJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public BufferLookup<SelectionElement> m_SelectionElements;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnerData.HasComponent(m_SelectionEntity) || !m_SelectionElements.HasBuffer(m_SelectionEntity))
			{
				return;
			}
			Owner owner = m_OwnerData[m_SelectionEntity];
			DynamicBuffer<SelectionElement> val = m_SelectionElements[m_SelectionEntity];
			if (!m_ServiceDistricts.HasBuffer(owner.m_Owner))
			{
				return;
			}
			DynamicBuffer<ServiceDistrict> val2 = m_ServiceDistricts[owner.m_Owner];
			val.Clear();
			val.EnsureCapacity(val2.Length);
			for (int i = 0; i < val2.Length; i++)
			{
				Entity district = val2[i].m_District;
				if (m_PrefabRefData.HasComponent(district))
				{
					val.Add(new SelectionElement(district));
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(owner.m_Owner);
		}
	}

	[BurstCompile]
	private struct UpdateServiceDistrictsJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectionEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public BufferLookup<SelectionElement> m_SelectionElements;

		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			if (!m_OwnerData.HasComponent(m_SelectionEntity) || !m_SelectionElements.HasBuffer(m_SelectionEntity))
			{
				return;
			}
			Owner owner = m_OwnerData[m_SelectionEntity];
			DynamicBuffer<SelectionElement> val = m_SelectionElements[m_SelectionEntity];
			if (m_ServiceDistricts.HasBuffer(owner.m_Owner))
			{
				DynamicBuffer<ServiceDistrict> val2 = m_ServiceDistricts[owner.m_Owner];
				val2.ResizeUninitialized(val.Length);
				for (int i = 0; i < val.Length; i++)
				{
					val2[i] = new ServiceDistrict(val[i].m_Entity);
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(owner.m_Owner);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MapTile> __Game_Areas_MapTile_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Native> __Game_Common_Native_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> __Game_Areas_MapTile_RO_ComponentTypeHandle;

		public BufferLookup<SelectionElement> __Game_Tools_SelectionElement_RW_BufferLookup;

		[ReadOnly]
		public BufferLookup<SelectionElement> __Game_Tools_SelectionElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> __Game_Areas_ServiceDistrict_RO_BufferLookup;

		public BufferLookup<ServiceDistrict> __Game_Areas_ServiceDistrict_RW_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Areas_MapTile_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MapTile>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Native_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Native>(true);
			__Game_Areas_MapTile_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MapTile>(true);
			__Game_Tools_SelectionElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SelectionElement>(false);
			__Game_Tools_SelectionElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SelectionElement>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Areas_ServiceDistrict_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDistrict>(true);
			__Game_Areas_ServiceDistrict_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDistrict>(false);
		}
	}

	public const string kToolID = "Selection Tool";

	private SearchSystem m_AreaSearchSystem;

	private MapTileSystem m_MapTileSystem;

	private MapTilePurchaseSystem m_MapTilePurchaseSystem;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private AudioManager m_AudioManager;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_DefinitionGroup;

	private EntityQuery m_TempGroup;

	private EntityQuery m_SoundQuery;

	private Entity m_SelectionEntity;

	private Entity m_LastOwner;

	private SelectionType m_LastType;

	private EntityArchetype m_SelectionArchetype;

	private State m_State;

	private ControlPoint m_StartPoint;

	private ControlPoint m_RaycastPoint;

	private IProxyAction m_SelectArea;

	private IProxyAction m_DeselectArea;

	private IProxyAction m_DiscardSelect;

	private IProxyAction m_DiscardDeselect;

	private bool m_ApplyBlocked;

	private TypeHandle __TypeHandle;

	public override string toolID => "Selection Tool";

	public SelectionType selectionType { get; set; }

	public Entity selectionOwner { get; set; }

	public State state => m_State;

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_SelectArea;
			yield return m_DeselectArea;
			yield return m_DiscardSelect;
			yield return m_DiscardDeselect;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_MapTileSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTileSystem>();
		m_MapTilePurchaseSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTilePurchaseSystem>();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_DefinitionGroup = GetDefinitionQuery();
		m_TempGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() });
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_SelectionArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<SelectionInfo>(),
			ComponentType.ReadWrite<SelectionElement>()
		});
		m_SelectArea = InputManager.instance.toolActionCollection.GetActionState("Select Area", "SelectionToolSystem");
		m_DeselectArea = InputManager.instance.toolActionCollection.GetActionState("Deselect Area", "SelectionToolSystem");
		m_DiscardSelect = InputManager.instance.toolActionCollection.GetActionState("Discard Select", "SelectionToolSystem");
		m_DiscardDeselect = InputManager.instance.toolActionCollection.GetActionState("Discard Deselect", "SelectionToolSystem");
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_State = State.Default;
		m_StartPoint = default(ControlPoint);
		m_ApplyBlocked = false;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (m_SelectionEntity != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).DestroyEntity(m_SelectionEntity);
			m_SelectionEntity = Entity.Null;
		}
		base.OnStopRunning();
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			switch (selectionType)
			{
			case SelectionType.MapTiles:
				if (m_ToolSystem.actionMode.IsGame() && m_MapTilePurchaseSystem.GetAvailableTiles() == 0)
				{
					base.applyAction.enabled = false;
					base.applyActionOverride = null;
					base.secondaryApplyAction.enabled = false;
					base.secondaryApplyActionOverride = null;
					base.cancelAction.enabled = false;
					base.cancelActionOverride = null;
					break;
				}
				if (((EntityQuery)(ref m_TempGroup)).CalculateEntityCount() <= 1)
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_SelectArea;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DeselectArea;
					base.cancelAction.enabled = false;
					base.cancelActionOverride = null;
					break;
				}
				switch (m_State)
				{
				case State.Default:
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_SelectArea;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DeselectArea;
					base.cancelAction.enabled = false;
					base.cancelActionOverride = null;
					break;
				case State.Selecting:
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_SelectArea;
					base.secondaryApplyAction.enabled = false;
					base.secondaryApplyActionOverride = null;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardSelect;
					break;
				case State.Deselecting:
					base.applyAction.enabled = false;
					base.applyActionOverride = null;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DeselectArea;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardDeselect;
					break;
				}
				break;
			case SelectionType.ServiceDistrict:
				switch (m_State)
				{
				case State.Default:
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_SelectArea;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DeselectArea;
					base.cancelAction.enabled = false;
					base.cancelActionOverride = null;
					break;
				case State.Selecting:
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_SelectArea;
					base.secondaryApplyAction.enabled = false;
					base.secondaryApplyActionOverride = null;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardSelect;
					break;
				case State.Deselecting:
					base.applyAction.enabled = false;
					base.applyActionOverride = null;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DeselectArea;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardDeselect;
					break;
				}
				break;
			default:
				base.applyAction.enabled = false;
				base.applyActionOverride = null;
				base.secondaryApplyAction.enabled = false;
				base.secondaryApplyActionOverride = null;
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
				break;
			}
		}
	}

	public override PrefabBase GetPrefab()
	{
		return null;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		return false;
	}

	public override void InitializeRaycast()
	{
		base.InitializeRaycast();
		SelectionType selectionType = this.selectionType;
		if ((uint)(selectionType - 1) <= 1u)
		{
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Areas | TypeMask.Water;
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.None;
		}
		m_ToolRaycastSystem.areaTypeMask = AreaUtils.GetTypeMask(GetAreaType(this.selectionType));
	}

	private AreaType GetAreaType(SelectionType selectionType)
	{
		return selectionType switch
		{
			SelectionType.ServiceDistrict => AreaType.District, 
			SelectionType.MapTiles => AreaType.MapTile, 
			_ => AreaType.None, 
		};
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if (m_LastOwner != selectionOwner || m_LastType != selectionType || m_SelectionEntity == Entity.Null)
		{
			EntityManager entityManager;
			if (m_SelectionEntity != Entity.Null)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).DestroyEntity(m_SelectionEntity);
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			m_SelectionEntity = ((EntityManager)(ref entityManager)).CreateEntity(m_SelectionArchetype);
			SelectionInfo selectionInfo = default(SelectionInfo);
			selectionInfo.m_SelectionType = selectionType;
			selectionInfo.m_AreaType = GetAreaType(selectionType);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<SelectionInfo>(m_SelectionEntity, selectionInfo);
			if (selectionOwner != Entity.Null)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponentData<Owner>(m_SelectionEntity, new Owner(selectionOwner));
			}
			m_LastOwner = selectionOwner;
			m_LastType = selectionType;
			base.requireAreas = m_ToolRaycastSystem.areaTypeMask;
			inputDeps = CopySelection(inputDeps);
		}
		UpdateActions();
		if (m_State != State.Default && !base.applyAction.enabled && !base.secondaryApplyAction.enabled)
		{
			m_State = State.Default;
		}
		if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
		{
			switch (m_State)
			{
			case State.Default:
				if (m_ApplyBlocked)
				{
					if (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
					{
						m_ApplyBlocked = false;
					}
					return Update(inputDeps);
				}
				if (base.secondaryApplyAction.WasPressedThisFrame())
				{
					return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
				}
				if (base.applyAction.WasPressedThisFrame())
				{
					return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
				}
				break;
			case State.Selecting:
				if (base.cancelAction.WasPressedThisFrame())
				{
					m_ApplyBlocked = true;
					return Cancel(inputDeps);
				}
				if (base.applyAction.WasPressedThisFrame() || base.applyAction.WasReleasedThisFrame())
				{
					return Apply(inputDeps);
				}
				break;
			case State.Deselecting:
				if (base.cancelAction.WasPressedThisFrame())
				{
					m_ApplyBlocked = true;
					return Apply(inputDeps);
				}
				if (base.secondaryApplyAction.WasPressedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
				{
					return Cancel(inputDeps);
				}
				break;
			}
			return Update(inputDeps);
		}
		if (m_State != State.Default && (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame()))
		{
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Selecting:
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			base.applyMode = ApplyMode.Clear;
			m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeEndSound);
			return UpdateDefinitions(inputDeps);
		case State.Deselecting:
			if (!m_RaycastPoint.Equals(default(ControlPoint)) && GetAllowApply())
			{
				inputDeps = ToggleTempEntity(inputDeps, select: false);
				inputDeps = UpdateSelection(inputDeps);
			}
			if (math.distance(m_StartPoint.m_Position, m_RaycastPoint.m_Position) > 1f)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeClearEndSound);
			}
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			base.applyMode = ApplyMode.Clear;
			return UpdateDefinitions(inputDeps);
		default:
			if (!m_RaycastPoint.Equals(default(ControlPoint)))
			{
				if (singleFrameOnly)
				{
					if (GetAllowApply())
					{
						inputDeps = ToggleTempEntity(inputDeps, select: false);
						inputDeps = UpdateSelection(inputDeps);
						GetRaycastResult(out m_RaycastPoint);
						base.applyMode = ApplyMode.Clear;
						return UpdateDefinitions(inputDeps);
					}
				}
				else
				{
					m_StartPoint = m_RaycastPoint;
					m_State = State.Deselecting;
				}
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeClearStartSound);
			}
			return Update(inputDeps);
		}
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Selecting:
			if (!m_RaycastPoint.Equals(default(ControlPoint)) && GetAllowApply())
			{
				inputDeps = ToggleTempEntity(inputDeps, select: true);
				inputDeps = UpdateSelection(inputDeps);
			}
			if (math.distance(m_StartPoint.m_Position, m_RaycastPoint.m_Position) > 1f)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeEndSound);
			}
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			base.applyMode = ApplyMode.Clear;
			return UpdateDefinitions(inputDeps);
		case State.Deselecting:
			if (math.distance(m_StartPoint.m_Position, m_RaycastPoint.m_Position) > 1f)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeClearEndSound);
			}
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_RaycastPoint);
			base.applyMode = ApplyMode.Clear;
			return UpdateDefinitions(inputDeps);
		default:
			if (!m_RaycastPoint.Equals(default(ControlPoint)))
			{
				if (singleFrameOnly)
				{
					if (GetAllowApply())
					{
						inputDeps = ToggleTempEntity(inputDeps, select: true);
						inputDeps = UpdateSelection(inputDeps);
						GetRaycastResult(out m_RaycastPoint);
						base.applyMode = ApplyMode.Clear;
						return UpdateDefinitions(inputDeps);
					}
				}
				else
				{
					m_StartPoint = m_RaycastPoint;
					m_State = State.Selecting;
				}
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_AreaMarqueeStartSound);
			}
			return Update(inputDeps);
		}
	}

	private JobHandle Update(JobHandle inputDeps)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		State state = m_State;
		if ((uint)(state - 1) <= 1u)
		{
			if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate) && controlPoint.Equals(m_RaycastPoint) && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			m_RaycastPoint = controlPoint;
			base.applyMode = ApplyMode.Clear;
			return UpdateDefinitions(inputDeps);
		}
		if (GetRaycastResult(out ControlPoint controlPoint2, out bool forceUpdate2) && controlPoint2.m_OriginalEntity == m_RaycastPoint.m_OriginalEntity && !forceUpdate2)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Updated>(m_RaycastPoint.m_OriginalEntity))
			{
				m_RaycastPoint = controlPoint2;
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
		}
		m_RaycastPoint = controlPoint2;
		base.applyMode = ApplyMode.Clear;
		return UpdateDefinitions(inputDeps);
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionGroup, m_ToolOutputBarrier, inputDeps);
		if (m_State != State.Default || m_RaycastPoint.m_OriginalEntity != Entity.Null)
		{
			NativeList<Entity> val2 = default(NativeList<Entity>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			GetSelectionQuad(out var quad);
			JobHandle dependencies;
			FindEntitiesJob findEntitiesJob = new FindEntitiesJob
			{
				m_StartPoint = ((m_State != State.Default) ? m_StartPoint : default(ControlPoint)),
				m_EndPoint = m_RaycastPoint,
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
				m_SelectionQuad = ((Quad3)(ref quad)).xz,
				m_AreaType = GetAreaType(selectionType),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val2
			};
			CreateDefinitionsJob createDefinitionsJob = new CreateDefinitionsJob
			{
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MapTileData = InternalCompilerInterface.GetComponentLookup<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entities = val2.AsDeferredJobArray()
			};
			EntityCommandBuffer val3 = m_ToolOutputBarrier.CreateCommandBuffer();
			createDefinitionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
			CreateDefinitionsJob createDefinitionsJob2 = createDefinitionsJob;
			JobHandle val4 = IJobExtensions.Schedule<FindEntitiesJob>(findEntitiesJob, JobHandle.CombineDependencies(inputDeps, dependencies));
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<CreateDefinitionsJob, Entity>(createDefinitionsJob2, val2, 4, val4);
			val2.Dispose(val5);
			m_AreaSearchSystem.AddSearchTreeReader(val4);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val5);
			val = JobHandle.CombineDependencies(val, val5);
		}
		return val;
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint)
	{
		if (selectionType == SelectionType.MapTiles && m_ToolSystem.actionMode.IsGame() && m_MapTilePurchaseSystem.GetAvailableTiles() == 0)
		{
			controlPoint = default(ControlPoint);
			return false;
		}
		return base.GetRaycastResult(out controlPoint);
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate)
	{
		if (selectionType == SelectionType.MapTiles && m_ToolSystem.actionMode.IsGame() && m_MapTilePurchaseSystem.GetAvailableTiles() == 0)
		{
			controlPoint = default(ControlPoint);
			forceUpdate = false;
			return false;
		}
		return base.GetRaycastResult(out controlPoint, out forceUpdate);
	}

	private JobHandle ToggleTempEntity(JobHandle inputDeps, bool select)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_TempGroup)).IsEmptyIgnoreFilter)
		{
			return inputDeps;
		}
		return JobChunkExtensions.Schedule<ToggleEntityJob>(new ToggleEntityJob
		{
			m_SelectionEntity = m_SelectionEntity,
			m_Select = select,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NativeType = InternalCompilerInterface.GetComponentTypeHandle<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MapTileType = InternalCompilerInterface.GetComponentTypeHandle<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_TempGroup, inputDeps);
	}

	private JobHandle CopySelection(JobHandle inputDeps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		switch (selectionType)
		{
		case SelectionType.ServiceDistrict:
			return CopyServiceDistricts(inputDeps);
		case SelectionType.MapTiles:
			if (m_ToolSystem.actionMode.IsEditor())
			{
				return CopyStartTiles(inputDeps);
			}
			return inputDeps;
		default:
			return inputDeps;
		}
	}

	private JobHandle UpdateSelection(JobHandle inputDeps)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		switch (selectionType)
		{
		case SelectionType.ServiceDistrict:
			return UpdateServiceDistricts(inputDeps);
		case SelectionType.MapTiles:
			if (m_ToolSystem.actionMode.IsEditor())
			{
				return UpdateStartTiles(inputDeps);
			}
			return inputDeps;
		default:
			return inputDeps;
		}
	}

	private JobHandle CopyStartTiles(JobHandle inputDeps)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<CopyStartTilesJob>(new CopyStartTilesJob
		{
			m_SelectionEntity = m_SelectionEntity,
			m_StartTiles = m_MapTileSystem.GetStartTiles(),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		}, inputDeps);
	}

	private JobHandle UpdateStartTiles(JobHandle inputDeps)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<UpdateStartTilesJob>(new UpdateStartTilesJob
		{
			m_SelectionEntity = m_SelectionEntity,
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartTiles = m_MapTileSystem.GetStartTiles(),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, inputDeps);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val);
		return val;
	}

	private JobHandle CopyServiceDistricts(JobHandle inputDeps)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<CopyServiceDistrictsJob>(new CopyServiceDistrictsJob
		{
			m_SelectionEntity = m_SelectionEntity,
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDistricts = InternalCompilerInterface.GetBufferLookup<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, inputDeps);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val);
		return val;
	}

	private JobHandle UpdateServiceDistricts(JobHandle inputDeps)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<UpdateServiceDistrictsJob>(new UpdateServiceDistrictsJob
		{
			m_SelectionEntity = m_SelectionEntity,
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectionElements = InternalCompilerInterface.GetBufferLookup<SelectionElement>(ref __TypeHandle.__Game_Tools_SelectionElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDistricts = InternalCompilerInterface.GetBufferLookup<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, inputDeps);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val);
		return val;
	}

	public bool GetSelectionQuad(out Quad3 quad)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		if ((Object)(object)main == (Object)null)
		{
			quad = default(Quad3);
			return false;
		}
		Transform transform = ((Component)main).transform;
		float3 val = default(float3);
		float3 val2 = float3.op_Implicit(transform.right);
		((float3)(ref val)).xz = ((float3)(ref val2)).xz;
		float3 val3 = val;
		val2 = default(float3);
		val = math.normalizesafe(val3, val2);
		float3 val4 = default(float3);
		((float3)(ref val4)).xz = MathUtils.Right(((float3)(ref val)).xz);
		float3 hitPosition = m_StartPoint.m_HitPosition;
		float3 val5 = m_RaycastPoint.m_HitPosition - hitPosition;
		float num = math.dot(val5, val);
		float num2 = math.dot(val5, val4);
		if (num < 0f)
		{
			val = -val;
			num = 0f - num;
		}
		if (num2 < 0f)
		{
			val4 = -val4;
			num2 = 0f - num2;
		}
		quad.a = hitPosition;
		quad.b = hitPosition + val4 * num2;
		quad.c = hitPosition + val * num + val4 * num2;
		quad.d = hitPosition + val * num;
		TerrainHeightData terrainData = m_TerrainSystem.GetHeightData();
		JobHandle deps;
		WaterSurfaceData data = m_WaterSystem.GetSurfaceData(out deps);
		((JobHandle)(ref deps)).Complete();
		quad.a.y = WaterUtils.SampleHeight(ref data, ref terrainData, quad.a);
		quad.b.y = WaterUtils.SampleHeight(ref data, ref terrainData, quad.b);
		quad.c.y = WaterUtils.SampleHeight(ref data, ref terrainData, quad.c);
		quad.d.y = WaterUtils.SampleHeight(ref data, ref terrainData, quad.d);
		if (!m_StartPoint.Equals(default(ControlPoint)))
		{
			return !m_RaycastPoint.Equals(default(ControlPoint));
		}
		return false;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public SelectionToolSystem()
	{
	}
}
