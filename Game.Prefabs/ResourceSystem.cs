using System.Runtime.CompilerServices;
using Game.Common;
using Game.Economy;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ResourceSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ResourceData> __Game_Prefabs_ResourceData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResourceInfo> __Game_Economy_ResourceInfo_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_ResourceData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceData>(false);
			__Game_Economy_ResourceInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceInfo>(true);
		}
	}

	private EntityQuery m_PrefabGroup;

	private EntityQuery m_InfoGroup;

	private PrefabSystem m_PrefabSystem;

	private NativeArray<Entity> m_ResourcePrefabs;

	private NativeArray<Entity> m_ResourceInfos;

	private JobHandle m_PrefabsReaders;

	private int m_BaseConsumptionSum;

	private TypeHandle __TypeHandle;

	public int BaseConsumptionSum => m_BaseConsumptionSum;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ResourceData>()
		};
		array[0] = val;
		m_PrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_InfoGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<ResourceInfo>()
		});
		m_ResourcePrefabs = new NativeArray<Entity>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ResourceInfos = new NativeArray<Entity>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_PrefabsReaders)).Complete();
		m_ResourcePrefabs.Dispose();
		m_ResourceInfos.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_PrefabsReaders)).Complete();
		m_PrefabsReaders = default(JobHandle);
		if (!((EntityQuery)(ref m_PrefabGroup)).IsEmptyIgnoreFilter)
		{
			EntityCommandBuffer val = default(EntityCommandBuffer);
			((EntityCommandBuffer)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref m_PrefabGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ResourceData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			float num = 0f;
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				NativeArray<ResourceData> nativeArray3 = ((ArchetypeChunk)(ref val3)).GetNativeArray<ResourceData>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					Entity val4 = nativeArray[j];
					ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(nativeArray2[j]);
					ResourceData resourceData = nativeArray3[j];
					resourceData.m_IsMaterial = prefab.m_IsMaterial;
					resourceData.m_IsProduceable = prefab.m_IsProduceable;
					resourceData.m_IsTradable = prefab.m_IsTradable;
					resourceData.m_IsLeisure = prefab.m_IsLeisure;
					resourceData.m_Weight = prefab.m_Weight;
					resourceData.m_Price = prefab.m_InitialPrice;
					resourceData.m_WealthModifier = prefab.m_WealthModifier;
					resourceData.m_BaseConsumption = prefab.m_BaseConsumption;
					resourceData.m_ChildWeight = prefab.m_ChildWeight;
					resourceData.m_TeenWeight = prefab.m_TeenWeight;
					resourceData.m_AdultWeight = prefab.m_AdultWeight;
					resourceData.m_ElderlyWeight = prefab.m_ElderlyWeight;
					resourceData.m_CarConsumption = prefab.m_CarConsumption;
					resourceData.m_RequireTemperature = prefab.m_RequireTemperature;
					resourceData.m_RequiredTemperature = prefab.m_RequiredTemperature;
					resourceData.m_RequireNaturalResource = prefab.m_RequireNaturalResource;
					resourceData.m_NeededWorkPerUnit = prefab.m_NeededWorkPerUnit;
					nativeArray3[j] = resourceData;
					num += math.lerp((float)HouseholdBehaviorSystem.GetWeight(200, resourceData, 1, leisureIncluded: false), (float)HouseholdBehaviorSystem.GetWeight(200, resourceData, 0, leisureIncluded: false), 0.2f);
					int num2 = (int)(prefab.m_Resource - 1);
					if (m_ResourcePrefabs[num2] == Entity.Null)
					{
						m_ResourcePrefabs[num2] = val4;
						m_ResourceInfos[num2] = ((EntityCommandBuffer)(ref val)).CreateEntity();
						((EntityCommandBuffer)(ref val)).AddComponent<ResourceInfo>(m_ResourceInfos[num2], new ResourceInfo
						{
							m_Resource = EconomyUtils.GetResource(prefab.m_Resource)
						});
						((EntityCommandBuffer)(ref val)).AddComponent<Created>(m_ResourceInfos[num2], default(Created));
					}
				}
			}
			((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
			((EntityCommandBuffer)(ref val)).Dispose();
			m_BaseConsumptionSum = Mathf.RoundToInt(num);
			val2.Dispose();
		}
		if (((EntityQuery)(ref m_InfoGroup)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<ArchetypeChunk> val5 = ((EntityQuery)(ref m_InfoGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle2 = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ResourceInfo> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<ResourceInfo>(ref __TypeHandle.__Game_Economy_ResourceInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		for (int k = 0; k < val5.Length; k++)
		{
			ArchetypeChunk val6 = val5[k];
			NativeArray<Entity> nativeArray4 = ((ArchetypeChunk)(ref val6)).GetNativeArray(entityTypeHandle2);
			NativeArray<ResourceInfo> nativeArray5 = ((ArchetypeChunk)(ref val6)).GetNativeArray<ResourceInfo>(ref componentTypeHandle3);
			for (int l = 0; l < nativeArray4.Length; l++)
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(nativeArray5[l].m_Resource);
				if (resourceIndex >= 0 && m_ResourceInfos[resourceIndex] != nativeArray4[l])
				{
					m_ResourceInfos[resourceIndex] = nativeArray4[l];
				}
			}
		}
		val5.Dispose();
	}

	public ResourcePrefabs GetPrefabs()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new ResourcePrefabs(m_ResourcePrefabs);
	}

	public void AddPrefabsReader(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabsReaders = JobHandle.CombineDependencies(m_PrefabsReaders, handle);
	}

	public Entity GetPrefab(Resource resource)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return m_ResourcePrefabs[EconomyUtils.GetResourceIndex(resource)];
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
	public ResourceSystem()
	{
	}
}
