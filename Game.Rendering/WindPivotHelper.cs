using System.Collections.Generic;
using UnityEngine;

namespace Game.Rendering;

public class WindPivotHelper : MonoBehaviour
{
	public enum PivotBakeMode
	{
		SingleDecompose,
		HierarchyDecompose
	}

	public PivotBakeMode m_BakedMode;

	public bool m_ShowBasePivot = true;

	public bool m_ShowLevel0Pivot = true;

	public bool m_ShowLevel0Guide = true;

	public bool m_ShowLevel1Pivot = true;

	public bool m_ShowLevel1Guide = true;

	public List<Vector3> m_PivotsP0 = new List<Vector3>();

	public List<Vector3> m_PivotsN0 = new List<Vector3>();

	public List<float> m_PivotsH0 = new List<float>();

	public List<Vector3> m_PivotsR1 = new List<Vector3>();

	public List<Vector3> m_PivotsP1 = new List<Vector3>();

	public List<Vector3> m_PivotsN1 = new List<Vector3>();

	public List<float> m_PivotsH1 = new List<float>();

	public void Clear()
	{
		m_PivotsP0.Clear();
		m_PivotsN0.Clear();
		m_PivotsH0.Clear();
		m_PivotsR1.Clear();
		m_PivotsP1.Clear();
		m_PivotsN1.Clear();
		m_PivotsH1.Clear();
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		float num = Mathf.Max(((Component)this).transform.lossyScale.x, Mathf.Max(((Component)this).transform.lossyScale.y, ((Component)this).transform.lossyScale.z));
		if (m_ShowLevel1Pivot || m_ShowLevel1Guide)
		{
			Color color = default(Color);
			((Color)(ref color))._002Ector(0f, 1f, 1f, 0.1f);
			for (int i = 0; i < m_PivotsP1.Count; i++)
			{
				Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(m_PivotsR1[i]);
				Vector3 val2 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(m_PivotsP1[i]);
				Vector3 val3 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyVector(m_PivotsN1[i] * m_PivotsH1[i]);
				if (m_ShowLevel1Guide)
				{
					Gizmos.color = color;
					Gizmos.DrawLine(val2, val);
				}
				if (m_ShowLevel1Pivot)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(val2, val2 + val3);
					Gizmos.DrawSphere(val2, 0.01f * num);
				}
			}
		}
		if (m_ShowLevel0Pivot || m_ShowLevel0Guide)
		{
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(1f, 1f, 0f, 0.1f);
			for (int j = 0; j < m_PivotsP0.Count; j++)
			{
				Vector3 val4 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(m_PivotsP0[j]);
				Vector3 val5 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyVector(m_PivotsN0[j] * m_PivotsH0[j]);
				if (m_ShowLevel0Guide)
				{
					Gizmos.color = color2;
					Gizmos.DrawLine(val4, ((Component)this).transform.position);
				}
				if (m_ShowLevel0Pivot)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(val4, val4 + val5);
					Gizmos.DrawSphere(val4, 0.01f * num);
				}
			}
		}
		if (m_ShowBasePivot)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(((Component)this).transform.position, 0.1f * num);
		}
	}
}
