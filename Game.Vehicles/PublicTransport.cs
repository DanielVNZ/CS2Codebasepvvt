using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct PublicTransport : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public PublicTransportFlags m_State;

	public uint m_DepartureFrame;

	public int m_RequestCount;

	public float m_PathElementTime;

	public float m_MaxBoardingDistance;

	public float m_MinWaitingDistance;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PublicTransportFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		uint departureFrame = m_DepartureFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(departureFrame);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		float maxBoardingDistance = m_MaxBoardingDistance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxBoardingDistance);
		float minWaitingDistance = m_MinWaitingDistance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minWaitingDistance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref uint departureFrame = ref m_DepartureFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref departureFrame);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.evacuationTransport)
		{
			ref int requestCount = ref m_RequestCount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestCount);
			ref float pathElementTime = ref m_PathElementTime;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.roadPatchImprovements)
		{
			ref float maxBoardingDistance = ref m_MaxBoardingDistance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxBoardingDistance);
			ref float minWaitingDistance = ref m_MinWaitingDistance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minWaitingDistance);
		}
		m_State = (PublicTransportFlags)state;
	}
}
