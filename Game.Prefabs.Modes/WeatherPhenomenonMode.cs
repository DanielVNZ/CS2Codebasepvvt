using System;
using Colossal.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class WeatherPhenomenonMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public EventPrefab m_Prefab;

		public float m_OccurrenceProbability;

		public Bounds1 m_OccurenceTemperature;

		public Bounds1 m_OccurenceRain;

		public Bounds1 m_Duration;

		public Bounds1 m_PhenomenonRadius;

		public Bounds1 m_HotspotRadius;

		public float m_HotspotInstability;

		public float m_DamageSeverity;

		public float m_DangerLevel;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			WeatherPhenomenon component = m_ModeDatas[i].m_Prefab.GetComponent<WeatherPhenomenon>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<WeatherPhenomenonData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			WeatherPhenomenon component = modeData.m_Prefab.GetComponent<WeatherPhenomenon>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			WeatherPhenomenonData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherPhenomenonData>(entity);
			componentData.m_OccurenceProbability = modeData.m_OccurrenceProbability;
			componentData.m_OccurenceTemperature = modeData.m_OccurenceTemperature;
			componentData.m_OccurenceRain = modeData.m_OccurenceRain;
			componentData.m_Duration = modeData.m_Duration;
			componentData.m_PhenomenonRadius = modeData.m_PhenomenonRadius;
			componentData.m_HotspotRadius = modeData.m_HotspotRadius;
			componentData.m_HotspotInstability = modeData.m_HotspotInstability;
			componentData.m_DamageSeverity = modeData.m_DamageSeverity;
			componentData.m_DangerLevel = modeData.m_DangerLevel;
			((EntityManager)(ref entityManager)).SetComponentData<WeatherPhenomenonData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			WeatherPhenomenon component = m_ModeDatas[i].m_Prefab.GetComponent<WeatherPhenomenon>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			WeatherPhenomenonData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WeatherPhenomenonData>(entity);
			componentData.m_OccurenceProbability = component.m_OccurrenceProbability;
			componentData.m_OccurenceTemperature = component.m_OccurenceTemperature;
			componentData.m_OccurenceRain = component.m_OccurenceRain;
			componentData.m_Duration = component.m_Duration;
			componentData.m_PhenomenonRadius = component.m_PhenomenonRadius;
			componentData.m_HotspotRadius = component.m_HotspotRadius;
			componentData.m_HotspotInstability = component.m_HotspotInstability;
			componentData.m_DamageSeverity = component.m_DamageSeverity;
			componentData.m_DangerLevel = component.m_DangerLevel;
			((EntityManager)(ref entityManager)).SetComponentData<WeatherPhenomenonData>(entity, componentData);
		}
	}
}
