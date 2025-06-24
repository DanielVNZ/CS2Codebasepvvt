using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

[InternalBufferCapacity(0)]
public struct Queue : IBufferElementData, ISerializable
{
	public Entity m_TargetEntity;

	public Sphere3 m_TargetArea;

	public ushort m_ObsoleteTime;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Entity targetEntity = m_TargetEntity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetEntity);
		ushort obsoleteTime = m_ObsoleteTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(obsoleteTime);
		float radius = m_TargetArea.radius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(radius);
		if (m_TargetArea.radius > 0f)
		{
			float3 position = m_TargetArea.position;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity targetEntity = ref m_TargetEntity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetEntity);
		ref ushort obsoleteTime = ref m_ObsoleteTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref obsoleteTime);
		ref float radius = ref m_TargetArea.radius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref radius);
		if (m_TargetArea.radius > 0f)
		{
			ref float3 position = ref m_TargetArea.position;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		}
	}
}
