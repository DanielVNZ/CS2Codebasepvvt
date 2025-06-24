using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TradedResourcesSection : InfoSectionBase
{
	private ResourcePrefabs m_ResourcePrefabs;

	protected override string group => "TradedResourcesSection";

	protected override bool displayForUpgrades => true;

	private NativeList<UIResource> rawMaterials { get; set; }

	private NativeList<UIResource> processedGoods { get; set; }

	private NativeList<UIResource> mail { get; set; }

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.CargoTransportStation>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return !((EntityManager)(ref entityManager)).HasComponent<Game.Companies.StorageCompany>(selectedEntity);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		rawMaterials.Clear();
		processedGoods.Clear();
		mail.Clear();
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

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef refData = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref refData) && m_PrefabSystem.TryGetPrefab<PrefabBase>(refData, out var prefab) && prefab.TryGet<Game.Prefabs.CargoTransportStation>(out var component))
		{
			ResourceInEditor[] tradedResources = component.m_TradedResources;
			for (int i = 0; i < tradedResources.Length; i++)
			{
				UIResource.CategorizeResources(EconomyUtils.GetResource(tradedResources[i]), 0, rawMaterials, processedGoods, mail, ((ComponentSystemBase)this).EntityManager, m_ResourcePrefabs);
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
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

	[Preserve]
	public TradedResourcesSection()
	{
	}
}
