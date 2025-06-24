using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Achievements;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Prefabs;
using Game.Routes;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PoliciesUISystem : UISystemBase
{
	internal static class BindingNames
	{
		internal const string kCityPolicies = "cityPolicies";

		internal const string kSetPolicy = "setPolicy";

		internal const string kSetCityPolicy = "setCityPolicy";
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PolicySliderData> __Game_Prefabs_PolicySliderData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PolicySliderData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PolicySliderData>(true);
		}
	}

	public const string kGroup = "policies";

	public Action EventPolicyUnlocked;

	private CitySystem m_CitySystem;

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private SelectedInfoUISystem m_InfoSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ImageSystem m_ImageSystem;

	private EntityQuery m_CityPoliciesQuery;

	private EntityQuery m_CityPoliciesUpdatedQuery;

	private EntityQuery m_DistrictPoliciesQuery;

	private EntityQuery m_BuildingPoliciesQuery;

	private EntityQuery m_RoutePoliciesQuery;

	private EntityQuery m_PolicyUnlockedQuery;

	private EntityArchetype m_PolicyEventArchetype;

	private List<UIPolicy> m_CityPolicies;

	private List<UIPolicy> m_SelectedInfoPolicies;

	private GetterValueBinding<List<UIPolicy>> m_CityPoliciesBinding;

	private TypeHandle __TypeHandle;

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_CityPolicies.Clear();
		m_SelectedInfoPolicies.Clear();
		m_CityPoliciesBinding.Update();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_InfoSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PolicyData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CityOptionData>(),
			ComponentType.ReadOnly<CityModifierData>()
		};
		array[0] = val;
		m_CityPoliciesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.City.City>(),
			ComponentType.ReadOnly<Policy>(),
			ComponentType.ReadOnly<Updated>()
		};
		array2[0] = val;
		m_CityPoliciesUpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PolicyData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<DistrictOptionData>(),
			ComponentType.ReadOnly<DistrictModifierData>()
		};
		array3[0] = val;
		m_DistrictPoliciesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PolicyData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BuildingOptionData>(),
			ComponentType.ReadOnly<BuildingModifierData>()
		};
		array4[0] = val;
		m_BuildingPoliciesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		EntityQueryDesc[] array5 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PolicyData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RouteOptionData>(),
			ComponentType.ReadOnly<RouteModifierData>()
		};
		array5[0] = val;
		m_RoutePoliciesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		m_PolicyUnlockedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PolicyEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Modify>()
		});
		m_CityPolicies = new List<UIPolicy>();
		m_SelectedInfoPolicies = new List<UIPolicy>();
		AddBinding((IBinding)(object)(m_CityPoliciesBinding = new GetterValueBinding<List<UIPolicy>>("policies", "cityPolicies", (Func<List<UIPolicy>>)BindCityPolicies, (IWriter<List<UIPolicy>>)(object)new DelegateWriter<List<UIPolicy>>((WriterDelegate<List<UIPolicy>>)WriteCityPolicies), (EqualityComparer<List<UIPolicy>>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool, float>("policies", "setPolicy", (Action<Entity, bool, float>)SetSelectedInfoPolicy, (IReader<Entity>)null, (IReader<bool>)null, (IReader<float>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool, float>("policies", "setCityPolicy", (Action<Entity, bool, float>)SetCityPolicy, (IReader<Entity>)null, (IReader<bool>)null, (IReader<float>)null));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		EventPolicyUnlocked = null;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (PrefabUtils.HasUnlockedPrefab<PolicyData>(((ComponentSystemBase)this).EntityManager, m_PolicyUnlockedQuery))
		{
			EventPolicyUnlocked?.Invoke();
			m_CityPoliciesBinding.Update();
		}
		if (!((EntityQuery)(ref m_CityPoliciesUpdatedQuery)).IsEmptyIgnoreFilter)
		{
			m_CityPoliciesBinding.Update();
		}
	}

	public void SetPolicy(Entity target, Entity policy, bool active, float adjustment = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ModifyPolicy(target, policy, active, adjustment);
	}

	public void SetCityPolicy(Entity policy, bool active, float adjustment)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ModifyPolicy(m_CitySystem.City, policy, active, adjustment);
		RefreshCityPolicyAchievement(policy, active);
	}

	private void RefreshCityPolicyAchievement(Entity policy, bool active)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Policy> val = default(DynamicBuffer<Policy>);
		if (!EntitiesExtensions.TryGetBuffer<Policy>(World.DefaultGameObjectInjectionWorld.EntityManager, m_CitySystem.City, true, ref val))
		{
			return;
		}
		bool flag = false;
		int num = 0;
		for (int i = 0; i < val.Length; i++)
		{
			if ((val[i].m_Flags & PolicyFlags.Active) != 0)
			{
				num++;
				if (val[i].m_Policy == policy)
				{
					flag = true;
				}
			}
		}
		if (active && !flag)
		{
			num++;
		}
		if (!active && flag)
		{
			num--;
		}
		PlatformManager.instance.IndicateAchievementProgress(Game.Achievements.Achievements.CallingtheShots, num, (IndicateType)1);
	}

	public void SetSelectedInfoPolicy(Entity policy, bool active, float adjustment = 0f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ModifyPolicy(m_InfoSystem.selectedEntity, policy, active, adjustment);
	}

	public void SetSelectedInfoPolicy(Entity target, Entity policy, bool active, float adjustment = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ModifyPolicy(target, policy, active, adjustment);
	}

	private void ModifyPolicy(Entity target, Entity policy, bool active, float adjustment)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		Entity val2 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_PolicyEventArchetype);
		((EntityCommandBuffer)(ref val)).SetComponent<Modify>(val2, new Modify(target, policy, active, adjustment));
	}

	private void FindAndSortPolicies(Entity entity, EntityQuery policyQuery, List<UIPolicy> list)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Policy> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(entity, true);
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PolicySliderData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PolicySliderData>(ref __TypeHandle.__Game_Prefabs_PolicySliderData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref policyQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			val2 = val[i];
			bool flag = ((ArchetypeChunk)(ref val2)).Has<PolicySliderData>(ref componentTypeHandle);
			val2 = val[i];
			NativeArray<PolicySliderData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PolicySliderData>(ref componentTypeHandle);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				PolicyPrefab prefab = m_PrefabSystem.GetPrefab<PolicyPrefab>(nativeArray[j]);
				int priority = 0;
				if (EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, nativeArray[j], ref uIObjectData))
				{
					priority = uIObjectData.m_Priority;
				}
				if (FilterPolicy(nativeArray[j], prefab, entity))
				{
					UIPolicy item = ExtractInfo(nativeArray[j], prefab, buffer, flag, flag ? nativeArray2[j] : default(PolicySliderData), priority);
					list.Add(item);
				}
			}
		}
		list.Sort();
		val.Dispose();
	}

	private bool FilterPolicy(Entity policy, PolicyPrefab prefab, Entity target)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (prefab.m_Visibility == PolicyVisibility.HideFromPolicyList)
		{
			return false;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<District>(target))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.City.City>(target))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Route>(target))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Building>(target))
					{
						return false;
					}
					BuildingOptionData optionData = default(BuildingOptionData);
					if (!EntitiesExtensions.TryGetComponent<BuildingOptionData>(((ComponentSystemBase)this).EntityManager, policy, ref optionData))
					{
						return false;
					}
					PrefabRef prefabRef = default(PrefabRef);
					BuildingData buildingData = default(BuildingData);
					if (BuildingUtils.HasOption(optionData, BuildingOption.PaidParking) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, target, ref prefabRef) && EntitiesExtensions.TryGetComponent<BuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref buildingData) && (buildingData.m_Flags & (Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar)) == 0 && HasParkingLanes(target))
					{
						return true;
					}
					PrefabRef prefabRef2 = default(PrefabRef);
					GarbageFacilityData garbageFacilityData = default(GarbageFacilityData);
					if (BuildingUtils.HasOption(optionData, BuildingOption.Empty) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, target, ref prefabRef2) && EntitiesExtensions.TryGetComponent<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref garbageFacilityData) && garbageFacilityData.m_LongTermStorage)
					{
						return true;
					}
					if (!BuildingUtils.HasOption(optionData, BuildingOption.Inactive))
					{
						return false;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<CityServiceUpkeep>(target))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						return ((EntityManager)(ref entityManager)).HasComponent<Efficiency>(target);
					}
					return false;
				}
			}
		}
		return true;
	}

	private UIPolicy ExtractInfo(Entity entity, PolicyPrefab prefab, DynamicBuffer<Policy> activePolicies, bool slider, PolicySliderData sliderData, int priority)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		string name = ((Object)prefab).name;
		string icon = ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon;
		bool active = false;
		bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity);
		float value = sliderData.m_Default;
		for (int i = 0; i < activePolicies.Length; i++)
		{
			if (activePolicies[i].m_Policy == entity)
			{
				active = (activePolicies[i].m_Flags & PolicyFlags.Active) != 0;
				value = activePolicies[i].m_Adjustment;
				break;
			}
		}
		string empty = default(string);
		if (!GameManager.instance.localizationManager.activeDictionary.TryGetValue($"Policy.TITLE[{name}]", ref empty))
		{
			empty = string.Empty;
		}
		int milestone = (flag ? ProgressionUtils.GetRequiredMilestone(((ComponentSystemBase)this).EntityManager, entity) : 0);
		return new UIPolicy(data: new UIPolicySlider(value, sliderData), id: name, localizedName: empty, priority: priority, icon: icon, entity: entity, active: active, locked: flag, uiTag: prefab.uiTag, milestone: milestone, slider: slider);
	}

	private bool HasParkingLanes(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, building, true, ref subLanes) && HasParkingLanes(subLanes))
		{
			return true;
		}
		DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubNet>(((ComponentSystemBase)this).EntityManager, building, true, ref subNets) && HasParkingLanes(subNets))
		{
			return true;
		}
		DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
		if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, building, true, ref subObjects) && HasParkingLanes(subObjects))
		{
			return true;
		}
		return false;
	}

	private bool HasParkingLanes(DynamicBuffer<Game.Objects.SubObject> subObjects)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
		for (int i = 0; i < subObjects.Length; i++)
		{
			Entity subObject = subObjects[i].m_SubObject;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subLanes) && HasParkingLanes(subLanes))
			{
				return true;
			}
			if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subObjects2) && HasParkingLanes(subObjects2))
			{
				return true;
			}
		}
		return false;
	}

	private bool HasParkingLanes(DynamicBuffer<Game.Net.SubNet> subNets)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		for (int i = 0; i < subNets.Length; i++)
		{
			Entity subNet = subNets[i].m_SubNet;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subNet, true, ref subLanes) && HasParkingLanes(subLanes))
			{
				return true;
			}
		}
		return false;
	}

	private bool HasParkingLanes(DynamicBuffer<Game.Net.SubLane> subLanes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		for (int i = 0; i < subLanes.Length; i++)
		{
			Entity subLane = subLanes[i].m_SubLane;
			if (EntitiesExtensions.TryGetComponent<Game.Net.ParkingLane>(((ComponentSystemBase)this).EntityManager, subLane, ref parkingLane))
			{
				if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) == 0)
				{
					return true;
				}
			}
			else if (EntitiesExtensions.TryGetComponent<Game.Net.ConnectionLane>(((ComponentSystemBase)this).EntityManager, subLane, ref connectionLane) && (connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
			{
				return true;
			}
		}
		return false;
	}

	private List<UIPolicy> BindCityPolicies()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_CityPolicies.Clear();
		FindAndSortPolicies(m_CitySystem.City, m_CityPoliciesQuery, m_CityPolicies);
		return m_CityPolicies;
	}

	private void WriteCityPolicies(IJsonWriter writer, List<UIPolicy> policies)
	{
		JsonWriterExtensions.ArrayBegin(writer, policies.Count);
		foreach (UIPolicy policy in policies)
		{
			policy.Write(m_PrefabUISystem, writer);
		}
		writer.ArrayEnd();
	}

	public bool GatherSelectedInfoPolicies(Entity target)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		m_SelectedInfoPolicies.Clear();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(target))
		{
			FindAndSortPolicies(target, m_BuildingPoliciesQuery, m_SelectedInfoPolicies);
		}
		else
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<District>(target))
			{
				FindAndSortPolicies(target, m_DistrictPoliciesQuery, m_SelectedInfoPolicies);
			}
			else
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Route>(target))
				{
					FindAndSortPolicies(target, m_RoutePoliciesQuery, m_SelectedInfoPolicies);
				}
			}
		}
		return m_SelectedInfoPolicies.Count > 0;
	}

	public void BindDistrictPolicies(IJsonWriter binder)
	{
		BindPolicies(binder, m_SelectedInfoPolicies);
	}

	public void BindBuildingPolicies(IJsonWriter binder)
	{
		BindPolicies(binder, m_SelectedInfoPolicies);
	}

	public void BindRoutePolicies(IJsonWriter binder)
	{
		BindPolicies(binder, m_SelectedInfoPolicies);
	}

	private void BindPolicies(IJsonWriter binder, List<UIPolicy> list)
	{
		JsonWriterExtensions.ArrayBegin(binder, list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Write(m_PrefabUISystem, binder);
		}
		binder.ArrayEnd();
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
	public PoliciesUISystem()
	{
	}
}
