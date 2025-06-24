using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Events;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class WeatherPhenomenon : ComponentBase
{
	public float m_OccurrenceProbability = 10f;

	public Bounds1 m_OccurenceTemperature = new Bounds1(0f, 15f);

	public Bounds1 m_OccurenceRain = new Bounds1(0f, 1f);

	public Bounds1 m_Duration = new Bounds1(15f, 90f);

	public Bounds1 m_PhenomenonRadius = new Bounds1(500f, 1000f);

	public Bounds1 m_HotspotRadius = new Bounds1(0.8f, 0.9f);

	public Bounds1 m_LightningInterval = new Bounds1(0f, 0f);

	[Range(0f, 1f)]
	public float m_HotspotInstability = 0.1f;

	[Range(0f, 100f)]
	public float m_DamageSeverity = 10f;

	[Tooltip("How dangerous the disaster is for the cims in the city. Determines how likely cims will leave shelter while the disaster is ongoing")]
	[Range(0f, 1f)]
	public float m_DangerLevel = 1f;

	public bool m_Evacuate;

	public bool m_StayIndoors;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WeatherPhenomenonData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.WeatherPhenomenon>());
		components.Add(ComponentType.ReadWrite<HotspotFrame>());
		components.Add(ComponentType.ReadWrite<Duration>());
		components.Add(ComponentType.ReadWrite<DangerLevel>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
		components.Add(ComponentType.ReadWrite<InterpolatedTransform>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		WeatherPhenomenonData weatherPhenomenonData = default(WeatherPhenomenonData);
		weatherPhenomenonData.m_OccurenceProbability = m_OccurrenceProbability;
		weatherPhenomenonData.m_HotspotInstability = m_HotspotInstability;
		weatherPhenomenonData.m_DamageSeverity = m_DamageSeverity;
		weatherPhenomenonData.m_DangerLevel = m_DangerLevel;
		weatherPhenomenonData.m_PhenomenonRadius = m_PhenomenonRadius;
		weatherPhenomenonData.m_HotspotRadius = m_HotspotRadius;
		weatherPhenomenonData.m_LightningInterval = m_LightningInterval;
		weatherPhenomenonData.m_Duration = m_Duration;
		weatherPhenomenonData.m_OccurenceTemperature = m_OccurenceTemperature;
		weatherPhenomenonData.m_OccurenceRain = m_OccurenceRain;
		weatherPhenomenonData.m_DangerFlags = (DangerFlags)0u;
		if (m_Evacuate)
		{
			weatherPhenomenonData.m_DangerFlags = DangerFlags.Evacuate;
		}
		if (m_StayIndoors)
		{
			weatherPhenomenonData.m_DangerFlags = DangerFlags.StayIndoors;
		}
		((EntityManager)(ref entityManager)).SetComponentData<WeatherPhenomenonData>(entity, weatherPhenomenonData);
	}
}
