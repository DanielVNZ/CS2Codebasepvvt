using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ExtractorVehicleSection : VehicleSection
{
	protected override string group => "ExtractorVehicleSection";

	private VehicleLocaleKey vehicleKey { get; set; }

	protected override void Reset()
	{
		base.Reset();
		vehicleKey = VehicleLocaleKey.Vehicle;
		base.stateKey = VehicleStateLocaleKey.Working;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.WorkVehicle>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		base.OnProcess();
		Owner owner = default(Owner);
		Attachment attachment = default(Attachment);
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, base.owner.entity, ref owner) && (EntitiesExtensions.TryGetComponent<Attachment>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref attachment) || (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref owner) && EntitiesExtensions.TryGetComponent<Attachment>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref attachment))) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, attachment.m_Attached, true, ref val) && val.Length > 0)
		{
			Entity renter = val[0].m_Renter;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(renter);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			switch (((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(val2).m_Output.m_Resource)
			{
			case Resource.Grain:
			case Resource.Vegetables:
			case Resource.Livestock:
			case Resource.Cotton:
				base.stateKey = VehicleStateLocaleKey.Farming;
				vehicleKey = VehicleLocaleKey.FarmVehicle;
				break;
			case Resource.Wood:
				base.stateKey = VehicleStateLocaleKey.Harvesting;
				vehicleKey = VehicleLocaleKey.ForestryVehicle;
				break;
			case Resource.Oil:
				base.stateKey = VehicleStateLocaleKey.Drilling;
				vehicleKey = VehicleLocaleKey.DrillingVehicle;
				break;
			case Resource.Ore:
			case Resource.Coal:
			case Resource.Stone:
				base.stateKey = VehicleStateLocaleKey.Mining;
				vehicleKey = VehicleLocaleKey.MiningVehicle;
				break;
			case Resource.Fish:
				base.stateKey = VehicleStateLocaleKey.Fishing;
				vehicleKey = VehicleLocaleKey.FishingVehicle;
				break;
			default:
				base.stateKey = VehicleStateLocaleKey.Extracting;
				vehicleKey = VehicleLocaleKey.ExtractingVehicle;
				break;
			}
			base.owner = new VehicleUIUtils.EntityWrapper(attachment.m_Attached);
			return;
		}
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, base.owner.entity, ref owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref prefabRef))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceObjectData>(prefabRef.m_Prefab))
			{
				base.owner = new VehicleUIUtils.EntityWrapper(owner.m_Owner);
				return;
			}
		}
		base.owner = new VehicleUIUtils.EntityWrapper(Entity.Null);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		base.OnWriteProperties(writer);
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public ExtractorVehicleSection()
	{
	}
}
