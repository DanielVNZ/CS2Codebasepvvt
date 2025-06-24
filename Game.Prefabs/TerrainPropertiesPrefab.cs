using System.Collections.Generic;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

public class TerrainPropertiesPrefab : PrefabBase
{
	public WaterSystem.WaterSource[] m_WaterSources;

	public int m_WaterSourceSteps;

	public int m_WaterVelocitySteps;

	public int m_WaterDepthSteps;

	public int m_WaterMaxSpeed;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TerrainPropertiesData>());
	}
}
