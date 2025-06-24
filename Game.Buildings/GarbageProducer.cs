using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct GarbageProducer : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_CollectionRequest;

	public int m_Garbage;

	public GarbageProducerFlags m_Flags;

	public byte m_DispatchIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity collectionRequest = m_CollectionRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(collectionRequest);
		int garbage = m_Garbage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbage);
		GarbageProducerFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ref Entity collectionRequest = ref m_CollectionRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref collectionRequest);
		ref int garbage = ref m_Garbage;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbage);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.garbageProducerFlags)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (GarbageProducerFlags)flags;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
