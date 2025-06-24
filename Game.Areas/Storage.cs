using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

public struct Storage : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Amount;

	public float m_WorkAmount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		float workAmount = m_WorkAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workAmount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.garbageFacilityRefactor)
		{
			ref float workAmount = ref m_WorkAmount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref workAmount);
		}
	}
}
