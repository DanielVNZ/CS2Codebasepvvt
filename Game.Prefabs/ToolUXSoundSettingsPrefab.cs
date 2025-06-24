using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class ToolUXSoundSettingsPrefab : PrefabBase
{
	public PrefabBase m_PolygonToolSelectPointSound;

	public PrefabBase m_PolygonToolDropPointSound;

	public PrefabBase m_PolygonToolRemovePointSound;

	public PrefabBase m_PolygonToolDeleteAreaSound;

	public PrefabBase m_PolygonToolFinishAreaSound;

	public PrefabBase m_BulldozeSound;

	public PrefabBase m_PropPlantBulldozeSound;

	public PrefabBase m_TerraformSound;

	public PrefabBase m_PlaceBuildingSound;

	public PrefabBase m_RelocateBuildingSound;

	public PrefabBase m_PlaceUpgradeSound;

	public PrefabBase m_PlacePropSound;

	public PrefabBase m_PlaceBuildingFailSound;

	public PrefabBase m_ZoningFillSound;

	public PrefabBase m_ZoningRemoveFillSound;

	public PrefabBase m_ZoningStartPaintSound;

	public PrefabBase m_ZoningEndPaintSound;

	public PrefabBase m_ZoningStartRemovePaintSound;

	public PrefabBase m_ZoningEndRemovePaintSound;

	public PrefabBase m_ZoningMarqueeStartSound;

	public PrefabBase m_ZoningMarqueeEndSound;

	public PrefabBase m_ZoningMarqueeClearStartSound;

	public PrefabBase m_ZoningMarqueeClearEndSound;

	public PrefabBase m_SelectEntitySound;

	public PrefabBase m_SnapSound;

	public PrefabBase m_NetExpandSound;

	public PrefabBase m_NetStartSound;

	public PrefabBase m_NetNodeSound;

	public PrefabBase m_NetBuildSound;

	public PrefabBase m_NetCancelSound;

	public PrefabBase m_NetElevationUpSound;

	public PrefabBase m_NetElevationDownSound;

	public PrefabBase m_TransportLineCompleteSound;

	public PrefabBase m_TransportLineStartSound;

	public PrefabBase m_TransportLineBuildSound;

	public PrefabBase m_TransportLineRemoveSound;

	public PrefabBase m_AreaMarqueeStartSound;

	public PrefabBase m_AreaMarqueeEndSound;

	public PrefabBase m_AreaMarqueeClearStartSound;

	public PrefabBase m_AreaMarqueeClearEndSound;

	public PrefabBase m_TutorialStartedSound;

	public PrefabBase m_TutorialCompletedSound;

	public PrefabBase m_CameraZoomInSound;

	public PrefabBase m_CameraZoomOutSound;

	public PrefabBase m_DeletetEntitySound;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ToolUXSoundSettingsData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		ToolUXSoundSettingsData toolUXSoundSettingsData = default(ToolUXSoundSettingsData);
		toolUXSoundSettingsData.m_PolygonToolSelectPointSound = orCreateSystemManaged.GetEntity(m_PolygonToolSelectPointSound);
		toolUXSoundSettingsData.m_PolygonToolDropPointSound = orCreateSystemManaged.GetEntity(m_PolygonToolDropPointSound);
		toolUXSoundSettingsData.m_PolygonToolRemovePointSound = orCreateSystemManaged.GetEntity(m_PolygonToolRemovePointSound);
		toolUXSoundSettingsData.m_PolygonToolDeleteAreaSound = orCreateSystemManaged.GetEntity(m_PolygonToolDeleteAreaSound);
		toolUXSoundSettingsData.m_PolygonToolFinishAreaSound = orCreateSystemManaged.GetEntity(m_PolygonToolFinishAreaSound);
		toolUXSoundSettingsData.m_BulldozeSound = orCreateSystemManaged.GetEntity(m_BulldozeSound);
		toolUXSoundSettingsData.m_PropPlantBulldozeSound = orCreateSystemManaged.GetEntity(m_PropPlantBulldozeSound);
		toolUXSoundSettingsData.m_TerraformSound = orCreateSystemManaged.GetEntity(m_TerraformSound);
		toolUXSoundSettingsData.m_PlaceBuildingSound = orCreateSystemManaged.GetEntity(m_PlaceBuildingSound);
		toolUXSoundSettingsData.m_RelocateBuildingSound = orCreateSystemManaged.GetEntity(m_RelocateBuildingSound);
		toolUXSoundSettingsData.m_PlaceUpgradeSound = orCreateSystemManaged.GetEntity(m_PlaceUpgradeSound);
		toolUXSoundSettingsData.m_PlaceBuildingFailSound = orCreateSystemManaged.GetEntity(m_PlaceBuildingFailSound);
		toolUXSoundSettingsData.m_ZoningFillSound = orCreateSystemManaged.GetEntity(m_ZoningFillSound);
		toolUXSoundSettingsData.m_ZoningRemoveFillSound = orCreateSystemManaged.GetEntity(m_ZoningRemoveFillSound);
		toolUXSoundSettingsData.m_ZoningStartPaintSound = orCreateSystemManaged.GetEntity(m_ZoningStartPaintSound);
		toolUXSoundSettingsData.m_ZoningEndPaintSound = orCreateSystemManaged.GetEntity(m_ZoningEndPaintSound);
		toolUXSoundSettingsData.m_ZoningStartRemovePaintSound = orCreateSystemManaged.GetEntity(m_ZoningStartRemovePaintSound);
		toolUXSoundSettingsData.m_ZoningEndRemovePaintSound = orCreateSystemManaged.GetEntity(m_ZoningEndRemovePaintSound);
		toolUXSoundSettingsData.m_ZoningMarqueeStartSound = orCreateSystemManaged.GetEntity(m_ZoningMarqueeStartSound);
		toolUXSoundSettingsData.m_ZoningMarqueeEndSound = orCreateSystemManaged.GetEntity(m_ZoningMarqueeEndSound);
		toolUXSoundSettingsData.m_ZoningMarqueeClearStartSound = orCreateSystemManaged.GetEntity(m_ZoningMarqueeClearStartSound);
		toolUXSoundSettingsData.m_ZoningMarqueeClearEndSound = orCreateSystemManaged.GetEntity(m_ZoningMarqueeClearEndSound);
		toolUXSoundSettingsData.m_SelectEntitySound = orCreateSystemManaged.GetEntity(m_SelectEntitySound);
		toolUXSoundSettingsData.m_SnapSound = orCreateSystemManaged.GetEntity(m_SnapSound);
		toolUXSoundSettingsData.m_PlacePropSound = orCreateSystemManaged.GetEntity(m_PlacePropSound);
		toolUXSoundSettingsData.m_NetExpandSound = orCreateSystemManaged.GetEntity(m_NetExpandSound);
		toolUXSoundSettingsData.m_NetStartSound = orCreateSystemManaged.GetEntity(m_NetStartSound);
		toolUXSoundSettingsData.m_NetNodeSound = orCreateSystemManaged.GetEntity(m_NetNodeSound);
		toolUXSoundSettingsData.m_NetBuildSound = orCreateSystemManaged.GetEntity(m_NetBuildSound);
		toolUXSoundSettingsData.m_NetCancelSound = orCreateSystemManaged.GetEntity(m_NetCancelSound);
		toolUXSoundSettingsData.m_NetElevationUpSound = orCreateSystemManaged.GetEntity(m_NetElevationUpSound);
		toolUXSoundSettingsData.m_NetElevationDownSound = orCreateSystemManaged.GetEntity(m_NetElevationDownSound);
		toolUXSoundSettingsData.m_TransportLineCompleteSound = orCreateSystemManaged.GetEntity(m_TransportLineCompleteSound);
		toolUXSoundSettingsData.m_TransportLineStartSound = orCreateSystemManaged.GetEntity(m_TransportLineStartSound);
		toolUXSoundSettingsData.m_TransportLineBuildSound = orCreateSystemManaged.GetEntity(m_TransportLineBuildSound);
		toolUXSoundSettingsData.m_TransportLineRemoveSound = orCreateSystemManaged.GetEntity(m_TransportLineRemoveSound);
		toolUXSoundSettingsData.m_AreaMarqueeStartSound = orCreateSystemManaged.GetEntity(m_AreaMarqueeStartSound);
		toolUXSoundSettingsData.m_AreaMarqueeEndSound = orCreateSystemManaged.GetEntity(m_AreaMarqueeEndSound);
		toolUXSoundSettingsData.m_AreaMarqueeClearStartSound = orCreateSystemManaged.GetEntity(m_AreaMarqueeClearStartSound);
		toolUXSoundSettingsData.m_AreaMarqueeClearEndSound = orCreateSystemManaged.GetEntity(m_AreaMarqueeClearEndSound);
		toolUXSoundSettingsData.m_TutorialStartedSound = orCreateSystemManaged.GetEntity(m_TutorialStartedSound);
		toolUXSoundSettingsData.m_TutorialCompletedSound = orCreateSystemManaged.GetEntity(m_TutorialCompletedSound);
		toolUXSoundSettingsData.m_CameraZoomInSound = orCreateSystemManaged.GetEntity(m_CameraZoomInSound);
		toolUXSoundSettingsData.m_CameraZoomOutSound = orCreateSystemManaged.GetEntity(m_CameraZoomOutSound);
		toolUXSoundSettingsData.m_DeletetEntitySound = orCreateSystemManaged.GetEntity(m_DeletetEntitySound);
		((EntityManager)(ref entityManager)).SetComponentData<ToolUXSoundSettingsData>(entity, toolUXSoundSettingsData);
	}
}
