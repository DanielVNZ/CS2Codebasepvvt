using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tutorials;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class EditorTutorialsUISystem : TutorialsUISystem
{
	private const string kEditorGroup = "editorTutorials";

	private EntityQuery m_TutorialCategoryQuery;

	private bool m_EditorTutorialsDisabled;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_018b: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01b5: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01df: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0209: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		base.OnCreate();
		m_TutorialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorTutorialSystem>();
		m_TutorialCategoryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<UIEditorTutorialGroupData>(),
			ComponentType.ReadOnly<UIObjectData>()
		});
		m_UnlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Unlock>(),
			ComponentType.ReadOnly<EditorTutorial>()
		});
		m_EditorTutorialsDisabled = true;
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("editorTutorials", "tutorialsDisabled", (Func<bool>)(() => m_EditorTutorialsDisabled), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("editorTutorials", "tutorialsEnabled", (Func<bool>)(() => m_TutorialSystem.tutorialEnabled), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("editorTutorials", "introActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.Intro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("editorTutorials", "listIntroActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.ListIntro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("editorTutorials", "listOutroActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.ListOutro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Entity>("editorTutorials", "next", (Func<Entity>)(() => m_TutorialSystem.nextListTutorial), (IWriter<Entity>)null, (EqualityComparer<Entity>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Entity>("editorTutorials", "advisorPanelVisible", (Func<Entity>)(() => m_TutorialSystem.nextListTutorial), (IWriter<Entity>)null, (EqualityComparer<Entity>)null));
		RawValueBinding val = new RawValueBinding("editorTutorials", "categories", (Action<IJsonWriter>)BindCategories);
		RawValueBinding binding = val;
		m_TutorialCategoriesBinding = val;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val2 = new RawValueBinding("editorTutorials", "activeTutorial", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			BindTutorial(writer, m_TutorialSystem.activeTutorial);
		});
		binding = val2;
		m_ActiveTutorialBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("editorTutorials", "activeTutorialPhase", (Action<IJsonWriter>)delegate(IJsonWriter writer)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			BindTutorialPhase(writer, m_TutorialSystem.activeTutorialPhase);
		});
		binding = val3;
		m_ActiveTutorialPhaseBinding = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("editorTutorials", "activeList", (Action<IJsonWriter>)base.BindActiveTutorialList);
		binding = val4;
		m_ActiveTutorialListBinding = val4;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)new TriggerBinding<bool>("editorTutorials", "completeListIntro", (Action<bool>)CompleteEditorIntro, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("editorTutorials", "toggleTutorials", (Action)ToggleTutorials));
	}

	private NativeList<UIObjectInfo> GetSortedCategories(Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_TutorialCategoryQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<UIObjectData> val2 = ((EntityQuery)(ref m_TutorialCategoryQuery)).ToComponentDataArray<UIObjectData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<UIObjectInfo> val3 = default(NativeList<UIObjectInfo>);
		val3._002Ector(AllocatorHandle.op_Implicit(allocator));
		for (int i = 0; i < val.Length; i++)
		{
			if (val2[i].m_Group == Entity.Null)
			{
				UIObjectInfo uIObjectInfo = new UIObjectInfo(val[i], val2[i].m_Priority);
				val3.Add(ref uIObjectInfo);
			}
		}
		NativeSortExtension.Sort<UIObjectInfo>(val3);
		val.Dispose();
		val2.Dispose();
		return val3;
	}

	private void BindCategories(IJsonWriter writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> sortedCategories = GetSortedCategories((Allocator)2);
		JsonWriterExtensions.ArrayBegin(writer, sortedCategories.Length);
		for (int i = 0; i < sortedCategories.Length; i++)
		{
			UIObjectInfo uIObjectInfo = sortedCategories[i];
			UITutorialGroupPrefab prefab = m_PrefabSystem.GetPrefab<UITutorialGroupPrefab>(uIObjectInfo.entity);
			writer.TypeBegin(TypeNames.kAdvisorCategory);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, uIObjectInfo.entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("shown");
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialShown>(uIObjectInfo.entity));
			writer.PropertyName("locked");
			writer.Write(false);
			writer.PropertyName("children");
			BindTutorialGroup(writer, uIObjectInfo.entity);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		sortedCategories.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!GameManager.instance.gameMode.IsGame())
		{
			base.OnUpdate();
		}
	}

	private void CompleteEditorIntro(bool value)
	{
		m_TutorialSystem.mode = TutorialMode.Default;
		m_TutorialSystem.tutorialEnabled = value;
	}

	protected override void CompleteActiveTutorialPhase()
	{
		if (GameManager.instance.gameMode.IsEditor())
		{
			m_TutorialSystem.CompleteCurrentTutorialPhase();
		}
	}

	private void ToggleTutorials()
	{
		m_TutorialSystem.tutorialEnabled = !m_TutorialSystem.tutorialEnabled;
		if (m_TutorialSystem.tutorialEnabled)
		{
			World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EditorTutorialSystem>().OnResetTutorials();
		}
	}

	[Preserve]
	public EditorTutorialsUISystem()
	{
	}
}
