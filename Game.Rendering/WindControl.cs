using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Rendering;

public class WindControl
{
	private struct SampledParameter<T>
	{
		public T current;

		public T previous;

		public void Reset(T value)
		{
			previous = value;
			current = value;
		}

		public void Update(T value)
		{
			previous = current;
			current = value;
		}
	}

	private static WindControl s_Instance;

	private ShaderVariablesWind m_ShaderVariablesWindCB;

	private static readonly int m_ShaderVariablesWind = Shader.PropertyToID("ShaderVariablesWind");

	private SampledParameter<float> _WindBaseStrengthPhase;

	private SampledParameter<float> _WindBaseStrengthPhase2;

	private SampledParameter<float> _WindTreeBaseStrengthPhase;

	private SampledParameter<float> _WindTreeBaseStrengthPhase2;

	private SampledParameter<float> _WindBaseStrengthVariancePeriod;

	private SampledParameter<float> _WindTreeBaseStrengthVariancePeriod;

	private SampledParameter<float> _WindGustStrengthPhase;

	private SampledParameter<float> _WindGustStrengthPhase2;

	private SampledParameter<float> _WindTreeGustStrengthPhase;

	private SampledParameter<float> _WindTreeGustStrengthPhase2;

	private SampledParameter<float> _WindGustStrengthVariancePeriod;

	private SampledParameter<float> _WindTreeGustStrengthVariancePeriod;

	private SampledParameter<float> _WindFlutterGustVariancePeriod;

	private SampledParameter<float> _WindTreeFlutterGustVariancePeriod;

	private float _LastParametersSamplingTime;

	private static readonly float3 kForward = new float3(0f, 0f, 1f);

	public static WindControl instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new WindControl();
			}
			return s_Instance;
		}
	}

	private WindControl()
	{
		RenderPipelineManager.beginCameraRendering += SetupGPUData;
	}

	public void Dispose()
	{
		RenderPipelineManager.beginCameraRendering -= SetupGPUData;
		s_Instance = null;
	}

	private bool GetWindComponent(Camera camera, out WindVolumeComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)camera.cameraType == 2)
		{
			Camera main = Camera.main;
			if ((Object)(object)main == (Object)null)
			{
				component = null;
				return false;
			}
			camera = main;
		}
		HDCamera orCreate = HDCamera.GetOrCreate(camera, 0);
		component = orCreate.volumeStack.GetComponent<WindVolumeComponent>();
		return true;
	}

	private void SetupGPUData(ScriptableRenderContext context, Camera camera)
	{
		if (GetWindComponent(camera, out var component))
		{
			CommandBuffer val = CommandBufferPool.Get("");
			UpdateCPUData(component);
			SetGlobalProperties(val, component);
			((ScriptableRenderContext)(ref context)).ExecuteCommandBuffer(val);
			((ScriptableRenderContext)(ref context)).Submit();
			val.Clear();
			CommandBufferPool.Release(val);
		}
	}

	private void UpdateCPUData(WindVolumeComponent wind)
	{
		if (Time.time - _LastParametersSamplingTime > ((VolumeParameter<float>)(object)wind.windParameterInterpolationDuration).value)
		{
			_LastParametersSamplingTime = Time.time;
			_WindBaseStrengthPhase.Update(((VolumeParameter<float>)(object)wind.windBaseStrengthPhase).value);
			_WindBaseStrengthPhase2.Update(((VolumeParameter<float>)(object)wind.windBaseStrengthPhase2).value);
			_WindTreeBaseStrengthPhase.Update(((VolumeParameter<float>)(object)wind.windTreeBaseStrengthPhase).value);
			_WindTreeBaseStrengthPhase2.Update(((VolumeParameter<float>)(object)wind.windTreeBaseStrengthPhase2).value);
			_WindBaseStrengthVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windBaseStrengthVariancePeriod).value);
			_WindTreeBaseStrengthVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windTreeBaseStrengthVariancePeriod).value);
			_WindGustStrengthPhase.Update(((VolumeParameter<float>)(object)wind.windGustStrengthPhase).value);
			_WindGustStrengthPhase2.Update(((VolumeParameter<float>)(object)wind.windGustStrengthPhase2).value);
			_WindTreeGustStrengthPhase.Update(((VolumeParameter<float>)(object)wind.windTreeGustStrengthPhase).value);
			_WindTreeGustStrengthPhase2.Update(((VolumeParameter<float>)(object)wind.windTreeGustStrengthPhase2).value);
			_WindGustStrengthVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windGustStrengthVariancePeriod).value);
			_WindTreeGustStrengthVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windTreeGustStrengthVariancePeriod).value);
			_WindFlutterGustVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windFlutterGustVariancePeriod).value);
			_WindTreeFlutterGustVariancePeriod.Update(((VolumeParameter<float>)(object)wind.windTreeFlutterGustVariancePeriod).value);
		}
	}

	private unsafe void SetGlobalProperties(CommandBuffer cmd, WindVolumeComponent wind)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		ProfilingScope val = default(ProfilingScope);
		((ProfilingScope)(ref val))._002Ector(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.WindGlobalProperties));
		try
		{
			float num = (Application.isPlaying ? Time.time : Time.realtimeSinceStartup);
			float num2 = math.radians(((VolumeParameter<float>)(object)wind.windDirection).value) + math.cos((float)Math.PI * 2f * num / ((VolumeParameter<float>)(object)wind.windDirectionVariancePeriod).value) * math.radians(((VolumeParameter<float>)(object)wind.windDirectionVariance).value);
			float3 val2 = math.mul(quaternion.Euler(0f, num2, 0f, (RotationOrder)4), kForward);
			float3 val3 = math.mul(quaternion.Euler(0f, ((VolumeParameter<float>)(object)wind.windDirection).value, 0f, (RotationOrder)4), kForward);
			float num3 = ((VolumeParameter<AnimationCurve>)(object)wind.windGustStrengthControl).value.Evaluate(num);
			float num4 = ((VolumeParameter<AnimationCurve>)(object)wind.windTreeGustStrengthControl).value.Evaluate(num);
			float4 val4 = default(float4);
			((float4)(ref val4))._002Ector(val2, 1f);
			float4 val5 = default(float4);
			((float4)(ref val5))._002Ector(val3, num);
			float4 zero = float4.zero;
			zero.w = math.min(1f, (Time.time - _LastParametersSamplingTime) / ((VolumeParameter<float>)(object)wind.windParameterInterpolationDuration).value);
			float4 val6 = default(float4);
			((float4)(ref val6))._002Ector(_WindBaseStrengthPhase.previous, _WindBaseStrengthPhase2.previous, _WindBaseStrengthPhase.current, _WindBaseStrengthPhase2.current);
			m_ShaderVariablesWindCB._WindData_0 = float4x4.op_Implicit(math.transpose(new float4x4(val4, val5, zero, val6)));
			float4 val7 = default(float4);
			((float4)(ref val7))._002Ector(((VolumeParameter<float>)(object)wind.windBaseStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windBaseStrengthOffset).value, ((VolumeParameter<float>)(object)wind.windTreeBaseStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windTreeBaseStrengthOffset).value);
			float4 val8 = default(float4);
			((float4)(ref val8))._002Ector(0f, ((VolumeParameter<float>)(object)wind.windGustStrength).value * num3 * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windGustStrengthOffset).value, _WindFlutterGustVariancePeriod.current);
			float4 val9 = default(float4);
			((float4)(ref val9))._002Ector(_WindGustStrengthVariancePeriod.current, _WindGustStrengthVariancePeriod.previous, ((VolumeParameter<float>)(object)wind.windGustInnerCosScale).value, ((VolumeParameter<float>)(object)wind.windFlutterStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value);
			float4 val10 = default(float4);
			((float4)(ref val10))._002Ector(((VolumeParameter<float>)(object)wind.windFlutterGustStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windFlutterGustStrengthOffset).value, ((VolumeParameter<float>)(object)wind.windFlutterGustStrengthScale).value, _WindFlutterGustVariancePeriod.previous);
			m_ShaderVariablesWindCB._WindData_1 = float4x4.op_Implicit(math.transpose(new float4x4(val7, val8, val9, val10)));
			float4 val11 = default(float4);
			((float4)(ref val11))._002Ector(_WindTreeBaseStrengthPhase.previous, _WindTreeBaseStrengthPhase2.previous, _WindTreeBaseStrengthPhase.current, _WindTreeBaseStrengthPhase2.current);
			float4 val12 = default(float4);
			((float4)(ref val12))._002Ector(0f, ((VolumeParameter<float>)(object)wind.windTreeGustStrength).value * num4 * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windTreeGustStrengthOffset).value, _WindTreeFlutterGustVariancePeriod.current);
			float4 val13 = default(float4);
			((float4)(ref val13))._002Ector(_WindTreeGustStrengthVariancePeriod.current, _WindTreeGustStrengthVariancePeriod.previous, ((VolumeParameter<float>)(object)wind.windTreeGustInnerCosScale).value, ((VolumeParameter<float>)(object)wind.windTreeFlutterStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value);
			float4 val14 = default(float4);
			((float4)(ref val14))._002Ector(((VolumeParameter<float>)(object)wind.windTreeFlutterGustStrength).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale).value * ((VolumeParameter<float>)(object)wind.windGlobalStrengthScale2).value, ((VolumeParameter<float>)(object)wind.windTreeFlutterGustStrengthOffset).value, ((VolumeParameter<float>)(object)wind.windTreeFlutterGustStrengthScale).value, _WindTreeFlutterGustVariancePeriod.previous);
			m_ShaderVariablesWindCB._WindData_2 = float4x4.op_Implicit(math.transpose(new float4x4(val11, val12, val13, val14)));
			float4 val15 = default(float4);
			((float4)(ref val15))._002Ector(_WindBaseStrengthVariancePeriod.previous, _WindTreeBaseStrengthVariancePeriod.previous, _WindBaseStrengthVariancePeriod.previous, _WindTreeBaseStrengthVariancePeriod.current);
			float4 val16 = default(float4);
			((float4)(ref val16))._002Ector(_WindGustStrengthPhase.previous, _WindGustStrengthPhase2.previous, _WindGustStrengthPhase.current, _WindGustStrengthPhase2.current);
			float4 val17 = default(float4);
			((float4)(ref val17))._002Ector(_WindTreeGustStrengthPhase.previous, _WindTreeGustStrengthPhase2.previous, _WindTreeGustStrengthPhase.current, _WindTreeGustStrengthPhase2.current);
			float4 val18 = default(float4);
			((float4)(ref val18))._002Ector(0f, 0f, 0f, 0f);
			m_ShaderVariablesWindCB._WindData_3 = float4x4.op_Implicit(math.transpose(new float4x4(val15, val16, val17, val18)));
			ConstantBuffer.PushGlobal<ShaderVariablesWind>(cmd, ref m_ShaderVariablesWindCB, m_ShaderVariablesWind);
		}
		finally
		{
			((IDisposable)(*(ProfilingScope*)(&val))/*cast due to .constrained prefix*/).Dispose();
		}
	}
}
