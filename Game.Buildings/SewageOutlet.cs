using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct SewageOutlet : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Capacity;

	public int m_LastProcessed;

	public int m_LastPurified;

	public int m_UsedPurified;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		int lastProcessed = m_LastProcessed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastProcessed);
		int lastPurified = m_LastPurified;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastPurified);
		int usedPurified = m_UsedPurified;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(usedPurified);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.waterPipeFlowSim)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.stormWater)
			{
				int num2 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			}
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version >= Version.sewageSelectedInfoFix)
			{
				int num3 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			}
		}
		else
		{
			ref int capacity = ref m_Capacity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
			ref int lastProcessed = ref m_LastProcessed;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastProcessed);
			ref int lastPurified = ref m_LastPurified;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastPurified);
			ref int usedPurified = ref m_UsedPurified;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref usedPurified);
		}
	}
}
