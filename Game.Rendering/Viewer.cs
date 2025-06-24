using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Rendering.Legacy;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Rendering;

public class Viewer
{
	private ViewerDistances m_ViewerDistances;

	private float m_TargetFocusDistance;

	private float m_FocusDistanceVelocity;

	private const int kCenterSampleCount = 4;

	private static int[] kSamplePattern32 = new int[64]
	{
		1, 1, 4, 1, 2, -1, -4, 0, -6, 1,
		-7, -1, -2, 2, 7, 2, 2, 3, -5, 4,
		-1, 4, 4, 4, -7, 5, -3, 5, 6, 5,
		1, 6, -4, 7, 5, 7, -1, -2, 6, -2,
		-6, -3, -3, -3, 0, -4, 4, -4, 2, -5,
		7, -5, -7, -6, -3, -6, 5, -6, -5, -7,
		-1, -7, 3, -7
	};

	public ViewerDistances viewerDistances => m_ViewerDistances;

	public float visibilityDistance => camera.farClipPlane;

	public float nearClipPlane => camera.nearClipPlane;

	public float3 position => float3.op_Implicit(((Component)camera).transform.position);

	public float3 forward => float3.op_Implicit(((Component)camera).transform.forward);

	public float3 right => float3.op_Implicit(((Component)camera).transform.right);

	public Camera camera { get; private set; }

	public LegacyFrustumPlanes frustumPlanes => CalculateFrustumPlanes(camera);

	public Bounds bounds => UpdateBounds();

	public bool shadowsAdjustStartDistance { get; set; } = true;

	public float pushCullingNearPlaneMultiplier { get; set; } = 0.9f;

	public float pushCullingNearPlaneValue { get; set; } = 100f;

	public bool shadowsAdjustFarDistance { get; set; } = true;

	public Viewer(Camera camera)
	{
		this.camera = camera;
	}

	public bool TryGetLODParameters(out LODParameters lodParameters)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ScriptableCullingParameters val = default(ScriptableCullingParameters);
		if (camera.TryGetCullingParameters(ref val))
		{
			lodParameters = ((ScriptableCullingParameters)(ref val)).lodParameters;
			return true;
		}
		lodParameters = default(LODParameters);
		return false;
	}

	protected Bounds UpdateBounds()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)camera).transform;
		float num = math.tan(math.radians(camera.fieldOfView * 0.5f));
		float num2 = num * camera.aspect;
		float3 val = float3.op_Implicit(transform.forward);
		float3 val2 = float3.op_Implicit(transform.right);
		float3 val3 = float3.op_Implicit(transform.up);
		float farClipPlane = camera.farClipPlane;
		float num3 = camera.nearClipPlane;
		float3 val4 = position;
		float3 val5 = val4 + val * farClipPlane - farClipPlane * val2 * num2 + val3 * num * farClipPlane;
		float3 val6 = val4 + val * farClipPlane + farClipPlane * val2 * num2 - val3 * num * farClipPlane;
		float3 val7 = val4 + val * farClipPlane - farClipPlane * val2 * num2 - val3 * num * farClipPlane;
		float3 val8 = val4 + val * farClipPlane + farClipPlane * val2 * num2 + val3 * num * farClipPlane;
		float3 val9 = val4 + val * num3;
		Bounds result = default(Bounds);
		((Bounds)(ref result))._002Ector(Vector3.zero, Vector3.one * float.NegativeInfinity);
		((Bounds)(ref result)).Encapsulate(float3.op_Implicit(val5));
		((Bounds)(ref result)).Encapsulate(float3.op_Implicit(val6));
		((Bounds)(ref result)).Encapsulate(float3.op_Implicit(val7));
		((Bounds)(ref result)).Encapsulate(float3.op_Implicit(val8));
		((Bounds)(ref result)).Encapsulate(float3.op_Implicit(val9));
		return result;
	}

	private static LegacyFrustumPlanes ExtractProjectionPlanes(float4x4 worldToProjectionMatrix)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		LegacyFrustumPlanes result = default(LegacyFrustumPlanes);
		float4 val = default(float4);
		((float4)(ref val))._002Ector(worldToProjectionMatrix.c0.w, worldToProjectionMatrix.c1.w, worldToProjectionMatrix.c2.w, worldToProjectionMatrix.c3.w);
		float4 val2 = default(float4);
		((float4)(ref val2))._002Ector(worldToProjectionMatrix.c0.x, worldToProjectionMatrix.c1.x, worldToProjectionMatrix.c2.x, worldToProjectionMatrix.c3.x);
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(val2.x + val.x, val2.y + val.y, val2.z + val.z);
		float num = 1f / math.length(val3);
		result.left.normal = val3 * num;
		result.left.distance = (val2.w + val.w) * num;
		((float3)(ref val3))._002Ector(0f - val2.x + val.x, 0f - val2.y + val.y, 0f - val2.z + val.z);
		num = 1f / math.length(val3);
		result.right.normal = val3 * num;
		result.right.distance = (0f - val2.w + val.w) * num;
		((float4)(ref val2))._002Ector(worldToProjectionMatrix.c0.y, worldToProjectionMatrix.c1.y, worldToProjectionMatrix.c2.y, worldToProjectionMatrix.c3.y);
		val3 = float3.op_Implicit(new Vector3(val2.x + val.x, val2.y + val.y, val2.z + val.z));
		num = 1f / math.length(val3);
		result.bottom.normal = val3 * num;
		result.bottom.distance = (val2.w + val.w) * num;
		val3 = float3.op_Implicit(new Vector3(0f - val2.x + val.x, 0f - val2.y + val.y, 0f - val2.z + val.z));
		num = 1f / math.length(val3);
		result.top.normal = val3 * num;
		result.top.distance = (0f - val2.w + val.w) * num;
		((float4)(ref val2))._002Ector(worldToProjectionMatrix.c0.z, worldToProjectionMatrix.c1.z, worldToProjectionMatrix.c2.z, worldToProjectionMatrix.c3.z);
		val3 = float3.op_Implicit(new Vector3(val2.x + val.x, val2.y + val.y, val2.z + val.z));
		num = 1f / math.length(val3);
		result.zNear.normal = val3 * num;
		result.zNear.distance = (val2.w + val.w) * num;
		val3 = float3.op_Implicit(new Vector3(0f - val2.x + val.x, 0f - val2.y + val.y, 0f - val2.z + val.z));
		num = 1f / math.length(val3);
		result.zFar.normal = val3 * num;
		result.zFar.distance = (0f - val2.w + val.w) * num;
		return result;
	}

	private static LegacyFrustumPlanes CalculateFrustumPlanes(Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ExtractProjectionPlanes(float4x4.op_Implicit(camera.projectionMatrix * camera.worldToCameraMatrix));
	}

	public void UpdateRaycast(RaycastSystem raycast, float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		float3 val = position;
		NativeArray<RaycastResult> result = raycast.GetResult(this);
		float num = visibilityDistance;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = -1f;
		for (int i = 0; i < result.Length; i++)
		{
			RaycastResult raycastResult = result[i];
			if (!(raycastResult.m_Owner == Entity.Null))
			{
				float num6 = math.distance(val, raycastResult.m_Hit.m_HitPosition);
				if (i == 0)
				{
					m_ViewerDistances.ground = num6;
					continue;
				}
				if (i - 1 < 4)
				{
					num5 = math.max(num5, num6);
					continue;
				}
				num = math.min(num, num6);
				num2 = math.max(num2, num6);
				num3 += num6;
				num4 += 1f;
			}
		}
		m_ViewerDistances.closestSurface = num;
		m_ViewerDistances.farthestSurface = num2;
		m_ViewerDistances.averageSurface = (num + num2) / 2f;
		if (num4 > 0f)
		{
			m_ViewerDistances.averageSurface = num3 / num4;
		}
		if (num5 >= 0f)
		{
			m_TargetFocusDistance = num5;
		}
		m_ViewerDistances.focus = MathUtils.SmoothDamp(m_ViewerDistances.focus, m_TargetFocusDistance, ref m_FocusDistanceVelocity, 0.3f, float.MaxValue, deltaTime);
		if ((Object)(object)camera != (Object)null)
		{
			camera.focusDistance = m_ViewerDistances.focus;
			UpdatePushNearCullingPlane();
			UpdateDistanceToSeaLevel();
		}
	}

	private void UpdateDistanceToSeaLevel()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Plane val = default(Plane);
		((Plane)(ref val))._002Ector(Vector3.up, 0f - WaterSystem.SeaLevel);
		float num4 = default(float);
		for (int i = 0; i < 4; i++)
		{
			float num2 = (((i & 1) != 0) ? 1f : 0f);
			float num3 = (((i & 2) != 0) ? 1f : 0f);
			Ray val2 = camera.ViewportPointToRay(new Vector3(num2, num3, 0f));
			num = (((Plane)(ref val)).Raycast(val2, ref num4) ? math.max(num, num4) : visibilityDistance);
		}
		m_ViewerDistances.maxDistanceToSeaLevel = num;
	}

	private void UpdatePushNearCullingPlane()
	{
		HDCamera orCreate = HDCamera.GetOrCreate(camera, 0);
		if (orCreate != null)
		{
			if (shadowsAdjustStartDistance)
			{
				float num = (m_ViewerDistances.closestSurface - pushCullingNearPlaneValue) * pushCullingNearPlaneMultiplier;
				num = math.clamp(num, nearClipPlane, visibilityDistance * 0.1f);
				orCreate.overrideNearPlaneForCullingOnly = num;
			}
			else
			{
				orCreate.overrideNearPlaneForCullingOnly = 0f;
			}
		}
	}

	public void Raycast(RaycastSystem raycast)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		float3 val = position;
		RaycastInput input = new RaycastInput
		{
			m_Flags = (RaycastFlags)0u,
			m_CollisionMask = (CollisionMask.OnGround | CollisionMask.Overground),
			m_NetLayerMask = Layer.All
		};
		input.m_TypeMask = TypeMask.Terrain | TypeMask.Water;
		input.m_Line = new Segment(val, val + float3.op_Implicit(Vector3.down) * visibilityDistance);
		raycast.AddInput(this, input);
		for (int i = 0; i < kSamplePattern32.Length; i += 2)
		{
			float num = (float)kSamplePattern32[i] / 7f * 0.5f + 0.5f;
			float num2 = (float)kSamplePattern32[i + 1] / 7f * 0.5f + 0.5f;
			Ray val2 = camera.ViewportPointToRay(new Vector3(num, num2, 0f));
			if (i < 8)
			{
				input.m_TypeMask = TypeMask.Terrain | TypeMask.StaticObjects | TypeMask.MovingObjects | TypeMask.Net | TypeMask.Water;
			}
			else
			{
				input.m_TypeMask = TypeMask.Terrain | TypeMask.Water;
			}
			input.m_Line = new Segment(val, val + float3.op_Implicit(((Ray)(ref val2)).direction) * visibilityDistance);
			raycast.AddInput(this, input);
		}
	}
}
