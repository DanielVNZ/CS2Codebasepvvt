using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct BoardingVehicle : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Vehicle;

	public Entity m_Testing;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity vehicle = m_Vehicle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicle);
		Entity testing = m_Testing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(testing);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity vehicle = ref m_Vehicle;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicle);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.boardingTest)
		{
			ref Entity testing = ref m_Testing;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref testing);
		}
	}
}
