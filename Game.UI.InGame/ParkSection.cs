using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ParkSection : InfoSectionBase
{
	protected override string group => "ParkSection";

	private int maintenance { get; set; }

	protected override void Reset()
	{
		maintenance = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(selectedEntity);
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetComponentWithUpgrades<ParkData>(selectedEntity, selectedPrefab, out ParkData data))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			maintenance = Mathf.CeilToInt(math.select((float)((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.Park>(selectedEntity).m_Maintenance / (float)data.m_MaintenancePool, 0f, data.m_MaintenancePool == 0) * 100f);
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("maintenance");
		writer.Write(maintenance);
	}

	[Preserve]
	public ParkSection()
	{
	}
}
