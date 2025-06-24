using System;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class LoadSection : InfoSectionBase
{
	private enum LoadKey
	{
		Water,
		IndustrialWaste,
		Garbage,
		Mail,
		None
	}

	protected override string group => "LoadSection";

	private float load { get; set; }

	private float capacity { get; set; }

	private LoadKey loadKey { get; set; }

	protected override Entity selectedEntity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = default(Controller);
			if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, base.selectedEntity, ref controller))
			{
				return controller.m_Controller;
			}
			return base.selectedEntity;
		}
	}

	protected override Entity selectedPrefab
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = default(Controller);
			PrefabRef prefabRef = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, base.selectedEntity, ref controller) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, controller.m_Controller, ref prefabRef))
			{
				return prefabRef.m_Prefab;
			}
			return base.selectedPrefab;
		}
	}

	protected override void Reset()
	{
		loadKey = LoadKey.None;
		load = 0f;
		capacity = 0f;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		FireEngineData fireEngineData = default(FireEngineData);
		GarbageTruckData garbageTruckData = default(GarbageTruckData);
		PostVanData postVanData = default(PostVanData);
		if (EntitiesExtensions.TryGetComponent<FireEngineData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref fireEngineData))
		{
			capacity = fireEngineData.m_ExtinguishingCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<GarbageTruckData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref garbageTruckData))
		{
			capacity = garbageTruckData.m_GarbageCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PostVanData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref postVanData))
		{
			capacity = postVanData.m_MailCapacity;
		}
		base.visible = capacity > 0f;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Game.Vehicles.FireEngine fireEngine = default(Game.Vehicles.FireEngine);
		Game.Vehicles.GarbageTruck garbageTruck = default(Game.Vehicles.GarbageTruck);
		Game.Vehicles.PostVan postVan = default(Game.Vehicles.PostVan);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.FireEngine>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref fireEngine))
		{
			load = fireEngine.m_ExtinguishingAmount;
			loadKey = LoadKey.Water;
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Vehicles.GarbageTruck>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref garbageTruck))
		{
			load = garbageTruck.m_Garbage;
			loadKey = (((garbageTruck.m_State & GarbageTruckFlags.IndustrialWasteOnly) != 0) ? LoadKey.IndustrialWaste : LoadKey.Garbage);
		}
		else if (EntitiesExtensions.TryGetComponent<Game.Vehicles.PostVan>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref postVan))
		{
			load = postVan.m_DeliveringMail + postVan.m_CollectedMail;
			loadKey = LoadKey.Mail;
		}
		base.tooltipKeys.Add(loadKey.ToString());
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("load");
		writer.Write(load);
		writer.PropertyName("capacity");
		writer.Write(capacity);
		writer.PropertyName("loadKey");
		writer.Write(Enum.GetName(typeof(LoadKey), loadKey));
	}

	[Preserve]
	public LoadSection()
	{
	}
}
