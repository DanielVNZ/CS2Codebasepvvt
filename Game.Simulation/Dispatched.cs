using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct Dispatched : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Handler;

	public Dispatched(Entity handler)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Handler = handler;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Handler);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity handler = ref m_Handler;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref handler);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.dispatchRefactoring)
		{
			uint num = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
	}
}
