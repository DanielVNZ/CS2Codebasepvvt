using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PoliceVehicleSection : VehicleSection
{
	protected override string group => "PoliceVehicleSection";

	private Entity criminalEntity { get; set; }

	private VehicleLocaleKey vehicleKey { get; set; }

	protected override void Reset()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		criminalEntity = Entity.Null;
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
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PoliceCar>(selectedEntity))
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PoliceCar componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PoliceCar>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PoliceCarData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PoliceCarData>(selectedPrefab);
		DynamicBuffer<ServiceDispatch> dispatches = default(DynamicBuffer<ServiceDispatch>);
		EntitiesExtensions.TryGetBuffer<ServiceDispatch>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref dispatches);
		DynamicBuffer<Passenger> val = default(DynamicBuffer<Passenger>);
		if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			Game.Creatures.Resident resident = default(Game.Creatures.Resident);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i].m_Passenger;
				if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, val2, ref resident))
				{
					val2 = resident.m_Citizen;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(val2))
				{
					criminalEntity = val2;
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		vehicleKey = (((EntityManager)(ref entityManager)).HasComponent<HelicopterData>(selectedPrefab) ? VehicleLocaleKey.PoliceHelicopter : VehicleUIUtils.GetPoliceVehicleLocaleKey(componentData2.m_PurposeMask));
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, dispatches, ((ComponentSystemBase)this).EntityManager);
		if ((componentData.m_State & PoliceCarFlags.AccidentTarget) != 0 && (componentData.m_State & PoliceCarFlags.AtTarget) == 0)
		{
			if (componentData.m_RequestCount <= 0 || !dispatches.IsCreated || dispatches.Length <= 0)
			{
				return;
			}
			ServiceDispatch serviceDispatch = dispatches[0];
			PoliceEmergencyRequest policeEmergencyRequest = default(PoliceEmergencyRequest);
			if (!EntitiesExtensions.TryGetComponent<PoliceEmergencyRequest>(((ComponentSystemBase)this).EntityManager, serviceDispatch.m_Request, ref policeEmergencyRequest))
			{
				return;
			}
			AccidentSite accidentSite = default(AccidentSite);
			if (EntitiesExtensions.TryGetComponent<AccidentSite>(((ComponentSystemBase)this).EntityManager, policeEmergencyRequest.m_Site, ref accidentSite) && accidentSite.m_Event != Entity.Null)
			{
				base.nextStop = new VehicleUIUtils.EntityWrapper(accidentSite.m_Event);
			}
			else
			{
				base.nextStop = new VehicleUIUtils.EntityWrapper(policeEmergencyRequest.m_Target);
			}
		}
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
		writer.PropertyName("criminal");
		if (criminalEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, criminalEntity);
		}
		writer.PropertyName("criminalEntity");
		if (criminalEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, criminalEntity);
		}
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public PoliceVehicleSection()
	{
	}
}
