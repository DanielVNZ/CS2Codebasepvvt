using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class SewageSection : InfoSectionBase
{
	protected override string group => "SewageSection";

	private float capacity { get; set; }

	private float lastProcessed { get; set; }

	private float lastPurified { get; set; }

	private float purification { get; set; }

	protected override void Reset()
	{
		capacity = 0f;
		lastProcessed = 0f;
		lastPurified = 0f;
		purification = 0f;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.SewageOutlet>(selectedEntity);
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.SewageOutlet componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.SewageOutlet>(selectedEntity);
		capacity = componentData.m_Capacity;
		lastProcessed = componentData.m_LastProcessed;
		lastPurified = componentData.m_LastPurified;
		if (TryGetComponentWithUpgrades<SewageOutletData>(selectedEntity, selectedPrefab, out SewageOutletData data))
		{
			purification = data.m_Purification;
		}
		base.tooltipKeys.Add(HasWaterSource() ? "Outlet" : "Treatment");
		if (purification > 0f)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.WaterPumpingStation>(selectedEntity))
			{
				base.tooltipKeys.Add("TreatmentPurification");
			}
			else
			{
				base.tooltipKeys.Add("OutletPurification");
			}
		}
	}

	private bool HasWaterSource()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
		if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Simulation.WaterSourceData>(subObject))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("capacity");
		writer.Write(capacity);
		writer.PropertyName("lastProcessed");
		writer.Write(lastProcessed);
		writer.PropertyName("lastPurified");
		writer.Write(lastPurified);
		writer.PropertyName("purification");
		writer.Write(purification);
	}

	[Preserve]
	public SewageSection()
	{
	}
}
