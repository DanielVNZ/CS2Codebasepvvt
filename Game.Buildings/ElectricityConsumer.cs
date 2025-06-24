using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct ElectricityConsumer : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_WantedConsumption;

	public int m_FulfilledConsumption;

	public short m_CooldownCounter;

	public ElectricityConsumerFlags m_Flags;

	public bool electricityConnected => (m_Flags & ElectricityConsumerFlags.Connected) != 0;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int wantedConsumption = m_WantedConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wantedConsumption);
		int fulfilledConsumption = m_FulfilledConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fulfilledConsumption);
		short cooldownCounter = m_CooldownCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cooldownCounter);
		ElectricityConsumerFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		ref int wantedConsumption = ref m_WantedConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref wantedConsumption);
		ref int fulfilledConsumption = ref m_FulfilledConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fulfilledConsumption);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.electricityFlashFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		else
		{
			ref short cooldownCounter = ref m_CooldownCounter;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref cooldownCounter);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.notificationData)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.bottleneckNotification)
			{
				byte flags = default(byte);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
				m_Flags = (ElectricityConsumerFlags)flags;
			}
			else
			{
				bool flag = default(bool);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref flag);
				if (!flag)
				{
					m_Flags = ElectricityConsumerFlags.Connected;
				}
			}
		}
		context = ((IReader)reader).context;
		if (!(((Context)(ref context)).version < Version.buildingEfficiencyRework))
		{
			return;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.utilityFeePrecision)
		{
			float num2 = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			return;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.electricityFeeEffect)
		{
			int num3 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
		}
	}
}
