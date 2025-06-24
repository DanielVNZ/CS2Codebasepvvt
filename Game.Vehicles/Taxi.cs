using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Taxi : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public TaxiFlags m_State;

	public float m_PathElementTime;

	public float m_StartDistance;

	public float m_MaxBoardingDistance;

	public float m_MinWaitingDistance;

	public int m_ExtraPathElementCount;

	public ushort m_NextStartingFee;

	public ushort m_CurrentFee;

	public Taxi(TaxiFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = flags;
		m_PathElementTime = 0f;
		m_StartDistance = 0f;
		m_MaxBoardingDistance = 0f;
		m_MinWaitingDistance = 0f;
		m_ExtraPathElementCount = 0;
		m_NextStartingFee = 0;
		m_CurrentFee = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		TaxiFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		int extraPathElementCount = m_ExtraPathElementCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extraPathElementCount);
		float startDistance = m_StartDistance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startDistance);
		ushort nextStartingFee = m_NextStartingFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextStartingFee);
		ushort currentFee = m_CurrentFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(currentFee);
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
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		ref int extraPathElementCount = ref m_ExtraPathElementCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref extraPathElementCount);
		m_State = (TaxiFlags)state;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiFee)
		{
			ref float startDistance = ref m_StartDistance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref startDistance);
			ref ushort nextStartingFee = ref m_NextStartingFee;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextStartingFee);
			ref ushort currentFee = ref m_CurrentFee;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref currentFee);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.roadPatchImprovements)
		{
			ref float maxBoardingDistance = ref m_MaxBoardingDistance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxBoardingDistance);
			ref float minWaitingDistance = ref m_MinWaitingDistance;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minWaitingDistance);
		}
	}
}
