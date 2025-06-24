using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Prefabs;
using UnityEngine;

namespace Game.UI.Editor;

public class PrefabItem : IItemPicker.Item, IComparable<PrefabItem>
{
	[CanBeNull]
	public PrefabBase prefab { get; set; }

	public List<string> tags { get; set; } = new List<string>();

	public int CompareTo(PrefabItem other)
	{
		if (base.favorite == other.favorite)
		{
			PrefabBase prefabBase = prefab;
			string strA = ((prefabBase != null) ? ((Object)prefabBase).name : null);
			PrefabBase prefabBase2 = other.prefab;
			return string.CompareOrdinal(strA, (prefabBase2 != null) ? ((Object)prefabBase2).name : null);
		}
		return -base.favorite.CompareTo(other.favorite);
	}
}
