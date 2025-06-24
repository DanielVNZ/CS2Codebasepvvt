using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct PropertyRenter : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Property;

	public int m_Rent;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity property = m_Property;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(property);
		int rent = m_Rent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(rent);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity property = ref m_Property;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref property);
		ref int rent = ref m_Rent;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rent);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.economyFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
	}
}
