using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.City;

public struct PlayerMoney : IComponentData, IQueryTypeParameter, ISerializable
{
	public const int kMaxMoney = 2000000000;

	private int m_Money;

	public bool m_Unlimited;

	public int money
	{
		get
		{
			if (!m_Unlimited)
			{
				return m_Money;
			}
			return 2000000000;
		}
	}

	public PlayerMoney(int amount)
	{
		m_Money = math.clamp(amount, -2000000000, 2000000000);
		m_Unlimited = false;
	}

	public void Add(int value)
	{
		m_Money = math.clamp(m_Money + value, -2000000000, 2000000000);
	}

	public void Subtract(int amount)
	{
		Add(-amount);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_Money;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		bool unlimited = m_Unlimited;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(unlimited);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref int reference = ref m_Money;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.unlimitedMoneyAndUnlockAllOptions)
		{
			ref bool unlimited = ref m_Unlimited;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref unlimited);
		}
	}
}
