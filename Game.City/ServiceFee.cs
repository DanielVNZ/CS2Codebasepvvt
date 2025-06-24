using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct ServiceFee : IBufferElementData, ISerializable
{
	public PlayerResource m_Resource;

	public float m_Fee;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		PlayerResource resource = m_Resource;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)resource);
		float fee = m_Fee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fee);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if ((int)((Context)(ref context)).purpose == 1)
		{
			int resource = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref resource);
			m_Resource = (PlayerResource)resource;
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_Fee = GetDefaultFee(m_Resource);
			return;
		}
		int resource2 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resource2);
		m_Resource = (PlayerResource)resource2;
		ref float fee = ref m_Fee;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fee);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.waterFeeReset && m_Resource == PlayerResource.Water)
		{
			m_Fee = 0.3f;
		}
	}

	public float GetDefaultFee(PlayerResource resource)
	{
		return resource switch
		{
			PlayerResource.BasicEducation => 100f, 
			PlayerResource.SecondaryEducation => 200f, 
			PlayerResource.HigherEducation => 300f, 
			PlayerResource.Healthcare => 100f, 
			PlayerResource.Garbage => 0.1f, 
			PlayerResource.Electricity => 0.2f, 
			PlayerResource.Water => 0.1f, 
			_ => 0f, 
		};
	}
}
