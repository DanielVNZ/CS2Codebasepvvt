using UnityEngine.Rendering;

namespace Game.Debug;

public class IntInputField : TextField
{
	public override string ValidateValue(string value)
	{
		if (string.IsNullOrEmpty(value) || int.TryParse(value, out var _))
		{
			return value;
		}
		return ((Field<string>)this).getter();
	}
}
