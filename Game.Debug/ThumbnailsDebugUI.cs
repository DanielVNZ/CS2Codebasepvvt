using System.Collections.Generic;
using Game.SceneFlow;
using Game.UI.Thumbnails;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class ThumbnailsDebugUI
{
	[DebugTab("Thumbnails", 0)]
	private static List<Widget> BuildThumbnailsDebugUI()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		ThumbnailCache tc = GameManager.instance?.thumbnailCache;
		if (tc != null)
		{
			((Widget)new Foldout()).displayName = "Thumbnails";
			return new List<Widget> { (Widget)new Button
			{
				displayName = "Refresh",
				action = delegate
				{
					tc.Refresh();
				}
			} };
		}
		return null;
	}
}
