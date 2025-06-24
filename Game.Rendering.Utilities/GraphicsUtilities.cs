using System.Collections.Generic;
using System.IO;
using Colossal;
using UnityEngine;

namespace Game.Rendering.Utilities;

public static class GraphicsUtilities
{
	public static void GenerateRandomWindowsTexture()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		byte[] collection = new byte[25]
		{
			255, 245, 235, 224, 214, 204, 194, 184, 173, 163,
			153, 143, 133, 122, 112, 102, 92, 82, 71, 61,
			51, 41, 31, 20, 0
		};
		int num = 125;
		Texture2D val = new Texture2D(num, num, (TextureFormat)3, false);
		Color32[] pixels = val.GetPixels32();
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 625; i++)
		{
			List<byte> list = new List<byte>(collection);
			Color32 val2 = Color32.op_Implicit(ColorUtils.NiceRandomColor());
			for (int j = 0; j < 5; j++)
			{
				for (int k = 0; k < 5; k++)
				{
					int index = ((k != 0 || j != 0) ? ((k != 4 || j != 4) ? Random.Range(0, list.Count - 1) : (list.Count - 1)) : 0);
					pixels[num2 + k + (num3 + j) * num] = val2;
					list.RemoveAt(index);
				}
			}
			num2 += 5;
			if (num2 == num)
			{
				num2 = 0;
				num3 += 5;
			}
		}
		val.SetPixels32(pixels);
		File.WriteAllBytes(Path.Combine(Application.dataPath, "Art/Resources/Textures/WindowsIdxMapGeneratedDebug.png"), ImageConversion.EncodeToPNG(val));
		Object.DestroyImmediate((Object)(object)val);
	}
}
