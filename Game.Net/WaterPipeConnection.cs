using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct WaterPipeConnection : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_FreshCapacity;

	public int m_SewageCapacity;

	public int m_StormCapacity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int freshCapacity = m_FreshCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshCapacity);
		int sewageCapacity = m_SewageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sewageCapacity);
		int stormCapacity = m_StormCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(stormCapacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref int freshCapacity = ref m_FreshCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshCapacity);
		ref int sewageCapacity = ref m_SewageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sewageCapacity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.stormWater)
		{
			ref int stormCapacity = ref m_StormCapacity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref stormCapacity);
		}
		else
		{
			m_StormCapacity = 5000;
		}
	}
}
