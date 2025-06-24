using System;
using System.Collections.Generic;
using Game.Rendering;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[]
{
	typeof(RenderPrefab),
	typeof(CharacterOverlay)
})]
public class ColorProperties : ComponentBase
{
	[Serializable]
	public class VariationSet
	{
		[FixedLength]
		[ColorUsage(true)]
		public Color[] m_Colors = (Color[])(object)new Color[3];

		public string m_VariationGroup;
	}

	[Serializable]
	public class ColorChannelBinding
	{
		public sbyte m_ChannelId;

		public bool m_CanBeModifiedByExternal;
	}

	[Serializable]
	public class VariationGroup
	{
		public string m_Name;

		[Range(0f, 100f)]
		public int m_Probability = 100;

		public ColorSyncFlags m_MeshSyncMode;

		public bool m_OverrideRandomness;

		public int3 m_VariationRanges = new int3(5, 5, 5);

		public int3 m_AlphaRanges = new int3(0, 0, 0);
	}

	public List<VariationSet> m_ColorVariations = new List<VariationSet>();

	[FixedLength]
	public List<ColorChannelBinding> m_ChannelsBinding = new List<ColorChannelBinding>
	{
		new ColorChannelBinding
		{
			m_ChannelId = 0
		},
		new ColorChannelBinding
		{
			m_ChannelId = 1
		},
		new ColorChannelBinding
		{
			m_ChannelId = 2
		}
	};

	public List<VariationGroup> m_VariationGroups;

	public int3 m_VariationRanges = new int3(5, 5, 5);

	public int3 m_AlphaRanges = new int3(0, 0, 0);

	public ColorSourceType m_ExternalColorSource;

	public bool SanityCheck(sbyte channel)
	{
		if (m_ChannelsBinding != null && channel >= 0)
		{
			return channel < m_ChannelsBinding.Count;
		}
		return false;
	}

	public bool CanBeModifiedByExternal(sbyte channel)
	{
		if (SanityCheck(channel))
		{
			return m_ChannelsBinding[channel].m_CanBeModifiedByExternal;
		}
		return true;
	}

	public Color GetColor(int index, sbyte channel)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (SanityCheck(channel) && m_ColorVariations.Count > 0)
		{
			index %= m_ColorVariations.Count;
			return m_ColorVariations[index].m_Colors[m_ChannelsBinding[channel].m_ChannelId];
		}
		return Color.white;
	}

	public int GetAlpha(int3 alphas, sbyte channel, int def)
	{
		if (SanityCheck(channel))
		{
			return ((int3)(ref alphas))[(int)m_ChannelsBinding[channel].m_ChannelId];
		}
		return def;
	}

	public float GetAlpha(float3 alphas, sbyte channel, float def)
	{
		if (SanityCheck(channel))
		{
			return ((float3)(ref alphas))[(int)m_ChannelsBinding[channel].m_ChannelId];
		}
		return def;
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ColorVariation>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		MeshColorSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<MeshColorSystem>();
		ColorVariation colorVariation = new ColorVariation
		{
			m_GroupID = orCreateSystemManaged.GetColorGroupID(null),
			m_SyncFlags = ColorSyncFlags.None,
			m_ColorSourceType = m_ExternalColorSource,
			m_Probability = 100
		};
		for (int i = 0; i < 3; i++)
		{
			if (CanBeModifiedByExternal((sbyte)i))
			{
				colorVariation.SetExternalChannelIndex(i, m_ChannelsBinding[i].m_ChannelId);
			}
			else
			{
				colorVariation.SetExternalChannelIndex(i, -1);
			}
		}
		int3 val = math.clamp(m_VariationRanges, int3.op_Implicit(0), int3.op_Implicit(100));
		int3 alphas = math.clamp(m_AlphaRanges, int3.op_Implicit(0), int3.op_Implicit(100));
		colorVariation.m_HueRange = (byte)val.x;
		colorVariation.m_SaturationRange = (byte)val.y;
		colorVariation.m_ValueRange = (byte)val.z;
		colorVariation.m_AlphaRange0 = (byte)GetAlpha(alphas, 0, 0);
		colorVariation.m_AlphaRange1 = (byte)GetAlpha(alphas, 1, 0);
		colorVariation.m_AlphaRange2 = (byte)GetAlpha(alphas, 2, 0);
		DynamicBuffer<ColorVariation> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ColorVariation>(entity, false);
		buffer.ResizeUninitialized(m_ColorVariations.Count);
		int num = 0;
		bool flag = false;
		if (m_VariationGroups != null)
		{
			for (int j = 0; j < m_VariationGroups.Count; j++)
			{
				VariationGroup variationGroup = m_VariationGroups[j];
				flag |= string.IsNullOrEmpty(variationGroup.m_Name);
				ColorVariation colorVariation2 = colorVariation;
				colorVariation2.m_GroupID = orCreateSystemManaged.GetColorGroupID(variationGroup.m_Name);
				colorVariation2.m_SyncFlags = variationGroup.m_MeshSyncMode;
				colorVariation2.m_Probability = (byte)math.clamp(variationGroup.m_Probability, 0, 100);
				if (variationGroup.m_OverrideRandomness)
				{
					val = math.clamp(variationGroup.m_VariationRanges, int3.op_Implicit(0), int3.op_Implicit(100));
					alphas = math.clamp(variationGroup.m_AlphaRanges, int3.op_Implicit(0), int3.op_Implicit(100));
					colorVariation2.m_HueRange = (byte)val.x;
					colorVariation2.m_SaturationRange = (byte)val.y;
					colorVariation2.m_ValueRange = (byte)val.z;
					colorVariation2.m_AlphaRange0 = (byte)GetAlpha(alphas, 0, 0);
					colorVariation2.m_AlphaRange1 = (byte)GetAlpha(alphas, 1, 0);
					colorVariation2.m_AlphaRange2 = (byte)GetAlpha(alphas, 2, 0);
				}
				for (int k = 0; k < m_ColorVariations.Count; k++)
				{
					if (m_ColorVariations[k].m_VariationGroup == variationGroup.m_Name)
					{
						ColorVariation colorVariation3 = colorVariation2;
						for (int l = 0; l < 3; l++)
						{
							colorVariation3.m_ColorSet[l] = GetColor(k, (sbyte)l);
						}
						buffer[num++] = colorVariation3;
					}
				}
			}
		}
		if (flag)
		{
			return;
		}
		for (int m = 0; m < m_ColorVariations.Count; m++)
		{
			if (string.IsNullOrEmpty(m_ColorVariations[m].m_VariationGroup))
			{
				ColorVariation colorVariation4 = colorVariation;
				for (int n = 0; n < 3; n++)
				{
					colorVariation4.m_ColorSet[n] = GetColor(m, (sbyte)n);
				}
				buffer[num++] = colorVariation4;
			}
		}
	}
}
