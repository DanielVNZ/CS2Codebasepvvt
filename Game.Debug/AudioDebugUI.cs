using System.Collections.Generic;
using Colossal;
using Game.Audio;
using Game.Audio.Radio;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class AudioDebugUI
{
	[DebugTab("Audio", -4)]
	private static List<Widget> BuildAudioDebugUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		Radio radio = AudioManager.instance.radio;
		List<Widget> list = new List<Widget>();
		ObservableList<Widget> obj = new ObservableList<Widget>();
		obj.Add((Widget)new Value
		{
			displayName = "Current radio station",
			getter = () => radio?.currentChannel?.name
		});
		obj.Add((Widget)new Value
		{
			displayName = "Current program",
			getter = () => radio?.currentChannel?.currentProgram?.name
		});
		obj.Add((Widget)new Value
		{
			displayName = "Currently playing",
			getter = () => radio?.currentlyPlayingClipName ?? ""
		});
		obj.Add((Widget)new Value
		{
			displayName = "Source0 progress",
			getter = delegate
			{
				Radio radio2 = radio;
				return ((radio2 != null && radio2.GetActiveSource() == 0) ? "x " : "") + FormatUtils.FormatTimeMs(radio?.GetAudioSourceTimeElapsed(0) ?? 0.0) + "/" + FormatUtils.FormatTimeMs(radio?.GetAudioSourceDuration(0) ?? 0.0);
			}
		});
		obj.Add((Widget)new Value
		{
			displayName = "Source0 remaining",
			getter = delegate
			{
				Radio radio2 = radio;
				return ((radio2 != null && radio2.GetActiveSource() == 0) ? "x " : "") + FormatUtils.FormatTimeMs(radio?.GetAudioSourceTimeRemaining(0) ?? 0.0);
			}
		});
		obj.Add((Widget)new Value
		{
			displayName = "Source1 progress",
			getter = delegate
			{
				Radio radio2 = radio;
				return ((radio2 != null && radio2.GetActiveSource() == 1) ? "x " : "") + FormatUtils.FormatTimeMs(radio?.GetAudioSourceTimeElapsed(1) ?? 0.0) + "/" + FormatUtils.FormatTimeMs(radio?.GetAudioSourceDuration(1) ?? 0.0);
			}
		});
		obj.Add((Widget)new Value
		{
			displayName = "Source1 remaining",
			getter = delegate
			{
				Radio radio2 = radio;
				return ((radio2 != null && radio2.GetActiveSource() == 1) ? "x " : "") + FormatUtils.FormatTimeMs(radio?.GetAudioSourceTimeRemaining(1) ?? 0.0);
			}
		});
		obj.Add((Widget)new Value
		{
			displayName = "Next check",
			getter = () => radio?.nextTimeCheck - (double)Time.timeSinceLevelLoad
		});
		obj.Add((Widget)new Button
		{
			displayName = "Reload",
			action = delegate
			{
				radio?.Reload();
			}
		});
		list.Add((Widget)new Container("Radio", obj));
		ObservableList<Widget> obj2 = new ObservableList<Widget>();
		obj2.Add((Widget)new Value
		{
			displayName = "Loaded clips",
			getter = GetLoadedClips
		});
		obj2.Add((Widget)new Value
		{
			displayName = "Playing clips",
			getter = GetPlayingClips
		});
		list.Add((Widget)new Container("Clips", obj2));
		return list;
		static string GetLoadedClips()
		{
			AudioManager.AudioSourcePool.Stats(out var loadedSize, out var maxLoadedSize, out var loadedCount, out var _, out var _);
			return $"{FormatUtils.FormatBytes((long)loadedSize)} / {FormatUtils.FormatBytes((long)maxLoadedSize)} ({loadedCount})";
		}
		static string GetPlayingClips()
		{
			AudioManager.AudioSourcePool.Stats(out var _, out var _, out var _, out var playingSize, out var playingCount);
			return $"{FormatUtils.FormatBytes((long)playingSize)} ({playingCount})";
		}
	}
}
