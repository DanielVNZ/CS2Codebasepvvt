using System;
using System.Collections.Generic;
using Colossal;
using Colossal.PSI.Common;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Prefabs/Content/", new Type[]
{
	typeof(ContentPrefab),
	typeof(UIWhatsNewPanelPrefab)
})]
public class DlcRequirement : ContentRequirementBase
{
	public DlcId m_Dlc;

	public override string GetDebugString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return StringUtils.Nicify(PlatformManager.instance.GetDlcName(m_Dlc)) + " DLC";
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override bool CheckRequirement()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return PlatformManager.instance.IsDlcOwned(m_Dlc);
	}
}
