using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public abstract class InfoSectionBase : UISystemBase, ISectionSource, IJsonWritable
{
	protected bool m_Dirty;

	protected NameSystem m_NameSystem;

	protected PrefabSystem m_PrefabSystem;

	protected EndFrameBarrier m_EndFrameBarrier;

	protected SelectedInfoUISystem m_InfoUISystem;

	public override GameMode gameMode => GameMode.Game;

	public bool visible { get; protected set; }

	protected virtual bool displayForDestroyedObjects => false;

	protected virtual bool displayForOutsideConnections => false;

	protected virtual bool displayForUnderConstruction => false;

	protected virtual bool displayForUpgrades => false;

	protected abstract string group { get; }

	protected List<string> tooltipKeys { get; set; }

	protected List<string> tooltipTags { get; set; }

	protected virtual Entity selectedEntity => m_InfoUISystem.selectedEntity;

	protected virtual Entity selectedPrefab => m_InfoUISystem.selectedPrefab;

	protected bool Destroyed
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Destroyed>(selectedEntity);
		}
	}

	protected bool OutsideConnection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(selectedEntity);
		}
	}

	protected bool UnderConstruction
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			UnderConstruction underConstruction = default(UnderConstruction);
			if (EntitiesExtensions.TryGetComponent<UnderConstruction>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref underConstruction))
			{
				return underConstruction.m_NewPrefab == Entity.Null;
			}
			return false;
		}
	}

	protected bool Upgrade
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ServiceUpgradeData>(selectedPrefab);
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		tooltipKeys = new List<string>();
		tooltipTags = new List<string>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_InfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
	}

	protected abstract void Reset();

	protected abstract void OnProcess();

	public abstract void OnWriteProperties(IJsonWriter writer);

	public void RequestUpdate()
	{
		m_Dirty = true;
	}

	private bool Visible()
	{
		if (visible && (!Destroyed || displayForDestroyedObjects) && (!OutsideConnection || displayForOutsideConnections) && (!UnderConstruction || displayForUnderConstruction))
		{
			if (Upgrade)
			{
				return displayForUpgrades;
			}
			return true;
		}
		return false;
	}

	protected virtual void OnPreUpdate()
	{
	}

	public void PerformUpdate()
	{
		OnPreUpdate();
		if (m_Dirty)
		{
			m_Dirty = false;
			tooltipKeys.Clear();
			tooltipTags.Clear();
			Reset();
			((ComponentSystemBase)this).Update();
			if (Visible())
			{
				OnProcess();
			}
		}
	}

	public void Write(IJsonWriter writer)
	{
		if (Visible())
		{
			writer.TypeBegin(((object)this).GetType().FullName);
			writer.PropertyName("group");
			writer.Write(group);
			writer.PropertyName("tooltipKeys");
			JsonWriterExtensions.ArrayBegin(writer, tooltipKeys.Count);
			for (int i = 0; i < tooltipKeys.Count; i++)
			{
				writer.Write(tooltipKeys[i]);
			}
			writer.ArrayEnd();
			writer.PropertyName("tooltipTags");
			JsonWriterExtensions.ArrayBegin(writer, tooltipTags.Count);
			for (int j = 0; j < tooltipTags.Count; j++)
			{
				writer.Write(tooltipTags[j]);
			}
			writer.ArrayEnd();
			OnWriteProperties(writer);
			writer.TypeEnd();
		}
		else
		{
			writer.WriteNull();
		}
	}

	protected bool TryGetComponentWithUpgrades<T>(Entity entity, Entity prefab, out T data) where T : unmanaged, IComponentData, ICombineData<T>
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return UpgradeUtils.TryGetCombinedComponent<T>(((ComponentSystemBase)this).EntityManager, entity, prefab, out data);
	}

	[Preserve]
	protected InfoSectionBase()
	{
	}
}
