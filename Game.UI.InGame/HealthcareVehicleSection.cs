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
public class HealthcareVehicleSection : VehicleSection
{
	protected override string group => "HealthcareVehicleSection";

	private Entity patientEntity { get; set; }

	private VehicleLocaleKey vehicleKey { get; set; }

	protected override void Reset()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		patientEntity = Entity.Null;
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
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Ambulance>(selectedEntity))
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.Ambulance componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.Ambulance>(selectedEntity);
		patientEntity = componentData.m_TargetPatient;
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, ((ComponentSystemBase)this).EntityManager);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		vehicleKey = (((EntityManager)(ref entityManager)).HasComponent<HelicopterData>(selectedPrefab) ? VehicleLocaleKey.MedicalHelicopter : VehicleLocaleKey.Ambulance);
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
		writer.PropertyName("patient");
		if (patientEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, patientEntity);
		}
		writer.PropertyName("patientEntity");
		if (patientEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, patientEntity);
		}
		writer.PropertyName("vehicleKey");
		writer.Write(Enum.GetName(typeof(VehicleLocaleKey), vehicleKey));
	}

	[Preserve]
	public HealthcareVehicleSection()
	{
	}
}
