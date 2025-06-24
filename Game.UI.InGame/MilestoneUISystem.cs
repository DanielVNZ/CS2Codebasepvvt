using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Achievements;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.PSI;
using Game.Settings;
using Game.Simulation;
using Game.Tools;
using Game.Tutorials;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class MilestoneUISystem : UISystemBase, IXPMessageHandler
{
	private struct ComparableMilestone : IComparable<ComparableMilestone>
	{
		public Entity m_Entity;

		public MilestoneData m_Data;

		public int CompareTo(ComparableMilestone other)
		{
			return m_Data.m_Index.CompareTo(other.m_Data.m_Index);
		}
	}

	private struct ServiceInfo : IComparable<ServiceInfo>
	{
		public Entity m_Entity;

		public PrefabData m_PrefabData;

		public int m_UIPriority;

		public bool m_DevTreeUnlocked;

		public int CompareTo(ServiceInfo other)
		{
			return m_UIPriority.CompareTo(other.m_UIPriority);
		}
	}

	private struct AssetInfo : IComparable<AssetInfo>
	{
		public Entity m_Entity;

		public PrefabData m_PrefabData;

		public int m_UIPriority1;

		public int m_UIPriority2;

		public int CompareTo(AssetInfo other)
		{
			int num = m_UIPriority1.CompareTo(other.m_UIPriority1);
			if (num != 0)
			{
				return num;
			}
			return m_UIPriority2.CompareTo(other.m_UIPriority2);
		}
	}

	private const string kGroup = "milestone";

	private PrefabSystem m_PrefabSystem;

	private IXPSystem m_XPSystem;

	private CitySystem m_CitySystem;

	private IMilestoneSystem m_XpMilestoneSystem;

	private ImageSystem m_ImageSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TutorialSystem m_TutorialSystem;

	private EntityQuery m_MilestoneLevelQuery;

	private EntityQuery m_MilestoneQuery;

	private EntityQuery m_LockedMilestoneQuery;

	private EntityQuery m_ModifiedMilestoneQuery;

	private EntityQuery m_MilestoneReachedEventQuery;

	private EntityQuery m_UnlockableAssetQuery;

	private EntityQuery m_UnlockableZoneQuery;

	private EntityQuery m_DevTreeNodeQuery;

	private EntityQuery m_UnlockableFeatureQuery;

	private EntityQuery m_UnlockablePolicyQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private GetterValueBinding<int> m_AchievedMilestoneBinding;

	private GetterValueBinding<bool> m_MaxMilestoneReachedBinding;

	private GetterValueBinding<int> m_AchievedMilestoneXPBinding;

	private GetterValueBinding<int> m_NextMilestoneXPBinding;

	private GetterValueBinding<int> m_TotalXPBinding;

	private RawEventBinding m_XpMessageAddedBinding;

	private RawValueBinding m_MilestonesBinding;

	private ValueBinding<Entity> m_UnlockedMilestoneBinding;

	private RawMapBinding<Entity> m_MilestoneDetailsBinding;

	private RawMapBinding<Entity> m_MilestoneUnlocksBinding;

	private RawMapBinding<Entity> m_UnlockDetailsBinding;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Expected O, but got Unknown
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Expected O, but got Unknown
		//IL_05e4: Expected O, but got Unknown
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Expected O, but got Unknown
		//IL_0610: Expected O, but got Unknown
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_XPSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<XPSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_XpMilestoneSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MilestoneSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TutorialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialSystem>();
		m_MilestoneLevelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneLevel>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<MilestoneData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_MilestoneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<MilestoneData>(),
			ComponentType.ReadOnly<Locked>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_LockedMilestoneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<MilestoneData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[0] = val;
		m_ModifiedMilestoneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		m_MilestoneReachedEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneReachedEvent>() });
		EntityQueryDesc[] array4 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<ServiceObjectData>(),
			ComponentType.ReadOnly<UnlockRequirement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array4[0] = val;
		m_UnlockableAssetQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
		EntityQueryDesc[] array5 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.ReadOnly<UnlockRequirement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array5[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<PlaceholderBuildingData>(),
			ComponentType.ReadOnly<PlaceableObjectData>(),
			ComponentType.ReadOnly<UnlockRequirement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array5[1] = val;
		m_UnlockableZoneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array5);
		EntityQueryDesc[] array6 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<DevTreeNodeData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array6[0] = val;
		m_DevTreeNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array6);
		EntityQueryDesc[] array7 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<FeatureData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<UnlockRequirement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array7[0] = val;
		m_UnlockableFeatureQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array7);
		m_UnlockablePolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<PolicyData>(),
			ComponentType.ReadOnly<UIObjectData>(),
			ComponentType.ReadOnly<UnlockRequirement>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		AddBinding((IBinding)(object)(m_AchievedMilestoneBinding = new GetterValueBinding<int>("milestone", "achievedMilestone", (Func<int>)GetAchievedMilestone, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_MaxMilestoneReachedBinding = new GetterValueBinding<bool>("milestone", "maxMilestoneReached", (Func<bool>)IsMaxMilestoneReached, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_AchievedMilestoneXPBinding = new GetterValueBinding<int>("milestone", "achievedMilestoneXP", (Func<int>)GetAchievedMilestoneXP, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_NextMilestoneXPBinding = new GetterValueBinding<int>("milestone", "nextMilestoneXP", (Func<int>)GetNextMilestoneXP, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_TotalXPBinding = new GetterValueBinding<int>("milestone", "totalXP", (Func<int>)GetTotalXP, (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawEventBinding val2 = new RawEventBinding("milestone", "xpMessageAdded");
		RawEventBinding binding = val2;
		m_XpMessageAddedBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("milestone", "milestones", (Action<IJsonWriter>)BindMilestones);
		RawValueBinding binding2 = val3;
		m_MilestonesBinding = val3;
		AddBinding((IBinding)(object)binding2);
		AddBinding((IBinding)(object)(m_UnlockedMilestoneBinding = new ValueBinding<Entity>("milestone", "unlockedMilestone", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)new TriggerBinding("milestone", "clearUnlockedMilestone", (Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_UnlockedMilestoneBinding.Update(Entity.Null);
		}));
		AddBinding((IBinding)(object)(m_MilestoneDetailsBinding = new RawMapBinding<Entity>("milestone", "milestoneDetails", (Action<IJsonWriter, Entity>)BindMilestoneDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_MilestoneUnlocksBinding = new RawMapBinding<Entity>("milestone", "milestoneUnlocks", (Action<IJsonWriter, Entity>)BindMilestoneUnlocks, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_UnlockDetailsBinding = new RawMapBinding<Entity>("milestone", "unlockDetails", (Action<IJsonWriter, Entity>)BindUnlockDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_UnlockedMilestoneBinding.Update(Entity.Null);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_AchievedMilestoneBinding.Update();
		m_MaxMilestoneReachedBinding.Update();
		m_TotalXPBinding.Update();
		m_AchievedMilestoneXPBinding.Update();
		m_NextMilestoneXPBinding.Update();
		if (!((EntityQuery)(ref m_MilestoneReachedEventQuery)).IsEmptyIgnoreFilter)
		{
			PublishReachedMilestones();
		}
		if (!((EntityQuery)(ref m_MilestoneReachedEventQuery)).IsEmptyIgnoreFilter || !((EntityQuery)(ref m_ModifiedMilestoneQuery)).IsEmptyIgnoreFilter)
		{
			m_MilestonesBinding.Update();
			((MapBindingBase<Entity>)(object)m_MilestoneDetailsBinding).Update();
		}
		m_XPSystem.TransferMessages(this);
	}

	public void AddMessage(XPMessage message)
	{
		if (((EventBindingBase)m_XpMessageAddedBinding).active)
		{
			IJsonWriter obj = m_XpMessageAddedBinding.EventBegin();
			obj.TypeBegin("milestone.XPMessage");
			obj.PropertyName("amount");
			obj.Write(message.amount);
			obj.PropertyName("reason");
			obj.Write(Enum.GetName(typeof(XPReason), message.reason));
			obj.TypeEnd();
			m_XpMessageAddedBinding.EventEnd();
		}
	}

	private int GetAchievedMilestone()
	{
		if (((EntityQuery)(ref m_MilestoneLevelQuery)).IsEmptyIgnoreFilter)
		{
			return 0;
		}
		return ((EntityQuery)(ref m_MilestoneLevelQuery)).GetSingleton<MilestoneLevel>().m_AchievedMilestone;
	}

	private bool IsMaxMilestoneReached()
	{
		return ((EntityQuery)(ref m_LockedMilestoneQuery)).IsEmpty;
	}

	private int GetAchievedMilestoneXP()
	{
		return m_XpMilestoneSystem.lastRequiredXP;
	}

	private int GetNextMilestoneXP()
	{
		return m_XpMilestoneSystem.nextRequiredXP;
	}

	private int GetTotalXP()
	{
		return m_CitySystem.XP;
	}

	private void PublishReachedMilestones()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_UnlockedMilestoneBinding.value;
		int num = GetMilestoneIndex(m_UnlockedMilestoneBinding.value);
		NativeArray<MilestoneReachedEvent> val2 = ((EntityQuery)(ref m_MilestoneReachedEventQuery)).ToComponentDataArray<MilestoneReachedEvent>(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val2.Length; i++)
		{
			if (val2[i].m_Index > num)
			{
				val = val2[i].m_Milestone;
				num = val2[i].m_Index;
			}
		}
		val2.Dispose();
		Telemetry.MilestoneUnlocked(num);
		PlatformManager.instance.IndicateAchievementProgress(Game.Achievements.Achievements.TheLastMileMarker, num, (IndicateType)1);
		if (SharedSettings.instance.userInterface.blockingPopupsEnabled && !m_CityConfigurationSystem.unlockAll)
		{
			m_UnlockedMilestoneBinding.Update(val);
		}
	}

	private int GetMilestoneIndex(Entity milestoneEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		MilestoneData milestoneData = default(MilestoneData);
		if (!(milestoneEntity != Entity.Null) || !EntitiesExtensions.TryGetComponent<MilestoneData>(((ComponentSystemBase)this).EntityManager, milestoneEntity, ref milestoneData))
		{
			return -1;
		}
		return milestoneData.m_Index;
	}

	private void BindMilestones(IJsonWriter writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ComparableMilestone> sortedMilestones = GetSortedMilestones((Allocator)3);
		JsonWriterExtensions.ArrayBegin(writer, sortedMilestones.Length);
		for (int i = 0; i < sortedMilestones.Length; i++)
		{
			Entity val = sortedMilestones[i].m_Entity;
			MilestoneData milestoneData = sortedMilestones[i].m_Data;
			writer.TypeBegin("milestone.Milestone");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, val);
			writer.PropertyName("index");
			writer.Write(milestoneData.m_Index);
			writer.PropertyName("major");
			writer.Write(milestoneData.m_Major);
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, val));
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		sortedMilestones.Dispose();
	}

	private NativeArray<ComparableMilestone> GetSortedMilestones(Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_MilestoneQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<MilestoneData> val2 = ((EntityQuery)(ref m_MilestoneQuery)).ToComponentDataArray<MilestoneData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<ComparableMilestone> val3 = default(NativeArray<ComparableMilestone>);
		val3._002Ector(val2.Length, allocator, (NativeArrayOptions)1);
		for (int i = 0; i < val2.Length; i++)
		{
			val3[i] = new ComparableMilestone
			{
				m_Entity = val[i],
				m_Data = val2[i]
			};
		}
		NativeSortExtension.Sort<ComparableMilestone>(val3);
		val.Dispose();
		val2.Dispose();
		return val3;
	}

	private void BindMilestoneDetails(IJsonWriter writer, Entity milestone)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		MilestoneData milestoneData = default(MilestoneData);
		if (milestone != Entity.Null && EntitiesExtensions.TryGetComponent<MilestoneData>(((ComponentSystemBase)this).EntityManager, milestone, ref milestoneData))
		{
			bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, milestone);
			writer.TypeBegin("milestone.MilestoneDetails");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, milestone);
			writer.PropertyName("index");
			writer.Write(milestoneData.m_Index);
			writer.PropertyName("xpRequirement");
			writer.Write(milestoneData.m_XpRequried);
			writer.PropertyName("reward");
			writer.Write(milestoneData.m_Reward);
			writer.PropertyName("devTreePoints");
			writer.Write(milestoneData.m_DevTreePoints);
			writer.PropertyName("mapTiles");
			writer.Write((!m_CityConfigurationSystem.unlockMapTiles) ? milestoneData.m_MapTiles : 0);
			writer.PropertyName("loanLimit");
			writer.Write(milestoneData.m_LoanLimit);
			MilestonePrefab prefab = m_PrefabSystem.GetPrefab<MilestonePrefab>(milestone);
			writer.PropertyName("image");
			writer.Write(prefab.m_Image);
			writer.PropertyName("backgroundColor");
			UnityWriters.Write(writer, prefab.m_BackgroundColor);
			writer.PropertyName("accentColor");
			UnityWriters.Write(writer, prefab.m_AccentColor);
			writer.PropertyName("textColor");
			UnityWriters.Write(writer, prefab.m_TextColor);
			writer.PropertyName("locked");
			writer.Write(flag);
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	private void BindMilestoneUnlocks(IJsonWriter writer, Entity milestoneEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		NativeList<Entity> unlockedDevTreeServices = GetUnlockedDevTreeServices(milestoneEntity, (Allocator)3);
		NativeList<Entity> unlockedZones = GetUnlockedZones(milestoneEntity, (Allocator)3);
		NativeList<Entity> unlockedAssets = GetUnlockedAssets(milestoneEntity, (Allocator)3);
		NativeList<UIObjectInfo> sortedUnlockedFeatures = GetSortedUnlockedFeatures(milestoneEntity, (Allocator)3);
		NativeList<ServiceInfo> sortedServices = GetSortedServices(unlockedDevTreeServices, unlockedAssets, (Allocator)3);
		NativeList<AssetInfo> sortedZones = GetSortedZones(unlockedZones, (Allocator)3);
		NativeList<UIObjectInfo> sortedPolicies = GetSortedPolicies(milestoneEntity, (Allocator)3);
		NativeList<AssetInfo> result = default(NativeList<AssetInfo>);
		result._002Ector(20, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> assetThemes = default(NativeList<Entity>);
		assetThemes._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		JsonWriterExtensions.ArrayBegin(writer, sortedUnlockedFeatures.Length + sortedZones.Length + sortedServices.Length + sortedPolicies.Length);
		for (int i = 0; i < sortedUnlockedFeatures.Length; i++)
		{
			UIObjectInfo uIObjectInfo = sortedUnlockedFeatures[i];
			FeaturePrefab prefab = m_PrefabSystem.GetPrefab<FeaturePrefab>(uIObjectInfo.prefabData);
			UIObject component = prefab.GetComponent<UIObject>();
			writer.TypeBegin("milestone.Feature");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, uIObjectInfo.entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(component.m_Icon);
			writer.TypeEnd();
		}
		for (int j = 0; j < sortedZones.Length; j++)
		{
			AssetInfo asset = sortedZones[j];
			PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(asset.m_PrefabData);
			BindAsset(writer, asset, prefab2, assetThemes);
		}
		for (int k = 0; k < sortedServices.Length; k++)
		{
			ServiceInfo serviceInfo = sortedServices[k];
			ServicePrefab prefab3 = m_PrefabSystem.GetPrefab<ServicePrefab>(serviceInfo.m_PrefabData);
			UIObject component2 = prefab3.GetComponent<UIObject>();
			FilterAndSortAssets(result, serviceInfo.m_Entity, unlockedAssets);
			writer.TypeBegin("milestone.Service");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, serviceInfo.m_Entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab3).name);
			writer.PropertyName("icon");
			writer.Write(component2.m_Icon);
			writer.PropertyName("devTreeUnlocked");
			writer.Write(serviceInfo.m_DevTreeUnlocked);
			writer.PropertyName("assets");
			JsonWriterExtensions.ArrayBegin(writer, result.Length);
			for (int l = 0; l < result.Length; l++)
			{
				AssetInfo asset2 = result[l];
				PrefabBase prefab4 = m_PrefabSystem.GetPrefab<PrefabBase>(asset2.m_PrefabData);
				BindAsset(writer, asset2, prefab4, assetThemes);
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
		for (int m = 0; m < sortedPolicies.Length; m++)
		{
			UIObjectInfo uIObjectInfo2 = sortedPolicies[m];
			PolicyPrefab prefab5 = m_PrefabSystem.GetPrefab<PolicyPrefab>(uIObjectInfo2.prefabData);
			UIObject component3 = prefab5.GetComponent<UIObject>();
			writer.TypeBegin("milestone.Policy");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, uIObjectInfo2.entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab5).name);
			writer.PropertyName("icon");
			writer.Write(component3.m_Icon);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		unlockedDevTreeServices.Dispose();
		unlockedAssets.Dispose();
		unlockedZones.Dispose();
		sortedUnlockedFeatures.Dispose();
		sortedServices.Dispose();
		result.Dispose();
		sortedZones.Dispose();
		sortedPolicies.Dispose();
		assetThemes.Dispose();
	}

	private NativeList<Entity> GetUnlockedZones(Entity milestoneEntity, Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> prefabs = ((EntityQuery)(ref m_UnlockableZoneQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> result = FilterUnlockedPrefabs(prefabs, milestoneEntity, allocator);
		prefabs.Dispose();
		return result;
	}

	private NativeList<AssetInfo> GetSortedZones(NativeList<Entity> unlockedZones, Allocator allocator)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		NativeList<AssetInfo> val = default(NativeList<AssetInfo>);
		val._002Ector(10, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < unlockedZones.Length; i++)
		{
			Entity val2 = unlockedZones[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val2);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			UIObjectData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<UIObjectData>(val2);
			int uIPriority = int.MinValue;
			if (componentData2.m_Group != Entity.Null && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, componentData2.m_Group, ref uIObjectData))
			{
				uIPriority = uIObjectData.m_Priority;
			}
			AssetInfo assetInfo = new AssetInfo
			{
				m_Entity = val2,
				m_PrefabData = componentData,
				m_UIPriority1 = uIPriority,
				m_UIPriority2 = componentData2.m_Priority
			};
			val.Add(ref assetInfo);
		}
		NativeSortExtension.Sort<AssetInfo>(val);
		return val;
	}

	private void BindAsset(IJsonWriter writer, AssetInfo asset, PrefabBase assetPrefab, NativeList<Entity> assetThemes)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("milestone.Asset");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, asset.m_Entity);
		writer.PropertyName("name");
		writer.Write(((Object)assetPrefab).name);
		writer.PropertyName("icon");
		writer.Write(ImageSystem.GetThumbnail(assetPrefab) ?? m_ImageSystem.placeholderIcon);
		writer.PropertyName("themes");
		GetThemes(assetThemes, asset.m_Entity);
		JsonWriterExtensions.ArrayBegin(writer, assetThemes.Length);
		for (int i = 0; i < assetThemes.Length; i++)
		{
			UnityWriters.Write(writer, assetThemes[i]);
		}
		writer.ArrayEnd();
		writer.TypeEnd();
	}

	private NativeList<Entity> GetUnlockedDevTreeServices(Entity milestoneEntity, Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<DevTreeNodeData> val = ((EntityQuery)(ref m_DevTreeNodeQuery)).ToComponentDataArray<DevTreeNodeData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashSet<Entity> val2 = default(NativeParallelHashSet<Entity>);
		val2._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			val2.Add(val[i].m_Service);
		}
		NativeArray<Entity> prefabs = val2.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> result = FilterUnlockedPrefabs(prefabs, milestoneEntity, allocator);
		val.Dispose();
		val2.Dispose();
		prefabs.Dispose();
		return result;
	}

	private NativeList<Entity> GetUnlockedAssets(Entity milestoneEntity, Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> prefabs = ((EntityQuery)(ref m_UnlockableAssetQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> result = FilterUnlockedPrefabs(prefabs, milestoneEntity, allocator);
		prefabs.Dispose();
		return result;
	}

	private NativeList<UIObjectInfo> GetSortedPolicies(Entity milestoneEntity, Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> prefabs = ((EntityQuery)(ref m_UnlockablePolicyQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> entities = FilterUnlockedPrefabs(prefabs, milestoneEntity, (Allocator)3);
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, entities, allocator);
		prefabs.Dispose();
		entities.Dispose();
		return sortedObjects;
	}

	private NativeList<Entity> FilterUnlockedPrefabs(NativeArray<Entity> prefabs, Entity milestoneEntity, Allocator allocator)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, UnlockFlags> requiredPrefabs = default(NativeParallelHashMap<Entity, UnlockFlags>);
		requiredPrefabs._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> result = default(NativeList<Entity>);
		result._002Ector(20, AllocatorHandle.op_Implicit(allocator));
		MilestoneData milestoneData = default(MilestoneData);
		for (int i = 0; i < prefabs.Length; i++)
		{
			Entity prefab = prefabs[i];
			requiredPrefabs.Clear();
			ProgressionUtils.CollectSubRequirements(((ComponentSystemBase)this).EntityManager, prefab, requiredPrefabs);
			Entity val = Entity.Null;
			int num = -1;
			Enumerator<Entity, UnlockFlags> enumerator = requiredPrefabs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValue<Entity, UnlockFlags> current = enumerator.Current;
					if ((current.Value & UnlockFlags.RequireAll) == 0)
					{
						continue;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<DevTreeNodeData>(current.Key))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<UnlockRequirementData>(current.Key))
						{
							if (EntitiesExtensions.TryGetComponent<MilestoneData>(((ComponentSystemBase)this).EntityManager, current.Key, ref milestoneData) && milestoneData.m_Index > num)
							{
								val = current.Key;
								num = milestoneData.m_Index;
							}
							continue;
						}
					}
					val = Entity.Null;
					break;
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			if (val == milestoneEntity && val != Entity.Null)
			{
				result.Add(ref prefab);
			}
		}
		requiredPrefabs.Dispose();
		return result;
	}

	private NativeList<UIObjectInfo> GetSortedUnlockedFeatures(Entity milestoneEntity, Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> prefabs = ((EntityQuery)(ref m_UnlockableFeatureQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<Entity> entities = FilterUnlockedPrefabs(prefabs, milestoneEntity, (Allocator)3);
		NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, entities, allocator);
		prefabs.Dispose();
		entities.Dispose();
		if (m_CityConfigurationSystem.unlockMapTiles)
		{
			int num = -1;
			for (int i = 0; i < sortedObjects.Length; i++)
			{
				UIObjectInfo uIObjectInfo = sortedObjects[i];
				if (((Object)m_PrefabSystem.GetPrefab<PrefabBase>(uIObjectInfo.prefabData)).name == "Map Tiles")
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				sortedObjects.RemoveAt(num);
			}
		}
		return sortedObjects;
	}

	private NativeList<ServiceInfo> GetSortedServices(NativeList<Entity> unlockedDevTreeServices, NativeList<Entity> unlockedAssets, Allocator allocator)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
		val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<ServiceInfo> val2 = default(NativeList<ServiceInfo>);
		val2._002Ector(10, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		EntityManager entityManager;
		for (int i = 0; i < unlockedDevTreeServices.Length; i++)
		{
			Entity val3 = unlockedDevTreeServices[i];
			if (val.Add(val3) && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, val3, ref uIObjectData))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val3);
				ServiceInfo serviceInfo = new ServiceInfo
				{
					m_Entity = val3,
					m_PrefabData = componentData,
					m_UIPriority = uIObjectData.m_Priority,
					m_DevTreeUnlocked = true
				};
				val2.Add(ref serviceInfo);
			}
		}
		UIObjectData uIObjectData2 = default(UIObjectData);
		for (int j = 0; j < unlockedAssets.Length; j++)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity service = ((EntityManager)(ref entityManager)).GetComponentData<ServiceObjectData>(unlockedAssets[j]).m_Service;
			if (val.Add(service) && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, service, ref uIObjectData2))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(service);
				ServiceInfo serviceInfo = new ServiceInfo
				{
					m_Entity = service,
					m_PrefabData = componentData2,
					m_UIPriority = uIObjectData2.m_Priority,
					m_DevTreeUnlocked = false
				};
				val2.Add(ref serviceInfo);
			}
		}
		NativeSortExtension.Sort<ServiceInfo>(val2);
		val.Dispose();
		return val2;
	}

	private void FilterAndSortAssets(NativeList<AssetInfo> result, Entity serviceEntity, NativeList<Entity> unlockedAssets)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		result.Clear();
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < unlockedAssets.Length; i++)
		{
			Entity val = unlockedAssets[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).GetComponentData<ServiceObjectData>(val).m_Service == serviceEntity)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				UIObjectData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<UIObjectData>(val);
				int uIPriority = int.MinValue;
				if (componentData2.m_Group != Entity.Null && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, componentData2.m_Group, ref uIObjectData))
				{
					uIPriority = uIObjectData.m_Priority;
				}
				AssetInfo assetInfo = new AssetInfo
				{
					m_Entity = val,
					m_PrefabData = componentData,
					m_UIPriority1 = uIPriority,
					m_UIPriority2 = componentData2.m_Priority
				};
				result.Add(ref assetInfo);
			}
		}
		NativeSortExtension.Sort<AssetInfo>(result);
	}

	private void GetThemes(NativeList<Entity> result, Entity assetEntity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		result.Clear();
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		if (!EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, assetEntity, true, ref val))
		{
			return;
		}
		Enumerator<ObjectRequirementElement> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ObjectRequirementElement current = enumerator.Current;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(current.m_Requirement))
				{
					result.Add(ref current.m_Requirement);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void BindUnlockDetails(IJsonWriter writer, Entity unlockEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (unlockEntity != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PrefabData>(unlockEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<UIObjectData>(unlockEntity))
				{
					bool locked = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, unlockEntity);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<FeatureData>(unlockEntity))
					{
						BindFeatureUnlock(writer, unlockEntity, locked);
						return;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ServiceData>(unlockEntity))
					{
						BindServiceUnlock(writer, unlockEntity, locked);
						return;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<ServiceObjectData>(unlockEntity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<PlaceableObjectData>(unlockEntity))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<ZoneData>(unlockEntity))
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (((EntityManager)(ref entityManager)).HasComponent<PolicyData>(unlockEntity))
								{
									BindPolicyUnlock(writer, unlockEntity, locked);
								}
								else
								{
									writer.WriteNull();
								}
								return;
							}
						}
					}
					BindAssetUnlock(writer, unlockEntity, locked);
					return;
				}
			}
		}
		writer.WriteNull();
	}

	private void BindFeatureUnlock(IJsonWriter writer, Entity featureEntity, bool locked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		FeaturePrefab prefab = m_PrefabSystem.GetPrefab<FeaturePrefab>(featureEntity);
		UIObject component = prefab.GetComponent<UIObject>();
		writer.TypeBegin("milestone.UnlockDetails");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, featureEntity);
		writer.PropertyName("icon");
		writer.Write(component.m_Icon);
		writer.PropertyName("titleId");
		writer.Write("Assets.NAME[" + ((Object)prefab).name + "]");
		writer.PropertyName("descriptionId");
		writer.Write("Assets.DESCRIPTION[" + ((Object)prefab).name + "]");
		writer.PropertyName("locked");
		writer.Write(locked);
		writer.PropertyName("hasDevTree");
		writer.Write(false);
		writer.TypeEnd();
	}

	private void BindServiceUnlock(IJsonWriter writer, Entity serviceEntity, bool locked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(serviceEntity);
		UIObject component = prefab.GetComponent<UIObject>();
		writer.TypeBegin("milestone.UnlockDetails");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, serviceEntity);
		writer.PropertyName("icon");
		writer.Write(component.m_Icon);
		writer.PropertyName("titleId");
		writer.Write("Services.NAME[" + ((Object)prefab).name + "]");
		writer.PropertyName("descriptionId");
		writer.Write("Services.DESCRIPTION[" + ((Object)prefab).name + "]");
		writer.PropertyName("locked");
		writer.Write(locked);
		writer.PropertyName("hasDevTree");
		writer.Write(HasDevTree(serviceEntity));
		writer.TypeEnd();
	}

	private bool HasDevTree(Entity serviceEntity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<DevTreeNodeData> val = ((EntityQuery)(ref m_DevTreeNodeQuery)).ToComponentDataArray<DevTreeNodeData>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i].m_Service == serviceEntity)
				{
					return true;
				}
			}
			return false;
		}
		finally
		{
			((IDisposable)val/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void BindAssetUnlock(IJsonWriter writer, Entity assetEntity, bool locked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(assetEntity);
		writer.TypeBegin("milestone.UnlockDetails");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, assetEntity);
		writer.PropertyName("icon");
		writer.Write(ImageSystem.GetThumbnail(prefab) ?? m_ImageSystem.placeholderIcon);
		writer.PropertyName("titleId");
		writer.Write("Assets.NAME[" + ((Object)prefab).name + "]");
		writer.PropertyName("descriptionId");
		writer.Write("Assets.DESCRIPTION[" + ((Object)prefab).name + "]");
		writer.PropertyName("locked");
		writer.Write(locked);
		writer.PropertyName("hasDevTree");
		writer.Write(false);
		writer.TypeEnd();
	}

	private void BindPolicyUnlock(IJsonWriter writer, Entity policyEntity, bool locked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(policyEntity);
		UIObject component = prefab.GetComponent<UIObject>();
		writer.TypeBegin("milestone.UnlockDetails");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, policyEntity);
		writer.PropertyName("icon");
		writer.Write(component.m_Icon);
		writer.PropertyName("titleId");
		writer.Write("Policy.TITLE[" + ((Object)prefab).name + "]");
		writer.PropertyName("descriptionId");
		writer.Write("Policy.DESCRIPTION[" + ((Object)prefab).name + "]");
		writer.PropertyName("locked");
		writer.Write(locked);
		writer.PropertyName("hasDevTree");
		writer.Write(false);
		writer.TypeEnd();
	}

	[Preserve]
	public MilestoneUISystem()
	{
	}
}
