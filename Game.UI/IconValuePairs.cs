namespace Game.UI;

public class IconValuePairs
{
	public struct IconValuePair
	{
		public string icon { get; }

		public float stop { get; }

		public IconValuePair(string icon, float stop)
		{
			this.icon = icon;
			this.stop = stop;
		}
	}

	private IconValuePair[] iconValuePairArray;

	public IconValuePairs(IconValuePair[] iconValuePairArray)
	{
		this.iconValuePairArray = iconValuePairArray;
	}

	public string GetIconFromValue(float value)
	{
		if (iconValuePairArray == null || iconValuePairArray.Length == 0)
		{
			return string.Empty;
		}
		IconValuePair[] array = iconValuePairArray;
		for (int i = 0; i < array.Length; i++)
		{
			IconValuePair iconValuePair = array[i];
			if (value <= iconValuePair.stop)
			{
				return iconValuePair.icon;
			}
		}
		IconValuePair[] array2 = iconValuePairArray;
		return array2[array2.Length - 1].icon;
	}
}
