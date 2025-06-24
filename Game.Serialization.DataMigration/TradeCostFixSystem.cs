using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Serialization.DataMigration;

[CompilerGenerated]
public class TradeCostFixSystem : GameSystemBase
{
	[BurstCompile]
	private struct TradeCostFixJob : IJobChunk
	{
		public BufferTypeHandle<TradeCost> m_TradeCostBufType;

		public BufferTypeHandle<Resources> m_ResourcesBufType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyDatas;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<TradeCost> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostBufType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				bufferAccessor[i].Clear();
			}
			if (!((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>())
			{
				return;
			}
			BufferAccessor<Resources> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesBufType);
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int j = 0; j < bufferAccessor2.Length; j++)
			{
				DynamicBuffer<Resources> resources = bufferAccessor2[j];
				PrefabRef prefabRef = nativeArray[j];
				StorageCompanyData storageCompanyData = m_StorageCompanyDatas[(Entity)prefabRef];
				StorageLimitData storageLimitData = m_StorageLimitDatas[(Entity)prefabRef];
				ResourceIterator iterator = ResourceIterator.GetIterator();
				int num = EconomyUtils.CountResources(storageCompanyData.m_StoredResources);
				while (iterator.Next())
				{
					if ((storageCompanyData.m_StoredResources & iterator.resource) != Resource.NoResource)
					{
						if (iterator.resource == Resource.OutgoingMail)
						{
							EconomyUtils.SetResources(Resource.OutgoingMail, resources, 0);
							continue;
						}
						int num2 = storageLimitData.m_Limit / num;
						EconomyUtils.SetResources(iterator.resource, resources, num2 / 2);
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
		public BufferTypeHandle<TradeCost> __Game_Companies_TradeCost_RW_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

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
			__Game_Companies_TradeCost_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TradeCost>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private TradeSystem m_TradeSystem;

	private EntityQuery m_TradeCostQuery;

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
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_TradeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TradeSystem>();
		m_TradeCostQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<TradeCost>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Deleted>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.TradeCostFix))
		{
			if (!((EntityQuery)(ref m_TradeCostQuery)).IsEmptyIgnoreFilter)
			{
				TradeCostFixJob tradeCostFixJob = new TradeCostFixJob
				{
					m_TradeCostBufType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ResourcesBufType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_StorageCompanyDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
				};
				((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<TradeCostFixJob>(tradeCostFixJob, m_TradeCostQuery, ((SystemBase)this).Dependency);
			}
			m_TradeSystem.SetDefaults();
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
	public TradeCostFixSystem()
	{
	}
}
