using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public class MilestonePrefab : PrefabBase
{
	public int m_Index;

	public int m_Reward;

	public int m_DevTreePoints;

	public int m_MapTiles;

	public int m_LoanLimit;

	public int m_XpRequried;

	public bool m_Major;

	public string m_Image;

	public Color m_BackgroundColor = new Color(0f, 0f, 0f, 0f);

	public Color m_AccentColor = new Color(0.18f, 0.235f, 0.337f, 1f);

	public Color m_TextColor = Color.white;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<MilestoneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<MilestoneData>(entity, new MilestoneData
		{
			m_Index = m_Index,
			m_Reward = m_Reward,
			m_DevTreePoints = m_DevTreePoints,
			m_MapTiles = m_MapTiles,
			m_LoanLimit = m_LoanLimit,
			m_XpRequried = m_XpRequried,
			m_Major = m_Major
		});
	}
}
