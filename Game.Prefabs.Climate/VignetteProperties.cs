using System;
using Game.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Prefabs.Climate;

[ComponentMenu("Weather/", new Type[] { typeof(WeatherPrefab) })]
public class VignetteProperties : OverrideablePropertiesComponent
{
	public ColorParameter m_Color = new ColorParameter(Color.black, false, false, true, false);

	public Vector2Parameter m_Center = new Vector2Parameter(new Vector2(0.5f, 0.5f), false);

	public ClampedFloatParameter m_Intensity = new ClampedFloatParameter(0f, 0f, 1f, false);

	public ClampedFloatParameter m_Smoothness = new ClampedFloatParameter(0.2f, 0.01f, 1f, false);

	public ClampedFloatParameter m_Roundness = new ClampedFloatParameter(1f, 0f, 1f, false);

	public BoolParameter m_Rounded = new BoolParameter(false, false);

	protected override void OnBindVolumeProperties(Volume volume)
	{
		Vignette component = null;
		VolumeHelper.GetOrCreateVolumeComponent<Vignette>(volume, ref component);
		m_Color = component.color;
		m_Center = component.center;
		m_Intensity = component.intensity;
		m_Smoothness = component.smoothness;
		m_Roundness = component.roundness;
		m_Rounded = component.rounded;
		((VolumeParameter<VignetteMode>)(object)component.mode).Override((VignetteMode)0);
	}
}
