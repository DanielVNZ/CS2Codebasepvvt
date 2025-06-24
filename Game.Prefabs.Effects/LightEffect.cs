using System;
using System.Collections.Generic;
using Colossal;
using Game.Reflection;
using Game.Rendering;
using Game.UI;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class LightEffect : ComponentBase
{
	private class ColorTemperatureSliderFieldFactory : IFieldBuilderFactory
	{
		private IconValuePairs iconValuePairs;

		private string GetIconSource(float value)
		{
			return iconValuePairs.GetIconFromValue(value);
		}

		public FieldBuilder TryCreate(Type memberType, object[] attributes)
		{
			iconValuePairs = new IconValuePairs(new IconValuePairs.IconValuePair[7]
			{
				new IconValuePairs.IconValuePair("Media/Editor/Temperature01.svg", 2500f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature02.svg", 3500f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature03.svg", 4500f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature04.svg", 6000f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature05.svg", 7000f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature06.svg", 10000f),
				new IconValuePairs.IconValuePair("Media/Editor/Temperature07.svg", 20000f)
			});
			return (IValueAccessor accessor) => new GradientSliderField
			{
				accessor = new CastAccessor<float>(accessor),
				displayName = "Color temperature",
				gradient = (ColorGradient)ColorUtils.GetTemperatureGradient(1500f, 20000f, 8),
				min = 1500f,
				max = 20000f,
				iconSrc = () => GetIconSource((float)accessor.GetValue())
			};
		}
	}

	public LightType m_Type;

	public SpotLightShape m_SpotShape;

	public AreaLightShape m_AreaShape;

	public float m_Range = 25f;

	[FormerlySerializedAs("m_LuxIntensity")]
	[HideInInspector]
	public float m_Intensity = 10f;

	public LightIntensity m_LightIntensity;

	[FormerlySerializedAs("m_LuxDistance")]
	public float m_LuxAtDistance = 5f;

	[HideInInspector]
	public LightUnit m_LightUnit = LightUnit.Lux;

	public bool m_EnableSpotReflector = true;

	[Range(1f, 179f)]
	public float m_SpotAngle = 150f;

	[Range(0f, 100f)]
	public float m_InnerSpotPercentage = 50f;

	public float m_ShapeRadius = 0.025f;

	[Range(0.05f, 20f)]
	public float m_AspectRatio = 1f;

	public float m_ShapeWidth = 0.5f;

	public float m_ShapeHeight = 0.5f;

	public bool m_UseColorTemperature = true;

	public Color m_Color = new Color(1f, 0.86f, 0.65f, 1f);

	[CustomField(typeof(ColorTemperatureSliderFieldFactory))]
	public float m_ColorTemperature = 6570f;

	public Texture m_Cookie;

	public bool m_AffectDiffuse = true;

	public bool m_AffectSpecular = true;

	public bool m_ApplyRangeAttenuation = true;

	[Range(0f, 16f)]
	public float m_LightDimmer = 1f;

	public float m_LodBias;

	[HideInInspector]
	public float m_BarnDoorAngle;

	[HideInInspector]
	public float m_BarnDoorLength;

	public bool m_UseVolumetric = true;

	[Range(0f, 16f)]
	public float m_VolumetricDimmer = 1f;

	public float m_VolumetricFadeDistance = 10000f;

	public void RecalculateIntensity(LightUnit oldUnit, LightUnit newUnit)
	{
		m_Intensity = LightUtils.ConvertLightIntensity(oldUnit, newUnit, this, m_Intensity);
		m_LightIntensity.m_Intensity = m_Intensity;
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LightEffectData>());
		components.Add(ComponentType.ReadWrite<EffectColorData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		int num = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(m_Range)), m_LodBias);
		float num2 = RenderingUtils.CalculateDistanceFactor(num);
		float invDistanceFactor = 1f / num2;
		if (m_LightIntensity != null)
		{
			if (m_LightUnit != m_LightIntensity.m_LightUnit)
			{
				RecalculateIntensity(m_LightUnit, m_LightIntensity.m_LightUnit);
			}
			m_Intensity = m_LightIntensity.m_Intensity;
			m_LightUnit = m_LightIntensity.m_LightUnit;
		}
		else
		{
			m_LightIntensity = new LightIntensity
			{
				m_Intensity = m_Intensity,
				m_LightUnit = m_LightUnit
			};
		}
		LightEffectData lightEffectData = new LightEffectData
		{
			m_Range = m_Range,
			m_DistanceFactor = num2,
			m_InvDistanceFactor = invDistanceFactor,
			m_MinLod = num
		};
		((EntityManager)(ref entityManager)).SetComponentData<LightEffectData>(entity, lightEffectData);
		EffectColorData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EffectColorData>(entity);
		componentData.m_Color = ComputeLightFinalColor();
		((EntityManager)(ref entityManager)).SetComponentData<EffectColorData>(entity, componentData);
	}

	public HDLightTypeAndShape GetLightTypeAndShape()
	{
		return (HDLightTypeAndShape)(m_Type switch
		{
			LightType.Spot => m_SpotShape switch
			{
				SpotLightShape.Cone => 3, 
				SpotLightShape.Box => 1, 
				SpotLightShape.Pyramid => 2, 
				_ => throw new NotImplementedException($"Spot shape not implemented {m_SpotShape}"), 
			}, 
			LightType.Point => 0, 
			LightType.Area => m_AreaShape switch
			{
				AreaLightShape.Rectangle => 5, 
				AreaLightShape.Tube => 6, 
				_ => throw new NotImplementedException($"Area shape not implemented {m_AreaShape}"), 
			}, 
			_ => throw new NotImplementedException($"Light type not implemented {m_Type}"), 
		});
	}

	public Color GetEmissionColor()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Color val = ((Color)(ref m_Color)).linear * m_LightIntensity.m_Intensity;
		if (m_UseColorTemperature)
		{
			val *= Mathf.CorrelatedColorTemperatureToRGB(m_ColorTemperature);
		}
		return val * m_LightDimmer;
	}

	private float CalculateLightIntensityPunctual(float intensity)
	{
		switch (m_Type)
		{
		case LightType.Point:
			if (m_LightUnit == LightUnit.Candela)
			{
				return intensity;
			}
			return LightUtils.ConvertPointLightLumenToCandela(intensity);
		case LightType.Spot:
			if (m_LightUnit == LightUnit.Candela)
			{
				return intensity;
			}
			if (m_EnableSpotReflector)
			{
				if (m_SpotShape == SpotLightShape.Cone)
				{
					return LightUtils.ConvertSpotLightLumenToCandela(intensity, m_SpotAngle * ((float)Math.PI / 180f), exact: true);
				}
				if (m_SpotShape == SpotLightShape.Pyramid)
				{
					LightUtils.CalculateAnglesForPyramid(m_AspectRatio, m_SpotAngle * ((float)Math.PI / 180f), out var angleA, out var angleB);
					return LightUtils.ConvertFrustrumLightLumenToCandela(intensity, angleA, angleB);
				}
				return LightUtils.ConvertPointLightLumenToCandela(intensity);
			}
			return LightUtils.ConvertPointLightLumenToCandela(intensity);
		default:
			return intensity;
		}
	}

	private float ComputeLightIntensity()
	{
		if (m_LightUnit == LightUnit.Lumen)
		{
			if (m_Type == LightType.Spot || m_Type == LightType.Point)
			{
				return CalculateLightIntensityPunctual(m_LightIntensity.m_Intensity);
			}
			return LightUtils.ConvertAreaLightLumenToLuminance(m_AreaShape, m_LightIntensity.m_Intensity, m_ShapeWidth, m_ShapeHeight);
		}
		if (m_LightUnit == LightUnit.Ev100)
		{
			return LightUtils.ConvertEvToLuminance(m_LightIntensity.m_Intensity);
		}
		if ((m_Type == LightType.Spot || m_Type == LightType.Point) && m_LightUnit == LightUnit.Lux)
		{
			if (m_Type == LightType.Spot && m_SpotShape == SpotLightShape.Box)
			{
				return m_LightIntensity.m_Intensity;
			}
			return LightUtils.ConvertLuxToCandela(m_LightIntensity.m_Intensity, m_LuxAtDistance);
		}
		return m_LightIntensity.m_Intensity;
	}

	public Color ComputeLightFinalColor()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Color val = ((Color)(ref m_Color)).linear * ComputeLightIntensity();
		if (m_UseColorTemperature)
		{
			val *= Mathf.CorrelatedColorTemperatureToRGB(m_ColorTemperature);
		}
		return val * m_LightDimmer;
	}
}
