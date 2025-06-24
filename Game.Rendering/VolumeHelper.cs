using System.Collections.Generic;
using Colossal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Rendering;

public static class VolumeHelper
{
	public const int kQualityVolumePriority = 100;

	public const int kGameVolumePriority = 50;

	public const int kOverrideVolumePriority = 2000;

	private const string kSectionName = "======Volumes======";

	private static List<Volume> m_Volumes = new List<Volume>();

	public static void Dispose()
	{
		for (int num = m_Volumes.Count - 1; num >= 0; num--)
		{
			DestroyVolume(m_Volumes[num]);
		}
	}

	private static VolumeProfile CreateVolumeProfile(string overrideName)
	{
		VolumeProfile obj = ScriptableObject.CreateInstance<VolumeProfile>();
		((Object)obj).name = overrideName + "Profile";
		((Object)obj).hideFlags = (HideFlags)52;
		return obj;
	}

	public static Volume CreateVolume(string name, int priority)
	{
		GameObject obj = OrderedGameObjectSpawner.Get("======Volumes======").Create(name);
		((Object)obj).hideFlags = (HideFlags)52;
		Volume component = obj.GetComponent<Volume>();
		component.priority = priority;
		component.sharedProfile = CreateVolumeProfile(name);
		m_Volumes.Add(component);
		return component;
	}

	public static void DestroyVolume(Volume volume)
	{
		m_Volumes.Remove(volume);
		if ((Object)(object)volume.sharedProfile != (Object)null)
		{
			CoreUtils.Destroy((Object)(object)volume.sharedProfile);
		}
		if ((Object)(object)volume != (Object)null)
		{
			CoreUtils.Destroy((Object)(object)((Component)volume).gameObject);
		}
	}

	public static void GetOrCreateVolumeComponent<PT>(Volume volume, ref PT component) where PT : VolumeComponent
	{
		GetOrCreateVolumeComponent(volume.profileRef, ref component);
	}

	public static void GetOrCreateVolumeComponent<PT>(VolumeProfile profile, ref PT component) where PT : VolumeComponent
	{
		if ((Object)(object)component == (Object)null && !profile.TryGet<PT>(ref component))
		{
			component = profile.Add<PT>(false);
			object obj = component;
			((VolumeParameter<int>)(object)((VolumeComponentWithQuality)(((obj is VolumeComponentWithQuality) ? obj : null)?)).quality).Override(3);
		}
	}
}
