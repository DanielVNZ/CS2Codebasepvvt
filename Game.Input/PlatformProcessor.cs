using Colossal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input;

public abstract class PlatformProcessor<TValue> : InputProcessor<TValue> where TValue : struct
{
	public Platform m_Platform = (Platform)255;

	public ProcessorDeviceType m_DeviceType;

	private bool? m_NeedProcess;

	protected bool needProcess
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			bool valueOrDefault = m_NeedProcess == true;
			if (!m_NeedProcess.HasValue)
			{
				valueOrDefault = PlatformExt.IsPlatformSet(m_Platform, Application.platform, false);
				m_NeedProcess = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}
}
