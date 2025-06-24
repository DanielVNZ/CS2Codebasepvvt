using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct WaterPumpingStation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Pollution;

	public int m_Capacity;

	public int m_LastProduction;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float pollution = m_Pollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pollution);
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		int lastProduction = m_LastProduction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastProduction);
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
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.waterPipeFlowSim)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPipePollution)
		{
			ref float pollution = ref m_Pollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref pollution);
		}
		else
		{
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		}
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterSelectedInfoFix)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.waterPipeFlowSim)
			{
				int num3 = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			}
			ref int lastProduction = ref m_LastProduction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastProduction);
		}
	}
}
