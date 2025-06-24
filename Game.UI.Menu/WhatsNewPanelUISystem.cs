using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Settings;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

[CompilerGenerated]
public class WhatsNewPanelUISystem : UISystemBase
{
	private struct DlcComparer : IComparer<Entity>
	{
		private EntityManager m_EntityManager;

		public DlcComparer(EntityManager entityManager)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_EntityManager = entityManager;
		}

		public int Compare(Entity a, Entity b)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			int num = ((EntityManager)(ref m_EntityManager)).GetComponentData<UIWhatsNewPanelPrefabData>(a).m_Id.CompareTo(((EntityManager)(ref m_EntityManager)).GetComponentData<UIWhatsNewPanelPrefabData>(b).m_Id);
			if (num == 0)
			{
				return ((Entity)(ref a)).CompareTo(b);
			}
			return num;
		}
	}

	private const string kGroup = "whatsnew";

	private RawValueBinding m_PanelBinding;

	private ValueBinding<bool> m_VisibilityBinding;

	private ValueBinding<int> m_InitialTabBinding;

	private EntityQuery m_Query;

	private PrefabSystem m_PrefabSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_003b: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		RawValueBinding val = new RawValueBinding("whatsnew", "panel", (Action<IJsonWriter>)BindPanel);
		RawValueBinding binding = val;
		m_PanelBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_VisibilityBinding = new ValueBinding<bool>("whatsnew", "visible", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_InitialTabBinding = new ValueBinding<int>("whatsnew", "initialTab", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("whatsnew", "close", (Action<bool>)OnClose, (IReader<bool>)null));
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIWhatsNewPanelPrefabData>() });
	}

	private void OnClose(bool dismiss)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		m_VisibilityBinding.Update(false);
		if (!dismiss)
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_Query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		NativeSortExtension.Sort<Entity, DlcComparer>(val, new DlcComparer(((ComponentSystemBase)this).EntityManager));
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				UIWhatsNewPanelPrefab prefab = m_PrefabSystem.GetPrefab<UIWhatsNewPanelPrefab>(current);
				DlcRequirement dlcRequirement = (((Object)(object)prefab == (Object)null) ? null : prefab.GetComponent<DlcRequirement>());
				if (!((Object)(object)dlcRequirement == (Object)null) && dlcRequirement.CheckRequirement())
				{
					string dlcName = PlatformManager.instance.GetDlcName(dlcRequirement.m_Dlc);
					if (!SharedSettings.instance.userState.seenWhatsNew.Contains(dlcName))
					{
						SharedSettings.instance.userState.seenWhatsNew.Add(dlcName);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		SharedSettings.instance.userInterface.showWhatsNewPanel = false;
	}

	protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (mode != GameMode.MainMenu)
		{
			return;
		}
		List<UIWhatsNewPanelPrefab> sortedWhatsNewTabs = GetSortedWhatsNewTabs(m_Query);
		int initialTab = int.MaxValue;
		bool flag = false;
		foreach (UIWhatsNewPanelPrefab item in sortedWhatsNewTabs)
		{
			DlcRequirement dlcRequirement = (((Object)(object)item == (Object)null) ? null : item.GetComponent<DlcRequirement>());
			if (!((Object)(object)dlcRequirement == (Object)null) && dlcRequirement.CheckRequirement() && !SharedSettings.instance.userState.seenWhatsNew.Contains(PlatformManager.instance.GetDlcName(dlcRequirement.m_Dlc)))
			{
				if (dlcRequirement.m_Dlc.id < initialTab)
				{
					initialTab = dlcRequirement.m_Dlc.id;
				}
				flag = true;
			}
		}
		if (flag)
		{
			int num = sortedWhatsNewTabs.FindIndex((UIWhatsNewPanelPrefab p) => p.GetComponent<DlcRequirement>().m_Dlc.id == initialTab);
			m_InitialTabBinding.Update((num >= 0) ? num : (sortedWhatsNewTabs.Count - 1));
			SharedSettings.instance.userInterface.showWhatsNewPanel = true;
		}
		else
		{
			m_InitialTabBinding.Update(sortedWhatsNewTabs.Count - 1);
		}
		m_PanelBinding.Update();
		m_VisibilityBinding.Update(SharedSettings.instance.userInterface.showWhatsNewPanel);
	}

	private void BindPanel(IJsonWriter writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		List<UIWhatsNewPanelPrefab> sortedWhatsNewTabs = GetSortedWhatsNewTabs(m_Query);
		JsonWriterExtensions.ArrayBegin(writer, sortedWhatsNewTabs.Count);
		for (int i = 0; i < sortedWhatsNewTabs.Count; i++)
		{
			UIWhatsNewPanelPrefab uIWhatsNewPanelPrefab = sortedWhatsNewTabs[i];
			DlcRequirement component = uIWhatsNewPanelPrefab.GetComponent<DlcRequirement>();
			writer.TypeBegin(typeof(UIWhatsNewPanelPrefab).FullName);
			writer.PropertyName("id");
			writer.Write(component.m_Dlc.id);
			writer.PropertyName("dlc");
			writer.Write(PlatformManager.instance.GetDlcName(component.m_Dlc));
			writer.PropertyName("pages");
			JsonWriterExtensions.ArrayBegin(writer, uIWhatsNewPanelPrefab.m_Pages.Length);
			for (int j = 0; j < uIWhatsNewPanelPrefab.m_Pages.Length; j++)
			{
				UIWhatsNewPanelPrefab.UIWhatsNewPanelPage uIWhatsNewPanelPage = uIWhatsNewPanelPrefab.m_Pages[j];
				writer.TypeBegin(typeof(UIWhatsNewPanelPrefab.UIWhatsNewPanelPage).FullName);
				writer.PropertyName("items");
				JsonWriterExtensions.ArrayBegin(writer, uIWhatsNewPanelPage.m_Items.Length);
				for (int k = 0; k < uIWhatsNewPanelPage.m_Items.Length; k++)
				{
					UIWhatsNewPanelPrefab.UIWhatsNewPanelPageItem uIWhatsNewPanelPageItem = uIWhatsNewPanelPage.m_Items[k];
					writer.TypeBegin(typeof(UIWhatsNewPanelPrefab.UIWhatsNewPanelPageItem).FullName);
					writer.PropertyName("images");
					JsonWriterExtensions.ArrayBegin(writer, uIWhatsNewPanelPageItem.m_Images.Length);
					for (int l = 0; l < uIWhatsNewPanelPageItem.m_Images.Length; l++)
					{
						UIWhatsNewPanelPrefab.UIWhatsNewPanelImage uIWhatsNewPanelImage = uIWhatsNewPanelPageItem.m_Images[l];
						writer.TypeBegin(typeof(UIWhatsNewPanelPrefab.UIWhatsNewPanelImage).FullName);
						writer.PropertyName("image");
						writer.Write(uIWhatsNewPanelImage.m_Uri);
						writer.PropertyName("aspectRatio");
						MathematicsWriters.Write(writer, uIWhatsNewPanelImage.m_AspectRatio);
						writer.PropertyName("width");
						writer.Write(uIWhatsNewPanelImage.m_Width);
						writer.TypeEnd();
					}
					writer.ArrayEnd();
					writer.PropertyName("title");
					if (uIWhatsNewPanelPageItem.m_TitleId != null)
					{
						writer.Write(uIWhatsNewPanelPageItem.m_TitleId);
					}
					else
					{
						writer.WriteNull();
					}
					writer.PropertyName("subtitle");
					if (uIWhatsNewPanelPageItem.m_SubTitleId != null)
					{
						writer.Write(uIWhatsNewPanelPageItem.m_SubTitleId);
					}
					else
					{
						writer.WriteNull();
					}
					writer.PropertyName("paragraphs");
					if (uIWhatsNewPanelPageItem.m_ParagraphsId != null)
					{
						writer.Write(uIWhatsNewPanelPageItem.m_ParagraphsId);
					}
					else
					{
						writer.WriteNull();
					}
					writer.PropertyName("justify");
					writer.Write((int)uIWhatsNewPanelPageItem.m_Justify);
					writer.PropertyName("width");
					writer.Write(uIWhatsNewPanelPageItem.m_Width);
					writer.TypeEnd();
				}
				writer.ArrayEnd();
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	private List<UIWhatsNewPanelPrefab> GetSortedWhatsNewTabs(EntityQuery query)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		NativeSortExtension.Sort<Entity, DlcComparer>(val, new DlcComparer(((ComponentSystemBase)this).EntityManager));
		List<UIWhatsNewPanelPrefab> list = new List<UIWhatsNewPanelPrefab>();
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				UIWhatsNewPanelPrefab prefab = m_PrefabSystem.GetPrefab<UIWhatsNewPanelPrefab>(current);
				DlcRequirement component = prefab.GetComponent<DlcRequirement>();
				if (PlatformManager.instance.IsDlcOwned(component.m_Dlc))
				{
					list.Add(prefab);
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[Preserve]
	public WhatsNewPanelUISystem()
	{
	}
}
