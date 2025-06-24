using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Atmosphere;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.SceneFlow;
using Game.Settings;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class PlanetarySystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct LightData
	{
		private readonly string m_Tag;

		public Transform transform { get; private set; }

		public Light light { get; private set; }

		public HDAdditionalLightData additionalData { get; private set; }

		public float initialIntensity { get; private set; }

		public bool isValid
		{
			get
			{
				if ((Object)(object)transform == (Object)null)
				{
					GameObject val = GameObject.FindGameObjectWithTag(m_Tag);
					if ((Object)(object)val != (Object)null)
					{
						transform = val.transform;
						light = val.GetComponent<Light>();
						additionalData = val.GetComponent<HDAdditionalLightData>();
						initialIntensity = additionalData.intensity;
					}
				}
				return (Object)(object)transform != (Object)null;
			}
		}

		public LightData(string tag)
		{
			m_Tag = tag;
			transform = null;
			light = null;
			additionalData = null;
			initialIntensity = 0f;
		}
	}

	private static class ShaderIDs
	{
		public static readonly int _Camera2World = Shader.PropertyToID("_Camera2World");

		public static readonly int _CameraData = Shader.PropertyToID("_CameraData");

		public static readonly int _SunDirection = Shader.PropertyToID("_SunDirection");

		public static readonly int _Luminance = Shader.PropertyToID("_Luminance");

		public static readonly int _Direction = Shader.PropertyToID("_Direction");

		public static readonly int _Tangent = Shader.PropertyToID("_Tangent");

		public static readonly int _BiTangent = Shader.PropertyToID("_BiTangent");

		public static readonly int _Albedo = Shader.PropertyToID("_Albedo");

		public static readonly int _OrenNayarCoefficients = Shader.PropertyToID("_OrenNayarCoefficients");

		public static readonly int _TexDiffuse = Shader.PropertyToID("_TexDiffuse");

		public static readonly int _TexNormal = Shader.PropertyToID("_TexNormal");

		public static readonly int _Corners = Shader.PropertyToID("_Corners");
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private static readonly float kDefaultLatitude = 41.9028f;

	private static readonly float kDefaultLongitude = 12.4964f;

	private SunMoonData m_SunMoonData;

	private TimeSystem m_TimeSystem;

	private LightData m_SunLight;

	private LightData m_MoonLight;

	private LightData m_NightLight;

	private RenderingSystem m_RenderingSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private PrefabSystem m_PrefabSystem;

	private const float kDaysInYear = 365f;

	private const float kInvDaysInYear = 0.002739726f;

	private const float kHoursInDay = 24f;

	private const float kInvHoursInDay = 1f / 24f;

	private const float kSecsInMin = 60f;

	private const float kInvSecsInMin = 1f / 60f;

	private const float kSecsInHour = 3600f;

	private const float kInvSecsInHour = 0.00027777778f;

	private const float kLunarCyclesPerYear = 12f;

	private const float kInvLunarCyclesPerYear = 1f / 12f;

	private int m_Year = 2020;

	private int m_Day = 127;

	private int m_Hour = 12;

	private int m_Minute;

	private float m_Second;

	private float m_Latitude = kDefaultLatitude;

	private float m_Longitude = kDefaultLongitude;

	private int m_NumberOfLunarCyclesPerYear = 1;

	private RenderTexture m_MoonTexture;

	private Material m_MoonMaterial;

	private int m_ClearPass;

	private int m_LitPass;

	private Vector2 m_OrenNayarCoefficients;

	private float m_SurfaceRoughness;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1383560598_0;

	private EntityQuery __query_1383560598_1;

	private EntityQuery __query_1383560598_2;

	public LightData SunLight => m_SunLight;

	public LightData MoonLight => m_MoonLight;

	public LightData NightLight => m_NightLight;

	public bool overrideTime { get; set; }

	public float latitude
	{
		get
		{
			return m_Latitude;
		}
		set
		{
			m_Latitude = math.clamp(value, -90f, 90f);
		}
	}

	public float longitude
	{
		get
		{
			return m_Longitude;
		}
		set
		{
			m_Longitude = math.clamp(value, -180f, 180f);
		}
	}

	public float debugTimeMultiplier { get; set; } = 1f;

	public int year
	{
		get
		{
			return m_Year;
		}
		set
		{
			m_Year = value;
		}
	}

	public int day
	{
		get
		{
			return m_Day;
		}
		set
		{
			m_Day = value;
		}
	}

	public int hour
	{
		get
		{
			return m_Hour;
		}
		set
		{
			m_Hour = value;
		}
	}

	public int minute
	{
		get
		{
			return m_Minute;
		}
		set
		{
			m_Minute = value;
		}
	}

	public float second
	{
		get
		{
			return m_Second;
		}
		set
		{
			m_Second = value;
		}
	}

	public float time
	{
		get
		{
			return (float)hour + (float)minute * (1f / 60f) + second * 0.00027777778f;
		}
		set
		{
			hour = Mathf.FloorToInt(value);
			value -= (float)hour;
			minute = Mathf.FloorToInt(value * 60f);
			value -= (float)minute * (1f / 60f);
			second = value * 3600f;
		}
	}

	public float dayOfYear
	{
		get
		{
			return (float)day + normalizedTime;
		}
		set
		{
			day = Mathf.FloorToInt(value);
			value -= (float)day;
			normalizedTime = value;
		}
	}

	public float normalizedDayOfYear
	{
		get
		{
			return (dayOfYear - 1f) * 0.002739726f;
		}
		set
		{
			dayOfYear = value * 365f + 1f;
		}
	}

	public float normalizedTime
	{
		get
		{
			return time * (1f / 24f);
		}
		set
		{
			time = value * 24f;
		}
	}

	public int numberOfLunarCyclesPerYear
	{
		get
		{
			return m_NumberOfLunarCyclesPerYear;
		}
		set
		{
			m_NumberOfLunarCyclesPerYear = math.max(0, value);
		}
	}

	public int moonDay => Mathf.FloorToInt((float)day * (1f / 12f) * (float)numberOfLunarCyclesPerYear);

	public float moonSurfaceRoughness
	{
		get
		{
			return m_SurfaceRoughness;
		}
		set
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			m_SurfaceRoughness = Mathf.Clamp01(value);
			float num = (float)Math.PI / 2f * m_SurfaceRoughness;
			float num2 = num * num;
			m_OrenNayarCoefficients = new Vector2(1f - 0.5f * num2 / (num2 + 0.33f), 0.45f * num2 / (num2 + 0.09f));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_MoonTexture = new RenderTexture(512, 512, 0, (RenderTextureFormat)0)
		{
			name = "MoonTexture",
			hideFlags = (HideFlags)52
		};
		m_MoonMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Satellites"));
		m_ClearPass = m_MoonMaterial.FindPass("Clear");
		m_LitPass = m_MoonMaterial.FindPass("LitSatellite");
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_SunLight = new LightData("SunLight");
		m_MoonLight = new LightData("MoonLight");
		m_NightLight = new LightData("NightLight");
		m_SunMoonData = default(SunMoonData);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		CoreUtils.Destroy((Object)(object)m_MoonTexture);
		CoreUtils.Destroy((Object)(object)m_MoonMaterial);
	}

	private static DateTime CreateDateTime(int year, int day, int hour, int minute, float second, float longitude)
	{
		return new DateTime(0L, DateTimeKind.Utc).AddYears(year - 1).AddDays(day - 1).AddHours(hour)
			.AddMinutes(minute)
			.AddSeconds(second)
			.AddSeconds(-43200f * longitude / 180f);
	}

	private void UpdateTime(float date, float time, int year)
	{
		normalizedDayOfYear = date;
		normalizedTime = time;
		m_Year = year;
	}

	public TopocentricCoordinates GetSunPosition(DateTime date, double latitude, double longitude)
	{
		return m_SunMoonData.GetSunPosition(date, latitude, longitude);
	}

	public MoonCoordinate GetMoonPosition(DateTime date, double latitude, double longitude)
	{
		return m_SunMoonData.GetMoonPosition(date, latitude, longitude);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		float num = latitude;
		float num2 = longitude;
		TimeSettingsData settings2 = default(TimeSettingsData);
		TimeData data2 = default(TimeData);
		if (GameManager.instance.gameMode == GameMode.Game)
		{
			bool flag = overrideTime;
			GameplaySettings gameplaySettings = SharedSettings.instance?.gameplay;
			if (gameplaySettings != null && !overrideTime && !gameplaySettings.dayNightVisual)
			{
				num = 51.2277f;
				num2 = 6.7735f;
				time = 14.5f;
				day = 177;
				year = 2020;
				flag = true;
			}
			TimeSettingsData settings = default(TimeSettingsData);
			TimeData data = default(TimeData);
			if (!flag && ((EntityQuery)(ref __query_1383560598_0)).TryGetSingleton<TimeSettingsData>(ref settings) && ((EntityQuery)(ref __query_1383560598_1)).TryGetSingleton<TimeData>(ref data))
			{
				double renderingFrame = (float)(m_RenderingSystem.frameIndex - data.m_FirstFrame) + m_RenderingSystem.frameTime;
				float timeOfYear = m_TimeSystem.GetTimeOfYear(settings, data, renderingFrame);
				float num3 = m_TimeSystem.GetTimeOfDay(settings, data, renderingFrame) * debugTimeMultiplier;
				int num4 = m_TimeSystem.GetYear(settings, data);
				UpdateTime(timeOfYear, num3, num4);
			}
		}
		else if (GameManager.instance.gameMode == GameMode.Editor && !overrideTime && ((EntityQuery)(ref __query_1383560598_0)).TryGetSingleton<TimeSettingsData>(ref settings2) && ((EntityQuery)(ref __query_1383560598_1)).TryGetSingleton<TimeData>(ref data2))
		{
			double renderingFrame2 = (float)(m_RenderingSystem.frameIndex - data2.m_FirstFrame) + m_RenderingSystem.frameTime;
			float timeOfYear2 = m_TimeSystem.GetTimeOfYear(settings2, data2, renderingFrame2);
			float num5 = m_TimeSystem.GetTimeOfDay(settings2, data2, renderingFrame2) * debugTimeMultiplier;
			int num6 = m_TimeSystem.GetYear(settings2, data2);
			UpdateTime(timeOfYear2, num5, num6);
		}
		if (m_SunLight.isValid)
		{
			JulianDateTime date = CreateDateTime(year, day, hour, minute, second, num2);
			float planetTime;
			float3 val = m_SunMoonData.GetSunPosition(date, num, num2).ToLocalCoordinates(out planetTime);
			float4x4 val2 = float4x4.LookAt(val, float3.zero, new float3(0f, 1f, 0f));
			float3 val3 = math.rotate(val2, new float3(0f, 0f, 1f));
			m_SunLight.transform.position = float3.op_Implicit(val);
			m_SunLight.transform.rotation = quaternion.op_Implicit(new quaternion(val2));
			m_SunLight.additionalData.intensity = m_SunLight.initialIntensity * math.smoothstep(0f, 0.3f, math.abs(math.min(0f, val3.y)));
		}
		if (m_MoonLight.isValid)
		{
			JulianDateTime date2 = CreateDateTime(year, moonDay, hour, minute, second, num2);
			MoonCoordinate moonPosition = m_SunMoonData.GetMoonPosition(date2, num, num2);
			float planetTime2;
			float3 val4 = moonPosition.topoCoords.ToLocalCoordinates(out planetTime2);
			float4x4 val5 = float4x4.LookAt(val4, float3.zero, new float3(0f, 1f, 0f));
			math.rotate(val5, new float3(0f, 0f, 1f));
			m_MoonLight.transform.position = float3.op_Implicit(val4);
			m_MoonLight.transform.rotation = quaternion.op_Implicit(new quaternion(val5));
			m_MoonLight.additionalData.distance = (float)moonPosition.distance;
			if (m_SunLight.isValid)
			{
				RenderMoon();
			}
		}
		if (m_NightLight.isValid && m_MoonLight.isValid)
		{
			float3 val6 = float3.op_Implicit(m_MoonLight.transform.position);
			val6.y = math.max(val6.y, 0.3f);
			float4x4 val7 = float4x4.LookAt(val6, float3.zero, new float3(0f, 1f, 0f));
			m_NightLight.transform.position = float3.op_Implicit(val6);
			m_NightLight.transform.rotation = quaternion.op_Implicit(new quaternion(val7));
		}
	}

	private void RenderMoon()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_MoonTexture != (Object)null && (Object)(object)m_MoonMaterial != (Object)null && (Object)(object)m_CameraUpdateSystem.activeCamera != (Object)null)
		{
			moonSurfaceRoughness = 0.8f;
			Camera activeCamera = m_CameraUpdateSystem.activeCamera;
			float num = Mathf.Tan(0.5f * activeCamera.fieldOfView * (float)Math.PI / 180f);
			Vector4 val = default(Vector4);
			((Vector4)(ref val))._002Ector(activeCamera.aspect * num, num, activeCamera.nearClipPlane, activeCamera.farClipPlane);
			m_MoonMaterial.SetMatrix(ShaderIDs._Camera2World, activeCamera.cameraToWorldMatrix);
			m_MoonMaterial.SetVector(ShaderIDs._CameraData, val);
			m_MoonMaterial.SetVector(ShaderIDs._SunDirection, Vector4.op_Implicit(m_SunLight.transform.forward));
			m_MoonMaterial.SetVector(ShaderIDs._Direction, Vector4.op_Implicit(m_MoonLight.transform.forward));
			m_MoonMaterial.SetVector(ShaderIDs._Tangent, Vector4.op_Implicit(m_MoonLight.transform.right));
			m_MoonMaterial.SetVector(ShaderIDs._BiTangent, Vector4.op_Implicit(m_MoonLight.transform.up));
			m_MoonMaterial.SetColor(ShaderIDs._Albedo, new Color(1f, 1f, 1f, 1f));
			m_MoonMaterial.SetVector(ShaderIDs._Corners, new Vector4(0f, 0f, 1f, 1f));
			m_MoonMaterial.SetVector(ShaderIDs._OrenNayarCoefficients, Vector4.op_Implicit(m_OrenNayarCoefficients));
			m_MoonMaterial.SetFloat(ShaderIDs._Luminance, 10f);
			AtmosphereData atmosphereData = default(AtmosphereData);
			if (((EntityQuery)(ref __query_1383560598_2)).TryGetSingleton<AtmosphereData>(ref atmosphereData) && m_PrefabSystem.TryGetPrefab<AtmospherePrefab>(atmosphereData.m_AtmospherePrefab, out var prefab))
			{
				m_MoonMaterial.SetTexture(ShaderIDs._TexDiffuse, (Texture)(object)prefab.m_MoonAlbedo);
				m_MoonMaterial.SetTexture(ShaderIDs._TexNormal, (Texture)(object)prefab.m_MoonNormal);
			}
			Graphics.Blit((Texture)null, m_MoonTexture, m_MoonMaterial, m_ClearPass);
			Graphics.Blit((Texture)null, m_MoonTexture, m_MoonMaterial, m_LitPass);
			((Texture)m_MoonTexture).IncrementUpdateCount();
			m_MoonLight.additionalData.surfaceTexture = (Texture)(object)m_MoonTexture;
		}
	}

	public void SetDefaults(Context context)
	{
		m_Latitude = kDefaultLatitude;
		m_Longitude = kDefaultLongitude;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float num = m_Latitude;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float num2 = m_Longitude;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float reference = ref m_Latitude;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref float reference2 = ref m_Longitude;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeSettingsData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1383560598_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<TimeData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1383560598_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<AtmosphereData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1383560598_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public PlanetarySystem()
	{
	}
}
