using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct StorageCompanyData : IComponentData, IQueryTypeParameter, ISerializable, ICombineData<StorageCompanyData>
{
	public Resource m_StoredResources;

	public int2 m_TransportInterval;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Resource storedResources = m_StoredResources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)storedResources);
		int2 transportInterval = m_TransportInterval;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(transportInterval);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		ulong storedResources = default(ulong);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref storedResources);
		m_StoredResources = (Resource)storedResources;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.transportInterval)
		{
			ref int2 transportInterval = ref m_TransportInterval;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportInterval);
		}
	}

	public void Combine(StorageCompanyData otherData)
	{
		m_StoredResources |= otherData.m_StoredResources;
	}
}
