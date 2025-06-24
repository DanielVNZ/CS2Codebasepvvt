using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct TouristHousehold : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Hotel;

	public uint m_LeavingTime;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity hotel = m_Hotel;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hotel);
		uint leavingTime = m_LeavingTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(leavingTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity hotel = ref m_Hotel;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hotel);
		ref uint leavingTime = ref m_LeavingTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref leavingTime);
	}
}
