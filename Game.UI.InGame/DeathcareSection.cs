using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DeathcareSection : InfoSectionBase
{
	protected override string group => "DeathcareSection";

	private int bodyCount { get; set; }

	private int bodyCapacity { get; set; }

	private float processingSpeed { get; set; }

	private float processingCapacity { get; set; }

	protected override void Reset()
	{
		bodyCount = 0;
		bodyCapacity = 0;
		processingSpeed = 0f;
		processingCapacity = 0f;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.DeathcareFacility>(selectedEntity);
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
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetComponentWithUpgrades<DeathcareFacilityData>(selectedEntity, selectedPrefab, out DeathcareFacilityData data))
		{
			bodyCapacity = data.m_StorageCapacity;
			base.tooltipKeys.Add(data.m_LongTermStorage ? "Cemetery" : "Crematorium");
		}
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref buffer))
		{
			processingSpeed = data.m_ProcessingRate * BuildingUtils.GetEfficiency(buffer);
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		bodyCount = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.DeathcareFacility>(selectedEntity).m_LongTermStoredCount;
		processingCapacity = data.m_ProcessingRate;
		DynamicBuffer<Patient> val = default(DynamicBuffer<Patient>);
		if (EntitiesExtensions.TryGetBuffer<Patient>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			bodyCount += val.Length;
		}
		if (bodyCount <= 0)
		{
			processingSpeed = 0f;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("bodyCount");
		writer.Write(bodyCount);
		writer.PropertyName("bodyCapacity");
		writer.Write(bodyCapacity);
		writer.PropertyName("processingSpeed");
		writer.Write(processingSpeed);
		writer.PropertyName("processingCapacity");
		writer.Write(processingCapacity);
	}

	[Preserve]
	public DeathcareSection()
	{
	}
}
