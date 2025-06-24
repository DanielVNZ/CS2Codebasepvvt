using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Policies;

[CompilerGenerated]
public class DefaultPoliciesSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct AddDefaultPoliciesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public BufferTypeHandle<Policy> m_PolicyType;

		[ReadOnly]
		public ComponentLookup<PolicySliderData> m_PolicySliderData;

		[ReadOnly]
		public BufferLookup<DefaultPolicyData> m_DefaultPolicyData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Policy> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				PrefabRef prefabRef = nativeArray[i];
				if (!m_DefaultPolicyData.HasBuffer(prefabRef.m_Prefab))
				{
					continue;
				}
				DynamicBuffer<DefaultPolicyData> val = m_DefaultPolicyData[prefabRef.m_Prefab];
				DynamicBuffer<Policy> val2 = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					DefaultPolicyData defaultPolicyData = val[j];
					if (m_PolicySliderData.HasComponent(defaultPolicyData.m_Policy))
					{
						PolicySliderData policySliderData = m_PolicySliderData[defaultPolicyData.m_Policy];
						val2.Add(new Policy(defaultPolicyData.m_Policy, PolicyFlags.Active, policySliderData.m_Default));
					}
					else
					{
						val2.Add(new Policy(defaultPolicyData.m_Policy, PolicyFlags.Active, 0f));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public BufferTypeHandle<Policy> __Game_Policies_Policy_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PolicySliderData> __Game_Prefabs_PolicySliderData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<DefaultPolicyData> __Game_Prefabs_DefaultPolicyData_RO_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Policies_Policy_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Policy>(false);
			__Game_Prefabs_PolicySliderData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PolicySliderData>(true);
			__Game_Prefabs_DefaultPolicyData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DefaultPolicyData>(true);
		}
	}

	private CitySystem m_CitySystem;

	private EntityQuery m_CreatedQuery;

	private EntityQuery m_CityConfigurationQuery;

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
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadWrite<Policy>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>()
		});
		m_CityConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceFeeParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedQuery);
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		AddDefaultPoliciesJob addDefaultPoliciesJob = new AddDefaultPoliciesJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PolicySliderData = InternalCompilerInterface.GetComponentLookup<PolicySliderData>(ref __TypeHandle.__Game_Prefabs_PolicySliderData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultPolicyData = InternalCompilerInterface.GetBufferLookup<DefaultPolicyData>(ref __TypeHandle.__Game_Prefabs_DefaultPolicyData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<AddDefaultPoliciesJob>(addDefaultPoliciesJob, m_CreatedQuery, ((SystemBase)this).Dependency);
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Context)(ref context)).purpose != 1 && ((int)((Context)(ref context)).purpose != 2 || !(((Context)(ref context)).version < Version.taxiFee)))
		{
			return;
		}
		Entity singletonEntity = ((EntityQuery)(ref m_CityConfigurationQuery)).GetSingletonEntity();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Policy> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(m_CitySystem.City, false);
		DynamicBuffer<DefaultPolicyData> val = default(DynamicBuffer<DefaultPolicyData>);
		if (!EntitiesExtensions.TryGetBuffer<DefaultPolicyData>(((ComponentSystemBase)this).EntityManager, singletonEntity, true, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			DefaultPolicyData defaultPolicyData = val[i];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PolicySliderData>(defaultPolicyData.m_Policy))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PolicySliderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PolicySliderData>(defaultPolicyData.m_Policy);
				buffer.Add(new Policy(defaultPolicyData.m_Policy, PolicyFlags.Active, componentData.m_Default));
			}
			else
			{
				buffer.Add(new Policy(defaultPolicyData.m_Policy, PolicyFlags.Active, 0f));
			}
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
	public DefaultPoliciesSystem()
	{
	}
}
