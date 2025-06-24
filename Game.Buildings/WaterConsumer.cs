using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct WaterConsumer : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Pollution;

	public int m_WantedConsumption;

	public int m_FulfilledFresh;

	public int m_FulfilledSewage;

	public byte m_FreshCooldownCounter;

	public byte m_SewageCooldownCounter;

	public WaterConsumerFlags m_Flags;

	public bool waterConnected => (m_Flags & WaterConsumerFlags.WaterConnected) != 0;

	public bool sewageConnected => (m_Flags & WaterConsumerFlags.SewageConnected) != 0;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float pollution = m_Pollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pollution);
		int wantedConsumption = m_WantedConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wantedConsumption);
		int fulfilledFresh = m_FulfilledFresh;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fulfilledFresh);
		int fulfilledSewage = m_FulfilledSewage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fulfilledSewage);
		byte freshCooldownCounter = m_FreshCooldownCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshCooldownCounter);
		byte sewageCooldownCounter = m_SewageCooldownCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sewageCooldownCounter);
		WaterConsumerFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipePollution)
		{
			ref float pollution = ref m_Pollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref pollution);
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.waterPipeFlowSim)
		{
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			int num3 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterConsumption)
		{
			ref int wantedConsumption = ref m_WantedConsumption;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref wantedConsumption);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.buildingEfficiencyRework)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.utilityFeePrecision)
			{
				float num4 = default(float);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num4);
			}
			else
			{
				context = ((IReader)reader).context;
				if (((Context)(ref context)).version >= Version.waterFee)
				{
					int num5 = default(int);
					((IReader)reader/*cast due to .constrained prefix*/).Read(ref num5);
				}
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipeFlowSim)
		{
			ref int fulfilledFresh = ref m_FulfilledFresh;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref fulfilledFresh);
			ref int fulfilledSewage = ref m_FulfilledSewage;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref fulfilledSewage);
			ref byte freshCooldownCounter = ref m_FreshCooldownCounter;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshCooldownCounter);
			ref byte sewageCooldownCounter = ref m_SewageCooldownCounter;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref sewageCooldownCounter);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterConsumerFlags)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (WaterConsumerFlags)flags;
		}
	}
}
