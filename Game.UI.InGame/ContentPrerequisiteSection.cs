using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class ContentPrerequisiteSection : InfoSectionBase
{
	private string contentPrefab { get; set; }

	protected override bool displayForUpgrades => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	protected override string group => "ContentPrerequisiteSection";

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ContentPrerequisiteData contentPrerequisiteData = default(ContentPrerequisiteData);
		base.visible = EntitiesExtensions.TryGetComponent<ContentPrerequisiteData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref contentPrerequisiteData) && !EntitiesExtensions.HasEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, contentPrerequisiteData.m_ContentPrerequisite);
	}

	protected override void Reset()
	{
		contentPrefab = string.Empty;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ContentPrerequisiteData contentPrerequisiteData = default(ContentPrerequisiteData);
		if (EntitiesExtensions.TryGetComponent<ContentPrerequisiteData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref contentPrerequisiteData))
		{
			contentPrefab = m_PrefabSystem.GetPrefabName(contentPrerequisiteData.m_ContentPrerequisite);
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("contentPrefab");
		writer.Write(contentPrefab);
	}

	[Preserve]
	public ContentPrerequisiteSection()
	{
	}
}
