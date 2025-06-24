using System;
using Cinemachine;
using Game.Audio;
using Game.Rendering;
using Game.SceneFlow;
using Unity.Entities;
using UnityEngine;

namespace Game;

public class CinematicCameraController : MonoBehaviour, IGameCameraController
{
	[SerializeField]
	private float m_MinMoveSpeed = 5f;

	[SerializeField]
	private float m_MaxMoveSpeed = 1000f;

	[SerializeField]
	private float m_MinZoomSpeed = 10f;

	[SerializeField]
	private float m_MaxZoomSpeed = 4000f;

	[SerializeField]
	private float m_RotateSpeed = 0.5f;

	[SerializeField]
	private float m_MaxHeight = 5000f;

	[SerializeField]
	private float m_MaxMovementSpeedHeight = 1000f;

	private Transform m_Anchor;

	private CinemachineVirtualCamera m_VCam;

	private CinemachineRestrictToTerrain m_RestrictToTerrain;

	private CameraInput m_CameraInput;

	private CameraUpdateSystem m_CameraUpdateSystem;

	public ICinemachineCamera virtualCamera => (ICinemachineCamera)(object)m_VCam;

	public float zoom
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return m_Anchor.position.y;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = m_Anchor.position;
			val.y = value;
			m_Anchor.position = val;
		}
	}

	public Vector3 pivot
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return m_Anchor.position;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_Anchor.position = value;
		}
	}

	public Vector3 position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return pivot;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			pivot = value;
		}
	}

	public Vector3 rotation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = m_Anchor.rotation;
			return ((Quaternion)(ref val)).eulerAngles;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_Anchor.rotation = Quaternion.Euler(value);
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

	public bool collisionsEnabled
	{
		get
		{
			return m_RestrictToTerrain.enableObjectCollisions;
		}
		set
		{
			m_RestrictToTerrain.enableObjectCollisions = value;
		}
	}

	public ref LensSettings lens => ref m_VCam.m_Lens;

	public Action eventCameraMove { get; set; }

	public float fov
	{
		get
		{
			return m_VCam.m_Lens.FieldOfView;
		}
		set
		{
			m_VCam.m_Lens.FieldOfView = value;
		}
	}

	public float dutch
	{
		get
		{
			return m_VCam.m_Lens.Dutch;
		}
		set
		{
			m_VCam.m_Lens.Dutch = value;
		}
	}

	public bool inputEnabled { get; set; } = true;

	private async void Awake()
	{
		if (await GameManager.instance.WaitForReadyState())
		{
			m_Anchor = new GameObject("CinematicCameraControllerAnchor").transform;
			m_VCam = ((Component)this).GetComponent<CinemachineVirtualCamera>();
			((CinemachineVirtualCameraBase)m_VCam).Follow = m_Anchor;
			m_RestrictToTerrain = ((Component)this).GetComponent<CinemachineRestrictToTerrain>();
			m_CameraInput = ((Component)this).GetComponent<CameraInput>();
			if ((Object)(object)m_CameraInput != (Object)null)
			{
				m_CameraInput.Initialize();
			}
			m_CameraUpdateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
			m_CameraUpdateSystem.cinematicCameraController = this;
			((Component)this).gameObject.SetActive(false);
		}
	}

	public void TryMatchPosition(IGameCameraController other)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		position = other.position;
		rotation = other.rotation;
	}

	public void UpdateCamera()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_CameraInput != (Object)null)
		{
			m_CameraInput.Refresh();
			if (m_CameraInput.any)
			{
				eventCameraMove?.Invoke();
			}
			if (inputEnabled)
			{
				UpdateController(m_CameraInput);
			}
		}
		AudioManager.instance?.UpdateAudioListener(((Component)this).transform.position, ((Component)this).transform.rotation);
	}

	private void UpdateController(CameraInput input)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		m_RestrictToTerrain.Refresh();
		Vector3 val = m_Anchor.position;
		m_RestrictToTerrain.ClampToTerrain(val, restrictToMapArea: true, out var terrainHeight);
		float num = Mathf.Min(val.y - terrainHeight, m_MaxMovementSpeedHeight) / m_MaxMovementSpeedHeight;
		Vector2 move = input.move;
		move *= Mathf.Lerp(m_MinMoveSpeed, m_MaxMoveSpeed, num);
		Vector2 val2 = input.rotate * m_RotateSpeed;
		float num2 = input.zoom * Mathf.Lerp(m_MinZoomSpeed, m_MaxZoomSpeed, num);
		Quaternion val3 = m_Anchor.rotation;
		Vector3 eulerAngles = ((Quaternion)(ref val3)).eulerAngles;
		val += Quaternion.AngleAxis(eulerAngles.y, Vector3.up) * new Vector3(move.x, 0f - num2, move.y);
		val = m_RestrictToTerrain.ClampToTerrain(val, restrictToMapArea: true, out var terrainHeight2);
		val.y = Mathf.Min(val.y, terrainHeight2 + m_MaxHeight);
		Quaternion val4 = Quaternion.Euler(Mathf.Clamp((eulerAngles.x + 90f) % 360f - val2.y, 0f, 180f) - 90f, eulerAngles.y + val2.x, 0f);
		if (m_RestrictToTerrain.enableObjectCollisions && m_RestrictToTerrain.CheckForCollision(val, m_RestrictToTerrain.previousPosition, val4, out var val5))
		{
			m_Anchor.position = val5;
		}
		else
		{
			m_Anchor.position = val;
		}
		m_Anchor.rotation = val4;
	}

	private void OnDestroy()
	{
		if ((Object)(object)m_Anchor != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)m_Anchor).gameObject);
		}
		if (m_CameraUpdateSystem != null)
		{
			m_CameraUpdateSystem.cinematicCameraController = null;
		}
	}
}
