using System;
using Colossal;
using UnityEngine;

namespace Game.Settings;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, Inherited = true)]
public class SettingsUIPlatformAttribute : Attribute
{
	private readonly Platform m_Platforms;

	private readonly bool m_DebugConditional;

	public bool IsPlatformSet(RuntimePlatform platform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return PlatformExt.IsPlatformSet(m_Platforms, platform, m_DebugConditional);
	}

	public SettingsUIPlatformAttribute(Platform platforms, bool debugConditional = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_Platforms = platforms;
		m_DebugConditional = debugConditional;
	}
}
