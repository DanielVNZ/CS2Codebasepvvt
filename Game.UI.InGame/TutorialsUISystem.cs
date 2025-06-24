using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tutorials;
using Game.UI.Editor;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TutorialsUISystem : UISystemBase
{
	internal static class BindingNames
	{
		internal const string kTutorialsDisabled = "tutorialsDisabled";

		internal const string kTutorialsEnabled = "tutorialsEnabled";

		internal const string kIntroActive = "introActive";

		internal const string kNext = "next";

		internal const string kActiveTutorial = "activeTutorial";

		internal const string kActiveTutorialPhase = "activeTutorialPhase";

		internal const string kCategories = "categories";

		internal const string kTutorials = "tutorials";

		internal const string kPending = "pending";

		internal const string kActiveList = "activeList";

		internal const string kActivateTutorial = "activateTutorial";

		internal const string kActivateTutorialPhase = "activateTutorialPhase";

		internal const string kForceTutorial = "forceTutorial";

		internal const string kActivateTutorialTrigger = "activateTutorialTrigger";

		internal const string kDisactivateTutorialTrigger = "disactivateTutorialTrigger";

		internal const string kSetTutorialTagActive = "setTutorialTagActive";

		internal const string kCompleteActiveTutorialPhase = "completeActiveTutorialPhase";

		internal const string kCompleteActiveTutorial = "completeActiveTutorial";

		internal const string kCompleteIntro = "completeIntro";

		internal const string kCompleteListIntro = "completeListIntro";

		internal const string kCompleteListOutro = "completeListOutro";

		internal const string kListIntroActive = "listIntroActive";

		internal const string kListOutroActive = "listOutroActive";

		internal const string kControlScheme = "controlScheme";

		internal const string kAdvisorPanelVisible = "advisorPanelVisible";

		internal const string kToggleTutorials = "toggleTutorials";
	}

	private enum AdvisorItemType
	{
		Tutorial,
		Group
	}

	private struct TypeHandle
	{
		public ComponentLookup<TutorialActivationData> __Game_Tutorials_TutorialActivationData_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tutorials_TutorialActivationData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TutorialActivationData>(false);
		}
	}

	private const string kGroup = "tutorials";

	protected PrefabSystem m_PrefabSystem;

	protected ITutorialSystem m_TutorialSystem;

	private ITutorialSystem m_EditorTutorialSystem;

	protected ITutorialUIActivationSystem m_ActivationSystem;

	protected ITutorialUIDeactivationSystem m_DeactivationSystem;

	protected ITutorialUITriggerSystem m_TriggerSystem;

	private EntityQuery m_TutorialConfigurationQuery;

	private EntityQuery m_TutorialCategoryQuery;

	protected EntityQuery m_UnlockQuery;

	protected RawValueBinding m_ActiveTutorialListBinding;

	protected RawValueBinding m_TutorialCategoriesBinding;

	private RawMapBinding<Entity> m_TutorialsBinding;

	protected RawValueBinding m_ActiveTutorialBinding;

	protected RawValueBinding m_ActiveTutorialPhaseBinding;

	private GetterValueBinding<Entity> m_TutorialPendingBinding;

	private int m_TutorialActiveVersion;

	private int m_PhaseActiveVersion;

	private int m_TriggerActiveVersion;

	private int m_TriggerCompletedVersion;

	private int m_TutorialShownVersion;

	private int m_PhaseShownVersion;

	private int m_PhaseCompletedVersion;

	private bool m_WasEnabled;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00da: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0342: Expected O, but got Unknown
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Expected O, but got Unknown
		//IL_03c4: Expected O, but got Unknown
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Expected O, but got Unknown
		//IL_03ee: Expected O, but got Unknown
		base.OnCreate();
		((ComponentSystemBase)this).Enabled = false;
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TutorialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialSystem>();
		m_ActivationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUIActivationSystem>();
		m_DeactivationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUIDeactivationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUITriggerSystem>();
		m_TutorialCategoryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<UITutorialGroupData>(),
			ComponentType.Exclude<UIEditorTutorialGroupData>(),
			ComponentType.ReadOnly<UIObjectData>()
		});
		m_UnlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		RawValueBinding val = new RawValueBinding("tutorials", "activeList", (Action<IJsonWriter>)BindActiveTutorialList);
		RawValueBinding binding = val;
		m_ActiveTutorialListBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("tutorials", "activateTutorial", (Action<Entity>)ActivateTutorial, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, Entity>("tutorials", "activateTutorialPhase", (Action<Entity, Entity>)ActivateTutorialPhase, (IReader<Entity>)null, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, Entity, bool>("tutorials", "forceTutorial", (Action<Entity, Entity, bool>)ForceTutorial, (IReader<Entity>)null, (IReader<Entity>)null, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("tutorials", "completeActiveTutorialPhase", (Action)CompleteActiveTutorialPhase));
		AddBinding((IBinding)new TriggerBinding("tutorials", "completeActiveTutorial", (Action)CompleteActiveTutorial));
		AddBinding((IBinding)(object)new TriggerBinding<string, bool>("tutorials", "setTutorialTagActive", (Action<string, bool>)OnSetTutorialTagActive, (IReader<string>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("tutorials", "activateTutorialTrigger", (Action<string>)ActivateTutorialTrigger, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("tutorials", "disactivateTutorialTrigger", (Action<string>)DisactivateTutorialTrigger, (IReader<string>)null));
		if (!(((object)this).GetType() == typeof(EditorTutorialsUISystem)))
		{
			AddBinding((IBinding)new TriggerBinding("tutorials", "completeListIntro", (Action)CompleteIntro));
			AddBinding((IBinding)new TriggerBinding("tutorials", "completeListOutro", (Action)CompleteOutro));
			AddBinding((IBinding)(object)new TriggerBinding<bool>("tutorials", "completeIntro", (Action<bool>)CompleteIntro, (IReader<bool>)null));
			AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tutorials", "tutorialsEnabled", (Func<bool>)(() => m_TutorialSystem.tutorialEnabled), (IWriter<bool>)null, (EqualityComparer<bool>)null));
			AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tutorials", "introActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.Intro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
			AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tutorials", "listIntroActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.ListIntro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
			AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("tutorials", "listOutroActive", (Func<bool>)(() => m_TutorialSystem.mode == TutorialMode.ListOutro), (IWriter<bool>)null, (EqualityComparer<bool>)null));
			AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<Entity>("tutorials", "next", (Func<Entity>)(() => m_TutorialSystem.nextListTutorial), (IWriter<Entity>)null, (EqualityComparer<Entity>)null));
			RawValueBinding val2 = new RawValueBinding("tutorials", "categories", (Action<IJsonWriter>)BindCategories);
			binding = val2;
			m_TutorialCategoriesBinding = val2;
			AddBinding((IBinding)(object)binding);
			AddBinding((IBinding)(object)(m_TutorialsBinding = new RawMapBinding<Entity>("tutorials", "tutorials", (Action<IJsonWriter, Entity>)BindTutorial, (IReader<Entity>)null, (IWriter<Entity>)null)));
			AddBinding((IBinding)(object)(m_TutorialPendingBinding = new GetterValueBinding<Entity>("tutorials", "pending", (Func<Entity>)(() => m_TutorialSystem.tutorialPending), (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
			RawValueBinding val3 = new RawValueBinding("tutorials", "activeTutorial", (Action<IJsonWriter>)delegate(IJsonWriter writer)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				BindTutorial(writer, m_TutorialSystem.activeTutorial);
			});
			binding = val3;
			m_ActiveTutorialBinding = val3;
			AddBinding((IBinding)(object)binding);
			RawValueBinding val4 = new RawValueBinding("tutorials", "activeTutorialPhase", (Action<IJsonWriter>)delegate(IJsonWriter writer)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				BindTutorialPhase(writer, m_TutorialSystem.activeTutorialPhase);
			});
			binding = val4;
			m_ActiveTutorialPhaseBinding = val4;
			AddBinding((IBinding)(object)binding);
			m_WasEnabled = m_TutorialSystem.tutorialEnabled;
			InputManager.instance.EventControlSchemeChanged += OnControlSchemeChanged;
		}
	}

	private void OnControlSchemeChanged(InputManager.ControlScheme controlScheme)
	{
		((MapBindingBase<Entity>)(object)m_TutorialsBinding).UpdateAll();
	}

	private void CompleteIntro()
	{
		m_TutorialSystem.mode = TutorialMode.Default;
	}

	private void CompleteOutro()
	{
		m_TutorialSystem.mode = TutorialMode.Default;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TutorialActive>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion2 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TutorialPhaseActive>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion3 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TutorialPhaseCompleted>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion4 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TutorialPhaseShown>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion5 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TriggerActive>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion6 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TriggerCompleted>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int componentOrderVersion7 = ((EntityManager)(ref entityManager)).GetComponentOrderVersion<TutorialShown>();
		bool flag = componentOrderVersion != m_TutorialActiveVersion;
		bool flag2 = componentOrderVersion2 != m_PhaseActiveVersion;
		bool flag3 = componentOrderVersion5 != m_TriggerActiveVersion;
		bool flag4 = componentOrderVersion6 != m_TriggerCompletedVersion;
		bool flag5 = componentOrderVersion7 != m_TutorialShownVersion;
		bool flag6 = componentOrderVersion4 != m_PhaseShownVersion;
		bool flag7 = componentOrderVersion3 != m_PhaseCompletedVersion;
		if (flag)
		{
			m_ActiveTutorialListBinding.Update();
		}
		if (m_TutorialsBinding != null && (flag || flag2 || flag3 || flag4 || flag7))
		{
			m_ActiveTutorialBinding.Update();
			m_ActiveTutorialPhaseBinding.Update();
			((MapBindingBase<Entity>)(object)m_TutorialsBinding).Update(m_TutorialSystem.activeTutorial);
		}
		if (PrefabUtils.HasUnlockedPrefabAny<TutorialData, TutorialPhaseData, TutorialTriggerData, TutorialListData>(((ComponentSystemBase)this).EntityManager, m_UnlockQuery) || m_WasEnabled != m_TutorialSystem.tutorialEnabled || flag5 || flag6)
		{
			m_TutorialCategoriesBinding.Update();
			if (m_TutorialsBinding != null)
			{
				((MapBindingBase<Entity>)(object)m_TutorialsBinding).UpdateAll();
			}
		}
		if (m_TutorialPendingBinding != null)
		{
			m_TutorialPendingBinding.Update();
		}
		m_TutorialActiveVersion = componentOrderVersion;
		m_PhaseActiveVersion = componentOrderVersion2;
		m_TriggerActiveVersion = componentOrderVersion5;
		m_TriggerCompletedVersion = componentOrderVersion6;
		m_TutorialShownVersion = componentOrderVersion7;
		m_PhaseShownVersion = componentOrderVersion4;
		m_PhaseCompletedVersion = componentOrderVersion3;
		if (m_TutorialSystem != null)
		{
			m_WasEnabled = m_TutorialSystem.tutorialEnabled;
		}
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
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
			writer.PropertyName("force");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<ForceAdvisor>(uIObjectInfo.entity));
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, uIObjectInfo.entity));
			writer.PropertyName("children");
			BindTutorialGroup(writer, uIObjectInfo.entity);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		sortedCategories.Dispose();
	}

	protected void BindTutorialGroup(IJsonWriter writer, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref elements))
		{
			NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
			for (int i = 0; i < sortedObjects.Length; i++)
			{
				Entity entity2 = sortedObjects[i].entity;
				PrefabData prefabData = sortedObjects[i].prefabData;
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(prefabData);
				UIObject component = prefab.GetComponent<UIObject>();
				writer.TypeBegin(TypeNames.kAdvisorItem);
				writer.PropertyName("entity");
				UnityWriters.Write(writer, entity2);
				writer.PropertyName("name");
				writer.Write(((Object)prefab).name);
				writer.PropertyName("icon");
				if (component.m_Icon == null)
				{
					writer.WriteNull();
				}
				else
				{
					writer.Write(component.m_Icon);
				}
				writer.PropertyName("type");
				writer.Write((!(prefab is TutorialPrefab)) ? 1 : 0);
				writer.PropertyName("shown");
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialShown>(entity2));
				writer.PropertyName("force");
				entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<ForceAdvisor>(entity2));
				writer.PropertyName("locked");
				writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity2));
				writer.PropertyName("children");
				BindTutorialGroup(writer, entity2);
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			sortedObjects.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
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

	protected void BindActiveTutorialList(IJsonWriter writer)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		Entity activeTutorialList = m_TutorialSystem.activeTutorialList;
		if (activeTutorialList != Entity.Null)
		{
			TutorialListPrefab prefab = m_PrefabSystem.GetPrefab<TutorialListPrefab>(activeTutorialList);
			NativeList<Entity> visibleListTutorials = GetVisibleListTutorials(activeTutorialList, (Allocator)2);
			NativeList<Entity> listHintTutorials = GetListHintTutorials(activeTutorialList, (Allocator)2);
			try
			{
				writer.TypeBegin(TypeNames.kTutorialList);
				writer.PropertyName("entity");
				UnityWriters.Write(writer, activeTutorialList);
				writer.PropertyName("name");
				writer.Write(((Object)prefab).name);
				writer.PropertyName("tutorials");
				JsonWriterExtensions.ArrayBegin(writer, visibleListTutorials.Length);
				Enumerator<Entity> enumerator = visibleListTutorials.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Entity current = enumerator.Current;
						BindTutorial(writer, current);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				writer.ArrayEnd();
				writer.PropertyName("hints");
				JsonWriterExtensions.ArrayBegin(writer, listHintTutorials.Length);
				enumerator = listHintTutorials.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Entity current2 = enumerator.Current;
						BindTutorial(writer, current2);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				writer.ArrayEnd();
				writer.PropertyName("intro");
				writer.Write(m_TutorialSystem.showListReminder);
				writer.TypeEnd();
				return;
			}
			finally
			{
				visibleListTutorials.Dispose();
			}
		}
		writer.WriteNull();
	}

	private NativeList<Entity> GetVisibleListTutorials(Entity listEntity, Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TutorialRef> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialRef>(listEntity, true);
		ComponentLookup<TutorialActivationData> componentLookup = InternalCompilerInterface.GetComponentLookup<TutorialActivationData>(ref __TypeHandle.__Game_Tutorials_TutorialActivationData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeList<Entity> result = default(NativeList<Entity>);
		result._002Ector(buffer.Length, AllocatorHandle.op_Implicit(allocator));
		Enumerator<TutorialRef> enumerator = buffer.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TutorialRef current = enumerator.Current;
				if (!componentLookup.HasComponent(current.m_Tutorial))
				{
					result.Add(ref current.m_Tutorial);
				}
			}
			return result;
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private NativeList<Entity> GetListHintTutorials(Entity listEntity, Allocator allocator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TutorialRef> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialRef>(listEntity, true);
		ComponentLookup<TutorialActivationData> componentLookup = InternalCompilerInterface.GetComponentLookup<TutorialActivationData>(ref __TypeHandle.__Game_Tutorials_TutorialActivationData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeList<Entity> result = default(NativeList<Entity>);
		result._002Ector(buffer.Length, AllocatorHandle.op_Implicit(allocator));
		Enumerator<TutorialRef> enumerator = buffer.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TutorialRef current = enumerator.Current;
				if (componentLookup.HasComponent(current.m_Tutorial))
				{
					result.Add(ref current.m_Tutorial);
				}
			}
			return result;
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private bool AlternativeCompleted(Entity tutorial)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Tutorials.TutorialAlternative> val = default(DynamicBuffer<Game.Tutorials.TutorialAlternative>);
		if (EntitiesExtensions.TryGetBuffer<Game.Tutorials.TutorialAlternative>(((ComponentSystemBase)this).EntityManager, tutorial, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<TutorialCompleted>(val[i].m_Alternative))
				{
					return true;
				}
			}
		}
		return false;
	}

	protected void BindTutorial(IJsonWriter writer, Entity tutorialEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<TutorialData>(tutorialEntity))
		{
			TutorialPrefab prefab = m_PrefabSystem.GetPrefab<TutorialPrefab>(tutorialEntity);
			writer.TypeBegin(TypeNames.kTutorial);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, tutorialEntity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab));
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, tutorialEntity));
			writer.PropertyName("priority");
			writer.Write(prefab.m_Priority);
			writer.PropertyName("active");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialActive>(tutorialEntity));
			writer.PropertyName("completed");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialCompleted>(tutorialEntity) || AlternativeCompleted(tutorialEntity));
			writer.PropertyName("shown");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialShown>(tutorialEntity));
			writer.PropertyName("force");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<ForceAdvisor>(tutorialEntity));
			writer.PropertyName("mandatory");
			writer.Write(prefab.m_Mandatory);
			writer.PropertyName("advisorActivation");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<AdvisorActivation>(m_TutorialSystem.activeTutorial));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<TutorialPhaseRef> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TutorialPhaseRef>(tutorialEntity, true);
			writer.PropertyName("phases");
			int num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				if (TutorialSystem.IsValidControlScheme(buffer[i].m_Phase, m_PrefabSystem))
				{
					num++;
				}
			}
			JsonWriterExtensions.ArrayBegin(writer, num);
			for (int j = 0; j < buffer.Length; j++)
			{
				Entity phase = buffer[j].m_Phase;
				if (TutorialSystem.IsValidControlScheme(phase, m_PrefabSystem))
				{
					BindTutorialPhase(writer, phase);
				}
			}
			writer.ArrayEnd();
			writer.PropertyName("filters");
			JsonWriterExtensions.Write(writer, GetFilters(prefab));
			writer.PropertyName("alternatives");
			DynamicBuffer<Game.Tutorials.TutorialAlternative> val = default(DynamicBuffer<Game.Tutorials.TutorialAlternative>);
			if (EntitiesExtensions.TryGetBuffer<Game.Tutorials.TutorialAlternative>(((ComponentSystemBase)this).EntityManager, tutorialEntity, true, ref val))
			{
				JsonWriterExtensions.ArrayBegin(writer, val.Length);
				for (int k = 0; k < val.Length; k++)
				{
					UnityWriters.Write(writer, val[k].m_Alternative);
				}
				writer.ArrayEnd();
			}
			else
			{
				writer.WriteNull();
			}
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	protected void BindTutorialPhase(IJsonWriter writer, Entity phaseEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		TutorialPhaseData tutorialPhaseData = default(TutorialPhaseData);
		if (EntitiesExtensions.TryGetComponent<TutorialPhaseData>(((ComponentSystemBase)this).EntityManager, phaseEntity, ref tutorialPhaseData))
		{
			TutorialPhasePrefab prefab = m_PrefabSystem.GetPrefab<TutorialPhasePrefab>(phaseEntity);
			TutorialBalloonPrefab tutorialBalloonPrefab = prefab as TutorialBalloonPrefab;
			writer.TypeBegin(TypeNames.kTutorialPhase);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, phaseEntity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("type");
			writer.Write((int)tutorialPhaseData.m_Type);
			writer.PropertyName("active");
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialPhaseActive>(phaseEntity));
			writer.PropertyName("shown");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialPhaseShown>(phaseEntity));
			writer.PropertyName("force");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<ForceAdvisor>(phaseEntity));
			writer.PropertyName("completed");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialPhaseCompleted>(phaseEntity));
			writer.PropertyName("forcesCompletion");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<Game.Tutorials.ForceTutorialCompletion>(phaseEntity));
			writer.PropertyName("isBranch");
			entityManager = ((ComponentSystemBase)this).EntityManager;
			writer.Write(((EntityManager)(ref entityManager)).HasComponent<TutorialPhaseBranch>(phaseEntity));
			writer.PropertyName("image");
			writer.Write((!string.IsNullOrWhiteSpace(prefab.m_Image)) ? prefab.m_Image : null);
			writer.PropertyName("overrideImagePS");
			writer.Write((!string.IsNullOrWhiteSpace(prefab.m_OverrideImagePS)) ? prefab.m_OverrideImagePS : null);
			writer.PropertyName("overrideImageXbox");
			writer.Write((!string.IsNullOrWhiteSpace(prefab.m_OverrideImageXBox)) ? prefab.m_OverrideImageXBox : null);
			writer.PropertyName("icon");
			writer.Write((!string.IsNullOrWhiteSpace(prefab.m_Icon)) ? prefab.m_Icon : null);
			writer.PropertyName("titleVisible");
			writer.Write(prefab.m_TitleVisible);
			writer.PropertyName("descriptionVisible");
			writer.Write(prefab.m_DescriptionVisible);
			writer.PropertyName("balloonTargets");
			if ((Object)(object)tutorialBalloonPrefab == (Object)null)
			{
				JsonWriterExtensions.WriteEmptyArray(writer);
			}
			else
			{
				JsonWriterExtensions.Write<TutorialBalloonPrefab.BalloonUITarget>(writer, (IList<TutorialBalloonPrefab.BalloonUITarget>)tutorialBalloonPrefab.m_UITargets);
			}
			writer.PropertyName("controlScheme");
			writer.Write((int)prefab.m_ControlScheme);
			writer.PropertyName("trigger");
			TutorialTrigger tutorialTrigger = default(TutorialTrigger);
			if (EntitiesExtensions.TryGetComponent<TutorialTrigger>(((ComponentSystemBase)this).EntityManager, phaseEntity, ref tutorialTrigger))
			{
				TutorialTriggerPrefabBase prefab2 = m_PrefabSystem.GetPrefab<TutorialTriggerPrefabBase>(tutorialTrigger.m_Trigger);
				writer.TypeBegin(TypeNames.kTutorialTrigger);
				writer.PropertyName("entity");
				UnityWriters.Write(writer, tutorialTrigger.m_Trigger);
				writer.PropertyName("name");
				writer.Write(((Object)prefab2).name);
				Dictionary<int, List<string>> blinkTags = prefab2.GetBlinkTags();
				List<int> list = blinkTags.Keys.ToList();
				list.Sort();
				writer.PropertyName("blinkTags");
				JsonWriterExtensions.ArrayBegin(writer, list.Count);
				foreach (int item in list)
				{
					List<string> list2 = blinkTags[item];
					JsonWriterExtensions.Write(writer, (IList<string>)list2);
				}
				writer.ArrayEnd();
				writer.PropertyName("displayUI");
				writer.Write(prefab2.m_DisplayUI);
				writer.PropertyName("active");
				entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<TriggerActive>(tutorialTrigger.m_Trigger));
				writer.PropertyName("completed");
				entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<TriggerCompleted>(tutorialTrigger.m_Trigger));
				writer.PropertyName("preCompleted");
				entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<TriggerPreCompleted>(tutorialTrigger.m_Trigger));
				writer.PropertyName("phaseBranching");
				writer.Write(prefab2.phaseBranching);
				writer.TypeEnd();
			}
			else
			{
				writer.WriteNull();
			}
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	private void ActivateTutorial(Entity tutorial)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialSystem.SetTutorial(tutorial, Entity.Null);
	}

	private void ActivateTutorialPhase(Entity tutorial, Entity phase)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialSystem.SetTutorial(tutorial, phase);
	}

	private void ForceTutorial(Entity tutorial, Entity phase, bool advisorActivation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialSystem.ForceTutorial(tutorial, phase, advisorActivation);
	}

	protected virtual void CompleteActiveTutorialPhase()
	{
		if (GameManager.instance.gameMode.IsGame())
		{
			m_TutorialSystem.CompleteCurrentTutorialPhase();
		}
	}

	private void CompleteActiveTutorial()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialSystem.CompleteTutorial(m_TutorialSystem.activeTutorial);
	}

	private void CompleteIntro(bool tutorialEnabled)
	{
		m_TutorialSystem.mode = TutorialMode.Default;
		m_TutorialSystem.tutorialEnabled = tutorialEnabled;
	}

	private void OnSetTutorialTagActive(string tag, bool active)
	{
		m_ActivationSystem.SetTag(tag, active);
		if (!active)
		{
			m_DeactivationSystem.DeactivateTag(tag);
		}
	}

	private void ActivateTutorialTrigger(string trigger)
	{
		m_TriggerSystem.ActivateTrigger(trigger);
	}

	private void DisactivateTutorialTrigger(string trigger)
	{
		m_TriggerSystem.DisactivateTrigger(trigger);
	}

	private static string[] GetFilters(TutorialPrefab prefab)
	{
		TutorialControlSchemeActivation component = prefab.GetComponent<TutorialControlSchemeActivation>();
		if (!((Object)(object)component != (Object)null))
		{
			return null;
		}
		return new string[1] { component.m_ControlScheme.ToString() };
	}

	protected override void OnGameLoadingComplete(Purpose purpose, GameMode gameMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoadingComplete(purpose, gameMode);
		((ComponentSystemBase)this).Enabled = gameMode.IsGameOrEditor();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public TutorialsUISystem()
	{
	}
}
