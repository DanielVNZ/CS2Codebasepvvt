using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class EducationSection : InfoSectionBase
{
	protected override string group => "EducationSection";

	private int studentCount { get; set; }

	private int studentCapacity { get; set; }

	private float graduationTime { get; set; }

	private float failProbability { get; set; }

	protected override void Reset()
	{
		studentCount = 0;
		studentCapacity = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.School>(selectedEntity);
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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetComponentWithUpgrades<SchoolData>(selectedEntity, selectedPrefab, out SchoolData data))
		{
			studentCapacity = data.m_StudentCapacity;
		}
		DynamicBuffer<Student> val = default(DynamicBuffer<Student>);
		if (EntitiesExtensions.TryGetBuffer<Student>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val))
		{
			studentCount = val.Length;
		}
		Game.Buildings.School school = default(Game.Buildings.School);
		if (EntitiesExtensions.TryGetComponent<Game.Buildings.School>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref school))
		{
			graduationTime = ((school.m_AverageGraduationTime > 0f) ? school.m_AverageGraduationTime : 0.5f);
			failProbability = school.m_AverageFailProbability;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("studentCount");
		writer.Write(studentCount);
		writer.PropertyName("studentCapacity");
		writer.Write(studentCapacity);
		writer.PropertyName("graduationTime");
		writer.Write(graduationTime);
		writer.PropertyName("failProbability");
		writer.Write(failProbability);
	}

	[Preserve]
	public EducationSection()
	{
	}
}
