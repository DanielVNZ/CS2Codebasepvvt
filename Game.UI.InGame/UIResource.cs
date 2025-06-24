using System;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;

namespace Game.UI.InGame;

public readonly struct UIResource : IJsonWritable, IComparable<UIResource>, IEquatable<UIResource>
{
	public enum ResourceStatus
	{
		None,
		Deficit,
		Normal,
		Surplus
	}

	public enum StorageType
	{
		None,
		Company,
		Warehouse,
		Cargo
	}

	public Resource key { get; }

	public int amount { get; }

	public ResourceStatus status { get; }

	public bool isRawMaterial { get; }

	public UIResource(Resource resource, int amount, EntityManager entityManager, ResourcePrefabs prefabs)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		key = resource;
		this.amount = amount;
		status = ResourceStatus.None;
		ResourceData resourceData = default(ResourceData);
		if (EntitiesExtensions.TryGetComponent<ResourceData>(entityManager, prefabs[resource], ref resourceData))
		{
			isRawMaterial = resourceData.m_IsMaterial;
		}
		else
		{
			isRawMaterial = false;
		}
	}

	public UIResource(Resources resource, EntityManager entityManager, ResourcePrefabs prefabs)
		: this(resource.m_Resource, resource.m_Amount, entityManager, prefabs)
	{
	}//IL_000d: Unknown result type (might be due to invalid IL or missing references)


	public UIResource(Resource resource, int amount, StorageType storageType, EntityManager entityManager, ResourcePrefabs prefabs)
		: this(resource, amount, entityManager, prefabs)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		switch (storageType)
		{
		case StorageType.Cargo:
			status = ((amount > StorageCompanySystem.kStationExportStartAmount) ? ResourceStatus.Surplus : ((amount < StorageCompanySystem.kStationLowStockAmount) ? ResourceStatus.Deficit : ResourceStatus.Normal));
			break;
		case StorageType.Warehouse:
			status = ((amount > StorageCompanySystem.kStorageExportStartAmount) ? ResourceStatus.Surplus : ((amount < StorageCompanySystem.kStorageLowStockAmount) ? ResourceStatus.Deficit : ResourceStatus.Normal));
			break;
		}
		if ((resource & (Resource)28672uL) != Resource.NoResource)
		{
			status = ResourceStatus.Normal;
		}
	}

	public UIResource(Resources resource, StorageType storageType, EntityManager entityManager, ResourcePrefabs prefabs)
		: this(resource.m_Resource, resource.m_Amount, storageType, entityManager, prefabs)
	{
	}//IL_000e: Unknown result type (might be due to invalid IL or missing references)


	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("key");
		writer.Write(Enum.GetName(typeof(Resource), key));
		writer.PropertyName("amount");
		writer.Write(amount);
		writer.PropertyName("status");
		writer.Write(Enum.GetName(typeof(ResourceStatus), status));
		writer.TypeEnd();
	}

	public int CompareTo(UIResource other)
	{
		if ((other.key & (Resource)28672uL) != Resource.NoResource && (key & (Resource)28672uL) == Resource.NoResource)
		{
			return -1;
		}
		if ((key & (Resource)28672uL) != Resource.NoResource && (other.key & (Resource)28672uL) == Resource.NoResource)
		{
			return 1;
		}
		int num = other.isRawMaterial.CompareTo(isRawMaterial);
		if (num != 0)
		{
			return num;
		}
		return other.amount.CompareTo(amount);
	}

	public bool Equals(UIResource other)
	{
		return key == other.key;
	}

	public override bool Equals(object obj)
	{
		if (obj is UIResource other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return key.GetHashCode();
	}

	public static void CategorizeResources(Resource resource, int amount, NativeList<UIResource> rawMaterials, NativeList<UIResource> processedGoods, NativeList<UIResource> mail, EntityManager entityManager, ResourcePrefabs resourcePrefabs, StorageType storageType = StorageType.None)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ResourceData resourceData = default(ResourceData);
		if ((resource & (Resource)28672uL) != Resource.NoResource)
		{
			UIResource uIResource = new UIResource(resource, amount, storageType, entityManager, resourcePrefabs);
			mail.Add(ref uIResource);
		}
		else if (EntitiesExtensions.TryGetComponent<ResourceData>(entityManager, resourcePrefabs[resource], ref resourceData) && resourceData.m_IsMaterial)
		{
			UIResource uIResource = new UIResource(resource, amount, storageType, entityManager, resourcePrefabs);
			rawMaterials.Add(ref uIResource);
		}
		else
		{
			UIResource uIResource = new UIResource(resource, amount, storageType, entityManager, resourcePrefabs);
			processedGoods.Add(ref uIResource);
		}
	}
}
