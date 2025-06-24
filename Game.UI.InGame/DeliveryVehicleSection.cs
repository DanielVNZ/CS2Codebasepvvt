using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DeliveryVehicleSection : VehicleSection
{
	protected override string group => "DeliveryVehicleSection";

	private Resource resource { get; set; }

	private VehicleLocaleKey vehicleKey { get; set; }

	protected override void Reset()
	{
		base.Reset();
		resource = Resource.NoResource;
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
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<DeliveryTruck>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity);
			}
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DeliveryTruck componentData = ((EntityManager)(ref entityManager)).GetComponentData<DeliveryTruck>(selectedEntity);
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val) && val.Length != 0)
		{
			Resource resource = Resource.NoResource;
			DeliveryTruck deliveryTruck = default(DeliveryTruck);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<DeliveryTruck>(((ComponentSystemBase)this).EntityManager, val[i].m_Vehicle, ref deliveryTruck))
				{
					resource |= deliveryTruck.m_Resource;
				}
			}
			this.resource = resource;
		}
		else
		{
			this.resource = componentData.m_Resource;
		}
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, ((ComponentSystemBase)this).EntityManager);
		vehicleKey = (((this.resource & (Resource)28672uL) != Resource.NoResource) ? VehicleLocaleKey.PostTruck : VehicleLocaleKey.DeliveryTruck);
		base.tooltipKeys.Add(vehicleKey.ToString());
		base.OnProcess();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		base.OnWriteProperties(writer);
		writer.PropertyName("resourceKey");
		writer.Write(Enum.GetName(typeof(Resource), resource));
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public DeliveryVehicleSection()
	{
	}
}
