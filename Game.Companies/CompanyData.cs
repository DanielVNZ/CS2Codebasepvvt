using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Companies;

public struct CompanyData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Random m_RandomSeed;

	public Entity m_Brand;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		uint state = m_RandomSeed.state;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(state);
		Entity brand = m_Brand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(brand);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref uint state = ref m_RandomSeed.state;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref Entity brand = ref m_Brand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref brand);
	}
}
