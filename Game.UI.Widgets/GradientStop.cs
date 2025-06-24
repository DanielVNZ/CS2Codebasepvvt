using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public struct GradientStop : IJsonWritable
{
	public float offset;

	public Color32 color;

	public GradientStop(float offset, Color32 color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.offset = offset;
		this.color = color;
	}

	public void Write(IJsonWriter writer)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("offset");
		writer.Write(offset);
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
		writer.TypeEnd();
	}
}
