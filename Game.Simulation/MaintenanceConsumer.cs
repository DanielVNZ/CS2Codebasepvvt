using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct MaintenanceConsumer : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Request;

	public byte m_DispatchIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity request = m_Request;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(request);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity request = ref m_Request;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref request);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
