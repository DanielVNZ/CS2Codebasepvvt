using Colossal.Mathematics;
using Game.Input;
using Unity.Mathematics;
using UnityEngine;

namespace Game;

public class CameraInput : MonoBehaviour
{
	public float m_MoveSmoothing = 1E-06f;

	public float m_RotateSmoothing = 1E-06f;

	public float m_ZoomSmoothing = 1E-06f;

	private ProxyAction m_MoveAction;

	private ProxyAction m_FastMoveAction;

	private ProxyAction m_RotateAction;

	private ProxyAction m_ZoomAction;

	public Vector2 move { get; private set; }

	public Vector2 rotate { get; private set; }

	public float zoom { get; private set; }

	public bool isMoving => m_MoveAction.IsInProgress();

	public bool any
	{
		get
		{
			if (!m_MoveAction.IsInProgress() && !m_FastMoveAction.IsInProgress() && !m_RotateAction.IsInProgress())
			{
				return m_ZoomAction.IsInProgress();
			}
			return true;
		}
	}

	public void Initialize()
	{
		m_MoveAction = InputManager.instance.FindAction("Camera", "Move");
		m_FastMoveAction = InputManager.instance.FindAction("Camera", "Move Fast");
		m_RotateAction = InputManager.instance.FindAction("Camera", "Rotate");
		m_ZoomAction = InputManager.instance.FindAction("Camera", "Zoom");
	}

	public void Refresh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		move = float2.op_Implicit(MathUtils.MaxAbs(float2.op_Implicit(m_MoveAction.ReadValue<Vector2>()), float2.op_Implicit(m_FastMoveAction.ReadValue<Vector2>())));
		rotate = m_RotateAction.ReadValue<Vector2>();
		zoom = m_ZoomAction.ReadValue<float>();
	}
}
