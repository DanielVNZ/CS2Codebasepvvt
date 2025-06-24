using UnityEngine;

public class DebugCamera : MonoBehaviour
{
	private float mainSpeed = 100f;

	private float shiftAdd = 250f;

	private float maxShift = 1000f;

	private float camSens = 0.25f;

	private Vector3 lastMouse = new Vector3(255f, 255f, 255f);

	private float totalRun = 1f;

	private void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		lastMouse = Input.mousePosition - lastMouse;
		lastMouse = new Vector3((0f - lastMouse.y) * camSens, lastMouse.x * camSens, 0f);
		lastMouse = new Vector3(((Component)this).transform.eulerAngles.x + lastMouse.x, ((Component)this).transform.eulerAngles.y + lastMouse.y, 0f);
		((Component)this).transform.eulerAngles = lastMouse;
		lastMouse = Input.mousePosition;
		Vector3 baseInput = GetBaseInput();
		if (Input.GetKey((KeyCode)304))
		{
			totalRun += Time.deltaTime;
			baseInput = baseInput * totalRun * shiftAdd;
			baseInput.x = Mathf.Clamp(baseInput.x, 0f - maxShift, maxShift);
			baseInput.y = Mathf.Clamp(baseInput.y, 0f - maxShift, maxShift);
			baseInput.z = Mathf.Clamp(baseInput.z, 0f - maxShift, maxShift);
		}
		else
		{
			totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
			baseInput *= mainSpeed;
		}
		baseInput *= Time.deltaTime;
		Vector3 position = ((Component)this).transform.position;
		if (Input.GetKey((KeyCode)32))
		{
			((Component)this).transform.Translate(baseInput);
			position.x = ((Component)this).transform.position.x;
			position.z = ((Component)this).transform.position.z;
			((Component)this).transform.position = position;
		}
		else
		{
			((Component)this).transform.Translate(baseInput);
		}
	}

	private Vector3 GetBaseInput()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		if (Input.GetKey((KeyCode)119))
		{
			val += new Vector3(0f, 0f, 1f);
		}
		if (Input.GetKey((KeyCode)115))
		{
			val += new Vector3(0f, 0f, -1f);
		}
		if (Input.GetKey((KeyCode)97))
		{
			val += new Vector3(-1f, 0f, 0f);
		}
		if (Input.GetKey((KeyCode)100))
		{
			val += new Vector3(1f, 0f, 0f);
		}
		if (Input.GetKey((KeyCode)113))
		{
			val += new Vector3(0f, -1f, 0f);
		}
		if (Input.GetKey((KeyCode)101))
		{
			val += new Vector3(0f, 1f, 0f);
		}
		return val;
	}
}
