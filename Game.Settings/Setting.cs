using System;
using System.Reflection;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Colossal.Reflection;
using Game.SceneFlow;
using Game.UI.Menu;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

public abstract class Setting : IEquatable<Setting>
{
	protected static ILog log = LogManager.GetLogger("SceneFlow");

	protected static SharedSettings settings => GameManager.instance?.settings;

	[Exclude]
	protected internal virtual bool builtIn => true;

	public event OnSettingsAppliedHandler onSettingsApplied;

	public bool Equals(Setting obj)
	{
		return Equals((object)obj);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		Type type = obj.GetType();
		if (!type.IsAssignableFrom(GetType()))
		{
			return false;
		}
		PropertyInfo property = type.GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
		if (property != null && !(bool)property.GetValue(this) && object.Equals(property.GetValue(this), property.GetValue(obj)))
		{
			return true;
		}
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (ReflectionUtils.GetAttribute<IgnoreEqualsAttribute>(propertyInfo.GetCustomAttributes(inherit: false)) == null && propertyInfo.CanRead && !object.Equals(propertyInfo.GetValue(this), propertyInfo.GetValue(obj)))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			num = (num * 937) ^ propertyInfo.GetValue(this).GetHashCode();
		}
		return num;
	}

	protected bool TryGetGameplayCameraController(ref CameraController controller)
	{
		if ((Object)(object)controller != (Object)null)
		{
			return true;
		}
		GameObject val = GameObject.FindGameObjectWithTag("GameplayCamera");
		if ((Object)(object)val != (Object)null)
		{
			controller = val.GetComponent<CameraController>();
			return true;
		}
		controller = null;
		return false;
	}

	protected bool TryGetGameplayCamera(ref HDAdditionalCameraData cameraData)
	{
		if ((Object)(object)cameraData != (Object)null)
		{
			return true;
		}
		Camera main = Camera.main;
		if ((Object)(object)main != (Object)null)
		{
			cameraData = ((Component)main).GetComponent<HDAdditionalCameraData>();
			return true;
		}
		cameraData = null;
		return false;
	}

	protected bool TryGetGameplayCamera(ref Camera camera)
	{
		if ((Object)(object)camera != (Object)null)
		{
			return true;
		}
		camera = Camera.main;
		if ((Object)(object)camera != (Object)null)
		{
			return true;
		}
		return false;
	}

	protected bool TryGetSunLight(ref Light sunLight)
	{
		if ((Object)(object)sunLight != (Object)null)
		{
			return true;
		}
		GameObject val = GameObject.FindGameObjectWithTag("SunLight");
		if ((Object)(object)val != (Object)null)
		{
			sunLight = val.GetComponent<Light>();
			return true;
		}
		sunLight = null;
		return false;
	}

	protected bool TryGetSunLightData(ref HDAdditionalLightData sunLightData)
	{
		if ((Object)(object)sunLightData != (Object)null)
		{
			return true;
		}
		GameObject val = GameObject.FindGameObjectWithTag("SunLight");
		if ((Object)(object)val != (Object)null)
		{
			sunLightData = val.GetComponent<HDAdditionalLightData>();
			return true;
		}
		sunLightData = null;
		return false;
	}

	public async void ApplyAndSave()
	{
		Apply();
		await AssetDatabase.global.SaveSettings();
	}

	public virtual void Apply()
	{
		log.VerboseFormat("Applying settings for {0}", (object)GetType());
		this.onSettingsApplied?.Invoke(this);
	}

	public abstract void SetDefaults();

	public virtual AutomaticSettings.SettingPageData GetPageData(string id, bool addPrefix)
	{
		return AutomaticSettings.FillSettingsPage(this, id, addPrefix);
	}

	internal void RegisterInOptionsUI(string name, bool addPrefix = false)
	{
		RegisterInOptionsUI(this, name, addPrefix);
	}

	internal static bool RegisterInOptionsUI(Setting instance, string name, bool addPrefix)
	{
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		OptionsUISystem optionsUISystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>() : null);
		if (optionsUISystem != null)
		{
			optionsUISystem.RegisterSetting(instance, name, addPrefix);
			return true;
		}
		return false;
	}

	internal static bool UnregisterInOptionsUI(string name)
	{
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		OptionsUISystem optionsUISystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>() : null);
		if (optionsUISystem != null)
		{
			optionsUISystem.UnregisterSettings(name);
			return true;
		}
		return false;
	}
}
