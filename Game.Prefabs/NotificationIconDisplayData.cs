using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct NotificationIconDisplayData : IComponentData, IQueryTypeParameter, IEnableableComponent, ISerializable, ISerializeAsEnabled
{
	public float2 m_MinParams;

	public float2 m_MaxParams;

	public int m_IconIndex;

	public uint m_CategoryMask;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write((m_MinParams + m_MaxParams) * 0.5f);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_MinParams);
		m_MaxParams = m_MinParams;
		m_IconIndex = 0;
		m_CategoryMask = 2147483648u;
	}
}
