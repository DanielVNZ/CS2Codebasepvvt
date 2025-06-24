using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class InitializeObsoleteSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeObsoleteJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<StackData> m_StackType;

		[ReadOnly]
		public ComponentTypeHandle<AreaGeometryData> m_AreaGeometryType;

		public ComponentTypeHandle<ObjectData> m_ObjectType;

		public ComponentTypeHandle<MovingObjectData> m_MovingObjectType;

		public ComponentTypeHandle<ObjectGeometryData> m_ObjectGeometryType;

		public ComponentTypeHandle<NetData> m_NetType;

		public ComponentTypeHandle<NetGeometryData> m_NetGeometryType;

		public ComponentTypeHandle<AggregateNetData> m_AggregateNetType;

		public ComponentTypeHandle<NetNameData> m_NetNameType;

		public ComponentTypeHandle<NetArrowData> m_NetArrowType;

		public ComponentTypeHandle<NetLaneData> m_NetLaneType;

		public ComponentTypeHandle<NetLaneArchetypeData> m_NetLaneArchetypeType;

		public ComponentTypeHandle<AreaData> m_AreaType;

		public BufferTypeHandle<SubMesh> m_SubMeshType;

		public BufferTypeHandle<NetGeometrySection> m_NetGeometrySectionType;

		[ReadOnly]
		public MeshSettingsData m_MeshSettingsData;

		[ReadOnly]
		public EntityArchetype m_ObjectArchetype;

		[ReadOnly]
		public EntityArchetype m_ObjectGeometryArchetype;

		[ReadOnly]
		public EntityArchetype m_NetGeometryNodeArchetype;

		[ReadOnly]
		public EntityArchetype m_NetGeometryEdgeArchetype;

		[ReadOnly]
		public EntityArchetype m_NetNodeCompositionArchetype;

		[ReadOnly]
		public EntityArchetype m_NetEdgeCompositionArchetype;

		[ReadOnly]
		public EntityArchetype m_NetAggregateArchetype;

		[ReadOnly]
		public EntityArchetype m_AreaLotArchetype;

		[ReadOnly]
		public EntityArchetype m_AreaDistrictArchetype;

		[ReadOnly]
		public EntityArchetype m_AreaMapTileArchetype;

		[ReadOnly]
		public EntityArchetype m_AreaSpaceArchetype;

		[ReadOnly]
		public EntityArchetype m_AreaSurfaceArchetype;

		[ReadOnly]
		public NetLaneArchetypeData m_NetLaneArchetypeData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<ObjectData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectData>(ref m_ObjectType);
			NativeArray<MovingObjectData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MovingObjectData>(ref m_MovingObjectType);
			NativeArray<ObjectGeometryData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ObjectGeometryData>(ref m_ObjectGeometryType);
			NativeArray<NetData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetData>(ref m_NetType);
			NativeArray<NetGeometryData> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetGeometryData>(ref m_NetGeometryType);
			NativeArray<AggregateNetData> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AggregateNetData>(ref m_AggregateNetType);
			NativeArray<NetNameData> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetNameData>(ref m_NetNameType);
			NativeArray<NetArrowData> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetArrowData>(ref m_NetArrowType);
			NativeArray<StackData> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StackData>(ref m_StackType);
			NativeArray<NetLaneData> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetLaneData>(ref m_NetLaneType);
			NativeArray<NetLaneArchetypeData> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetLaneArchetypeData>(ref m_NetLaneArchetypeType);
			NativeArray<AreaData> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AreaData>(ref m_AreaType);
			NativeArray<AreaGeometryData> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AreaGeometryData>(ref m_AreaGeometryType);
			BufferAccessor<SubMesh> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubMesh>(ref m_SubMeshType);
			BufferAccessor<NetGeometrySection> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetGeometrySection>(ref m_NetGeometrySectionType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			AreaGeometryData areaGeometryData = default(AreaGeometryData);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				if (nativeArray.Length != 0)
				{
					ref ObjectData reference = ref CollectionUtils.ElementAt<ObjectData>(nativeArray, num);
					if (nativeArray3.Length != 0)
					{
						ref ObjectGeometryData reference2 = ref CollectionUtils.ElementAt<ObjectGeometryData>(nativeArray3, num);
						reference.m_Archetype = m_ObjectGeometryArchetype;
						reference2.m_MinLod = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(MathUtils.Size(reference2.m_Bounds)));
						reference2.m_Layers = MeshLayer.Default;
					}
					else
					{
						reference.m_Archetype = m_ObjectArchetype;
					}
					if (nativeArray2.Length != 0)
					{
						CollectionUtils.ElementAt<MovingObjectData>(nativeArray2, num).m_StoppedArchetype = m_ObjectGeometryArchetype;
					}
				}
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<SubMesh> val2 = bufferAccessor[num];
					if (val2.Length == 0)
					{
						SubMesh subMesh = new SubMesh(m_MeshSettingsData.m_MissingObjectMesh, SubMeshFlags.DefaultMissingMesh, 0);
						if (nativeArray9.Length != 0)
						{
							subMesh.m_Flags |= SubMeshFlags.IsStackMiddle;
						}
						val2.Add(subMesh);
					}
				}
				if (nativeArray4.Length != 0)
				{
					ref NetData reference3 = ref CollectionUtils.ElementAt<NetData>(nativeArray4, num);
					if (nativeArray5.Length != 0)
					{
						ref NetGeometryData reference4 = ref CollectionUtils.ElementAt<NetGeometryData>(nativeArray5, num);
						reference3.m_NodeArchetype = m_NetGeometryNodeArchetype;
						reference3.m_EdgeArchetype = m_NetGeometryEdgeArchetype;
						reference4.m_NodeCompositionArchetype = m_NetNodeCompositionArchetype;
						reference4.m_EdgeCompositionArchetype = m_NetEdgeCompositionArchetype;
					}
				}
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<NetGeometrySection> val3 = bufferAccessor2[num];
					if (val3.Length == 0)
					{
						NetGeometrySection netGeometrySection = new NetGeometrySection
						{
							m_Section = m_MeshSettingsData.m_MissingNetSection
						};
						val3.Add(netGeometrySection);
					}
				}
				if (nativeArray6.Length != 0)
				{
					ref AggregateNetData reference5 = ref CollectionUtils.ElementAt<AggregateNetData>(nativeArray6, num);
					if (nativeArray7.Length != 0)
					{
						ref NetNameData reference6 = ref CollectionUtils.ElementAt<NetNameData>(nativeArray7, num);
						reference6.m_Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192);
						reference6.m_SelectedColor = new Color32((byte)192, (byte)192, byte.MaxValue, (byte)192);
					}
					if (nativeArray8.Length != 0)
					{
						ref NetArrowData reference7 = ref CollectionUtils.ElementAt<NetArrowData>(nativeArray8, num);
						reference7.m_RoadColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192);
						reference7.m_TrackColor = new Color32(byte.MaxValue, byte.MaxValue, (byte)192, (byte)192);
					}
					reference5.m_Archetype = m_NetAggregateArchetype;
				}
				if (nativeArray10.Length != 0)
				{
					CollectionUtils.ElementAt<NetLaneData>(nativeArray10, num).m_Flags &= ~LaneFlags.PseudoRandom;
					nativeArray11[num] = m_NetLaneArchetypeData;
				}
				if (nativeArray12.Length == 0)
				{
					continue;
				}
				ref AreaData reference8 = ref CollectionUtils.ElementAt<AreaData>(nativeArray12, num);
				if (CollectionUtils.TryGet<AreaGeometryData>(nativeArray13, num, ref areaGeometryData))
				{
					switch (areaGeometryData.m_Type)
					{
					case AreaType.Lot:
						reference8.m_Archetype = m_AreaLotArchetype;
						break;
					case AreaType.District:
						reference8.m_Archetype = m_AreaDistrictArchetype;
						break;
					case AreaType.MapTile:
						reference8.m_Archetype = m_AreaMapTileArchetype;
						break;
					case AreaType.Space:
						reference8.m_Archetype = m_AreaSpaceArchetype;
						break;
					case AreaType.Surface:
						reference8.m_Archetype = m_AreaSurfaceArchetype;
						break;
					}
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
		public ComponentTypeHandle<StackData> __Game_Prefabs_StackData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ObjectData> __Game_Prefabs_ObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<MovingObjectData> __Game_Prefabs_MovingObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetData> __Game_Prefabs_NetData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetGeometryData> __Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AggregateNetData> __Game_Prefabs_AggregateNetData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetNameData> __Game_Prefabs_NetNameData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetArrowData> __Game_Prefabs_NetArrowData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetLaneData> __Game_Prefabs_NetLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetLaneArchetypeData> __Game_Prefabs_NetLaneArchetypeData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<AreaData> __Game_Prefabs_AreaData_RW_ComponentTypeHandle;

		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<NetGeometrySection> __Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle;

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
			__Game_Prefabs_StackData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StackData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AreaGeometryData>(true);
			__Game_Prefabs_ObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectData>(false);
			__Game_Prefabs_MovingObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MovingObjectData>(false);
			__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(false);
			__Game_Prefabs_NetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetData>(false);
			__Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetGeometryData>(false);
			__Game_Prefabs_AggregateNetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AggregateNetData>(false);
			__Game_Prefabs_NetNameData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetNameData>(false);
			__Game_Prefabs_NetArrowData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetArrowData>(false);
			__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneData>(false);
			__Game_Prefabs_NetLaneArchetypeData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneArchetypeData>(false);
			__Game_Prefabs_AreaData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AreaData>(false);
			__Game_Prefabs_SubMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(false);
			__Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetGeometrySection>(false);
		}
	}

	private EntityQuery m_ObsoleteQuery;

	private EntityQuery m_MeshSettingsQuery;

	private HashSet<ComponentType> m_ArchetypeComponents;

	private Dictionary<Type, PrefabBase> m_PrefabInstances;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<NetData>(),
			ComponentType.ReadOnly<AggregateNetData>(),
			ComponentType.ReadOnly<NetLaneArchetypeData>(),
			ComponentType.ReadOnly<AreaData>()
		};
		val.Disabled = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		array[0] = val;
		m_ObsoleteQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_MeshSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshSettingsData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ObsoleteQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		m_ArchetypeComponents = new HashSet<ComponentType>();
		m_PrefabInstances = new Dictionary<Type, PrefabBase>();
		EntityArchetype archetype = GetArchetype<ObjectPrefab>();
		EntityArchetype archetype2 = GetArchetype<StaticObjectPrefab>();
		EntityArchetype archetype3 = GetArchetype<NetGeometryPrefab, Game.Net.Node>();
		EntityArchetype archetype4 = GetArchetype<NetGeometryPrefab, Edge>();
		EntityArchetype archetype5 = GetArchetype<NetGeometryPrefab, NetCompositionData, NetCompositionCrosswalk>();
		EntityArchetype archetype6 = GetArchetype<NetGeometryPrefab, NetCompositionData, NetCompositionLane>();
		EntityArchetype archetype7 = GetArchetype<AggregateNetPrefab>();
		EntityArchetype archetype8 = GetArchetype<LotPrefab, Area>();
		EntityArchetype archetype9 = GetArchetype<DistrictPrefab, Area>();
		EntityArchetype archetype10 = GetArchetype<MapTilePrefab, Area>();
		EntityArchetype archetype11 = GetArchetype<SpacePrefab, Area>();
		EntityArchetype archetype12 = GetArchetype<SurfacePrefab, Area>();
		NetLaneArchetypeData netLaneArchetypeData = default(NetLaneArchetypeData);
		netLaneArchetypeData.m_LaneArchetype = GetArchetype<NetLanePrefab, Lane>();
		netLaneArchetypeData.m_AreaLaneArchetype = GetArchetype<NetLanePrefab, Lane, AreaLane>();
		netLaneArchetypeData.m_EdgeLaneArchetype = GetArchetype<NetLanePrefab, Lane, EdgeLane>();
		netLaneArchetypeData.m_NodeLaneArchetype = GetArchetype<NetLanePrefab, Lane, NodeLane>();
		netLaneArchetypeData.m_EdgeSlaveArchetype = GetArchetype<NetLanePrefab, Lane, SlaveLane, EdgeLane>();
		netLaneArchetypeData.m_NodeSlaveArchetype = GetArchetype<NetLanePrefab, Lane, SlaveLane, NodeLane>();
		netLaneArchetypeData.m_EdgeMasterArchetype = GetArchetype<NetLanePrefab, Lane, MasterLane, EdgeLane>();
		netLaneArchetypeData.m_NodeMasterArchetype = GetArchetype<NetLanePrefab, Lane, MasterLane, NodeLane>();
		foreach (KeyValuePair<Type, PrefabBase> item in m_PrefabInstances)
		{
			Object.DestroyImmediate((Object)(object)item.Value);
		}
		m_ArchetypeComponents = null;
		m_PrefabInstances = null;
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<InitializeObsoleteJob>(new InitializeObsoleteJob
		{
			m_StackType = InternalCompilerInterface.GetComponentTypeHandle<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AreaGeometryType = InternalCompilerInterface.GetComponentTypeHandle<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectType = InternalCompilerInterface.GetComponentTypeHandle<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MovingObjectType = InternalCompilerInterface.GetComponentTypeHandle<MovingObjectData>(ref __TypeHandle.__Game_Prefabs_MovingObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryType = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetType = InternalCompilerInterface.GetComponentTypeHandle<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateNetType = InternalCompilerInterface.GetComponentTypeHandle<AggregateNetData>(ref __TypeHandle.__Game_Prefabs_AggregateNetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetNameType = InternalCompilerInterface.GetComponentTypeHandle<NetNameData>(ref __TypeHandle.__Game_Prefabs_NetNameData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetArrowType = InternalCompilerInterface.GetComponentTypeHandle<NetArrowData>(ref __TypeHandle.__Game_Prefabs_NetArrowData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneType = InternalCompilerInterface.GetComponentTypeHandle<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneArchetypeType = InternalCompilerInterface.GetComponentTypeHandle<NetLaneArchetypeData>(ref __TypeHandle.__Game_Prefabs_NetLaneArchetypeData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<AreaData>(ref __TypeHandle.__Game_Prefabs_AreaData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshType = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometrySectionType = InternalCompilerInterface.GetBufferTypeHandle<NetGeometrySection>(ref __TypeHandle.__Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MeshSettingsData = ((EntityQuery)(ref m_MeshSettingsQuery)).GetSingleton<MeshSettingsData>(),
			m_ObjectArchetype = archetype,
			m_ObjectGeometryArchetype = archetype2,
			m_NetGeometryNodeArchetype = archetype3,
			m_NetGeometryEdgeArchetype = archetype4,
			m_NetNodeCompositionArchetype = archetype5,
			m_NetEdgeCompositionArchetype = archetype6,
			m_NetAggregateArchetype = archetype7,
			m_AreaLotArchetype = archetype8,
			m_AreaDistrictArchetype = archetype9,
			m_AreaMapTileArchetype = archetype10,
			m_AreaSpaceArchetype = archetype11,
			m_AreaSurfaceArchetype = archetype12,
			m_NetLaneArchetypeData = netLaneArchetypeData
		}, m_ObsoleteQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
	}

	private T GetPrefabInstance<T>() where T : PrefabBase
	{
		if (m_PrefabInstances.TryGetValue(typeof(T), out var value))
		{
			return (T)value;
		}
		T val = ScriptableObject.CreateInstance<T>();
		m_PrefabInstances.Add(typeof(T), val);
		return val;
	}

	private EntityArchetype GetArchetype<T>() where T : PrefabBase
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		T prefabInstance = GetPrefabInstance<T>();
		m_ArchetypeComponents.Clear();
		prefabInstance.GetArchetypeComponents(m_ArchetypeComponents);
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Created>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Updated>());
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(m_ArchetypeComponents));
	}

	private EntityArchetype GetArchetype<T, TComponentType>() where T : PrefabBase
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		T prefabInstance = GetPrefabInstance<T>();
		m_ArchetypeComponents.Clear();
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType>());
		prefabInstance.GetArchetypeComponents(m_ArchetypeComponents);
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Created>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Updated>());
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(m_ArchetypeComponents));
	}

	private EntityArchetype GetArchetype<T, TComponentType1, TComponentType2>() where T : PrefabBase
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		T prefabInstance = GetPrefabInstance<T>();
		m_ArchetypeComponents.Clear();
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType1>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType2>());
		prefabInstance.GetArchetypeComponents(m_ArchetypeComponents);
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Created>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Updated>());
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(m_ArchetypeComponents));
	}

	private EntityArchetype GetArchetype<T, TComponentType1, TComponentType2, TComponentType3>() where T : PrefabBase
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		T prefabInstance = GetPrefabInstance<T>();
		m_ArchetypeComponents.Clear();
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType1>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType2>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<TComponentType3>());
		prefabInstance.GetArchetypeComponents(m_ArchetypeComponents);
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Created>());
		m_ArchetypeComponents.Add(ComponentType.ReadWrite<Updated>());
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(m_ArchetypeComponents));
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
	public InitializeObsoleteSystem()
	{
	}
}
