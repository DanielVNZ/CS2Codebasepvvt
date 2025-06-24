using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UIWhatsNewPanelPrefab : PrefabBase
{
	[Serializable]
	public class UIWhatsNewPanelPage
	{
		public UIWhatsNewPanelPageItem[] m_Items;
	}

	[Serializable]
	public class UIWhatsNewPanelPageItem
	{
		public enum Justify
		{
			Left,
			Center,
			Right
		}

		public UIWhatsNewPanelImage[] m_Images;

		[CanBeNull]
		public string m_TitleId;

		[CanBeNull]
		public string m_SubTitleId;

		[CanBeNull]
		public string m_ParagraphsId;

		public Justify m_Justify;

		[Range(25f, 100f)]
		[Tooltip("The percentage of the total width this item should use.")]
		public int m_Width = 100;
	}

	[Serializable]
	public class UIWhatsNewPanelImage
	{
		[NotNull]
		public string m_Uri;

		public int2 m_AspectRatio;

		[Range(10f, 100f)]
		[Tooltip("The percentage of the total width this image should use.")]
		public int m_Width = 100;
	}

	public UIWhatsNewPanelPage[] m_Pages;

	public override void GetPrefabComponents(HashSet<ComponentType> prefabComponents)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(prefabComponents);
		prefabComponents.Add(ComponentType.ReadWrite<UIWhatsNewPanelPrefabData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		UIWhatsNewPanelPrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<UIWhatsNewPanelPrefabData>(entity);
		DlcRequirement component = base.prefab.GetComponent<DlcRequirement>();
		componentData.m_Id = component.m_Dlc.id;
		((EntityManager)(ref entityManager)).SetComponentData<UIWhatsNewPanelPrefabData>(entity, componentData);
	}
}
