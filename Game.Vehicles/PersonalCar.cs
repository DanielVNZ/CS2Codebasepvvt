using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct PersonalCar : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Keeper;

	public PersonalCarFlags m_State;

	public PersonalCar(Entity keeper, PersonalCarFlags state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Keeper = keeper;
		m_State = state;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity keeper = m_Keeper;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(keeper);
		PersonalCarFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity keeper = ref m_Keeper;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref keeper);
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		m_State = (PersonalCarFlags)state;
	}
}
