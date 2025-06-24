using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PoliceSection : InfoSectionBase
{
	protected override string group => "PoliceSection";

	private int prisonerCount { get; set; }

	private int prisonerCapacity { get; set; }

	protected override void Reset()
	{
		prisonerCount = 0;
		prisonerCapacity = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PoliceStation>(selectedEntity);
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
		if (TryGetComponentWithUpgrades<PoliceStationData>(selectedEntity, selectedPrefab, out PoliceStationData data))
		{
			prisonerCapacity = data.m_JailCapacity;
		}
		DynamicBuffer<Occupant> val = default(DynamicBuffer<Occupant>);
		if (EntitiesExtensions.TryGetBuffer<Occupant>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			prisonerCount = val.Length;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("prisonerCount");
		writer.Write(prisonerCount);
		writer.PropertyName("prisonerCapacity");
		writer.Write(prisonerCapacity);
	}

	[Preserve]
	public PoliceSection()
	{
	}
}
