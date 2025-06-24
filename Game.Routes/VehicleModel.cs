using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct VehicleModel : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_PrimaryPrefab;

	public Entity m_SecondaryPrefab;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity primaryPrefab = m_PrimaryPrefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(primaryPrefab);
		Entity secondaryPrefab = m_SecondaryPrefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(secondaryPrefab);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity primaryPrefab = ref m_PrimaryPrefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref primaryPrefab);
		ref Entity secondaryPrefab = ref m_SecondaryPrefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref secondaryPrefab);
	}
}
