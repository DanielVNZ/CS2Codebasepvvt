using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class UpkeepSection : InfoSectionBase
{
	private readonly struct UIUpkeepItem : IJsonWritable, IComparable<UIUpkeepItem>
	{
		public int count { get; }

		public int amount { get; }

		public int price { get; }

		public Resource localeKey { get; }

		public string titleId { get; }

		public UIUpkeepItem(int amount, int price, Resource localeKey, string titleId)
		{
			count = 1;
			this.amount = amount;
			this.price = price;
			this.localeKey = localeKey;
			this.titleId = titleId;
		}

		private UIUpkeepItem(int count, int amount, int price, Resource localeKey, string titleId)
		{
			this.count = count;
			this.amount = amount;
			this.price = price;
			this.localeKey = localeKey;
			this.titleId = titleId;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(UIUpkeepItem).FullName);
			writer.PropertyName("count");
			writer.Write(count);
			writer.PropertyName("amount");
			writer.Write(amount);
			writer.PropertyName("price");
			writer.Write(price);
			writer.PropertyName("localeKey");
			writer.Write(Enum.GetName(typeof(Resource), localeKey));
			writer.PropertyName("titleId");
			writer.Write(titleId);
			writer.PropertyName("localeKey");
			writer.Write(Enum.GetName(typeof(Resource), localeKey));
			writer.TypeEnd();
		}

		public int CompareTo(UIUpkeepItem other)
		{
			return amount.CompareTo(other.amount);
		}

		public static UIUpkeepItem operator +(UIUpkeepItem a, UIUpkeepItem b)
		{
			return new UIUpkeepItem(a.count + 1, a.amount + b.amount, a.price + b.price, b.localeKey, b.titleId);
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<Employee> __Game_Companies_Employee_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Companies_Employee_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(false);
		}
	}

	private ResourceSystem m_ResourceSystem;

	private PrefabUISystem m_PrefabUISystem;

	private EntityQuery m_BudgetDataQuery;

	private EntityQuery m_EconomyParameterQuery;

	private TypeHandle __TypeHandle;

	protected override string group => "UpkeepSection";

	private Dictionary<string, UIUpkeepItem> moneyUpkeep { get; set; }

	private Dictionary<Resource, UIUpkeepItem> resourceUpkeep { get; set; }

	private List<UIUpkeepItem> upkeeps { get; set; }

	private int total { get; set; }

	private bool inactive { get; set; }

	protected override bool displayForUpgrades => true;

	protected override void Reset()
	{
		moneyUpkeep.Clear();
		resourceUpkeep.Clear();
		upkeeps.Clear();
		total = 0;
		inactive = false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_BudgetDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceBudgetData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		resourceUpkeep = new Dictionary<Resource, UIUpkeepItem>(5);
		moneyUpkeep = new Dictionary<string, UIUpkeepItem>(5);
		upkeeps = new List<UIUpkeepItem>(10);
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<ServiceUpkeepData>(selectedPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceObjectData>(selectedPrefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<WorkplaceData>(selectedPrefab);
			}
			return false;
		}
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	private void CalculateServiceUpkeepDatas(Entity entity, Entity prefabEntity, Entity buildingOwnerEntity, DynamicBuffer<ServiceUpkeepData> serviceUpkeepDatas, bool inactiveBuilding, bool inactiveUpgrade)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		string prefabName = m_PrefabSystem.GetPrefabName(prefabEntity);
		ServiceUsage serviceUsage = default(ServiceUsage);
		for (int i = 0; i < serviceUpkeepDatas.Length; i++)
		{
			ServiceUpkeepData serviceUpkeepData = serviceUpkeepDatas[i];
			Resource resource = serviceUpkeepData.m_Upkeep.m_Resource;
			int num = serviceUpkeepData.m_Upkeep.m_Amount;
			m_PrefabUISystem.GetTitleAndDescription(prefabEntity, out var titleId, out var descriptionId);
			if (!((EntityQuery)(ref m_BudgetDataQuery)).IsEmptyIgnoreFilter && resource == Resource.Money)
			{
				int num2 = CityServiceUpkeepSystem.CalculateUpkeep(num, selectedPrefab, ((EntityQuery)(ref m_BudgetDataQuery)).GetSingletonEntity(), ((ComponentSystemBase)this).EntityManager);
				if (inactiveBuilding || inactiveUpgrade)
				{
					num2 = (int)((float)num2 * 0.1f);
				}
				if (!moneyUpkeep.TryGetValue(prefabName, out var value))
				{
					moneyUpkeep[prefabName] = value;
				}
				Dictionary<string, UIUpkeepItem> dictionary = moneyUpkeep;
				descriptionId = prefabName;
				dictionary[descriptionId] += new UIUpkeepItem(num, num2, Resource.Money, titleId);
			}
			else
			{
				if (inactiveUpgrade)
				{
					continue;
				}
				int num3 = Mathf.RoundToInt((float)num * EconomyUtils.GetMarketPrice(resource, m_ResourceSystem.GetPrefabs(), ((ComponentSystemBase)this).EntityManager));
				if (serviceUpkeepData.m_ScaleWithUsage && EntitiesExtensions.TryGetComponent<ServiceUsage>(((ComponentSystemBase)this).EntityManager, buildingOwnerEntity, ref serviceUsage))
				{
					num = (int)((float)num * serviceUsage.m_Usage);
					num3 = (int)((float)num3 * serviceUsage.m_Usage);
				}
				if (num != 0 && num3 != 0)
				{
					if (!resourceUpkeep.TryGetValue(resource, out var value2))
					{
						resourceUpkeep[resource] = value2;
					}
					resourceUpkeep[resource] += new UIUpkeepItem(num, num3, resource, string.Empty);
					if (!base.tooltipKeys.Contains(resource.ToString()))
					{
						base.tooltipKeys.Add(resource.ToString());
					}
				}
			}
		}
		if (((EntityQuery)(ref m_EconomyParameterQuery)).IsEmptyIgnoreFilter || !(entity == buildingOwnerEntity))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<WorkplaceData>(selectedPrefab))
		{
			int upkeepOfEmployeeWage = CityServiceUpkeepSystem.GetUpkeepOfEmployeeWage(InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef), buildingOwnerEntity, ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(), inactiveBuilding);
			if (!moneyUpkeep.TryGetValue(Resource.Money.ToString(), out var value3))
			{
				moneyUpkeep[Resource.Money.ToString()] = value3;
			}
			moneyUpkeep[Resource.Money.ToString()] += new UIUpkeepItem(upkeepOfEmployeeWage, upkeepOfEmployeeWage, Resource.Money, string.Empty);
		}
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		Building building = default(Building);
		Extension extension = default(Extension);
		inactive = (EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref building) && BuildingUtils.CheckOption(building, BuildingOption.Inactive)) || (EntitiesExtensions.TryGetComponent<Extension>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref extension) && (extension.m_Flags & ExtensionFlags.Disabled) != 0);
		DynamicBuffer<ServiceUpkeepData> serviceUpkeepDatas = default(DynamicBuffer<ServiceUpkeepData>);
		if (EntitiesExtensions.TryGetBuffer<ServiceUpkeepData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, true, ref serviceUpkeepDatas))
		{
			Entity owner = selectedEntity;
			bool inactiveBuilding = inactive;
			bool inactiveUpgrade = false;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			Owner owner2 = default(Owner);
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref owner2) && EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, owner2.m_Owner, ref building))
			{
				owner = owner2.m_Owner;
				inactiveUpgrade = inactive;
				inactiveBuilding = BuildingUtils.CheckOption(building, BuildingOption.Inactive);
			}
			CalculateServiceUpkeepDatas(owner, selectedPrefab, owner, serviceUpkeepDatas, inactiveBuilding, inactiveUpgrade);
			DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
			if (EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
			{
				PrefabRef prefabRef = default(PrefabRef);
				DynamicBuffer<ServiceUpkeepData> serviceUpkeepDatas2 = default(DynamicBuffer<ServiceUpkeepData>);
				for (int i = 0; i < val.Length; i++)
				{
					InstalledUpgrade installedUpgrade = val[i];
					if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, installedUpgrade.m_Upgrade, ref prefabRef) && EntitiesExtensions.TryGetBuffer<ServiceUpkeepData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref serviceUpkeepDatas2))
					{
						CalculateServiceUpkeepDatas(installedUpgrade.m_Upgrade, prefabRef.m_Prefab, owner, serviceUpkeepDatas2, inactiveBuilding, BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive));
					}
				}
			}
		}
		foreach (KeyValuePair<string, UIUpkeepItem> item in moneyUpkeep)
		{
			upkeeps.Add(item.Value);
			total += item.Value.price;
		}
		foreach (KeyValuePair<Resource, UIUpkeepItem> item2 in resourceUpkeep)
		{
			upkeeps.Add(item2.Value);
			total += item2.Value.price;
		}
		upkeeps.Sort();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("upkeeps");
		JsonWriterExtensions.ArrayBegin(writer, upkeeps.Count);
		for (int i = 0; i < upkeeps.Count; i++)
		{
			JsonWriterExtensions.Write<UIUpkeepItem>(writer, upkeeps[i]);
		}
		writer.ArrayEnd();
		writer.PropertyName("total");
		writer.Write(total);
		writer.PropertyName("inactive");
		writer.Write(inactive);
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
	public UpkeepSection()
	{
	}
}
