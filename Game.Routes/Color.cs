using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine;

namespace Game.Routes;

public struct Color : IComponentData, IQueryTypeParameter, ISerializable
{
	public Color32 m_Color;

	public Color(Color32 color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Color = color;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Color);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Color);
	}
}
