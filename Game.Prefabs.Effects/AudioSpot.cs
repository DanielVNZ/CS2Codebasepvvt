using System;
using System.Collections.Generic;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class AudioSpot : ComponentBase
{
	[EditorName("Sound Effects")]
	public EffectPrefab[] m_SFXes;

	public float m_MinimumInterval;

	public float m_MaximumInterval;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AudioSpotData>());
		components.Add(ComponentType.ReadWrite<AudioSourceData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		if (m_SFXes != null)
		{
			for (int i = 0; i < m_SFXes.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < m_SFXes[i].components.Count; j++)
				{
					ComponentBase componentBase = m_SFXes[i].components[j];
					if (componentBase is SFX)
					{
						flag = true;
						if (((SFX)componentBase).m_Loop)
						{
							string name = ((Object)World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>().GetPrefab<EffectPrefab>(entity)).name;
							ComponentBase.baseLog.WarnFormat("Warning: AudioSpot {0} SFX {1} is looping", (object)name, (object)j);
						}
					}
				}
				if (!flag)
				{
					ComponentBase.baseLog.WarnFormat("Warning: AudioSpot {0} has SFX without SFX component", (object)((Object)this).name);
				}
			}
			DynamicBuffer<AudioSourceData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AudioSourceData>(entity, false);
			buffer.ResizeUninitialized(m_SFXes.Length);
			for (int k = 0; k < m_SFXes.Length; k++)
			{
				buffer[k] = new AudioSourceData
				{
					m_SFXEntity = orCreateSystemManaged.GetEntity(m_SFXes[k])
				};
			}
		}
		else
		{
			ComponentBase.baseLog.WarnFormat("Warning: AudioSpot {0} has no sound effects", (object)((Object)this).name);
		}
		((EntityManager)(ref entityManager)).SetComponentData<AudioSpotData>(entity, new AudioSpotData
		{
			m_Interval = new float2(m_MinimumInterval, m_MaximumInterval)
		});
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_SFXes != null)
		{
			EffectPrefab[] sFXes = m_SFXes;
			foreach (EffectPrefab item in sFXes)
			{
				prefabs.Add(item);
			}
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
