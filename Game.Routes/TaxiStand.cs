using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct TaxiStand : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TaxiRequest;

	public TaxiStandFlags m_Flags;

	public ushort m_StartingFee;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity taxiRequest = m_TaxiRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(taxiRequest);
		TaxiStandFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		ushort startingFee = m_StartingFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startingFee);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		ref Entity taxiRequest = ref m_TaxiRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref taxiRequest);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiStandFlags)
		{
			uint flags = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (TaxiStandFlags)flags;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiFee)
		{
			ref ushort startingFee = ref m_StartingFee;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref startingFee);
		}
	}
}
