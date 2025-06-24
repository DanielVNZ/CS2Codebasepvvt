using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class AreaDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct AreaGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_LotOption;

		[ReadOnly]
		public bool m_DistrictOption;

		[ReadOnly]
		public bool m_MapTileOption;

		[ReadOnly]
		public bool m_SpaceOption;

		[ReadOnly]
		public bool m_SurfaceOption;

		[ReadOnly]
		public ComponentTypeHandle<Area> m_AreaType;

		[ReadOnly]
		public ComponentTypeHandle<Lot> m_LotType;

		[ReadOnly]
		public ComponentTypeHandle<District> m_DistrictType;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> m_MapTileType;

		[ReadOnly]
		public ComponentTypeHandle<Space> m_SpaceType;

		[ReadOnly]
		public ComponentTypeHandle<Surface> m_SurfaceType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			Color val;
			float num;
			if (((ArchetypeChunk)(ref chunk)).Has<Lot>(ref m_LotType))
			{
				if (!m_LotOption)
				{
					return;
				}
				val = Color.cyan;
				num = AreaUtils.GetMinNodeDistance(AreaType.Lot) * 0.5f;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<District>(ref m_DistrictType))
			{
				if (!m_DistrictOption)
				{
					return;
				}
				val = Color.white;
				num = AreaUtils.GetMinNodeDistance(AreaType.District) * 0.5f;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<MapTile>(ref m_MapTileType))
			{
				if (!m_MapTileOption)
				{
					return;
				}
				val = Color.yellow;
				num = AreaUtils.GetMinNodeDistance(AreaType.MapTile) * 0.5f;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Space>(ref m_SpaceType))
			{
				if (!m_SpaceOption)
				{
					return;
				}
				val = Color.green;
				num = AreaUtils.GetMinNodeDistance(AreaType.Space) * 0.5f;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Surface>(ref m_SurfaceType))
			{
				if (!m_SurfaceOption)
				{
					return;
				}
				val = Color.magenta;
				num = AreaUtils.GetMinNodeDistance(AreaType.Surface) * 0.5f;
			}
			else
			{
				val = Color.black;
				num = AreaUtils.GetMinNodeDistance(AreaType.None) * 0.5f;
			}
			NativeArray<Area> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Area>(ref m_AreaType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			if (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType))
			{
				val = Color.red;
			}
			else if (((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType))
			{
				val = Color.blue;
			}
			Color val2 = Color.gray * 0.5f;
			float num2 = num * 0.2f;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Area area = nativeArray[i];
				DynamicBuffer<Node> val3 = bufferAccessor[i];
				DynamicBuffer<Triangle> val4 = bufferAccessor2[i];
				float3 val5 = val3[0].m_Position;
				if (val3[0].m_Elevation == float.MinValue)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireSphere(val5, num, val, 1, 2, 0, 36);
				}
				else
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val5, num, val);
				}
				for (int j = 1; j < val3.Length; j++)
				{
					float3 position = val3[j].m_Position;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val5, position, val);
					if (val3[j].m_Elevation == float.MinValue)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireSphere(position, num, val, 1, 2, 0, 36);
					}
					else
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(position, num, val);
					}
					val5 = position;
				}
				if ((area.m_Flags & AreaFlags.Complete) != 0)
				{
					float3 position2 = val3[0].m_Position;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val5, position2, val);
				}
				for (int k = 0; k < val4.Length; k++)
				{
					Triangle triangle = val4[k];
					float3 position3 = val3[triangle.m_Indices.x].m_Position;
					float3 position4 = val3[triangle.m_Indices.y].m_Position;
					float3 position5 = val3[triangle.m_Indices.z].m_Position;
					float3 val6 = position4 - position3;
					float3 val7 = position5 - position4;
					float3 val8 = position3 - position5;
					MathUtils.TryNormalize(ref val6, num2);
					MathUtils.TryNormalize(ref val7, num2);
					MathUtils.TryNormalize(ref val8, num2);
					position3 += val6 - val8;
					position4 += val7 - val6;
					position5 += val8 - val7;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position3, position4, val2);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position4, position5, val2);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position5, position3, val2);
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
		public ComponentTypeHandle<Area> __Game_Areas_Area_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lot> __Game_Areas_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<District> __Game_Areas_District_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MapTile> __Game_Areas_MapTile_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Space> __Game_Areas_Space_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Surface> __Game_Areas_Surface_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

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
			__Game_Areas_Area_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Area>(true);
			__Game_Areas_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lot>(true);
			__Game_Areas_District_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<District>(true);
			__Game_Areas_MapTile_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MapTile>(true);
			__Game_Areas_Space_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Space>(true);
			__Game_Areas_Surface_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Surface>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
		}
	}

	private EntityQuery m_AreaGroup;

	private GizmosSystem m_GizmosSystem;

	private Option m_LotOption;

	private Option m_DistrictOption;

	private Option m_MapTileOption;

	private Option m_SpaceOption;

	private Option m_SurfaceOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_AreaGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Triangle>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Hidden>()
		});
		m_LotOption = AddOption("Lots", defaultEnabled: true);
		m_DistrictOption = AddOption("Districts", defaultEnabled: true);
		m_MapTileOption = AddOption("Map Tiles", defaultEnabled: false);
		m_SpaceOption = AddOption("Spaces", defaultEnabled: true);
		m_SurfaceOption = AddOption("Surfaces", defaultEnabled: true);
		((ComponentSystemBase)this).RequireForUpdate(m_AreaGroup);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<AreaGizmoJob>(new AreaGizmoJob
		{
			m_LotOption = m_LotOption.enabled,
			m_DistrictOption = m_DistrictOption.enabled,
			m_MapTileOption = m_MapTileOption.enabled,
			m_SpaceOption = m_SpaceOption.enabled,
			m_SurfaceOption = m_SurfaceOption.enabled,
			m_AreaType = InternalCompilerInterface.GetComponentTypeHandle<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LotType = InternalCompilerInterface.GetComponentTypeHandle<Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictType = InternalCompilerInterface.GetComponentTypeHandle<District>(ref __TypeHandle.__Game_Areas_District_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MapTileType = InternalCompilerInterface.GetComponentTypeHandle<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpaceType = InternalCompilerInterface.GetComponentTypeHandle<Space>(ref __TypeHandle.__Game_Areas_Space_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SurfaceType = InternalCompilerInterface.GetComponentTypeHandle<Surface>(ref __TypeHandle.__Game_Areas_Surface_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, m_AreaGroup, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
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
	public AreaDebugSystem()
	{
	}
}
