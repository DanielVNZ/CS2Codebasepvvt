using Colossal.UI.Binding;
using Unity.Mathematics;

namespace Game.UI.InGame;

public struct Int2Property : IJsonWritable
{
	public string labelId;

	public int2 value;

	public string unit;

	public bool signed;

	public string icon;

	public string valueIcon;

	public void Write(IJsonWriter writer)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("Game.UI.Common.Number2Property");
		writer.PropertyName("labelId");
		writer.Write(labelId);
		writer.PropertyName("value");
		MathematicsWriters.Write(writer, value);
		writer.PropertyName("unit");
		writer.Write(unit);
		writer.PropertyName("signed");
		writer.Write(signed);
		writer.PropertyName("icon");
		writer.Write(icon);
		writer.PropertyName("valueIcon");
		writer.Write(valueIcon);
		writer.TypeEnd();
	}
}
