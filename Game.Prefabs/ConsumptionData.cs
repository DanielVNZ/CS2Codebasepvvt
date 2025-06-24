using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct ConsumptionData : IComponentData, IQueryTypeParameter, ICombineData<ConsumptionData>, ISerializable
{
	public int m_Upkeep;

	public float m_ElectricityConsumption;

	public float m_WaterConsumption;

	public float m_GarbageAccumulation;

	public float m_TelecomNeed;

	public void AddArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (m_ElectricityConsumption > 0f)
		{
			components.Add(ComponentType.ReadWrite<ElectricityConsumer>());
		}
		if (m_WaterConsumption > 0f)
		{
			components.Add(ComponentType.ReadWrite<WaterConsumer>());
		}
		if (m_GarbageAccumulation > 0f)
		{
			components.Add(ComponentType.ReadWrite<GarbageProducer>());
		}
		if (m_TelecomNeed > 0f)
		{
			components.Add(ComponentType.ReadWrite<TelecomConsumer>());
		}
	}

	public void Combine(ConsumptionData otherData)
	{
		m_Upkeep += otherData.m_Upkeep;
		m_ElectricityConsumption += otherData.m_ElectricityConsumption;
		m_WaterConsumption += otherData.m_WaterConsumption;
		m_GarbageAccumulation += otherData.m_GarbageAccumulation;
		m_TelecomNeed = math.max(m_TelecomNeed, otherData.m_TelecomNeed);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int upkeep = m_Upkeep;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(upkeep);
		float electricityConsumption = m_ElectricityConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(electricityConsumption);
		float waterConsumption = m_WaterConsumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(waterConsumption);
		float garbageAccumulation = m_GarbageAccumulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbageAccumulation);
		float telecomNeed = m_TelecomNeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(telecomNeed);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ref int upkeep = ref m_Upkeep;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref upkeep);
		ref float electricityConsumption = ref m_ElectricityConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref electricityConsumption);
		ref float waterConsumption = ref m_WaterConsumption;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref waterConsumption);
		ref float garbageAccumulation = ref m_GarbageAccumulation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbageAccumulation);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.telecomNeed)
		{
			ref float telecomNeed = ref m_TelecomNeed;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref telecomNeed);
		}
	}
}
