using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct LotData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_MaxRadius;

	public Color32 m_RangeColor;

	public bool m_OnWater;

	public bool m_AllowOverlap;

	public LotData(float maxRadius, Color32 rangeColor, bool onWater, bool allowOverlap)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_MaxRadius = maxRadius;
		m_RangeColor = rangeColor;
		m_OnWater = onWater;
		m_AllowOverlap = allowOverlap;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float maxRadius = m_MaxRadius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxRadius);
		bool onWater = m_OnWater;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(onWater);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float maxRadius = ref m_MaxRadius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxRadius);
		ref bool onWater = ref m_OnWater;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref onWater);
	}
}
