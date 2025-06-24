using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.Input;
using Game.Settings;
using Game.Tools;
using UnityEngine;

namespace Game.UI;

public class InputBindings : CompositeBinding, IDisposable
{
	private const string kGroup = "input";

	private const float kCameraInputSensitivity = 0.2f;

	private const float kCameraInputSensitivitySqr = 0.040000003f;

	private CameraController m_CameraController;

	private readonly ValueBinding<bool> m_CameraMovingBinding;

	private readonly EventBinding<bool> m_CameraBarrierBinding;

	private readonly EventBinding<bool> m_ToolBarrierBinding;

	private readonly EventBinding<bool> m_ToolActionPerformedBinding;

	private InputBarrier m_CameraInputBarrier;

	private InputBarrier m_ToolInputBarrier;

	public InputBindings()
	{
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("input", "mouseOverUI", (Func<bool>)(() => InputManager.instance.mouseOverUI), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("input", "hideCursor", (Func<bool>)(() => InputManager.instance.hideCursor), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("input", "controlScheme", (Func<int>)(() => (int)InputManager.instance.activeControlScheme), (IWriter<int>)null, (EqualityComparer<int>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("input", "scrollSensitivity", (Func<float>)(() => SharedSettings.instance.input.finalScrollSensitivity), (IWriter<float>)null, (EqualityComparer<float>)null));
		((CompositeBinding)this).AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Vector2>("input", "gamepadPointerPosition", (Func<Vector2>)(() => InputManager.instance.gamepadPointerPosition), (IWriter<Vector2>)null, (EqualityComparer<Vector2>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_CameraMovingBinding = new ValueBinding<bool>("input", "cameraMoving", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ToolActionPerformedBinding = new EventBinding<bool>("input", "toolActionPerformed", (IWriter<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_CameraBarrierBinding = new EventBinding<bool>("input", "cameraBarrier", (IWriter<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ToolBarrierBinding = new EventBinding<bool>("input", "toolBarrier", (IWriter<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<bool>("input", "onGamepadPointerEvent", (Action<bool>)OnGamepadPointerEvent, (IReader<bool>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<int, int, int, int>("input", "setActiveTextFieldRect", (Action<int, int, int, int>)SetActiveTextfieldRect, (IReader<int>)null, (IReader<int>)null, (IReader<int>)null, (IReader<int>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new GetterValueBinding<bool>("input", "useTextFieldInputBarrier", (Func<bool>)(() => PlatformManager.instance.passThroughVKeyboard), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		m_CameraInputBarrier = InputManager.instance.CreateMapBarrier("Camera", "InputBindings");
		m_ToolInputBarrier = InputManager.instance.CreateMapBarrier("Tool", "InputBindings");
		ToolBaseSystem.EventToolActionPerformed += OnToolActionPerformed;
	}

	public void Dispose()
	{
		m_CameraInputBarrier.Dispose();
		m_ToolInputBarrier.Dispose();
		ToolBaseSystem.EventToolActionPerformed -= OnToolActionPerformed;
	}

	public override bool Update()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if ((Object)(object)m_CameraController != (Object)null || CameraController.TryGet(out m_CameraController))
		{
			foreach (ProxyAction inputAction in m_CameraController.inputActions)
			{
				if (!inputAction.IsInProgress())
				{
					continue;
				}
				Type valueType = inputAction.valueType;
				if (valueType == typeof(float))
				{
					if (Mathf.Abs(inputAction.ReadRawValue<float>()) >= 0.2f)
					{
						flag = true;
						break;
					}
				}
				else if (valueType == typeof(Vector2))
				{
					Vector2 val = inputAction.ReadRawValue<Vector2>(disableAll: true);
					if (((Vector2)(ref val)).sqrMagnitude >= 0.040000003f)
					{
						flag = true;
						break;
					}
				}
			}
		}
		m_CameraMovingBinding.Update(flag);
		m_CameraInputBarrier.blocked = ((EventBindingBase)m_CameraBarrierBinding).observerCount > 0;
		m_ToolInputBarrier.blocked = ((EventBindingBase)m_ToolBarrierBinding).observerCount > 0;
		return ((CompositeBinding)this).Update();
	}

	private void OnToolActionPerformed(ProxyAction action)
	{
		m_ToolActionPerformedBinding.Trigger(true);
	}

	private void OnGamepadPointerEvent(bool pointerOverUI)
	{
		InputManager.instance.mouseOverUI = pointerOverUI;
	}

	private void SetActiveTextfieldRect(int x, int y, int width, int height)
	{
		PlatformManager instance = PlatformManager.instance;
		if (instance != null)
		{
			instance.SetActiveTextFieldRect(x, y, width, height);
		}
	}
}
