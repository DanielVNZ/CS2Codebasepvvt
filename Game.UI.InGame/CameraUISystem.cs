using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class CameraUISystem : UISystemBase
{
	private const string kGroup = "camera";

	private CameraUpdateSystem m_CameraUpdateSystem;

	private GetterValueBinding<Entity> m_FocusedEntityBinding;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		AddBinding((IBinding)(object)(m_FocusedEntityBinding = new GetterValueBinding<Entity>("camera", "focusedEntity", (Func<Entity>)GetFocusedEntity, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("camera", "focusEntity", (Action<Entity>)FocusEntity, (IReader<Entity>)null));
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_FocusedEntityBinding.Update();
	}

	private Entity GetFocusedEntity()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null))
		{
			return Entity.Null;
		}
		return m_CameraUpdateSystem.orbitCameraController.followedEntity;
	}

	private void FocusEntity(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (entity != Entity.Null && (Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null && entity != m_CameraUpdateSystem.orbitCameraController.followedEntity)
		{
			m_CameraUpdateSystem.orbitCameraController.followedEntity = entity;
			m_CameraUpdateSystem.orbitCameraController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
			m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.orbitCameraController;
		}
		if (entity == Entity.Null && m_CameraUpdateSystem.activeCameraController == m_CameraUpdateSystem.orbitCameraController)
		{
			m_CameraUpdateSystem.gamePlayController.TryMatchPosition(m_CameraUpdateSystem.orbitCameraController);
			m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.gamePlayController;
		}
	}

	[Preserve]
	public CameraUISystem()
	{
	}
}
