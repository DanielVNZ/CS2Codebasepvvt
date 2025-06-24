using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PollutionData : IComponentData, IQueryTypeParameter, ICombineData<PollutionData>, ISerializable
{
	public float m_GroundPollution;

	public float m_AirPollution;

	public float m_NoisePollution;

	public bool m_ScaleWithRenters;

	public float GetValue(BuildingStatusType statusType)
	{
		return statusType switch
		{
			BuildingStatusType.GroundPollutionSource => m_GroundPollution, 
			BuildingStatusType.AirPollutionSource => m_AirPollution, 
			BuildingStatusType.NoisePollutionSource => m_NoisePollution, 
			_ => 0f, 
		};
	}

	public void AddArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public void Combine(PollutionData otherData)
	{
		m_GroundPollution += otherData.m_GroundPollution;
		m_AirPollution += otherData.m_AirPollution;
		m_NoisePollution += otherData.m_NoisePollution;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float groundPollution = m_GroundPollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groundPollution);
		float airPollution = m_AirPollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(airPollution);
		float noisePollution = m_NoisePollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noisePollution);
		bool scaleWithRenters = m_ScaleWithRenters;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(scaleWithRenters);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.pollutionMultiplierChange)
		{
			ref bool scaleWithRenters = ref m_ScaleWithRenters;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref scaleWithRenters);
		}
		else
		{
			m_ScaleWithRenters = true;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.pollutionFloatFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_GroundPollution = num;
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			m_AirPollution = num2;
			int num3 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			m_NoisePollution = num3;
		}
		else
		{
			ref float groundPollution = ref m_GroundPollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref groundPollution);
			ref float airPollution = ref m_AirPollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref airPollution);
			ref float noisePollution = ref m_NoisePollution;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref noisePollution);
		}
	}
}
