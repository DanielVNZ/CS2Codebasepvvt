using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Prefabs;

public class WeatherAudioPrefab : PrefabBase
{
	[Header("Water")]
	[FormerlySerializedAs("m_SmallWaterAudio")]
	[Tooltip("The water ambient audio clip")]
	public EffectPrefab m_WaterAmbientAudio;

	[Tooltip("The water audio's intensity that will be applied to the audio clip's volume")]
	public float m_WaterAudioIntensity;

	[Tooltip("The water audio's fade in & fade out speed")]
	public float m_WaterFadeSpeed;

	[Tooltip("The water audio will not be heard when the camera zoom is bigger than it")]
	public int m_WaterAudioEnabledZoom;

	[Tooltip("The near cell's index distance that can play the water audio ")]
	public int m_WaterAudioNearDistance;

	[Header("Lightning")]
	[Tooltip("The lightning sound speed that affect the delay")]
	public float m_LightningSoundSpeed;

	[Tooltip("The lightning audio clip")]
	public EffectPrefab m_LightningAudio;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<WeatherAudioData>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		WeatherAudioData weatherAudioData = new WeatherAudioData
		{
			m_WaterFadeSpeed = m_WaterFadeSpeed,
			m_WaterAudioIntensity = m_WaterAudioIntensity,
			m_WaterAudioEnabledZoom = m_WaterAudioEnabledZoom,
			m_WaterAudioNearDistance = m_WaterAudioNearDistance,
			m_WaterAmbientAudio = orCreateSystemManaged.GetEntity(m_WaterAmbientAudio),
			m_LightningAudio = orCreateSystemManaged.GetEntity(m_LightningAudio),
			m_LightningSoundSpeed = m_LightningSoundSpeed
		};
		((EntityManager)(ref entityManager)).SetComponentData<WeatherAudioData>(entity, weatherAudioData);
	}
}
