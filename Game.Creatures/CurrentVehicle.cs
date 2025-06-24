using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct CurrentVehicle : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Vehicle;

	public CreatureVehicleFlags m_Flags;

	public CurrentVehicle(Entity vehicle, CreatureVehicleFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Vehicle = vehicle;
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity vehicle = m_Vehicle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicle);
		CreatureVehicleFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity vehicle = ref m_Vehicle;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicle);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (CreatureVehicleFlags)flags;
	}
}
