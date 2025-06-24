using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct GroupCreature : IBufferElementData, IEmptySerializable
{
	public Entity m_Creature;

	public GroupCreature(Entity creature)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Creature = creature;
	}
}
