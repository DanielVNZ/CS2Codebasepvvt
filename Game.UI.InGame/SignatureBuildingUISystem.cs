using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Prefabs;
using Game.Serialization;
using Game.Settings;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class SignatureBuildingUISystem : UISystemBase, IPreDeserialize
{
	private const string kGroup = "signatureBuildings";

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_UnlockedSignatureBuildingQuery;

	private ValueBinding<List<Entity>> m_UnlockSignaturesBinding;

	private bool m_SkipUpdate = true;

	private int m_LastListCount;

	private bool m_NeedTriggerUpdate;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_UnlockedSignatureBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		AddBinding((IBinding)(object)(m_UnlockSignaturesBinding = new ValueBinding<List<Entity>>("signatureBuildings", "unlockedSignatures", new List<Entity>(), (IWriter<List<Entity>>)(object)new ListWriter<Entity>((IWriter<Entity>)null), (EqualityComparer<List<Entity>>)null)));
		AddBinding((IBinding)new TriggerBinding("signatureBuildings", "removeUnlockedSignature", (Action)RemoveUnlockedSignature));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (m_SkipUpdate)
		{
			m_SkipUpdate = false;
		}
		else
		{
			if (!SharedSettings.instance.userInterface.blockingPopupsEnabled || m_CityConfigurationSystem.unlockAll)
			{
				return;
			}
			if (!((EntityQuery)(ref m_UnlockedSignatureBuildingQuery)).IsEmptyIgnoreFilter)
			{
				NativeArray<Unlock> val = ((EntityQuery)(ref m_UnlockedSignatureBuildingQuery)).ToComponentDataArray<Unlock>(AllocatorHandle.op_Implicit((Allocator)3));
				for (int i = 0; i < val.Length; i++)
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(val[i].m_Prefab))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<UIObjectData>(val[i].m_Prefab))
						{
							AddUnlockedSignature(val[i].m_Prefab);
						}
					}
				}
				val.Dispose();
			}
			if (m_UnlockSignaturesBinding.value.Count != m_LastListCount || m_NeedTriggerUpdate)
			{
				m_UnlockSignaturesBinding.TriggerUpdate();
				m_LastListCount = m_UnlockSignaturesBinding.value.Count;
				m_NeedTriggerUpdate = false;
			}
		}
	}

	public void AddUnlockedSignature(Entity prefab)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!m_UnlockSignaturesBinding.value.Contains(prefab))
		{
			m_UnlockSignaturesBinding.value.Insert(0, prefab);
			m_NeedTriggerUpdate = true;
		}
	}

	private void RemoveUnlockedSignature()
	{
		m_UnlockSignaturesBinding.value.RemoveAt(0);
		m_NeedTriggerUpdate = true;
	}

	public void ClearUnlockedSignature()
	{
		m_UnlockSignaturesBinding.value.Clear();
		m_NeedTriggerUpdate = true;
	}

	public void PreDeserialize(Context context)
	{
		m_UnlockSignaturesBinding.value.Clear();
		m_SkipUpdate = false;
	}

	public void SkipUpdate()
	{
		m_SkipUpdate = true;
	}

	[Preserve]
	public SignatureBuildingUISystem()
	{
	}
}
