using System;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PublicTransportVehicleSection : VehicleWithLineSection
{
	protected override string group => "PublicTransportVehicleSection";

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
			if (((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PublicTransport>(selectedEntity);
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PublicTransport componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PublicTransport>(selectedEntity);
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, ((ComponentSystemBase)this).EntityManager);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PublicTransportVehicleData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PublicTransportVehicleData>(selectedPrefab);
		vehicleKey = (((componentData2.m_PurposeMask & PublicTransportPurpose.PrisonerTransport) != 0) ? VehicleLocaleKey.PrisonVan : (((componentData2.m_PurposeMask & PublicTransportPurpose.Evacuation) != 0 && (componentData.m_State & PublicTransportFlags.Evacuating) != 0) ? VehicleLocaleKey.EvacuationBus : VehicleLocaleKey.PublicTransportVehicle));
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
	public PublicTransportVehicleSection()
	{
	}
}
