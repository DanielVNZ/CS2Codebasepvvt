using System;
using Colossal.OdinSerializer.Utilities;

namespace Game.Prefabs;

[ComponentMenu("UI/", new Type[] { })]
public class UITagPrefab : UITagPrefabBase
{
	public override string uiTag
	{
		get
		{
			if (!StringExtensions.IsNullOrWhitespace(m_Override))
			{
				return m_Override;
			}
			return base.uiTag;
		}
	}
}
