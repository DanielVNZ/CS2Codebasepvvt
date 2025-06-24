using System.Runtime.CompilerServices;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Areas;

[CompilerGenerated]
public class ServiceDistrictSystem : GameSystemBase
{
	[BurstCompile]
	private struct RemoveServiceDistrictsJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<Entity> m_DeletedDistricts;

		public BufferTypeHandle<ServiceDistrict> m_ServiceDistrictType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ServiceDistrict> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDistrict>(ref m_ServiceDistrictType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<ServiceDistrict> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity district = val[j].m_District;
					for (int k = 0; k < m_DeletedDistricts.Length; k++)
					{
						if (m_DeletedDistricts[k] == district)
						{
							val.RemoveAt(j--);
							break;
						}
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
		public BufferTypeHandle<ServiceDistrict> __Game_Areas_ServiceDistrict_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Areas_ServiceDistrict_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDistrict>(false);
		}
	}

	private EntityQuery m_DeletedDistrictQuery;

	private EntityQuery m_ServiceDistrictQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DeletedDistrictQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<District>(),
			ComponentType.Exclude<Temp>()
		});
		m_ServiceDistrictQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ServiceDistrict>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DeletedDistrictQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ServiceDistrictQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<Entity> deletedDistricts = ((EntityQuery)(ref m_DeletedDistrictQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<RemoveServiceDistrictsJob>(new RemoveServiceDistrictsJob
		{
			m_DeletedDistricts = deletedDistricts,
			m_ServiceDistrictType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_ServiceDistrictQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		deletedDistricts.Dispose(val2);
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
	public ServiceDistrictSystem()
	{
	}
}
