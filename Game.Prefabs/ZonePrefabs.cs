using Game.Zones;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

public struct ZonePrefabs
{
	private NativeArray<Entity> m_ZonePrefabs;

	public Entity this[ZoneType type] => m_ZonePrefabs[(int)type.m_Index];

	public ZonePrefabs(NativeArray<Entity> zonePrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_ZonePrefabs = zonePrefabs;
	}
}
