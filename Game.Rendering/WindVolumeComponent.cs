using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering;

public class WindVolumeComponent : VolumeComponent
{
	public ClampedFloatParameter windGlobalStrengthScale = new ClampedFloatParameter(1f, 0f, 3f, false);

	public ClampedFloatParameter windGlobalStrengthScale2 = new ClampedFloatParameter(1f, 0f, 3f, false);

	public ClampedFloatParameter windDirection = new ClampedFloatParameter(65f, 0f, 360f, false);

	public ClampedFloatParameter windDirectionVariance = new ClampedFloatParameter(25f, 0f, 90f, false);

	public ClampedFloatParameter windDirectionVariancePeriod = new ClampedFloatParameter(15f, 0.01f, 20f, false);

	public ClampedFloatParameter windParameterInterpolationDuration = new ClampedFloatParameter(0.5f, 0.0001f, 5f, false);

	[Header("Grass Base")]
	public ClampedFloatParameter windBaseStrength = new ClampedFloatParameter(15f, 0f, 75f, false);

	public ClampedFloatParameter windBaseStrengthOffset = new ClampedFloatParameter(0.25f, 0f, 3f, false);

	public ClampedFloatParameter windBaseStrengthPhase = new ClampedFloatParameter(3f, 0f, 10f, false);

	public ClampedFloatParameter windBaseStrengthPhase2 = new ClampedFloatParameter(0f, 0f, 10f, false);

	public ClampedFloatParameter windBaseStrengthVariancePeriod = new ClampedFloatParameter(10f, 0.01f, 20f, false);

	[Header("Grass Gust")]
	public ClampedFloatParameter windGustStrength = new ClampedFloatParameter(25f, 0f, 75f, false);

	public ClampedFloatParameter windGustStrengthOffset = new ClampedFloatParameter(1f, 0f, 5f, false);

	public ClampedFloatParameter windGustStrengthPhase = new ClampedFloatParameter(3f, 0f, 10f, false);

	public ClampedFloatParameter windGustStrengthPhase2 = new ClampedFloatParameter(3f, 0f, 10f, false);

	public ClampedFloatParameter windGustStrengthVariancePeriod = new ClampedFloatParameter(2f, 0.01f, 10f, false);

	public ClampedFloatParameter windGustInnerCosScale = new ClampedFloatParameter(2f, 0f, 5f, false);

	public AnimationCurveParameter windGustStrengthControl = new AnimationCurveParameter(new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(10f, 1f)
	}), false);

	[Header("Grass Flutter")]
	public ClampedFloatParameter windFlutterStrength = new ClampedFloatParameter(0.4f, 0f, 10f, false);

	public ClampedFloatParameter windFlutterGustStrength = new ClampedFloatParameter(0.2f, 0f, 10f, false);

	public ClampedFloatParameter windFlutterGustStrengthOffset = new ClampedFloatParameter(50f, 0f, 75f, false);

	public ClampedFloatParameter windFlutterGustStrengthScale = new ClampedFloatParameter(75f, 0f, 75f, false);

	public ClampedFloatParameter windFlutterGustVariancePeriod = new ClampedFloatParameter(0.25f, 0.01f, 2f, false);

	[Header("Tree Base")]
	public ClampedFloatParameter windTreeBaseStrength = new ClampedFloatParameter(0.25f, 0f, 10f, false);

	public ClampedFloatParameter windTreeBaseStrengthOffset = new ClampedFloatParameter(1f, 0f, 5f, false);

	public ClampedFloatParameter windTreeBaseStrengthPhase = new ClampedFloatParameter(0.5f, 0f, 2f, false);

	public ClampedFloatParameter windTreeBaseStrengthPhase2 = new ClampedFloatParameter(0f, 0f, 2f, false);

	public ClampedFloatParameter windTreeBaseStrengthVariancePeriod = new ClampedFloatParameter(6f, 0.01f, 20f, false);

	[Header("Tree Gust")]
	public ClampedFloatParameter windTreeGustStrength = new ClampedFloatParameter(1f, 0f, 10f, false);

	public ClampedFloatParameter windTreeGustStrengthOffset = new ClampedFloatParameter(1f, 0f, 5f, false);

	public ClampedFloatParameter windTreeGustStrengthPhase = new ClampedFloatParameter(2f, 0f, 10f, false);

	public ClampedFloatParameter windTreeGustStrengthPhase2 = new ClampedFloatParameter(3f, 0f, 10f, false);

	public ClampedFloatParameter windTreeGustStrengthVariancePeriod = new ClampedFloatParameter(4f, 0.01f, 10f, false);

	public ClampedFloatParameter windTreeGustInnerCosScale = new ClampedFloatParameter(2f, 0f, 5f, false);

	public AnimationCurveParameter windTreeGustStrengthControl = new AnimationCurveParameter(new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(10f, 1f)
	}), false);

	[Header("Tree Flutter")]
	public ClampedFloatParameter windTreeFlutterStrength = new ClampedFloatParameter(0.1f, 0f, 5f, false);

	public ClampedFloatParameter windTreeFlutterGustStrength = new ClampedFloatParameter(0.5f, 0f, 5f, false);

	public ClampedFloatParameter windTreeFlutterGustStrengthOffset = new ClampedFloatParameter(12.5f, 0f, 75f, false);

	public ClampedFloatParameter windTreeFlutterGustStrengthScale = new ClampedFloatParameter(25f, 0f, 75f, false);

	public ClampedFloatParameter windTreeFlutterGustVariancePeriod = new ClampedFloatParameter(0.1f, 0.01f, 2f, false);
}
