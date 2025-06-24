using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Events;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class Crime : ComponentBase
{
	public EventTargetType m_RandomTargetType = EventTargetType.Citizen;

	public CrimeType m_CrimeType;

	[Tooltip("The probability for citizen to do the crime first time")]
	public Bounds1 m_OccurenceProbability = new Bounds1(0f, 50f);

	[Tooltip("The probability for criminal to do the crime again")]
	public Bounds1 m_RecurrenceProbability = new Bounds1(0f, 100f);

	[Tooltip("The police may be requested after crime start min seconds, and will be surely requested after max seconds")]
	public Bounds1 m_AlarmDelay = new Bounds1(5f, 10f);

	[Tooltip("The crime can be finished earlier without caught if it's last longer than min seconds, if the crime last longer than max seconds without police arrive and secure the crime scene, the criminal will not be caught")]
	public Bounds1 m_CrimeDuration = new Bounds1(20f, 60f);

	[Tooltip("The absolute part of steal money amount from crime victim, random between min and max, combined with relative for final steal amount")]
	public Bounds1 m_CrimeIncomeAbsolute = new Bounds1(100f, 1000f);

	[Tooltip("The percentage of steal money amount from victim's money, random between min and max, combined with absolute for final steal amount")]
	public Bounds1 m_CrimeIncomeRelative = new Bounds1(0f, 0.25f);

	[Tooltip("The criminal will be put into jail for how many game days")]
	public Bounds1 m_JailTimeRange = new Bounds1(0.125f, 1f);

	[Tooltip("The criminal will be sentenced to prison for how many game days after jail time")]
	public Bounds1 m_PrisonTimeRange = new Bounds1(1f, 100f);

	[Tooltip("The probability of the criminal in jail sentence to prison")]
	public float m_PrisonProbability = 50f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CrimeData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.Crime>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		CrimeData crimeData = default(CrimeData);
		crimeData.m_RandomTargetType = m_RandomTargetType;
		crimeData.m_CrimeType = m_CrimeType;
		crimeData.m_OccurenceProbability = m_OccurenceProbability;
		crimeData.m_RecurrenceProbability = m_RecurrenceProbability;
		crimeData.m_AlarmDelay = m_AlarmDelay;
		crimeData.m_CrimeDuration = m_CrimeDuration;
		crimeData.m_CrimeIncomeAbsolute = m_CrimeIncomeAbsolute;
		crimeData.m_CrimeIncomeRelative = m_CrimeIncomeRelative;
		crimeData.m_JailTimeRange = m_JailTimeRange;
		crimeData.m_PrisonTimeRange = m_PrisonTimeRange;
		crimeData.m_PrisonProbability = m_PrisonProbability;
		((EntityManager)(ref entityManager)).SetComponentData<CrimeData>(entity, crimeData);
	}
}
