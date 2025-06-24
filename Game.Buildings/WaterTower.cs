using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct WaterTower : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_StoredWater;

	public int m_Polluted;

	public int m_LastStoredWater;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int storedWater = m_StoredWater;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(storedWater);
		int polluted = m_Polluted;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(polluted);
		int lastStoredWater = m_LastStoredWater;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastStoredWater);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref int storedWater = ref m_StoredWater;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref storedWater);
		ref int polluted = ref m_Polluted;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref polluted);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterSelectedInfoFix)
		{
			ref int lastStoredWater = ref m_LastStoredWater;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastStoredWater);
		}
	}
}
