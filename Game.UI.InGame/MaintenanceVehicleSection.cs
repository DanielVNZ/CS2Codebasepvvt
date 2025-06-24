using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class MaintenanceVehicleSection : VehicleSection
{
	protected override string group => "MaintenanceVehicleSection";

	private int workShift { get; set; }

	protected override void Reset()
	{
		base.Reset();
		workShift = 0;
	}

	protected bool Visible()
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
		if (((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.MaintenanceVehicle>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<MaintenanceVehicleData>(selectedPrefab);
				}
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.MaintenanceVehicle componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.MaintenanceVehicle>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		MaintenanceVehicleData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MaintenanceVehicleData>(selectedPrefab);
		componentData2.m_MaintenanceCapacity = Mathf.CeilToInt((float)componentData2.m_MaintenanceCapacity * componentData.m_Efficiency);
		workShift = Mathf.CeilToInt((1f - math.select((float)componentData.m_Maintained / (float)componentData2.m_MaintenanceCapacity, 0f, componentData2.m_MaintenanceCapacity == 0)) * 100f);
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, ((ComponentSystemBase)this).EntityManager);
		base.OnProcess();
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		base.OnWriteProperties(writer);
		writer.PropertyName("workShift");
		writer.Write(workShift);
	}

	[Preserve]
	public MaintenanceVehicleSection()
	{
	}
}
