using System;
using System.Collections.Generic;
using Colossal.Localization;
using Colossal.UI.Binding;
using Game.Settings;

namespace Game.UI.Localization;

public class LocalizationBindings : CompositeBinding, IDisposable
{
	public enum DebugMode
	{
		None,
		Id,
		Fallback
	}

	private const string kGroup = "l10n";

	private readonly LocalizationManager m_LocalizationManager;

	private readonly GetterValueBinding<string[]> m_LocalesBinding;

	private readonly ValueBinding<int> m_DebugModeBinding;

	private readonly EventBinding m_ActiveDictionaryChangedBinding;

	private readonly RawMapBinding<string> m_IndexCountsBinding;

	public DebugMode debugMode
	{
		get
		{
			return (DebugMode)m_DebugModeBinding.value;
		}
		set
		{
			m_DebugModeBinding.Update((int)value);
		}
	}

	public LocalizationBindings(LocalizationManager localizationManager)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007c: Expected O, but got Unknown
		m_LocalizationManager = localizationManager;
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_LocalesBinding = new GetterValueBinding<string[]>("l10n", "locales", (Func<string[]>)(() => m_LocalizationManager.GetSupportedLocales()), (IWriter<string[]>)(object)new ArrayWriter<string>((IWriter<string>)new StringWriter(), false), (EqualityComparer<string[]>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_DebugModeBinding = new ValueBinding<int>("l10n", "debugMode", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		EventBinding val = new EventBinding("l10n", "activeDictionaryChanged");
		EventBinding val2 = val;
		m_ActiveDictionaryChangedBinding = val;
		((CompositeBinding)this).AddBinding((IBinding)(object)val2);
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_IndexCountsBinding = new RawMapBinding<string>("l10n", "indexCounts", (Action<IJsonWriter, string>)BindIndexCounts, (IReader<string>)null, (IWriter<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string>("l10n", "selectLocale", (Action<string>)SelectLocale, (IReader<string>)null));
		m_LocalizationManager.onSupportedLocalesChanged += OnSupportedLocalesChanged;
		m_LocalizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
	}

	public void Dispose()
	{
		m_LocalizationManager.onSupportedLocalesChanged -= OnSupportedLocalesChanged;
		m_LocalizationManager.onActiveDictionaryChanged -= OnActiveDictionaryChanged;
	}

	private void OnSupportedLocalesChanged()
	{
		m_LocalesBinding.Update();
	}

	private void OnActiveDictionaryChanged()
	{
		m_ActiveDictionaryChangedBinding.Trigger();
		((MapBindingBase<string>)(object)m_IndexCountsBinding).UpdateAll();
	}

	private void BindIndexCounts(IJsonWriter binder, string key)
	{
		binder.Write(m_LocalizationManager.activeDictionary.indexCounts.TryGetValue(key, out var value) ? value : 0);
	}

	private void SelectLocale(string localeID)
	{
		LocalizationManager localizationManager = m_LocalizationManager;
		if (localizationManager != null)
		{
			localizationManager.SetActiveLocale(localeID);
		}
		InterfaceSettings interfaceSettings = SharedSettings.instance?.userInterface;
		if (interfaceSettings != null)
		{
			interfaceSettings.locale = localeID;
		}
	}
}
