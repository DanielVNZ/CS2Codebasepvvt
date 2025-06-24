using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class UIPollutionConfigurationPrefab : PrefabBase
{
	public UIPollutionThresholds m_GroundPollution;

	public UIPollutionThresholds m_AirPollution;

	public UIPollutionThresholds m_NoisePollution;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<UIPollutionConfigurationData>());
	}
}
