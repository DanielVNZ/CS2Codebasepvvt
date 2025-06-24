using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Settings;

public static class ScreenHelper
{
	private static List<DisplayInfo> m_DisplayInfos;

	public static ScreenResolution currentResolution
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			Resolution resolution = Screen.currentResolution;
			if (!Screen.fullScreen)
			{
				return new ScreenResolution
				{
					width = Screen.width,
					height = Screen.height,
					refreshRate = ((Resolution)(ref resolution)).refreshRateRatio
				};
			}
			return new ScreenResolution(resolution);
		}
	}

	private static ScreenResolution[] resolutions { get; set; }

	private static ScreenResolution[] simpleResolutions { get; set; }

	public static DisplayMode currentDisplayMode
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected I4, but got Unknown
			FullScreenMode fullScreenMode = Screen.fullScreenMode;
			return (int)fullScreenMode switch
			{
				0 => DisplayMode.Fullscreen, 
				1 => DisplayMode.FullscreenWindow, 
				3 => DisplayMode.Window, 
				_ => DisplayMode.Window, 
			};
		}
	}

	public static ScreenResolution[] GetAvailableResolutions(bool all)
	{
		if (!all)
		{
			return simpleResolutions;
		}
		return resolutions;
	}

	static ScreenHelper()
	{
		m_DisplayInfos = new List<DisplayInfo>();
		RebuildResolutions();
	}

	public static void RebuildResolutions()
	{
		resolutions = (from x in new List<ScreenResolution>(Screen.resolutions.Select((Resolution r) => new ScreenResolution(r)))
			orderby x.width descending, x.height descending, ((RefreshRate)(ref x.refreshRate)).value descending
			select x).ToArray();
		simpleResolutions = (from r in resolutions
			group r by (width: r.width, height: r.height, refreshRate: (int)Math.Round(((RefreshRate)(ref r.refreshRate)).value)) into g
			select g.Aggregate(g.First(), (ScreenResolution a, ScreenResolution b) => (!(b.refreshRateDelta < a.refreshRateDelta)) ? a : b)).ToArray();
	}

	public static ScreenResolution GetClosestAvailable(ScreenResolution sample, bool all)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		ScreenResolution screenResolution = default(ScreenResolution);
		ScreenResolution[] availableResolutions = GetAvailableResolutions(all);
		for (int i = 0; i < availableResolutions.Length; i++)
		{
			ScreenResolution screenResolution2 = availableResolutions[i];
			if (Math.Abs(screenResolution2.width - sample.width) + Math.Abs(screenResolution2.height - sample.height) < Math.Abs(screenResolution.width - sample.width) + Math.Abs(screenResolution.height - sample.height))
			{
				screenResolution = screenResolution2;
			}
			else if (screenResolution2.width == screenResolution.width && screenResolution2.height == screenResolution.height)
			{
				RefreshRate refreshRate = screenResolution2.refreshRate;
				if (Math.Abs(((RefreshRate)(ref refreshRate)).value - ((RefreshRate)(ref sample.refreshRate)).value) < Math.Abs(((RefreshRate)(ref screenResolution.refreshRate)).value - ((RefreshRate)(ref sample.refreshRate)).value))
				{
					screenResolution = screenResolution2;
				}
			}
		}
		ScreenResolution result = ((screenResolution.width > 0 && screenResolution.height > 0) ? screenResolution : sample);
		result.Sanitize();
		return result;
	}

	public static bool HideAdditionalResolutionOption()
	{
		return simpleResolutions.Length == resolutions.Length;
	}

	public static bool HasMultipleDisplay()
	{
		m_DisplayInfos.Clear();
		Screen.GetDisplayLayout(m_DisplayInfos);
		return m_DisplayInfos.Count > 1;
	}

	public static FullScreenMode GetFullscreenMode(DisplayMode displayMode)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return (FullScreenMode)(displayMode switch
		{
			DisplayMode.Fullscreen => 0, 
			DisplayMode.FullscreenWindow => 1, 
			DisplayMode.Window => 3, 
			_ => 3, 
		});
	}
}
