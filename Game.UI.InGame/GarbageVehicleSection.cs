using System;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Common;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class GarbageVehicleSection : VehicleSection
{
	protected override string group => "GarbageVehicleSection";

	private VehicleLocaleKey vehicleKey { get; set; }

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
			if (((EntityManager)(ref entityManager)).HasComponent<GarbageTruck>(selectedEntity))
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageTruck componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbageTruck>(selectedEntity);
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, ((ComponentSystemBase)this).EntityManager);
		vehicleKey = (((componentData.m_State & GarbageTruckFlags.IndustrialWasteOnly) != 0) ? VehicleLocaleKey.IndustrialWasteTruck : VehicleLocaleKey.GarbageTruck);
		base.tooltipKeys.Add(vehicleKey.ToString());
		base.OnProcess();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		base.OnWriteProperties(writer);
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public GarbageVehicleSection()
	{
	}
}
