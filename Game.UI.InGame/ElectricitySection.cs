using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ElectricitySection : InfoSectionBase
{
	protected override string group => "ElectricitySection";

	private int capacity { get; set; }

	private int production { get; set; }

	protected override void Reset()
	{
		capacity = 0;
		production = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<ElectricityProducer>(selectedEntity);
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
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ElectricityProducer componentData = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityProducer>(selectedEntity);
		capacity = componentData.m_Capacity;
		production = componentData.m_LastProduction;
		if (TryGetComponentWithUpgrades<SolarPoweredData>(selectedEntity, selectedPrefab, out SolarPoweredData _))
		{
			base.tooltipKeys.Add("Solar");
		}
		if (TryGetComponentWithUpgrades<WindPoweredData>(selectedEntity, selectedPrefab, out WindPoweredData _))
		{
			base.tooltipKeys.Add("Wind");
		}
		if (TryGetComponentWithUpgrades<GarbagePoweredData>(selectedEntity, selectedPrefab, out GarbagePoweredData _))
		{
			base.tooltipKeys.Add("Garbage");
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.WaterPowered>(selectedEntity))
		{
			base.tooltipKeys.Add("Water");
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("capacity");
		writer.Write(capacity);
		writer.PropertyName("production");
		writer.Write(production);
	}

	[Preserve]
	public ElectricitySection()
	{
	}
}
