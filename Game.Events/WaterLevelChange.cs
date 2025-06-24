using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Events;

public struct WaterLevelChange : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Intensity;

	public float m_MaxIntensity;

	public float m_DangerHeight;

	public float2 m_Direction;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref float intensity = ref m_Intensity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref intensity);
		ref float maxIntensity = ref m_MaxIntensity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxIntensity);
		ref float dangerHeight = ref m_DangerHeight;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dangerHeight);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.tsunamiDirection)
		{
			float2 val = default(float2);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float intensity = m_Intensity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(intensity);
		float maxIntensity = m_MaxIntensity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxIntensity);
		float dangerHeight = m_DangerHeight;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dangerHeight);
		float2 direction = m_Direction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(direction);
	}
}
