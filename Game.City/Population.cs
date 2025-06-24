using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct Population : IComponentData, IQueryTypeParameter, IDefaultSerializable, ISerializable
{
	public int m_Population;

	public int m_PopulationWithMoveIn;

	public int m_AverageHappiness;

	public int m_AverageHealth;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref int population = ref m_Population;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref population);
		ref int populationWithMoveIn = ref m_PopulationWithMoveIn;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref populationWithMoveIn);
		ref int averageHappiness = ref m_AverageHappiness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageHappiness);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.averageHealth)
		{
			ref int averageHealth = ref m_AverageHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageHealth);
		}
		else
		{
			m_AverageHealth = 50;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int population = m_Population;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(population);
		int populationWithMoveIn = m_PopulationWithMoveIn;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(populationWithMoveIn);
		int averageHappiness = m_AverageHappiness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageHappiness);
		int averageHealth = m_AverageHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageHealth);
	}

	public void SetDefaults(Context context)
	{
		m_Population = 0;
		m_PopulationWithMoveIn = 0;
		m_AverageHappiness = 50;
		m_AverageHealth = 50;
	}
}
