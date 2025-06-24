using System;
using System.Collections.Generic;
using Game.Prefabs.Climate;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class SeasonColorFilter : ComponentBase
{
	[Serializable]
	public class ColorFilter
	{
		public SeasonPrefab m_SeasonFilter;

		public string[] m_VariationGroups;

		public int m_OverrideProbability = -1;
	}

	public ColorFilter[] m_ColorFilters;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_ColorFilters.Length; i++)
		{
			SeasonPrefab seasonFilter = m_ColorFilters[i].m_SeasonFilter;
			if ((Object)(object)seasonFilter != (Object)null)
			{
				prefabs.Add(seasonFilter);
			}
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Prefabs.ColorFilter>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		MeshColorSystem orCreateSystemManaged2 = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<MeshColorSystem>();
		DynamicBuffer<Game.Prefabs.ColorFilter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Prefabs.ColorFilter>(entity, false);
		int num = 0;
		for (int i = 0; i < m_ColorFilters.Length; i++)
		{
			num += m_ColorFilters[i].m_VariationGroups.Length;
		}
		buffer.ResizeUninitialized(num);
		num = 0;
		for (int j = 0; j < m_ColorFilters.Length; j++)
		{
			ColorFilter colorFilter = m_ColorFilters[j];
			Game.Prefabs.ColorFilter colorFilter2 = new Game.Prefabs.ColorFilter
			{
				m_AgeFilter = AgeMask.Any,
				m_GenderFilter = GenderMask.Any,
				m_OverrideProbability = (sbyte)math.clamp(colorFilter.m_OverrideProbability, -1, 100),
				m_OverrideAlpha = float3.op_Implicit(-1f)
			};
			if ((Object)(object)colorFilter.m_SeasonFilter != (Object)null)
			{
				colorFilter2.m_EntityFilter = orCreateSystemManaged.GetEntity(colorFilter.m_SeasonFilter);
				colorFilter2.m_Flags |= ColorFilterFlags.SeasonFilter;
			}
			for (int k = 0; k < colorFilter.m_VariationGroups.Length; k++)
			{
				colorFilter2.m_GroupID = orCreateSystemManaged2.GetColorGroupID(colorFilter.m_VariationGroups[k]);
				buffer[num++] = colorFilter2;
			}
		}
	}
}
