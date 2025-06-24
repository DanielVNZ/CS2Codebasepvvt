using System;
using System.Collections.Generic;
using Game.Audio;
using Game.Effects;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class SFX : ComponentBase
{
	public AudioClip m_AudioClip;

	[Range(0f, 1f)]
	public float m_Volume = 1f;

	[Range(-3f, 3f)]
	public float m_Pitch = 1f;

	[Range(0f, 1f)]
	public float m_SpatialBlend = 1f;

	[Range(0f, 1f)]
	public float m_Doppler = 1f;

	public float m_Spread;

	public AudioRolloffMode m_RolloffMode = (AudioRolloffMode)1;

	public float2 m_MinMaxDistance = new float2(1f, 200f);

	public bool m_Loop;

	public MixerGroup m_MixerGroup;

	public byte m_Priority = 128;

	public AnimationCurve m_RolloffCurve;

	public float3 m_SourceSize;

	public float2 m_FadeTimes;

	public bool m_RandomStartTime;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AudioEffectData>());
		components.Add(ComponentType.ReadWrite<AudioSourceData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		AudioManager existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<AudioManager>();
		((EntityManager)(ref entityManager)).SetComponentData<AudioEffectData>(entity, new AudioEffectData
		{
			m_AudioClipId = existingSystemManaged.RegisterSFX(this),
			m_MaxDistance = m_MinMaxDistance.y,
			m_SourceSize = m_SourceSize,
			m_FadeTimes = m_FadeTimes
		});
		DynamicBuffer<AudioSourceData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AudioSourceData>(entity, false);
		buffer.ResizeUninitialized(1);
		buffer[0] = new AudioSourceData
		{
			m_SFXEntity = entity
		};
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
