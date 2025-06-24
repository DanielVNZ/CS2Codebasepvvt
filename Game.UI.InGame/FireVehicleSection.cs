using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class FireVehicleSection : VehicleSection
{
	protected override string group => "FireVehicleSection";

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
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.FireEngine>(selectedEntity))
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.FireEngine componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.FireEngine>(selectedEntity);
		DynamicBuffer<ServiceDispatch> dispatches = default(DynamicBuffer<ServiceDispatch>);
		EntitiesExtensions.TryGetBuffer<ServiceDispatch>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref dispatches);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		vehicleKey = (((EntityManager)(ref entityManager)).HasComponent<HelicopterData>(selectedPrefab) ? VehicleLocaleKey.FireHelicopter : VehicleLocaleKey.FireEngine);
		base.stateKey = VehicleUIUtils.GetStateKey(selectedEntity, componentData, dispatches, ((ComponentSystemBase)this).EntityManager);
		if (base.stateKey == VehicleStateLocaleKey.Dispatched)
		{
			if (!dispatches.IsCreated || dispatches.Length == 0)
			{
				return;
			}
			ServiceDispatch serviceDispatch = dispatches[0];
			FireRescueRequest fireRescueRequest = default(FireRescueRequest);
			if (!EntitiesExtensions.TryGetComponent<FireRescueRequest>(((ComponentSystemBase)this).EntityManager, serviceDispatch.m_Request, ref fireRescueRequest))
			{
				return;
			}
			OnFire onFire = default(OnFire);
			Destroyed destroyed = default(Destroyed);
			if (EntitiesExtensions.TryGetComponent<OnFire>(((ComponentSystemBase)this).EntityManager, fireRescueRequest.m_Target, ref onFire) && onFire.m_Event != Entity.Null)
			{
				base.nextStop = new VehicleUIUtils.EntityWrapper(onFire.m_Event);
			}
			else if (EntitiesExtensions.TryGetComponent<Destroyed>(((ComponentSystemBase)this).EntityManager, fireRescueRequest.m_Target, ref destroyed) && destroyed.m_Event != Entity.Null)
			{
				base.nextStop = new VehicleUIUtils.EntityWrapper(destroyed.m_Event);
			}
			else if (fireRescueRequest.m_Target != Entity.Null)
			{
				base.nextStop = new VehicleUIUtils.EntityWrapper(fireRescueRequest.m_Target);
			}
		}
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
	public FireVehicleSection()
	{
	}
}
