using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct WaitingPassengers : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Count;

	public int m_OngoingAccumulation;

	public int m_ConcludedAccumulation;

	public ushort m_SuccessAccumulation;

	public ushort m_AverageWaitingTime;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int count = m_Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		int ongoingAccumulation = m_OngoingAccumulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(ongoingAccumulation);
		int concludedAccumulation = m_ConcludedAccumulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(concludedAccumulation);
		ushort successAccumulation = m_SuccessAccumulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(successAccumulation);
		ushort averageWaitingTime = m_AverageWaitingTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageWaitingTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		ref int count = ref m_Count;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref count);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.passengerWaitTimeCost2)
		{
			ref int ongoingAccumulation = ref m_OngoingAccumulation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref ongoingAccumulation);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.passengerWaitTimeCost)
		{
			ref int concludedAccumulation = ref m_ConcludedAccumulation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref concludedAccumulation);
			ref ushort successAccumulation = ref m_SuccessAccumulation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref successAccumulation);
			ref ushort averageWaitingTime = ref m_AverageWaitingTime;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageWaitingTime);
		}
	}
}
