using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Economy;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class GarbageSection : InfoSectionBase
{
	private enum LoadKey
	{
		IndustrialWaste,
		Garbage
	}

	protected override string group => "GarbageSection";

	private int garbage { get; set; }

	private int garbageCapacity { get; set; }

	private int processingSpeed { get; set; }

	private int processingCapacity { get; set; }

	private LoadKey loadKey { get; set; }

	protected override void Reset()
	{
		garbage = 0;
		garbageCapacity = 0;
		processingSpeed = 0;
		processingCapacity = 0;
		loadKey = LoadKey.Garbage;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.GarbageFacility>(selectedEntity);
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
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Game.Buildings.GarbageFacility garbageFacility = default(Game.Buildings.GarbageFacility);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.GarbageFacility>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref garbageFacility))
		{
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref resources))
			{
				garbage = EconomyUtils.GetResources(Resource.Garbage, resources);
			}
			if (TryGetComponentWithUpgrades<GarbageFacilityData>(selectedEntity, selectedPrefab, out GarbageFacilityData data))
			{
				garbageCapacity = data.m_GarbageCapacity;
				processingSpeed = garbageFacility.m_ProcessingRate;
				processingCapacity = data.m_ProcessingSpeed;
				if (data.m_LongTermStorage)
				{
					base.tooltipKeys.Add("Landfill");
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ResourceProducer>(selectedEntity))
				{
					base.tooltipKeys.Add("RecyclingCenter");
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<ElectricityProducer>(selectedEntity))
				{
					base.tooltipKeys.Add("Incinerator");
				}
				if (data.m_IndustrialWasteOnly)
				{
					base.tooltipKeys.Add("HazardousWaste");
					loadKey = LoadKey.IndustrialWaste;
				}
			}
		}
		DynamicBuffer<Game.Areas.SubArea> val = default(DynamicBuffer<Game.Areas.SubArea>);
		if (!EntitiesExtensions.TryGetBuffer<Game.Areas.SubArea>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			return;
		}
		Storage storage = default(Storage);
		StorageAreaData prefabStorageData = default(StorageAreaData);
		for (int i = 0; i < val.Length; i++)
		{
			Entity area = val[i].m_Area;
			if (EntitiesExtensions.TryGetComponent<Storage>(((ComponentSystemBase)this).EntityManager, area, ref storage))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(area);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Geometry componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Geometry>(area);
				if (EntitiesExtensions.TryGetComponent<StorageAreaData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref prefabStorageData))
				{
					garbageCapacity += AreaUtils.CalculateStorageCapacity(componentData2, prefabStorageData);
					garbage += storage.m_Amount;
				}
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("garbage");
		writer.Write(garbage);
		writer.PropertyName("garbageCapacity");
		writer.Write(garbageCapacity);
		writer.PropertyName("processingSpeed");
		writer.Write(processingSpeed);
		writer.PropertyName("processingCapacity");
		writer.Write(processingCapacity);
		writer.PropertyName("loadKey");
		writer.Write(Enum.GetName(typeof(LoadKey), loadKey));
	}

	[Preserve]
	public GarbageSection()
	{
	}
}
