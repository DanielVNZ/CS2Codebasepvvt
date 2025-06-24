using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Common;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class AreaInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct FixPlaceholdersJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferTypeHandle<PlaceholderObjectElement> m_PlaceholderObjectElementType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<PlaceholderObjectElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceholderObjectElement>(ref m_PlaceholderObjectElementType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<PlaceholderObjectElement> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					if (m_DeletedData.HasComponent(val[j].m_Object))
					{
						val.RemoveAtSwapBack(j--);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ValidateSubAreasJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_AreaGeometryData;

		public BufferTypeHandle<SubArea> m_SubAreaType;

		public BufferTypeHandle<SubAreaNode> m_SubAreaNodeType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<SubArea> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubArea>(ref m_SubAreaType);
			BufferAccessor<SubAreaNode> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubAreaNode>(ref m_SubAreaNodeType);
			AreaGeometryData areaData = default(AreaGeometryData);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<SubArea> val = bufferAccessor[i];
				DynamicBuffer<SubAreaNode> val2 = bufferAccessor2[i];
				int num = 0;
				int num2 = 0;
				for (int j = 0; j < val.Length; j++)
				{
					SubArea subArea = val[j];
					int2 nodeRange = subArea.m_NodeRange;
					subArea.m_NodeRange.x = num2;
					if (nodeRange.x != nodeRange.y && m_AreaGeometryData.TryGetComponent(subArea.m_Prefab, ref areaData))
					{
						float minNodeDistance = AreaUtils.GetMinNodeDistance(areaData);
						SubAreaNode subAreaNode = val2[nodeRange.x];
						val2[num2++] = subAreaNode;
						for (int k = nodeRange.x + 1; k < nodeRange.y; k++)
						{
							SubAreaNode subAreaNode2 = val2[k];
							if (math.distance(((float3)(ref subAreaNode.m_Position)).xz, ((float3)(ref subAreaNode2.m_Position)).xz) >= minNodeDistance)
							{
								subAreaNode = subAreaNode2;
								val2[num2++] = subAreaNode2;
							}
						}
						subAreaNode = val2[nodeRange.x];
						while (num2 > subArea.m_NodeRange.x)
						{
							SubAreaNode subAreaNode3 = val2[num2 - 1];
							if (math.distance(((float3)(ref subAreaNode.m_Position)).xz, ((float3)(ref subAreaNode3.m_Position)).xz) >= minNodeDistance)
							{
								break;
							}
							num2--;
						}
					}
					else
					{
						for (int l = nodeRange.x; l < nodeRange.y; l++)
						{
							val2[num2++] = val2[l];
						}
					}
					subArea.m_NodeRange.y = num2;
					int num3 = nodeRange.y - nodeRange.x;
					int num4 = subArea.m_NodeRange.y - subArea.m_NodeRange.x;
					if (num4 < num3)
					{
						Debug.Log((object)$"Invalid prefab sub-area nodes removed: {num3} => {num4}");
					}
					if (num4 >= 3)
					{
						val[num++] = subArea;
					}
					else
					{
						num2 = subArea.m_NodeRange.x;
					}
				}
				if (num < val.Length)
				{
					val.RemoveRange(num, val.Length - num);
				}
				if (num2 < val2.Length)
				{
					val2.RemoveRange(num2, val2.Length - num2);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
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

		public ComponentTypeHandle<AreaColorData> __Game_Prefabs_AreaColorData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LotData> __Game_Prefabs_LotData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DistrictData> __Game_Prefabs_DistrictData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MapTileData> __Game_Prefabs_MapTileData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpaceData> __Game_Prefabs_SpaceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SurfaceData> __Game_Prefabs_SurfaceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StorageAreaData> __Game_Prefabs_StorageAreaData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TerrainAreaData> __Game_Prefabs_TerrainAreaData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle;

		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RW_BufferTypeHandle;

		public BufferTypeHandle<SubArea> __Game_Prefabs_SubArea_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public BufferTypeHandle<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		public BufferTypeHandle<SubAreaNode> __Game_Prefabs_SubAreaNode_RW_BufferTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_AreaColorData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AreaColorData>(false);
			__Game_Prefabs_AreaGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AreaGeometryData>(false);
			__Game_Prefabs_LotData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LotData>(true);
			__Game_Prefabs_DistrictData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DistrictData>(true);
			__Game_Prefabs_MapTileData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MapTileData>(true);
			__Game_Prefabs_SpaceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpaceData>(true);
			__Game_Prefabs_SurfaceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SurfaceData>(true);
			__Game_Prefabs_StorageAreaData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StorageAreaData>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ExtractorAreaData>(true);
			__Game_Prefabs_TerrainAreaData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TerrainAreaData>(true);
			__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableObjectData>(false);
			__Game_Prefabs_SubObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(false);
			__Game_Prefabs_SubArea_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubArea>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceholderObjectElement>(false);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_SubAreaNode_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubAreaNode>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_SubAreaQuery;

	private EntityQuery m_PlaceholderQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<AreaData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_SubAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadWrite<SubArea>(),
			ComponentType.ReadWrite<SubAreaNode>()
		});
		m_PlaceholderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AreaData>(),
			ComponentType.ReadOnly<PlaceholderObjectElement>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_PrefabQuery, m_SubAreaQuery });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!((EntityQuery)(ref m_PrefabQuery)).IsEmptyIgnoreFilter)
		{
			InitializeAreaPrefabs();
		}
		if (!((EntityQuery)(ref m_SubAreaQuery)).IsEmptyIgnoreFilter)
		{
			ValidateSubAreas();
		}
	}

	private void InitializeAreaPrefabs()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		bool flag = false;
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<AreaColorData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<AreaColorData>(ref __TypeHandle.__Game_Prefabs_AreaColorData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<AreaGeometryData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<LotData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<LotData>(ref __TypeHandle.__Game_Prefabs_LotData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<DistrictData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<DistrictData>(ref __TypeHandle.__Game_Prefabs_DistrictData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<MapTileData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<MapTileData>(ref __TypeHandle.__Game_Prefabs_MapTileData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SpaceData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<SpaceData>(ref __TypeHandle.__Game_Prefabs_SpaceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SurfaceData> componentTypeHandle9 = InternalCompilerInterface.GetComponentTypeHandle<SurfaceData>(ref __TypeHandle.__Game_Prefabs_SurfaceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<StorageAreaData> componentTypeHandle10 = InternalCompilerInterface.GetComponentTypeHandle<StorageAreaData>(ref __TypeHandle.__Game_Prefabs_StorageAreaData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ExtractorAreaData> componentTypeHandle11 = InternalCompilerInterface.GetComponentTypeHandle<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<TerrainAreaData> componentTypeHandle12 = InternalCompilerInterface.GetComponentTypeHandle<TerrainAreaData>(ref __TypeHandle.__Game_Prefabs_TerrainAreaData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SpawnableObjectData> componentTypeHandle13 = InternalCompilerInterface.GetComponentTypeHandle<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubObject> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubArea> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				if (((ArchetypeChunk)(ref val2)).Has<Deleted>(ref componentTypeHandle))
				{
					flag = ((ArchetypeChunk)(ref val2)).Has<SpawnableObjectData>(ref componentTypeHandle13);
					continue;
				}
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
				NativeArray<AreaColorData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<AreaColorData>(ref componentTypeHandle3);
				NativeArray<AreaGeometryData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<AreaGeometryData>(ref componentTypeHandle4);
				NativeArray<SpawnableObjectData> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<SpawnableObjectData>(ref componentTypeHandle13);
				NativeArray<LotData> nativeArray5 = ((ArchetypeChunk)(ref val2)).GetNativeArray<LotData>(ref componentTypeHandle5);
				AreaType areaType = AreaType.None;
				GeometryFlags geometryFlags = (GeometryFlags)0;
				NativeArray<ExtractorAreaData> val3 = default(NativeArray<ExtractorAreaData>);
				if (nativeArray5.Length != 0)
				{
					areaType = AreaType.Lot;
					geometryFlags = GeometryFlags.PhysicalGeometry | GeometryFlags.PseudoRandom;
					if (((ArchetypeChunk)(ref val2)).Has<StorageAreaData>(ref componentTypeHandle10))
					{
						geometryFlags |= GeometryFlags.CanOverrideObjects;
					}
					val3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ExtractorAreaData>(ref componentTypeHandle11);
				}
				else if (((ArchetypeChunk)(ref val2)).Has<DistrictData>(ref componentTypeHandle6))
				{
					areaType = AreaType.District;
					geometryFlags = GeometryFlags.OnWaterSurface;
				}
				else if (((ArchetypeChunk)(ref val2)).Has<MapTileData>(ref componentTypeHandle7))
				{
					areaType = AreaType.MapTile;
					geometryFlags = GeometryFlags.ProtectedArea | GeometryFlags.OnWaterSurface;
				}
				else if (((ArchetypeChunk)(ref val2)).Has<SpaceData>(ref componentTypeHandle8))
				{
					areaType = AreaType.Space;
				}
				else if (((ArchetypeChunk)(ref val2)).Has<SurfaceData>(ref componentTypeHandle9))
				{
					areaType = AreaType.Surface;
				}
				if (((ArchetypeChunk)(ref val2)).Has<TerrainAreaData>(ref componentTypeHandle12))
				{
					geometryFlags |= GeometryFlags.ShiftTerrain;
				}
				float minNodeDistance = AreaUtils.GetMinNodeDistance(areaType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					AreaPrefab prefab = m_PrefabSystem.GetPrefab<AreaPrefab>(nativeArray[j]);
					AreaColorData areaColorData = nativeArray2[j];
					AreaGeometryData areaGeometryData = nativeArray3[j];
					areaGeometryData.m_Type = areaType;
					areaGeometryData.m_Flags = geometryFlags;
					areaGeometryData.m_SnapDistance = minNodeDistance;
					if (prefab.Has<ClearArea>())
					{
						areaGeometryData.m_Flags |= GeometryFlags.ClearArea;
					}
					if (prefab.Has<ClipArea>())
					{
						areaGeometryData.m_Flags |= GeometryFlags.ClipTerrain;
					}
					if (val3.IsCreated && val3[j].m_MapFeature != MapFeature.Forest)
					{
						areaGeometryData.m_Flags |= GeometryFlags.CanOverrideObjects;
					}
					if (areaType == AreaType.Lot)
					{
						LotData lotData = nativeArray5[j];
						if (lotData.m_OnWater)
						{
							areaGeometryData.m_Flags |= GeometryFlags.OnWaterSurface | GeometryFlags.RequireWater;
						}
						if (lotData.m_AllowOverlap)
						{
							areaGeometryData.m_Flags &= ~GeometryFlags.PhysicalGeometry;
							areaGeometryData.m_Flags |= GeometryFlags.HiddenIngame;
						}
					}
					areaColorData.m_FillColor = Color32.op_Implicit(prefab.m_Color);
					areaColorData.m_EdgeColor = Color32.op_Implicit(prefab.m_EdgeColor);
					areaColorData.m_SelectionFillColor = Color32.op_Implicit(prefab.m_SelectionColor);
					areaColorData.m_SelectionEdgeColor = Color32.op_Implicit(prefab.m_SelectionEdgeColor);
					if (prefab.TryGet<RenderedArea>(out var component))
					{
						areaGeometryData.m_LodBias = component.m_LodBias;
					}
					nativeArray2[j] = areaColorData;
					nativeArray3[j] = areaGeometryData;
				}
				BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<SubObject>(ref bufferTypeHandle);
				for (int k = 0; k < bufferAccessor.Length; k++)
				{
					AreaSubObjects component2 = m_PrefabSystem.GetPrefab<AreaPrefab>(nativeArray[k]).GetComponent<AreaSubObjects>();
					DynamicBuffer<SubObject> val4 = bufferAccessor[k];
					for (int l = 0; l < component2.m_SubObjects.Length; l++)
					{
						AreaSubObjectInfo obj = component2.m_SubObjects[l];
						ObjectPrefab prefab2 = obj.m_Object;
						SubObject subObject = new SubObject
						{
							m_Prefab = m_PrefabSystem.GetEntity(prefab2),
							m_Position = default(float3),
							m_Rotation = quaternion.identity,
							m_Probability = 100
						};
						if (obj.m_BorderPlacement)
						{
							subObject.m_Flags |= SubObjectFlags.EdgePlacement;
						}
						val4.Add(subObject);
					}
				}
				if (nativeArray4.Length != 0)
				{
					NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
					for (int m = 0; m < nativeArray4.Length; m++)
					{
						Entity obj2 = nativeArray6[m];
						SpawnableObjectData spawnableObjectData = nativeArray4[m];
						SpawnableArea component3 = m_PrefabSystem.GetPrefab<AreaPrefab>(nativeArray[m]).GetComponent<SpawnableArea>();
						for (int n = 0; n < component3.m_Placeholders.Length; n++)
						{
							AreaPrefab prefab3 = component3.m_Placeholders[n];
							Entity entity = m_PrefabSystem.GetEntity(prefab3);
							EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).GetBuffer<PlaceholderObjectElement>(entity, false).Add(new PlaceholderObjectElement(obj2));
						}
						spawnableObjectData.m_Probability = component3.m_Probability;
						nativeArray4[m] = spawnableObjectData;
					}
				}
				BufferAccessor<SubArea> bufferAccessor2 = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<SubArea>(ref bufferTypeHandle2);
				for (int num = 0; num < bufferAccessor2.Length; num++)
				{
					MasterArea component4 = m_PrefabSystem.GetPrefab<AreaPrefab>(nativeArray[num]).GetComponent<MasterArea>();
					DynamicBuffer<SubArea> val5 = bufferAccessor2[num];
					for (int num2 = 0; num2 < component4.m_SlaveAreas.Length; num2++)
					{
						AreaPrefab area = component4.m_SlaveAreas[num2].m_Area;
						val5.Add(new SubArea
						{
							m_Prefab = m_PrefabSystem.GetEntity(area),
							m_NodeRange = int2.op_Implicit(-1)
						});
					}
				}
			}
			if (flag)
			{
				JobHandle dependency = JobChunkExtensions.ScheduleParallel<FixPlaceholdersJob>(new FixPlaceholdersJob
				{
					m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PlaceholderObjectElementType = InternalCompilerInterface.GetBufferTypeHandle<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
				}, m_PlaceholderQuery, ((SystemBase)this).Dependency);
				((SystemBase)this).Dependency = dependency;
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void ValidateSubAreas()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<ValidateSubAreasJob>(new ValidateSubAreasJob
		{
			m_AreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaType = InternalCompilerInterface.GetBufferTypeHandle<SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreaNodeType = InternalCompilerInterface.GetBufferTypeHandle<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_SubAreaQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
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
	public AreaInitializeSystem()
	{
	}
}
