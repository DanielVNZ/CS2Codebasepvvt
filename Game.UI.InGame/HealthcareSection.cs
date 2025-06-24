using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class HealthcareSection : InfoSectionBase
{
	protected override string group => "HealthcareSection";

	private int patientCount { get; set; }

	private int patientCapacity { get; set; }

	protected override void Reset()
	{
		patientCount = 0;
		patientCapacity = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Hospital>(selectedEntity) && TryGetComponentWithUpgrades<HospitalData>(selectedEntity, selectedPrefab, out HospitalData data))
		{
			patientCapacity = data.m_PatientCapacity;
		}
		base.visible = patientCapacity > 0;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Patient> val = default(DynamicBuffer<Patient>);
		if (EntitiesExtensions.TryGetBuffer<Patient>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			patientCount = val.Length;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("patientCount");
		writer.Write(patientCount);
		writer.PropertyName("patientCapacity");
		writer.Write(patientCapacity);
	}

	[Preserve]
	public HealthcareSection()
	{
	}
}
