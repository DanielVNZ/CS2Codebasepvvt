using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class WaterSection : InfoSectionBase
{
	protected override string group => "WaterSection";

	private float pollution { get; set; }

	private int capacity { get; set; }

	private int lastProduction { get; set; }

	protected override void Reset()
	{
		pollution = 0f;
		capacity = 0;
		lastProduction = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.WaterPumpingStation>(selectedEntity);
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
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Game.Buildings.WaterPumpingStation waterPumpingStation = default(Game.Buildings.WaterPumpingStation);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.WaterPumpingStation>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref waterPumpingStation))
		{
			pollution = waterPumpingStation.m_Pollution;
			capacity = waterPumpingStation.m_Capacity;
			lastProduction = waterPumpingStation.m_LastProduction;
		}
		if (TryGetComponentWithUpgrades<WaterPumpingStationData>(selectedEntity, selectedPrefab, out WaterPumpingStationData data) && data.m_Capacity > 0 && data.m_Types != AllowedWaterTypes.None)
		{
			base.tooltipKeys.Add("Pumping");
		}
		if ((double)pollution > 0.01)
		{
			base.tooltipKeys.Add("Pollution");
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("pollution");
		writer.Write(pollution);
		writer.PropertyName("capacity");
		writer.Write(capacity);
		writer.PropertyName("lastProduction");
		writer.Write(lastProduction);
	}

	[Preserve]
	public WaterSection()
	{
	}
}
