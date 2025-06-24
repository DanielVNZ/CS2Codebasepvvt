using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
using Game.Common;
using Game.Objects;
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

namespace Game.Rendering;

[CompilerGenerated]
public class AreaColorSystem : GameSystemBase
{
	[BurstCompile]
	private struct FillColorDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Batch> m_BatchType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Lot> m_LotType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Color> m_ColorData;

		[NativeDisableParallelForRestriction]
		public NativeList<AreaColorData> m_AreaColorData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Batch> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Batch>(ref m_BatchType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<Triangle> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Lot>(ref m_LotType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Batch batch = nativeArray[i];
				if (((NativeHeapBlock)(ref batch.m_BatchAllocation)).Empty)
				{
					continue;
				}
				DynamicBuffer<Triangle> val = bufferAccessor[i];
				Vector3 val2 = default(Vector3);
				if (nativeArray2.Length != 0)
				{
					Owner owner = nativeArray2[i];
					while (true)
					{
						if (m_ColorData.HasComponent(owner.m_Owner))
						{
							Color color = m_ColorData[owner.m_Owner];
							if (flag)
							{
								if (color.m_Index != 0)
								{
									val2.x = (float)(int)color.m_Index + 0.5f;
									val2.y = (float)(int)color.m_Value * 0.003921569f;
									val2.z = 1f;
								}
							}
							else if (color.m_SubColor)
							{
								val2.x = (float)(int)color.m_Index + 0.5f;
								val2.y = (float)(int)color.m_Value * 0.003921569f;
								val2.z = 1f;
							}
							break;
						}
						if (!m_OwnerData.HasComponent(owner.m_Owner))
						{
							break;
						}
						owner = m_OwnerData[owner.m_Owner];
					}
				}
				int begin = (int)((NativeHeapBlock)(ref batch.m_BatchAllocation)).Begin;
				for (int j = 0; j < val.Length; j++)
				{
					m_AreaColorData[begin++] = new AreaColorData
					{
						m_Color = float3.op_Implicit(val2)
					};
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
		public ComponentTypeHandle<Batch> __Game_Areas_Batch_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lot> __Game_Areas_Lot_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Color> __Game_Objects_Color_RO_ComponentLookup;

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
			__Game_Areas_Batch_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Batch>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Areas_Lot_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lot>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Color>(true);
		}
	}

	private AreaBatchSystem m_AreaBatchSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_AreaQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AreaBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaBatchSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Batch>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_ToolSystem.activeInfoview != (Object)null)
		{
			JobHandle dependencies;
			JobHandle val = JobChunkExtensions.ScheduleParallel<FillColorDataJob>(new FillColorDataJob
			{
				m_BatchType = InternalCompilerInterface.GetComponentTypeHandle<Batch>(ref __TypeHandle.__Game_Areas_Batch_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LotType = InternalCompilerInterface.GetComponentTypeHandle<Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ColorData = InternalCompilerInterface.GetComponentLookup<Color>(ref __TypeHandle.__Game_Objects_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaColorData = m_AreaBatchSystem.GetColorData(out dependencies)
			}, m_AreaQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			m_AreaBatchSystem.AddColorWriter(val);
			((SystemBase)this).Dependency = val;
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
	public AreaColorSystem()
	{
	}
}
