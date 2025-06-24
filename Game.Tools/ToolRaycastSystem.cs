using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Notifications;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

public class ToolRaycastSystem : GameSystemBase
{
	private ToolSystem m_ToolSystem;

	private RaycastSystem m_RaycastSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	public RaycastFlags raycastFlags { get; set; }

	public TypeMask typeMask { get; set; }

	public CollisionMask collisionMask { get; set; }

	public Layer netLayerMask { get; set; }

	public AreaTypeMask areaTypeMask { get; set; }

	public RouteType routeType { get; set; }

	public TransportType transportType { get; set; }

	public IconLayerMask iconLayerMask { get; set; }

	public UtilityTypes utilityTypeMask { get; set; }

	public float3 rayOffset { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
	}

	public bool GetRaycastResult(out RaycastResult result)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<RaycastResult> result2 = m_RaycastSystem.GetResult(this);
		if (result2.Length != 0)
		{
			result = result2[0];
			return result.m_Owner != Entity.Null;
		}
		result = default(RaycastResult);
		return false;
	}

	public static Segment CalculateRaycastLine(Camera mainCamera)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Ray val = mainCamera.ScreenPointToRay(InputManager.instance.mousePosition);
		float3 val2 = float3.op_Implicit(((Ray)(ref val)).direction);
		float3 val3 = float3.op_Implicit(((Component)mainCamera).transform.forward);
		Segment val4 = default(Segment);
		val4.a = float3.op_Implicit(((Ray)(ref val)).origin);
		val4.b = val4.a + val2 * (mainCamera.farClipPlane / math.clamp(math.dot(val2, val3), 0.25f, 1f));
		return val4;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != null)
		{
			m_ToolSystem.activeTool.InitializeRaycast();
		}
		if (m_CameraUpdateSystem.TryGetViewer(out var viewer))
		{
			if (m_ToolSystem.fullUpdateRequired)
			{
				raycastFlags |= RaycastFlags.ToolDisable;
			}
			else
			{
				raycastFlags &= ~RaycastFlags.ToolDisable;
			}
			if (InputManager.instance.controlOverWorld)
			{
				raycastFlags &= ~RaycastFlags.UIDisable;
			}
			else
			{
				raycastFlags |= RaycastFlags.UIDisable;
			}
			RaycastInput input = new RaycastInput
			{
				m_Line = CalculateRaycastLine(viewer.camera),
				m_Offset = rayOffset,
				m_TypeMask = typeMask,
				m_Flags = raycastFlags,
				m_CollisionMask = collisionMask,
				m_NetLayerMask = netLayerMask,
				m_AreaTypeMask = areaTypeMask,
				m_RouteType = routeType,
				m_TransportType = transportType,
				m_IconLayerMask = iconLayerMask,
				m_UtilityTypeMask = utilityTypeMask
			};
			m_RaycastSystem.AddInput(this, input);
		}
	}

	[Preserve]
	public ToolRaycastSystem()
	{
	}
}
