using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Common;
using Game.Notifications;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { })]
public class NotificationIconPrefab : PrefabBase
{
	public Texture2D m_Icon;

	public string m_Description;

	public string m_TargetDescription;

	public Bounds1 m_DisplaySize = new Bounds1(3f, 3f);

	public Bounds1 m_PulsateAmplitude = new Bounds1(0.01f, 0.1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NotificationIconData>());
		components.Add(ComponentType.ReadWrite<NotificationIconDisplayData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Icon>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		RefreshArchetype(entityManager, entity);
	}

	protected virtual void RefreshArchetype(EntityManager entityManager, Entity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		((EntityManager)(ref entityManager)).SetComponentData<NotificationIconData>(entity, new NotificationIconData
		{
			m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet))
		});
	}
}
