using Colossal.Mathematics;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Rendering;

public static class ZoneMeshHelpers
{
	public static Mesh CreateMesh(int2 resolution, int2 factor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		int num = (resolution.x + 1) * (resolution.y + 1) + resolution.x * resolution.y;
		int indexCount = GetIndexCount(resolution);
		Vector3[] array = (Vector3[])(object)new Vector3[num];
		Vector2[] array2 = (Vector2[])(object)new Vector2[num];
		int[] array3 = new int[indexCount];
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i <= resolution.y; i++)
		{
			float num4 = ((float)i - (float)resolution.y * 0.5f) * (float)factor.y * 8f;
			float num5 = (resolution.y - i) * factor.y;
			for (int j = 0; j <= resolution.x; j++)
			{
				float num6 = ((float)j - (float)resolution.x * 0.5f) * (float)factor.x * 8f;
				float num7 = (resolution.x - j) * factor.x;
				array[num2] = new Vector3(num6, 0f, num4);
				array2[num2] = new Vector2(num7, num5);
				num2++;
			}
		}
		for (int k = 0; k < resolution.y; k++)
		{
			float num8 = ((float)k + (0.5f - (float)resolution.y * 0.5f)) * (float)factor.y * 8f;
			float num9 = ((float)resolution.y - 0.5f - (float)k) * (float)factor.y;
			for (int l = 0; l < resolution.x; l++)
			{
				float num10 = ((float)l + (0.5f - (float)resolution.x * 0.5f)) * (float)factor.x * 8f;
				float num11 = ((float)resolution.x - 0.5f - (float)l) * (float)factor.x;
				int num12 = k * (resolution.x + 1) + l;
				int num13 = num12 + 1;
				int num14 = num12 + (resolution.x + 1);
				int num15 = num12 + (resolution.x + 2);
				array3[num3++] = num2;
				array3[num3++] = num13;
				array3[num3++] = num12;
				array3[num3++] = num2;
				array3[num3++] = num15;
				array3[num3++] = num13;
				array3[num3++] = num2;
				array3[num3++] = num14;
				array3[num3++] = num15;
				array3[num3++] = num2;
				array3[num3++] = num12;
				array3[num3++] = num14;
				array[num2] = new Vector3(num10, 0f, num8);
				array2[num2] = new Vector2(num11, num9);
				num2++;
			}
		}
		return new Mesh
		{
			name = $"Zone {resolution.x}x{resolution.y}",
			vertices = array,
			uv = array2,
			triangles = array3
		};
	}

	public static int GetIndexCount(int2 resolution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return resolution.x * resolution.y * 4 * 3;
	}

	public static Bounds3 GetBounds(int2 resolution)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		((float3)(ref val))._002Ector((float)resolution.x * 4f, 0f, (float)resolution.y * 4f);
		val.y = math.cmax(((float3)(ref val)).xz);
		return new Bounds3(-val, val);
	}
}
