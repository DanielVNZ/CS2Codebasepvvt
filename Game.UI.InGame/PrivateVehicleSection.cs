using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PrivateVehicleSection : VehicleSection
{
	protected override string group => "PrivateVehicleSection";

	private Entity keeperEntity { get; set; }

	private VehicleLocaleKey vehicleKey { get; set; }

	protected override void Reset()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		keeperEntity = Entity.Null;
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<PersonalCar>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<Taxi>(selectedEntity);
				}
				return true;
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, ((ComponentSystemBase)this).EntityManager);
		keeperEntity = Entity.Null;
		if (base.stateKey != VehicleStateLocaleKey.Parked)
		{
			PersonalCar personalCar = default(PersonalCar);
			keeperEntity = (EntitiesExtensions.TryGetComponent<PersonalCar>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref personalCar) ? personalCar.m_Keeper : Entity.Null);
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		vehicleKey = (((EntityManager)(ref entityManager)).HasComponent<Taxi>(selectedEntity) ? VehicleLocaleKey.Taxi : VehicleLocaleKey.HouseholdVehicle);
		base.tooltipKeys.Add(vehicleKey.ToString());
		base.OnProcess();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnWriteProperties(writer);
		writer.PropertyName("keeper");
		if (keeperEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, keeperEntity);
		}
		writer.PropertyName("keeperEntity");
		if (keeperEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, keeperEntity);
		}
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public PrivateVehicleSection()
	{
	}
}
