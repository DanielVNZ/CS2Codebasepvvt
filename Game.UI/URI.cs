using System.Text.RegularExpressions;
using Unity.Entities;

namespace Game.UI;

public static class URI
{
	private static readonly Regex kEntityPattern = new Regex("^entity:\\/\\/(\\d+)\\/(\\d+)$", RegexOptions.Compiled);

	private static readonly Regex kInfoviewPattern = new Regex("^infoview:\\/\\/(\\d+)\\/(\\d+)$", RegexOptions.Compiled);

	public static string FromEntity(Entity entity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return $"entity://{entity.Index}/{entity.Version}";
	}

	public static bool TryParseEntity(string input, out Entity entity)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Match match = kEntityPattern.Match(input);
		if (match.Success)
		{
			entity = new Entity
			{
				Index = int.Parse(match.Groups[1].Value),
				Version = int.Parse(match.Groups[2].Value)
			};
			return true;
		}
		entity = Entity.Null;
		return false;
	}

	public static string FromInfoView(Entity entity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return $"infoview://{entity.Index}/{entity.Version}";
	}

	public static bool TryParseInfoview(string input, out Entity entity)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Match match = kInfoviewPattern.Match(input);
		if (match.Success)
		{
			entity = new Entity
			{
				Index = int.Parse(match.Groups[1].Value),
				Version = int.Parse(match.Groups[2].Value)
			};
			return true;
		}
		entity = Entity.Null;
		return false;
	}
}
