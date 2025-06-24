using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public struct Damaged : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Damage;

	public Damaged(float3 damage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Damage = damage;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Damage);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.damageTypes)
		{
			ref float3 damage = ref m_Damage;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref damage);
		}
		else
		{
			ref float y = ref m_Damage.y;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref y);
		}
	}
}
