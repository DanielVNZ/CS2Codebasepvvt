using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;

namespace Game.Vehicles;

public struct TrainCurrentLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrainBogieLane m_Front;

	public TrainBogieLane m_Rear;

	public TrainBogieCache m_FrontCache;

	public TrainBogieCache m_RearCache;

	public float m_Duration;

	public float m_Distance;

	public TrainCurrentLane(PathElement pathElement)
	{
		m_Front = new TrainBogieLane(pathElement);
		m_Rear = new TrainBogieLane(pathElement);
		m_FrontCache = new TrainBogieCache(pathElement);
		m_RearCache = new TrainBogieCache(pathElement);
		m_Duration = 0f;
		m_Distance = 0f;
	}

	public TrainCurrentLane(ParkedTrain parkedTrain)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		m_Front = new TrainBogieLane(parkedTrain.m_FrontLane, parkedTrain.m_CurvePosition.x);
		m_Rear = new TrainBogieLane(parkedTrain.m_RearLane, parkedTrain.m_CurvePosition.y);
		m_FrontCache = new TrainBogieCache(parkedTrain.m_FrontLane, parkedTrain.m_CurvePosition.x);
		m_RearCache = new TrainBogieCache(parkedTrain.m_RearLane, parkedTrain.m_CurvePosition.y);
		m_Duration = 0f;
		m_Distance = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		TrainBogieLane front = m_Front;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogieLane>(front);
		TrainBogieLane rear = m_Rear;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogieLane>(rear);
		TrainBogieCache frontCache = m_FrontCache;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogieCache>(frontCache);
		TrainBogieCache rearCache = m_RearCache;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<TrainBogieCache>(rearCache);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ref TrainBogieLane front = ref m_Front;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogieLane>(ref front);
		ref TrainBogieLane rear = ref m_Rear;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogieLane>(ref rear);
		ref TrainBogieCache frontCache = ref m_FrontCache;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogieCache>(ref frontCache);
		ref TrainBogieCache rearCache = ref m_RearCache;
		((IReader)reader/*cast due to .constrained prefix*/).Read<TrainBogieCache>(ref rearCache);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficFlowFixes)
		{
			ref float duration = ref m_Duration;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
			ref float distance = ref m_Distance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		}
	}
}
