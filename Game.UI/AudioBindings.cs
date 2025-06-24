using System;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI;

public class AudioBindings : CompositeBinding
{
	private const string kGroup = "audio";

	private UISoundCollection m_SoundCollection;

	public AudioBindings()
	{
		m_SoundCollection = Resources.Load<UISoundCollection>("Audio/UI Sounds");
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string, float>("audio", "playSound", (Action<string, float>)PlayUISound, (IReader<string>)null, (IReader<float>)null));
	}

	private void PlayUISound(string soundName, float volume)
	{
		if ((Object)(object)m_SoundCollection != (Object)null)
		{
			m_SoundCollection.PlaySound(soundName, volume);
		}
	}
}
