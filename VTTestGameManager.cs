using System;
using Colossal;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Logging;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class VTTestGameManager : MonoBehaviour
{
	[Header("VT Settings")]
	public bool m_OverrideVTSettings;

	public int m_VTMipBias;

	public FilterMode m_VTFilterMode = (FilterMode)2;

	private uint m_FrameIndex;

	private float m_FrameTime;

	private WindControl m_WindControl;

	[Header("Camera")]
	public Camera movingCamera;

	public float cameraSpeed = 0.1f;

	public Transform cameraStart;

	public Transform cameraEnd;

	private float cameraPosition;

	private bool movingBackward;

	private World m_World;

	private TextureStreamingSystem m_TextureStreamingSystem;

	private GizmosSystem m_GizmosSystem;

	private void Awake()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		m_WindControl = WindControl.instance;
		LogManager.SetDefaultEffectiveness(Level.Info);
		m_World = new World("Game", (WorldFlags)9);
		World.DefaultGameObjectInjectionWorld = m_World;
		m_GizmosSystem = m_World.GetOrCreateSystemManaged<GizmosSystem>();
		m_TextureStreamingSystem = m_World.GetOrCreateSystemManaged<TextureStreamingSystem>();
		if (m_OverrideVTSettings)
		{
			m_TextureStreamingSystem.Initialize(m_VTMipBias, m_VTFilterMode);
		}
		else
		{
			m_TextureStreamingSystem.Initialize();
		}
		cameraPosition = 0f;
	}

	private void OnDestroy()
	{
		Gizmos.ReleaseResources();
		m_World.Dispose();
		m_WindControl.Dispose();
	}

	private void Update()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		m_FrameIndex += 5u;
		m_FrameTime = Time.deltaTime;
		float2 val = float2.op_Implicit(m_FrameIndex % new uint2(60u, 3600u)) + new float2(m_FrameTime);
		float4 xyxy = ((float2)(ref val)).xyxy;
		xyxy *= new float4(1f / 60f, 0.00027777778f, (float)Math.PI / 30f, 0.0017453294f);
		Shader.SetGlobalVector("colossal_SimulationTime", float4.op_Implicit(xyxy));
		float num = (float)(m_FrameIndex % 216000) + m_FrameTime;
		Shader.SetGlobalFloat("colossal_SimulationTime2", num);
		((ComponentSystemBase)m_TextureStreamingSystem).Update();
		if (!((Object)(object)movingCamera != (Object)null) || !((Object)(object)cameraEnd != (Object)null) || !((Object)(object)cameraStart != (Object)null))
		{
			return;
		}
		Vector3 val2 = cameraEnd.position - cameraStart.position;
		float num2 = cameraSpeed * Time.deltaTime;
		if (movingBackward)
		{
			cameraPosition -= num2;
			if (cameraPosition < 0f)
			{
				cameraPosition = 0f;
				movingBackward = false;
			}
		}
		else
		{
			cameraPosition += num2;
			if (cameraPosition > ((Vector3)(ref val2)).magnitude)
			{
				cameraPosition = ((Vector3)(ref val2)).magnitude;
				movingBackward = true;
			}
		}
		((Component)movingCamera).transform.position = cameraStart.position + ((Vector3)(ref val2)).normalized * cameraPosition;
	}

	private void LateUpdate()
	{
		((ComponentSystemBase)m_GizmosSystem).Update();
	}
}
