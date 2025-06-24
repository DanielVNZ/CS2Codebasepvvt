using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public struct Moving : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Velocity;

	public float3 m_AngularVelocity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 velocity = m_Velocity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(velocity);
		float3 angularVelocity = m_AngularVelocity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(angularVelocity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ref float3 velocity = ref m_Velocity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref velocity);
		ref float3 angularVelocity = ref m_AngularVelocity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref angularVelocity);
		if (!math.all(math.isfinite(m_Velocity)) || !math.all(math.isfinite(m_AngularVelocity)))
		{
			m_Velocity = default(float3);
			m_AngularVelocity = default(float3);
		}
	}
}
