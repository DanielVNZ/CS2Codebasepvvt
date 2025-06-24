using System;
using Cinemachine;
using Colossal.Mathematics;
using Game.Audio;
using Game.Rendering;
using Game.SceneFlow;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game;

public class OrbitCameraController : MonoBehaviour, IGameCameraController
{
	public enum Mode
	{
		Follow,
		PhotoMode,
		Editor
	}

	private static readonly float kPivotVerticalOffset = 10f;

	public float2 m_ZoomRange = new float2(10f, 10000f);

	public float m_FollowSmoothing = 0.01f;

	private Entity m_Entity;

	private float m_FollowTimer;

	private float2 m_Rotation;

	private GameObject m_Anchor;

	private CinemachineVirtualCamera m_VCam;

	private CinemachineOrbitalTransposer m_Transposer;

	private CinemachineRestrictToTerrain m_Collider;

	private CameraInput m_CameraInput;

	private CameraUpdateSystem m_CameraUpdateSystem;

	public Entity followedEntity
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (!((Behaviour)this).isActiveAndEnabled)
			{
				return Entity.Null;
			}
			return m_Entity;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (m_Entity != value)
			{
				m_Entity = value;
				xOffset = 0f;
				yOffset = 0f;
				m_FollowTimer = 0f;
				if (((Behaviour)this).isActiveAndEnabled)
				{
					RefreshAudioFollow(value != Entity.Null);
				}
			}
		}
	}

	public Mode mode { get; set; }

	public Vector3 pivot
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return m_Anchor.transform.position;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			m_Anchor.transform.position = value;
		}
	}

	public Vector3 position
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.position;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			((Component)this).transform.position = value;
			m_Anchor.transform.position = value + m_Anchor.transform.rotation * new Vector3(0f, 0f, zoom);
		}
	}

	public bool controllerEnabled
	{
		get
		{
			return ((Behaviour)this).isActiveAndEnabled;
		}
		set
		{
			((Component)this).gameObject.SetActive(value);
		}
	}

	public bool inputEnabled { get; set; } = true;

	public Vector3 rotation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = m_Anchor.transform.rotation;
			return ((Quaternion)(ref val)).eulerAngles;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			m_Rotation = new float2(value.y, value.x);
		}
	}

	public float zoom { get; set; }

	public float yOffset { get; set; }

	public float xOffset { get; set; }

	public ICinemachineCamera virtualCamera => (ICinemachineCamera)(object)m_VCam;

	public ref LensSettings lens => ref m_VCam.m_Lens;

	public bool collisionsEnabled
	{
		get
		{
			return m_Collider.enableObjectCollisions;
		}
		set
		{
			m_Collider.enableObjectCollisions = value;
		}
	}

	public Action EventCameraMove { get; set; }

	private async void Awake()
	{
		if (await GameManager.instance.WaitForReadyState())
		{
			m_Anchor = new GameObject("OrbitCameraAnchor");
			Transform transform = m_Anchor.transform;
			m_VCam = ((Component)this).GetComponent<CinemachineVirtualCamera>();
			m_Transposer = m_VCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
			m_Collider = ((Component)this).GetComponent<CinemachineRestrictToTerrain>();
			if ((Object)(object)m_VCam != (Object)null)
			{
				((CinemachineVirtualCameraBase)m_VCam).LookAt = transform;
				((CinemachineVirtualCameraBase)m_VCam).Follow = transform;
			}
			((Component)this).gameObject.SetActive(false);
			m_CameraInput = ((Component)this).GetComponent<CameraInput>();
			if ((Object)(object)m_CameraInput != (Object)null)
			{
				m_CameraInput.Initialize();
			}
			m_CameraUpdateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
			m_CameraUpdateSystem.orbitCameraController = this;
			zoom = Mathf.Clamp(zoom, m_ZoomRange.x, m_ZoomRange.y);
		}
	}

	private void OnEnable()
	{
		RefreshAudioFollow(active: true);
	}

	private void OnDisable()
	{
		RefreshAudioFollow(active: false);
	}

	private void RefreshAudioFollow(bool active)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (AudioManager.instance != null)
		{
			AudioManager.instance.followed = (active ? m_Entity : Entity.Null);
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)m_Anchor != (Object)null)
		{
			Object.Destroy((Object)(object)m_Anchor);
		}
		if (m_CameraUpdateSystem != null)
		{
			m_CameraUpdateSystem.cinematicCameraController = null;
		}
	}

	public void TryMatchPosition(IGameCameraController other)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		rotation = other.rotation;
		if (other is CinematicCameraController)
		{
			m_Collider.ClampToTerrain(other.position, restrictToMapArea: false, out var terrainHeight);
			float num = other.position.y - terrainHeight - kPivotVerticalOffset;
			zoom = Mathf.Clamp(num / Mathf.Sin((float)Math.PI / 180f * Mathf.Abs(other.rotation.x)), m_ZoomRange.x, m_ZoomRange.y);
			pivot = new Vector3(other.position.x, num, other.position.z) + Quaternion.Euler(other.rotation) * new Vector3(0f, 0f, zoom);
		}
		else
		{
			zoom = Mathf.Clamp(other.zoom, m_ZoomRange.x, m_ZoomRange.y);
			pivot = other.pivot;
		}
	}

	public void UpdateCamera()
	{
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		m_Collider.Refresh();
		m_CameraInput.Refresh();
		if (inputEnabled && (Object)(object)m_CameraInput != (Object)null)
		{
			Vector2 rotate = m_CameraInput.rotate;
			m_Rotation.x = (m_Rotation.x + rotate.x) % 360f;
			m_Rotation.y = Mathf.Clamp((m_Rotation.y + 90f) % 360f - rotate.y, 0f, 180f) - 90f;
			float num = m_CameraInput.zoom;
			zoom = Mathf.Clamp(math.pow(zoom, 1f + num), m_ZoomRange.x, m_ZoomRange.y);
			if (followedEntity == Entity.Null)
			{
				Vector2 move = m_CameraInput.move;
				Vector3 val = m_Anchor.transform.position;
				val = m_Collider.ClampToTerrain(val, restrictToMapArea: true, out var _);
				Vector2 val2 = move * zoom;
				Vector3 val3 = val;
				float3 val4 = new float3(0f, 1f, 0f);
				Quaternion val5 = m_Anchor.transform.rotation;
				Vector3 val6 = val3 + float3.op_Implicit(math.mul(quaternion.AxisAngle(val4, math.radians(((Quaternion)(ref val5)).eulerAngles.y)), new float3(val2.x, 0f, val2.y)));
				val6 = m_Collider.ClampToTerrain(val6, restrictToMapArea: true, out var terrainHeight2);
				val6.y = terrainHeight2 + kPivotVerticalOffset;
				m_Anchor.transform.position = val6;
			}
			if (TryGetPosition(followedEntity, World.DefaultGameObjectInjectionWorld.EntityManager, out var val7, out var _, out var radius))
			{
				m_Anchor.transform.rotation = quaternion.op_Implicit(quaternion.Euler(math.radians(m_Rotation.y), math.radians(m_Rotation.x), 0f, (RotationOrder)4));
				float3 val9 = float3.op_Implicit(pivot) - val7;
				m_FollowTimer += Time.deltaTime;
				float num2 = math.pow(m_FollowSmoothing, Time.deltaTime) * math.smoothstep(0.5f, 0f, m_FollowTimer);
				val9 *= num2;
				m_Anchor.transform.position = float3.op_Implicit(val7 + val9 + math.mul(quaternion.op_Implicit(m_Anchor.transform.rotation), new float3(xOffset, yOffset, 0f)));
			}
			else
			{
				m_Anchor.transform.rotation = quaternion.op_Implicit(quaternion.Euler(math.radians(m_Rotation.y), math.radians(m_Rotation.x), 0f, (RotationOrder)4));
			}
			((CinemachineTransposer)m_Transposer).m_FollowOffset.z = 0f - zoom - radius;
		}
		Transform transform = ((Component)this).transform;
		AudioManager.instance?.UpdateAudioListener(transform.position, transform.rotation);
		if (m_CameraInput.isMoving || MapTilesUISystem.mapTileViewActive)
		{
			EventCameraMove?.Invoke();
		}
	}

	private static bool TryGetPosition(Entity e, EntityManager entityManager, out float3 position, out quaternion rotation, out float radius)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		int elementIndex = -1;
		if (e != Entity.Null && SelectedInfoUISystem.TryGetPosition(e, entityManager, ref elementIndex, out var _, out position, out var bounds, out rotation, reinterpolate: true))
		{
			position.y = MathUtils.Center(((Bounds3)(ref bounds)).y);
			float3 val = (bounds.max - bounds.min) / 2f;
			radius = Mathf.Min(new float[3] { val.x, val.y, val.z });
			return true;
		}
		position = float3.zero;
		rotation = quaternion.identity;
		radius = 0f;
		return false;
	}
}
