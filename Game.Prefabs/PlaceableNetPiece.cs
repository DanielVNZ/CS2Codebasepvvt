using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPiecePrefab) })]
public class PlaceableNetPiece : ComponentBase
{
	public uint m_ConstructionCost;

	public uint m_ElevationCost;

	public float m_UpkeepCost;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceableNetPieceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<PlaceableNetPieceData>(entity, new PlaceableNetPieceData
		{
			m_ConstructionCost = m_ConstructionCost,
			m_ElevationCost = m_ElevationCost,
			m_UpkeepCost = m_UpkeepCost
		});
	}
}
