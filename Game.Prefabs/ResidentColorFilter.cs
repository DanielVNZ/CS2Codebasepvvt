using System;
using System.Collections.Generic;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[]
{
	typeof(RenderPrefab),
	typeof(CharacterOverlay)
})]
public class ResidentColorFilter : ComponentBase
{
	[Serializable]
	public class ColorFilter
	{
		public AgeMask m_AgeFilter;

		public GenderMask m_GenderFilter;

		public string[] m_VariationGroups;

		public int m_OverrideProbability = -1;

		public float3 m_OverrideAlpha = float3.op_Implicit(-1f);
	}

	public ColorFilter[] m_ColorFilters;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Prefabs.ColorFilter>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		MeshColorSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<MeshColorSystem>();
		DynamicBuffer<Game.Prefabs.ColorFilter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Prefabs.ColorFilter>(entity, false);
		int num = 0;
		for (int i = 0; i < m_ColorFilters.Length; i++)
		{
			num += m_ColorFilters[i].m_VariationGroups.Length;
		}
		buffer.ResizeUninitialized(num);
		num = 0;
		ColorProperties component = GetComponent<ColorProperties>();
		for (int j = 0; j < m_ColorFilters.Length; j++)
		{
			ColorFilter colorFilter = m_ColorFilters[j];
			Game.Prefabs.ColorFilter colorFilter2 = new Game.Prefabs.ColorFilter
			{
				m_AgeFilter = colorFilter.m_AgeFilter,
				m_GenderFilter = colorFilter.m_GenderFilter,
				m_OverrideProbability = (sbyte)math.clamp(colorFilter.m_OverrideProbability, -1, 100),
				m_OverrideAlpha = float3.op_Implicit(-1f)
			};
			if ((Object)(object)component != (Object)null)
			{
				float3 alphas = math.select(math.saturate(colorFilter.m_OverrideAlpha), float3.op_Implicit(-1f), colorFilter.m_OverrideAlpha < 0f);
				colorFilter2.m_OverrideAlpha.x = component.GetAlpha(alphas, 0, -1f);
				colorFilter2.m_OverrideAlpha.y = component.GetAlpha(alphas, 1, -1f);
				colorFilter2.m_OverrideAlpha.z = component.GetAlpha(alphas, 2, -1f);
			}
			for (int k = 0; k < colorFilter.m_VariationGroups.Length; k++)
			{
				colorFilter2.m_GroupID = orCreateSystemManaged.GetColorGroupID(colorFilter.m_VariationGroups[k]);
				buffer[num++] = colorFilter2;
			}
		}
	}
}
