using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPrefab) })]
public class AuxiliaryNets : ComponentBase
{
	public bool m_LinkEndOffsets = true;

	public AuxiliaryNetInfo[] m_AuxiliaryNets;

	public override bool ignoreUnlockDependencies => true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_AuxiliaryNets.Length; i++)
		{
			prefabs.Add(m_AuxiliaryNets[i].m_Prefab);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceableNetData>());
		components.Add(ComponentType.ReadWrite<AuxiliaryNet>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.SubNet>());
	}
}
