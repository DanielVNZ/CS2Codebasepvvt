using Cinemachine;
using Colossal.Mathematics;
using Game.Rendering;
using Game.SceneFlow;
using Game.Simulation;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game;

public class CinemachineRestrictToTerrain : CinemachineExtension
{
	public float m_MapSurfacePadding = 1f;

	public bool m_RestrictToMapArea = true;

	private CameraCollisionSystem m_CollisionSystem;

	private CameraUpdateSystem m_CameraSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	public bool enableObjectCollisions { get; set; } = true;

	public Vector3 previousPosition { get; set; }

	protected void Start()
	{
		m_CollisionSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraCollisionSystem>();
		m_CameraSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_TerrainSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<WaterSystem>();
	}

	public void Refresh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		previousPosition = ((Component)this).transform.position;
	}

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, ref CameraState state, float deltaTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if ((int)stage == 0)
		{
			float terrainHeight;
			Vector3 rawPosition = ClampToTerrain(state.RawPosition, m_RestrictToMapArea, out terrainHeight);
			state.RawPosition = rawPosition;
			if (enableObjectCollisions && CheckForCollision(state.RawPosition, previousPosition, state.RawOrientation, out var position))
			{
				state.RawPosition = position;
			}
		}
	}

	public bool CheckForCollision(Vector3 currentPosition, Vector3 lastPosition, Quaternion rotation, out Vector3 position)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (m_CollisionSystem != null && m_CameraSystem != null && (Object)(object)m_CameraSystem.activeCamera != (Object)null)
		{
			float3 position2 = float3.op_Implicit(currentPosition);
			float3 val = float3.op_Implicit(lastPosition);
			float nearClipPlane = m_CameraSystem.activeCamera.nearClipPlane;
			float2 val2 = default(float2);
			val2.y = m_CameraSystem.activeCamera.fieldOfView;
			val2.x = Camera.VerticalToHorizontalFieldOfView(val2.y, m_CameraSystem.activeCamera.aspect);
			m_CollisionSystem.CheckCollisions(ref position2, val, quaternion.op_Implicit(rotation), 200f, 200f, nearClipPlane * 2f + 1f, nearClipPlane, 0.001f, val2);
			position = float3.op_Implicit(position2);
			return true;
		}
		position = Vector3.zero;
		return false;
	}

	public Vector3 ClampToTerrain(Vector3 position, bool restrictToMapArea, out float terrainHeight)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		terrainHeight = 0f;
		TerrainHeightData data = m_TerrainSystem.GetHeightData();
		if (data.isCreated)
		{
			if (restrictToMapArea)
			{
				Bounds3 val = (GameManager.instance.gameMode.IsEditor() ? TerrainUtils.GetEditorCameraBounds(m_TerrainSystem, ref data) : TerrainUtils.GetBounds(ref data));
				float3 max = val.max;
				max.y = val.min.y + math.max(val.max.y - val.min.y, 4096f);
				val.max = max;
				position = float3.op_Implicit(MathUtils.Clamp(float3.op_Implicit(position), val));
			}
			if (m_WaterSystem.Loaded)
			{
				JobHandle deps;
				WaterSurfaceData data2 = m_WaterSystem.GetSurfaceData(out deps);
				((JobHandle)(ref deps)).Complete();
				if (data2.isCreated)
				{
					terrainHeight = WaterUtils.SampleHeight(ref data2, ref data, float3.op_Implicit(position));
				}
			}
			else
			{
				terrainHeight = TerrainUtils.SampleHeight(ref data, float3.op_Implicit(position));
			}
			position.y = Mathf.Max(position.y, terrainHeight += m_MapSurfacePadding);
		}
		return position;
	}
}
