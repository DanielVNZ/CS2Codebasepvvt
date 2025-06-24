using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

[InternalBufferCapacity(9)]
public struct MapFeatureElement : IBufferElementData, ISerializable
{
	public float m_Amount;

	public float m_RenewalRate;

	public MapFeatureElement(float amount, float regenerationRate)
	{
		m_Amount = amount;
		m_RenewalRate = regenerationRate;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		float renewalRate = m_RenewalRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(renewalRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref float amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.naturalResourceRenewalRate)
		{
			ref float renewalRate = ref m_RenewalRate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref renewalRate);
		}
	}
}
