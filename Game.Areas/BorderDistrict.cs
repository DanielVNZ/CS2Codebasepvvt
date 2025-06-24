using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

public struct BorderDistrict : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Left;

	public Entity m_Right;

	public BorderDistrict(Entity left, Entity right)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Left = left;
		m_Right = right;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity left = m_Left;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(left);
		Entity right = m_Right;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(right);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity left = ref m_Left;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref left);
		ref Entity right = ref m_Right;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref right);
	}
}
