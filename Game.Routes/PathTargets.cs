using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Routes;

public struct PathTargets : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_StartLane;

	public Entity m_EndLane;

	public float2 m_CurvePositions;

	public float3 m_ReadyStartPosition;

	public float3 m_ReadyEndPosition;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity startLane = m_StartLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startLane);
		Entity endLane = m_EndLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endLane);
		float2 curvePositions = m_CurvePositions;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePositions);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity startLane = ref m_StartLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startLane);
		ref Entity endLane = ref m_EndLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endLane);
		ref float2 curvePositions = ref m_CurvePositions;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePositions);
	}
}
