using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct Loan : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Amount;

	public uint m_LastModified;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.noAutoPaybackLoans)
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.loanLastModified)
		{
			ref uint lastModified = ref m_LastModified;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastModified);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		uint lastModified = m_LastModified;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastModified);
	}
}
