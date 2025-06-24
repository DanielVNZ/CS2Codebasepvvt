using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PrefabRef : IComponentData, IQueryTypeParameter, IStrideSerializable, ISerializable
{
	public Entity m_Prefab;

	public PrefabRef(Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Prefab);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Prefab);
	}

	public int GetStride(Context context)
	{
		return 4;
	}

	public static implicit operator Entity(PrefabRef prefabRef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return prefabRef.m_Prefab;
	}
}
