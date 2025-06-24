using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class StorageSection : InfoSectionBase
{
	private enum StorageStatus
	{
		None,
		Empty,
		NearlyEmpty,
		Balanced,
		NearlyFull,
		Full
	}

	private Entity m_CompanyEntity;

	private ResourcePrefabs m_ResourcePrefabs;

	protected override string group => "StorageSection";

	private int stored { get; set; }

	private int capacity { get; set; }

	private StorageStatus status { get; set; }

	private UIResource.StorageType storageType { get; set; }

	private NativeList<UIResource> rawMaterials { get; set; }

	private NativeList<UIResource> processedGoods { get; set; }

	private NativeList<UIResource> mail { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		rawMaterials.Clear();
		processedGoods.Clear();
		mail.Clear();
		stored = 0;
		capacity = 0;
		m_CompanyEntity = Entity.Null;
		status = StorageStatus.None;
		storageType = UIResource.StorageType.Company;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		rawMaterials = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		processedGoods = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		mail = new NativeList<UIResource>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ResourcePrefabs = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		rawMaterials.Dispose();
		processedGoods.Dispose();
		mail.Dispose();
		base.OnDestroy();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		StorageLimitData data = default(StorageLimitData);
		if (((EntityManager)(ref entityManager)).HasComponent<CargoTransportStationData>(selectedPrefab) && TryGetComponentWithUpgrades<StorageLimitData>(selectedEntity, selectedPrefab, out data))
		{
			capacity = data.m_Limit;
			storageType = UIResource.StorageType.Cargo;
		}
		Entity val = Entity.Null;
		PrefabRef prefabRef = default(PrefabRef);
		if (CompanyUIUtils.HasCompany(((ComponentSystemBase)this).EntityManager, selectedEntity, selectedPrefab, out m_CompanyEntity) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, m_CompanyEntity, ref prefabRef) && EntitiesExtensions.TryGetComponent<StorageLimitData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref data))
		{
			val = prefabRef.m_Prefab;
			bool flag = false;
			IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
			if (EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref industrialProcessData))
			{
				bool num = EconomyUtils.GetWeight(((ComponentSystemBase)this).EntityManager, industrialProcessData.m_Input1.m_Resource, m_ResourcePrefabs) > 0f;
				bool flag2 = EconomyUtils.GetWeight(((ComponentSystemBase)this).EntityManager, industrialProcessData.m_Input2.m_Resource, m_ResourcePrefabs) > 0f;
				bool flag3 = EconomyUtils.GetWeight(((ComponentSystemBase)this).EntityManager, industrialProcessData.m_Output.m_Resource, m_ResourcePrefabs) > 0f;
				flag = num || flag2 || flag3;
			}
			PropertyRenter propertyRenter = default(PropertyRenter);
			PrefabRef prefabRef2 = default(PrefabRef);
			BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
			SpawnableBuildingData spawnable = default(SpawnableBuildingData);
			BuildingData building = default(BuildingData);
			if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef2) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref buildingPropertyData) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref spawnable) && EntitiesExtensions.TryGetComponent<BuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef2.m_Prefab, ref building))
			{
				bool flag4 = buildingPropertyData.m_AllowedStored != Resource.NoResource;
				if (flag4)
				{
					storageType = UIResource.StorageType.Warehouse;
				}
				capacity = (flag ? (flag4 ? data.GetAdjustedLimitForWarehouse(spawnable, building) : data.m_Limit) : 0);
			}
			else if (EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref buildingPropertyData) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref spawnable) && EntitiesExtensions.TryGetComponent<BuildingData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref building))
			{
				bool flag5 = buildingPropertyData.m_AllowedStored != Resource.NoResource;
				if (flag5)
				{
					storageType = UIResource.StorageType.Warehouse;
				}
				capacity = (flag ? (flag5 ? data.GetAdjustedLimitForWarehouse(spawnable, building) : data.m_Limit) : 0);
			}
			else
			{
				capacity = data.m_Limit;
			}
		}
		Entity val2 = Entity.Null;
		Attached attached = default(Attached);
		if (EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref attached))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasBuffer<InstalledUpgrade>(attached.m_Parent))
			{
				val2 = attached.m_Parent;
				goto IL_02f4;
			}
		}
		PropertyRenter propertyRenter2 = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref propertyRenter2) && EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, propertyRenter2.m_Property, ref attached))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasBuffer<InstalledUpgrade>(attached.m_Parent))
			{
				val2 = attached.m_Parent;
			}
		}
		goto IL_02f4;
		IL_02f4:
		if (val2 != Entity.Null && val == Entity.Null)
		{
			val = selectedPrefab;
		}
		if (val2 != Entity.Null && TryGetComponentWithUpgrades<StorageLimitData>(val2, val, out StorageLimitData data2))
		{
			capacity = data2.m_Limit;
		}
		return capacity > 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Resources> val = default(DynamicBuffer<Resources>);
		if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val) || EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, m_CompanyEntity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Resources resources = val[i];
				if (EconomyUtils.GetWeight(((ComponentSystemBase)this).EntityManager, resources.m_Resource, m_ResourcePrefabs) != 0f)
				{
					UIResource.CategorizeResources(resources.m_Resource, resources.m_Amount, rawMaterials, processedGoods, mail, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs, storageType);
					stored += val[i].m_Amount;
				}
			}
		}
		stored = math.min(math.max(stored, 0), capacity);
		status = ((stored == capacity) ? StorageStatus.Full : (((double)stored >= (double)capacity * 0.8) ? StorageStatus.NearlyFull : (((double)stored >= (double)capacity * 0.2) ? StorageStatus.Balanced : ((stored <= 0) ? StorageStatus.Empty : StorageStatus.NearlyEmpty))));
		if (TryGetComponentWithUpgrades<StorageCompanyData>(selectedEntity, selectedPrefab, out StorageCompanyData data))
		{
			foreach (Resource value in Enum.GetValues(typeof(Resource)))
			{
				UIResource uIResource = new UIResource(value, 0, storageType, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs);
				if ((data.m_StoredResources & value) != Resource.NoResource && EconomyUtils.GetWeight(((ComponentSystemBase)this).EntityManager, value, m_ResourcePrefabs) != 0f && !NativeListExtensions.Contains<UIResource, UIResource>(rawMaterials, uIResource) && !NativeListExtensions.Contains<UIResource, UIResource>(processedGoods, uIResource) && !NativeListExtensions.Contains<UIResource, UIResource>(mail, uIResource))
				{
					UIResource.CategorizeResources(value, 0, rawMaterials, processedGoods, mail, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs, storageType);
				}
			}
		}
		if (storageType != UIResource.StorageType.None)
		{
			base.tooltipKeys.Add(storageType.ToString());
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("stored");
		writer.Write(stored);
		writer.PropertyName("capacity");
		writer.Write(capacity);
		writer.PropertyName("status");
		writer.Write(Enum.GetName(typeof(StorageStatus), status));
		writer.PropertyName("storageType");
		writer.Write(Enum.GetName(typeof(UIResource.StorageType), storageType));
		NativeSortExtension.Sort<UIResource>(rawMaterials);
		writer.PropertyName("rawMaterials");
		JsonWriterExtensions.ArrayBegin(writer, rawMaterials.Length);
		for (int i = 0; i < rawMaterials.Length; i++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, rawMaterials[i]);
		}
		writer.ArrayEnd();
		NativeSortExtension.Sort<UIResource>(processedGoods);
		writer.PropertyName("processedGoods");
		JsonWriterExtensions.ArrayBegin(writer, processedGoods.Length);
		for (int j = 0; j < processedGoods.Length; j++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, processedGoods[j]);
		}
		writer.ArrayEnd();
		NativeSortExtension.Sort<UIResource>(mail);
		writer.PropertyName("mail");
		JsonWriterExtensions.ArrayBegin(writer, mail.Length);
		for (int k = 0; k < mail.Length; k++)
		{
			JsonWriterExtensions.Write<UIResource>(writer, mail[k]);
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public StorageSection()
	{
	}
}
