using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct ServiceBudgetData : IBufferElementData, ISerializable
{
	public Entity m_Service;

	public int m_Budget;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity service = ref m_Service;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref service);
		ref int budget = ref m_Budget;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref budget);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.serviceImportBudgets)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity service = m_Service;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(service);
		int budget = m_Budget;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(budget);
	}
}
