using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Events;

public struct WeatherPhenomenon : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_PhenomenonPosition;

	public float3 m_HotspotPosition;

	public float3 m_HotspotVelocity;

	public float m_PhenomenonRadius;

	public float m_HotspotRadius;

	public float m_Intensity;

	public float m_LightningTimer;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float3 phenomenonPosition = m_PhenomenonPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(phenomenonPosition);
		float3 hotspotPosition = m_HotspotPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hotspotPosition);
		float3 hotspotVelocity = m_HotspotVelocity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hotspotVelocity);
		float phenomenonRadius = m_PhenomenonRadius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(phenomenonRadius);
		float hotspotRadius = m_HotspotRadius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(hotspotRadius);
		float intensity = m_Intensity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(intensity);
		float lightningTimer = m_LightningTimer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lightningTimer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		ref float3 phenomenonPosition = ref m_PhenomenonPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref phenomenonPosition);
		ref float3 hotspotPosition = ref m_HotspotPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hotspotPosition);
		ref float3 hotspotVelocity = ref m_HotspotVelocity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hotspotVelocity);
		ref float phenomenonRadius = ref m_PhenomenonRadius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref phenomenonRadius);
		ref float hotspotRadius = ref m_HotspotRadius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref hotspotRadius);
		ref float intensity = ref m_Intensity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref intensity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.lightningSimulation)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.weatherPhenomenonFix)
			{
				float num = default(float);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			}
			ref float lightningTimer = ref m_LightningTimer;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lightningTimer);
		}
	}
}
