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
public class UpgradePropertiesSection : InfoSectionBase
{
	private enum UpgradeType
	{
		SubBuilding,
		Extension
	}

	private static readonly string kMainBuildingName = "mainBuildingName";

	protected override string group => "UpgradePropertiesSection";

	protected override bool displayForUpgrades => true;

	private Entity mainBuilding { get; set; }

	private Entity upgrade { get; set; }

	private UpgradeType type { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mainBuilding = Entity.Null;
		upgrade = Entity.Null;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<ServiceUpgradeData>(selectedPrefab);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		upgrade = selectedPrefab;
		Owner owner = default(Owner);
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref owner))
		{
			mainBuilding = owner.m_Owner;
			Attachment attachment = default(Attachment);
			if (EntitiesExtensions.TryGetComponent<Attachment>(((ComponentSystemBase)this).EntityManager, mainBuilding, ref attachment) && attachment.m_Attached != Entity.Null)
			{
				mainBuilding = attachment.m_Attached;
			}
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		type = (((EntityManager)(ref entityManager)).HasComponent<BuildingExtensionData>(selectedPrefab) ? UpgradeType.Extension : UpgradeType.SubBuilding);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("mainBuilding");
		UnityWriters.Write(writer, mainBuilding);
		writer.PropertyName(kMainBuildingName);
		m_NameSystem.BindName(writer, mainBuilding);
		writer.PropertyName("upgrade");
		UnityWriters.Write(writer, upgrade);
		writer.PropertyName("type");
		writer.Write(type.ToString());
	}

	[Preserve]
	public UpgradePropertiesSection()
	{
	}
}
