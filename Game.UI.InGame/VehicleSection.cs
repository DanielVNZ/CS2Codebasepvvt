using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public abstract class VehicleSection : InfoSectionBase
{
	protected VehicleStateLocaleKey stateKey { get; set; }

	protected VehicleUIUtils.EntityWrapper owner { get; set; }

	protected bool fromOutside { get; set; }

	protected VehicleUIUtils.EntityWrapper nextStop { get; set; }

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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		stateKey = VehicleStateLocaleKey.Unknown;
		owner = new VehicleUIUtils.EntityWrapper(Entity.Null);
		fromOutside = false;
		nextStop = new VehicleUIUtils.EntityWrapper(Entity.Null);
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity val = ((EntityManager)(ref entityManager)).GetComponentData<Owner>(selectedEntity).m_Owner;
		owner = new VehicleUIUtils.EntityWrapper(val);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		fromOutside = ((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(val);
		VehicleStateLocaleKey vehicleStateLocaleKey = stateKey;
		if (vehicleStateLocaleKey != VehicleStateLocaleKey.Returning && vehicleStateLocaleKey != VehicleStateLocaleKey.Patrolling && vehicleStateLocaleKey != VehicleStateLocaleKey.Collecting && vehicleStateLocaleKey != VehicleStateLocaleKey.Working)
		{
			nextStop = new VehicleUIUtils.EntityWrapper(VehicleUIUtils.GetDestination(((ComponentSystemBase)this).EntityManager, selectedEntity));
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("stateKey");
		writer.Write(Enum.GetName(typeof(VehicleStateLocaleKey), stateKey));
		writer.PropertyName("owner");
		owner.Write(writer, m_NameSystem);
		writer.PropertyName("fromOutside");
		writer.Write(fromOutside);
		writer.PropertyName("nextStop");
		nextStop.Write(writer, m_NameSystem);
	}

	[Preserve]
	protected VehicleSection()
	{
	}
}
