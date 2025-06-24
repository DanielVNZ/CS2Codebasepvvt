using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Creatures;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class DummyHumanSection : InfoSectionBase
{
	protected override string group => "DummyHumanSection";

	private Entity originEntity { get; set; }

	private Entity destinationEntity { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		originEntity = Entity.Null;
		destinationEntity = Entity.Null;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Resident resident = default(Resident);
		if (EntitiesExtensions.TryGetComponent<Resident>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref resident))
		{
			return resident.m_Citizen == Entity.Null;
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		CurrentVehicle currentVehicle = default(CurrentVehicle);
		if (EntitiesExtensions.TryGetComponent<CurrentVehicle>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref currentVehicle))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			originEntity = ((EntityManager)(ref entityManager)).GetComponentData<Owner>(currentVehicle.m_Vehicle).m_Owner;
			destinationEntity = VehicleUIUtils.GetDestination(((ComponentSystemBase)this).EntityManager, currentVehicle.m_Vehicle);
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("origin");
		if (originEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, originEntity);
		}
		writer.PropertyName("originEntity");
		if (originEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, originEntity);
		}
		writer.PropertyName("destination");
		if (destinationEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, destinationEntity);
		}
		writer.PropertyName("destinationEntity");
		if (destinationEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, destinationEntity);
		}
	}

	[Preserve]
	public DummyHumanSection()
	{
	}
}
