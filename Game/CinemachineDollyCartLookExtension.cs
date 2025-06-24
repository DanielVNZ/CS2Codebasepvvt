using Cinemachine;
using UnityEngine;

namespace Game;

public class CinemachineDollyCartLookExtension : CinemachineExtension
{
	public struct DollyLookAngleOverride
	{
		public bool m_OverrideLookAngle;

		public Vector3 m_Angle;
	}

	public DollyLookAngleOverride[] m_Angles;

	public float GetMaxPos(bool looped)
	{
		int num = m_Angles.Length - 1;
		if (num < 1)
		{
			return 0f;
		}
		return looped ? (num + 1) : num;
	}

	public virtual float StandardizePos(float pos, bool looped)
	{
		float maxPos = GetMaxPos(looped);
		if (looped && maxPos > 0f)
		{
			pos %= maxPos;
			if (pos < 0f)
			{
				pos += maxPos;
			}
			return pos;
		}
		return Mathf.Clamp(pos, 0f, maxPos);
	}

	private float GetBoundingIndices(float pos, bool looped, out int indexA, out int indexB)
	{
		pos = StandardizePos(pos, looped);
		int num = m_Angles.Length;
		if (num < 2)
		{
			indexA = (indexB = 0);
		}
		else
		{
			indexA = Mathf.FloorToInt(pos);
			if (indexA >= num)
			{
				pos -= GetMaxPos(looped);
				indexA = 0;
			}
			indexB = indexA + 1;
			if (indexB == num)
			{
				if (looped)
				{
					indexB = 0;
				}
				else
				{
					indexB--;
					indexA--;
				}
			}
		}
		return pos;
	}

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, Stage stage, ref CameraState state, float deltaTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if ((int)stage != 1)
		{
			return;
		}
		CinemachineDollyCart component = ((Component)this).GetComponent<CinemachineDollyCart>();
		if (m_Angles.Length != 0 && (int)component.m_PositionUnits == 0)
		{
			CinemachinePathBase path = component.m_Path;
			int indexA;
			int indexB;
			float boundingIndices = GetBoundingIndices(component.m_Position, path.Looped, out indexA, out indexB);
			Quaternion val;
			if (indexA == indexB)
			{
				val = GetAngleOffset(path, indexA);
			}
			else
			{
				Quaternion angleOffset = GetAngleOffset(path, indexA);
				Quaternion angleOffset2 = GetAngleOffset(path, indexB);
				val = Quaternion.Slerp(angleOffset, angleOffset2, boundingIndices - (float)indexA);
			}
			Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
			eulerAngles.z = 0f;
			Quaternion val2 = component.m_Path.EvaluateOrientation(boundingIndices);
			Vector3 eulerAngles2 = ((Quaternion)(ref val2)).eulerAngles;
			eulerAngles2.z = 0f;
			state.RawOrientation = Quaternion.Euler(eulerAngles2 + eulerAngles);
		}
	}

	private Quaternion GetAngleOffset(CinemachinePathBase path, int t)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (m_Angles[t].m_OverrideLookAngle)
		{
			Quaternion val = path.EvaluateOrientation((float)t);
			Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
			eulerAngles.z = 0f;
			return Quaternion.Euler(m_Angles[t].m_Angle - eulerAngles);
		}
		return Quaternion.Euler(0f, 0f, 0f);
	}
}
