using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class FireSection : InfoSectionBase
{
	protected override string group => "FireSection";

	private float vehicleEfficiency { get; set; }

	private bool disasterResponder { get; set; }

	protected override void Reset()
	{
		vehicleEfficiency = 0f;
		disasterResponder = false;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.FireStation>(selectedEntity);
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (TryGetComponentWithUpgrades<FireStationData>(selectedEntity, selectedPrefab, out FireStationData data) && EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref buffer))
		{
			disasterResponder = data.m_DisasterResponseCapacity > 0;
			float efficiency = BuildingUtils.GetEfficiency(buffer);
			vehicleEfficiency = data.m_VehicleEfficiency * (0.5f + efficiency * 0.5f) * 100f;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("vehicleEfficiency");
		writer.Write(vehicleEfficiency);
		writer.PropertyName("disasterResponder");
		writer.Write(disasterResponder);
	}

	[Preserve]
	public FireSection()
	{
	}
}
