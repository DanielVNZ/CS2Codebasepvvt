using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct CollectedCityServiceBudgetData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int3 m_Workplaces;

	public int m_Count;

	public int m_Export;

	public int m_BaseCost;

	public int m_Wages;

	public int m_FullWages;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ref int3 workplaces = ref m_Workplaces;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workplaces);
		ref int count = ref m_Count;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref count);
		ref int wages = ref m_Wages;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref wages);
		ref int fullWages = ref m_FullWages;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fullWages);
		ref int export = ref m_Export;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref export);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.netUpkeepCost)
		{
			ref int baseCost = ref m_BaseCost;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref baseCost);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int3 workplaces = m_Workplaces;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workplaces);
		int count = m_Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		int wages = m_Wages;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wages);
		int fullWages = m_FullWages;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fullWages);
		int export = m_Export;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(export);
		int baseCost = m_BaseCost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(baseCost);
	}
}
