using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class InitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeEdgesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> m_WaterPipeConnectionData;

		public BufferTypeHandle<ServiceCoverage> m_ServiceCoverageType;

		public BufferTypeHandle<ResourceAvailability> m_ResourceAvailabilityType;

		public ComponentTypeHandle<WaterPipeConnection> m_WaterPipeConnectionType;

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
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<ServiceCoverage> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceCoverage>(ref m_ServiceCoverageType);
			BufferAccessor<ResourceAvailability> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ResourceAvailability>(ref m_ResourceAvailabilityType);
			NativeArray<WaterPipeConnection> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterPipeConnection>(ref m_WaterPipeConnectionType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<ServiceCoverage> val = bufferAccessor[i];
				val.ResizeUninitialized(9);
				for (int j = 0; j < val.Length; j++)
				{
					val[j] = default(ServiceCoverage);
				}
			}
			for (int k = 0; k < bufferAccessor2.Length; k++)
			{
				DynamicBuffer<ResourceAvailability> val2 = bufferAccessor2[k];
				val2.ResizeUninitialized(34);
				for (int l = 0; l < val2.Length; l++)
				{
					val2[l] = default(ResourceAvailability);
				}
			}
			for (int m = 0; m < nativeArray2.Length; m++)
			{
				PrefabRef prefabRef = nativeArray[m];
				WaterPipeConnection waterPipeConnection = nativeArray2[m];
				waterPipeConnection.m_FreshCapacity = m_WaterPipeConnectionData[prefabRef.m_Prefab].m_FreshCapacity;
				waterPipeConnection.m_SewageCapacity = m_WaterPipeConnectionData[prefabRef.m_Prefab].m_SewageCapacity;
				waterPipeConnection.m_StormCapacity = m_WaterPipeConnectionData[prefabRef.m_Prefab].m_StormCapacity;
				nativeArray2[m] = waterPipeConnection;
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<WaterPipeConnectionData> __Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup;

		public BufferTypeHandle<ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferTypeHandle;

		public BufferTypeHandle<ResourceAvailability> __Game_Net_ResourceAvailability_RW_BufferTypeHandle;

		public ComponentTypeHandle<WaterPipeConnection> __Game_Net_WaterPipeConnection_RW_ComponentTypeHandle;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPipeConnectionData>(true);
			__Game_Net_ServiceCoverage_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceCoverage>(false);
			__Game_Net_ResourceAvailability_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ResourceAvailability>(false);
			__Game_Net_WaterPipeConnection_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeConnection>(false);
		}
	}

	private EntityQuery m_CreatedEdgesQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Created>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceCoverage>(),
			ComponentType.ReadWrite<ResourceAvailability>(),
			ComponentType.ReadWrite<WaterPipeConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_CreatedEdgesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedEdgesQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		InitializeEdgesJob initializeEdgesJob = new InitializeEdgesJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeConnectionData = InternalCompilerInterface.GetComponentLookup<WaterPipeConnectionData>(ref __TypeHandle.__Game_Prefabs_WaterPipeConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverageType = InternalCompilerInterface.GetBufferTypeHandle<ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceAvailabilityType = InternalCompilerInterface.GetBufferTypeHandle<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterPipeConnectionType = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeConnection>(ref __TypeHandle.__Game_Net_WaterPipeConnection_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeEdgesJob>(initializeEdgesJob, m_CreatedEdgesQuery, ((SystemBase)this).Dependency);
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
	public InitializeSystem()
	{
	}
}
