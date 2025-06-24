using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public struct ColorGradient : IJsonWritable
{
	public GradientStop[] stops;

	public ColorGradient(GradientStop[] stops)
	{
		this.stops = stops;
	}

	public static explicit operator ColorGradient(Gradient gradient)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		List<GradientStop> list = new List<GradientStop>();
		GradientColorKey[] colorKeys = gradient.colorKeys;
		foreach (GradientColorKey val in colorKeys)
		{
			list.Add(new GradientStop(val.time, Color32.op_Implicit(val.color)));
		}
		return new ColorGradient(list.ToArray());
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("stops");
		int num = ((stops != null) ? stops.Length : 0);
		JsonWriterExtensions.ArrayBegin(writer, num);
		for (int i = 0; i < num; i++)
		{
			JsonWriterExtensions.Write<GradientStop>(writer, stops[i]);
		}
		writer.ArrayEnd();
		writer.TypeEnd();
	}
}
