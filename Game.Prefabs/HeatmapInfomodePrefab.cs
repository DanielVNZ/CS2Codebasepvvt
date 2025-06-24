using System;
using System.Collections.Generic;
using Game.Rendering;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tools/Infomode/", new Type[] { })]
public class HeatmapInfomodePrefab : GradientInfomodeBasePrefab
{
	public HeatmapData m_Type;

	public override string infomodeTypeLocaleKey => "TerrainColor";

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<InfoviewHeatmapData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<InfoviewHeatmapData>(entity, new InfoviewHeatmapData
		{
			m_Type = m_Type
		});
	}

	public override int GetColorGroup(out int secondaryGroup)
	{
		switch (m_Type)
		{
		case HeatmapData.AirPollution:
		case HeatmapData.Wind:
		case HeatmapData.TelecomCoverage:
		case HeatmapData.Oil:
		case HeatmapData.Noise:
			secondaryGroup = 1;
			return 0;
		case HeatmapData.WaterFlow:
		case HeatmapData.WaterPollution:
		case HeatmapData.Fish:
			secondaryGroup = -1;
			return 1;
		default:
			secondaryGroup = -1;
			return 0;
		}
	}

	public override bool CanActivateBoth(InfomodePrefab other)
	{
		if (other is HeatmapInfomodePrefab heatmapInfomodePrefab && HasArrowsOnWater() && heatmapInfomodePrefab.HasArrowsOnWater())
		{
			return false;
		}
		return base.CanActivateBoth(other);
	}

	private bool HasArrowsOnWater()
	{
		HeatmapData type = m_Type;
		if ((uint)(type - 4) <= 1u)
		{
			return true;
		}
		return false;
	}
}
