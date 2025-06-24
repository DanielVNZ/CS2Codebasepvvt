using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ShelterSection : InfoSectionBase
{
	protected override string group => "ShelterSection";

	private int sheltered { get; set; }

	private int shelterCapacity { get; set; }

	private int consumables { get; set; }

	private int consumableCapacity { get; set; }

	protected override void Reset()
	{
		sheltered = 0;
		shelterCapacity = 0;
		consumables = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.EmergencyShelter>(selectedEntity);
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetComponentWithUpgrades<EmergencyShelterData>(selectedEntity, selectedPrefab, out EmergencyShelterData data))
		{
			shelterCapacity = data.m_ShelterCapacity;
		}
		DynamicBuffer<Occupant> val = default(DynamicBuffer<Occupant>);
		if (EntitiesExtensions.TryGetBuffer<Occupant>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			sheltered = val.Length;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("sheltered");
		writer.Write(sheltered);
		writer.PropertyName("shelterCapacity");
		writer.Write(shelterCapacity);
	}

	[Preserve]
	public ShelterSection()
	{
	}
}
