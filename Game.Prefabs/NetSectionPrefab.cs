using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Section/", new Type[] { })]
public class NetSectionPrefab : PrefabBase
{
	public NetSubSectionInfo[] m_SubSections;

	public NetPieceInfo[] m_Pieces;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_SubSections != null)
		{
			for (int i = 0; i < m_SubSections.Length; i++)
			{
				prefabs.Add(m_SubSections[i].m_Section);
			}
		}
		if (m_Pieces != null)
		{
			for (int j = 0; j < m_Pieces.Length; j++)
			{
				prefabs.Add(m_Pieces[j].m_Piece);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetSectionData>());
		components.Add(ComponentType.ReadWrite<NetSubSection>());
		components.Add(ComponentType.ReadWrite<NetSectionPiece>());
	}
}
