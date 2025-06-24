using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class StatusSection : InfoSectionBase
{
	private ImageSystem m_ImageSystem;

	private bool m_Dead;

	protected override string group => "StatusSection";

	private NativeList<CitizenCondition> conditions { get; set; }

	private NativeList<Notification> notifications { get; set; }

	private CitizenHappiness happiness { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		conditions.Clear();
		notifications.Clear();
		happiness = default(CitizenHappiness);
		m_Dead = false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		conditions = new NativeList<CitizenCondition>(AllocatorHandle.op_Implicit((Allocator)4));
		notifications = new NativeList<Notification>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		conditions.Dispose();
		notifications.Dispose();
		base.OnDestroy();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<HouseholdMember>(selectedEntity);
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		HouseholdMember componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(selectedEntity);
		happiness = CitizenUIUtils.GetCitizenHappiness(componentData);
		conditions = CitizenUIUtils.GetCitizenConditions(((ComponentSystemBase)this).EntityManager, selectedEntity, componentData, componentData2, conditions);
		notifications = NotificationsSection.GetNotifications(((ComponentSystemBase)this).EntityManager, selectedEntity, notifications);
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref currentTransport))
		{
			notifications = NotificationsSection.GetNotifications(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, notifications);
		}
		m_Dead = CitizenUtils.IsDead(((ComponentSystemBase)this).EntityManager, selectedEntity);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("happiness");
		if (m_Dead)
		{
			writer.WriteNull();
		}
		else
		{
			JsonWriterExtensions.Write<CitizenHappiness>(writer, happiness);
		}
		writer.PropertyName("conditions");
		if (m_Dead)
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
		else
		{
			JsonWriterExtensions.ArrayBegin(writer, conditions.Length);
			for (int i = 0; i < conditions.Length; i++)
			{
				JsonWriterExtensions.Write<CitizenCondition>(writer, conditions[i]);
			}
			writer.ArrayEnd();
		}
		writer.PropertyName("notifications");
		JsonWriterExtensions.ArrayBegin(writer, notifications.Length);
		for (int j = 0; j < notifications.Length; j++)
		{
			Entity entity = notifications[j].entity;
			NotificationIconPrefab prefab = m_PrefabSystem.GetPrefab<NotificationIconPrefab>(entity);
			writer.TypeBegin("selectedInfo.NotificationData");
			writer.PropertyName("key");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("iconPath");
			writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public StatusSection()
	{
	}
}
