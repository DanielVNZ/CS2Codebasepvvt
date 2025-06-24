using System;
using Colossal.AssetPipeline.Native;
using Colossal.Serialization.Entities;

namespace Game.Serialization;

public static class SerializationUtils
{
	public static bool IsCompressed(this BufferFormat format)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)format != 1)
		{
			return (int)format == 2;
		}
		return true;
	}

	public static CompressionFormat BufferToCompressionFormat(BufferFormat format)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((int)format != 1)
		{
			if ((int)format == 2)
			{
				return (CompressionFormat)0;
			}
			throw new FormatException($"Invalid format {format}");
		}
		return (CompressionFormat)1;
	}
}
