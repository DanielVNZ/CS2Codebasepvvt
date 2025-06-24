using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class MeshColorSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindUpdatedMeshColorsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> m_RentersUpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<ColorUpdated> m_ColorUpdatedType;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColors;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		public ParallelWriter<Entity> m_Queue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RentersUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RentersUpdated>(ref m_RentersUpdatedType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity property = nativeArray[i].m_Property;
					if (m_MeshColors.HasBuffer(property))
					{
						m_Queue.Enqueue(property);
					}
					AddSubObjects(property);
				}
				return;
			}
			NativeArray<ColorUpdated> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ColorUpdated>(ref m_ColorUpdatedType);
			if (nativeArray2.Length != 0)
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity route = nativeArray2[j].m_Route;
					AddRouteVehicles(route);
				}
				return;
			}
			NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val = nativeArray3[k];
				m_Queue.Enqueue(val);
			}
		}

		private void AddSubObjects(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(owner, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (m_MeshColors.HasBuffer(subObject))
				{
					m_Queue.Enqueue(subObject);
				}
				AddSubObjects(subObject);
			}
		}

		private void AddRouteVehicles(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteVehicle> val = default(DynamicBuffer<RouteVehicle>);
			if (!m_RouteVehicles.TryGetBuffer(owner, ref val))
			{
				return;
			}
			DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (m_LayoutElements.TryGetBuffer(vehicle, ref val2) && val2.Length != 0)
				{
					for (int j = 0; j < val2.Length; j++)
					{
						if (m_MeshColors.HasBuffer(val2[j].m_Vehicle))
						{
							m_Queue.Enqueue(val2[j].m_Vehicle);
						}
					}
				}
				else if (m_MeshColors.HasBuffer(vehicle))
				{
					m_Queue.Enqueue(vehicle);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct ListUpdatedMeshColorsJob : IJob
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct EntityComparer : IComparer<Entity>
		{
			public int Compare(Entity x, Entity y)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return x.Index - y.Index;
			}
		}

		public NativeQueue<Entity> m_Queue;

		public NativeList<Entity> m_List;

		public void Execute()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int count = m_Queue.Count;
			if (count == 0)
			{
				return;
			}
			int length = m_List.Length;
			m_List.ResizeUninitialized(length + count);
			for (int i = 0; i < count; i++)
			{
				m_List[length + i] = m_Queue.Dequeue();
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_List, default(EntityComparer));
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_List.Length)
			{
				Entity val2 = m_List[num++];
				if (val2 != val)
				{
					m_List[num2++] = val2;
					val = val2;
				}
			}
			if (num2 < m_List.Length)
			{
				m_List.RemoveRange(num2, m_List.Length - num2);
			}
		}
	}

	private struct CopyColorData
	{
		public Entity m_Source;

		public Entity m_Target;

		public uint m_RandomSeed;

		public int m_ColorIndex;

		public sbyte m_ExternalChannel0;

		public sbyte m_ExternalChannel1;

		public sbyte m_ExternalChannel2;

		public byte m_HueRange;

		public byte m_SaturationRange;

		public byte m_ValueRange;

		public byte m_AlphaRange0;

		public byte m_AlphaRange1;

		public byte m_AlphaRange2;

		public bool hasVariationRanges => (m_HueRange != 0) | (m_SaturationRange != 0) | (m_ValueRange != 0);

		public bool hasAlphaRanges => (m_AlphaRange0 != 0) | (m_AlphaRange1 != 0) | (m_AlphaRange2 != 0);

		public int GetExternalChannelIndex(int colorIndex)
		{
			return colorIndex switch
			{
				0 => m_ExternalChannel0, 
				1 => m_ExternalChannel1, 
				2 => m_ExternalChannel2, 
				_ => -1, 
			};
		}
	}

	private enum UpdateStage
	{
		Default,
		IgnoreSubs,
		IgnoreOwners
	}

	[BurstCompile]
	private struct SetMeshColorsJob : IJobParallelForDefer
	{
		private struct SyncData
		{
			public ColorGroupID m_GroupID;

			public uint m_RandomSeed;

			public int m_ColorIndex;
		}

		private struct SearchData
		{
			public Entity m_ColorSource;

			public Game.Prefabs.AgeMask m_Age;

			public GenderMask m_Gender;

			public bool m_ExternalSearched;

			public bool m_FiltersSearched;
		}

		private struct ColorData
		{
			public ColorVariation m_Color;

			public int m_Probability;

			public int m_Index;

			public uint m_RandomSeed;
		}

		private struct ColorDatas
		{
			public ColorData m_Match;

			public ColorData m_Unmatch;

			public ColorData m_Unsync;

			public uint m_SeedOffset;
		}

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public Entity m_DefaultBrand;

		[ReadOnly]
		public Entity m_Season1;

		[ReadOnly]
		public Entity m_Season2;

		[ReadOnly]
		public Entity m_OverrideEntity;

		[ReadOnly]
		public Entity m_OverrideMesh;

		[ReadOnly]
		public float m_SeasonBlend;

		[ReadOnly]
		public int m_OverrideIndex;

		[ReadOnly]
		public UpdateStage m_Stage;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Plant> m_PlantData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRouteData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> m_RouteColorData;

		[ReadOnly]
		public ComponentLookup<CompanyData> m_CompanyData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BrandData> m_BrandData;

		[ReadOnly]
		public ComponentLookup<CreatureData> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<ResidentData> m_ResidentData;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<ColorVariation> m_ColorVariations;

		[ReadOnly]
		public BufferLookup<ColorFilter> m_ColorFilters;

		[ReadOnly]
		public BufferLookup<OverlayElement> m_OverlayElements;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshColor> m_MeshColors;

		public ParallelWriter<CopyColorData> m_CopyColors;

		public unsafe void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			if (m_Stage != UpdateStage.Default && m_OwnerData.HasComponent(val) == (m_Stage == UpdateStage.IgnoreSubs))
			{
				return;
			}
			PrefabRef prefabRef = m_PrefabRefData[val];
			DynamicBuffer<MeshColor> meshColors = m_MeshColors[val];
			DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
			int num = 0;
			int num2 = 0;
			DynamicBuffer<SubMesh> val3 = default(DynamicBuffer<SubMesh>);
			if (m_SubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val3))
			{
				num = 1;
				num2 = val3.Length;
			}
			bool flag = false;
			DynamicBuffer<SubMeshGroup> val4 = default(DynamicBuffer<SubMeshGroup>);
			if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val4))
			{
				if (m_MeshGroups.TryGetBuffer(val, ref val2))
				{
					num = val2.Length;
				}
				num2 = 0;
				MeshGroup meshGroup = default(MeshGroup);
				for (int i = 0; i < num; i++)
				{
					CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
					SubMeshGroup subMeshGroup = val4[(int)meshGroup.m_SubMeshGroup];
					num2 += subMeshGroup.m_SubMeshRange.y - subMeshGroup.m_SubMeshRange.x;
					for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
					{
						SubMesh subMesh = val3[j];
						if (m_OverlayElements.HasBuffer(subMesh.m_SubMesh))
						{
							flag = true;
							num2 += 8;
							break;
						}
					}
				}
			}
			if (num2 == 0)
			{
				meshColors.Clear();
				return;
			}
			SyncData* syncData = stackalloc SyncData[num2 * 2];
			meshColors.ResizeUninitialized(num2);
			num2 = 0;
			PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
			if (!m_PseudoRandomSeedData.TryGetComponent(val, ref pseudoRandomSeed))
			{
				Random random = m_RandomSeed.GetRandom(index);
				pseudoRandomSeed.m_Seed = (ushort)((Random)(ref random)).NextUInt(65536u);
			}
			SearchData searchData = default(SearchData);
			int syncIndex = 0;
			SubMeshGroup subMeshGroup2 = default(SubMeshGroup);
			DynamicBuffer<OverlayElement> overlayElements = default(DynamicBuffer<OverlayElement>);
			DynamicBuffer<CharacterElement> val5 = default(DynamicBuffer<CharacterElement>);
			for (int k = 0; k < num; k++)
			{
				MeshGroup meshGroup2 = default(MeshGroup);
				if (val4.IsCreated)
				{
					CollectionUtils.TryGet<MeshGroup>(val2, k, ref meshGroup2);
					subMeshGroup2 = val4[(int)meshGroup2.m_SubMeshGroup];
				}
				else
				{
					subMeshGroup2.m_SubMeshRange = new int2(0, val3.Length);
				}
				for (int l = subMeshGroup2.m_SubMeshRange.x; l < subMeshGroup2.m_SubMeshRange.y; l++)
				{
					SubMesh subMesh2 = val3[l];
					Random random2 = pseudoRandomSeed.GetRandom((uint)(PseudoRandomSeed.kColorVariation | (subMesh2.m_RandomSeed << 16)));
					SetColor(meshColors, syncData, num2++, val, subMesh2.m_SubMesh, ref random2, ref searchData, ref syncIndex);
				}
				if (!flag)
				{
					continue;
				}
				for (int m = subMeshGroup2.m_SubMeshRange.x; m < subMeshGroup2.m_SubMeshRange.y; m++)
				{
					SubMesh subMesh3 = val3[m];
					if (m_OverlayElements.TryGetBuffer(subMesh3.m_SubMesh, ref overlayElements))
					{
						CharacterElement characterElement = default(CharacterElement);
						if (m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val5))
						{
							characterElement = val5[(int)meshGroup2.m_SubMeshGroup];
						}
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight0, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight1, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight2, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight3, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight4, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight5, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight6, pseudoRandomSeed, ref searchData, ref syncIndex);
						SetColor(meshColors, syncData, num2++, val, overlayElements, characterElement.m_OverlayWeights.m_Weight7, pseudoRandomSeed, ref searchData, ref syncIndex);
						break;
					}
				}
			}
		}

		private unsafe void SetColor(DynamicBuffer<MeshColor> meshColors, SyncData* syncData, int colorIndex, Entity entity, DynamicBuffer<OverlayElement> overlayElements, BlendWeight overlayWeight, PseudoRandomSeed pseudoRandomSeed, ref SearchData searchData, ref int syncIndex)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Entity prefab = Entity.Null;
			if (overlayWeight.m_Index >= 0 && overlayWeight.m_Index < overlayElements.Length && overlayWeight.m_Weight > 0f)
			{
				prefab = overlayElements[overlayWeight.m_Index].m_Overlay;
			}
			Random random = pseudoRandomSeed.GetRandom((uint)(PseudoRandomSeed.kColorVariation | (overlayWeight.m_Index << 16)));
			SetColor(meshColors, syncData, colorIndex, entity, prefab, ref random, ref searchData, ref syncIndex);
		}

		private unsafe void SetColor(DynamicBuffer<MeshColor> meshColors, SyncData* syncData, int colorIndex, Entity entity, Entity prefab, ref Random random, ref SearchData searchData, ref int syncIndex)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			MeshColor meshColor = new MeshColor
			{
				m_ColorSet = new ColorSet(Color.white)
			};
			SyncData syncValue = new SyncData
			{
				m_GroupID = new ColorGroupID(-1),
				m_RandomSeed = 0u,
				m_ColorIndex = -1
			};
			DynamicBuffer<ColorVariation> val = default(DynamicBuffer<ColorVariation>);
			if (m_ColorVariations.TryGetBuffer(prefab, ref val))
			{
				ColorDatas colors = default(ColorDatas);
				ColorDatas colors2 = default(ColorDatas);
				int num = 0;
				int num2 = 0;
				uint randomSeed = 0u;
				bool flag = false;
				bool flag2 = false;
				ColorGroupID colorGroupID = new ColorGroupID(-2);
				ColorFilter colorFilter = default(ColorFilter);
				ref ColorDatas reference = ref colors;
				float num3 = 0f;
				DynamicBuffer<ColorFilter> val2 = default(DynamicBuffer<ColorFilter>);
				if (m_ColorFilters.TryGetBuffer(prefab, ref val2) && !searchData.m_FiltersSearched)
				{
					FindFilters(entity, ref searchData.m_Age, ref searchData.m_Gender);
					searchData.m_FiltersSearched = true;
				}
				int num4 = math.select(-1, m_OverrideIndex, entity == m_OverrideEntity && prefab == m_OverrideMesh && m_OverrideIndex < val.Length);
				for (int i = 0; i < val.Length; i++)
				{
					ColorVariation color = val[i];
					if (color.m_GroupID != colorGroupID)
					{
						num = 0;
						num2 = -1;
						randomSeed = 0u;
						colorGroupID = color.m_GroupID;
						flag = false;
						colorFilter.m_OverrideProbability = -1;
						colorFilter.m_OverrideAlpha = float3.op_Implicit(-1f);
						reference = ref colors;
						num3 = 0f;
						if (color.m_SyncFlags != ColorSyncFlags.None)
						{
							for (int j = 0; j < syncIndex; j++)
							{
								if (syncData[j].m_GroupID == colorGroupID)
								{
									num2 = syncData[j].m_ColorIndex;
									randomSeed = syncData[j].m_RandomSeed;
									flag = true;
									break;
								}
							}
							flag2 = flag2 || flag;
						}
						if (val2.IsCreated)
						{
							for (int k = 0; k < val2.Length; k++)
							{
								ColorFilter colorFilter2 = val2[k];
								if (colorFilter2.m_GroupID != colorGroupID || (colorFilter2.m_AgeFilter & searchData.m_Age) == 0 || (colorFilter2.m_GenderFilter & searchData.m_Gender) == 0)
								{
									continue;
								}
								if ((colorFilter2.m_Flags & ColorFilterFlags.SeasonFilter) != 0)
								{
									if (m_Season1 != colorFilter2.m_EntityFilter)
									{
										if (!(m_Season2 == colorFilter2.m_EntityFilter))
										{
											continue;
										}
										reference = ref colors2;
										num3 = m_SeasonBlend;
									}
									reference.m_SeedOffset += (uint)(k * -1571468583);
								}
								if (colorFilter2.m_OverrideProbability >= 0)
								{
									colorFilter.m_OverrideProbability = colorFilter2.m_OverrideProbability;
								}
								colorFilter.m_OverrideAlpha = math.select(colorFilter.m_OverrideAlpha, colorFilter2.m_OverrideAlpha, colorFilter2.m_OverrideAlpha >= 0f);
							}
						}
					}
					if (num4 != -1)
					{
						color.m_Probability = (byte)math.select(0, 100, i == num4);
					}
					else if (colorFilter.m_OverrideProbability != -1)
					{
						color.m_Probability = (byte)colorFilter.m_OverrideProbability;
					}
					bool3 val3 = colorFilter.m_OverrideAlpha >= 0f;
					if (math.any(val3))
					{
						color.m_ColorSet.m_Channel0.a = math.select(color.m_ColorSet.m_Channel0.a, colorFilter.m_OverrideAlpha.x, val3.x);
						color.m_ColorSet.m_Channel1.a = math.select(color.m_ColorSet.m_Channel1.a, colorFilter.m_OverrideAlpha.y, val3.y);
						color.m_ColorSet.m_Channel2.a = math.select(color.m_ColorSet.m_Channel2.a, colorFilter.m_OverrideAlpha.z, val3.z);
					}
					if (color.m_SyncFlags != ColorSyncFlags.None)
					{
						bool flag3 = true;
						if ((color.m_SyncFlags & ColorSyncFlags.SameGroup) != ColorSyncFlags.None)
						{
							flag3 = flag3 && flag;
						}
						if ((color.m_SyncFlags & ColorSyncFlags.DifferentGroup) != ColorSyncFlags.None)
						{
							flag3 = flag3 && !flag;
						}
						if ((color.m_SyncFlags & ColorSyncFlags.SameIndex) != ColorSyncFlags.None)
						{
							flag3 = flag3 && num == num2;
						}
						if ((color.m_SyncFlags & ColorSyncFlags.DifferentIndex) != ColorSyncFlags.None)
						{
							flag3 = flag3 && num != num2;
						}
						ref ColorData reference2 = ref reference.m_Unmatch;
						if (flag3)
						{
							reference2 = ref reference.m_Match;
						}
						reference2.m_Probability += color.m_Probability;
						if (((Random)(ref random)).NextInt(reference2.m_Probability) < color.m_Probability)
						{
							reference2.m_Color = color;
							reference2.m_Index = num;
							reference2.m_RandomSeed = randomSeed;
						}
					}
					else
					{
						reference.m_Unsync.m_Probability += color.m_Probability;
						if (((Random)(ref random)).NextInt(reference.m_Unsync.m_Probability) < color.m_Probability)
						{
							reference.m_Unsync.m_Color = color;
							reference.m_Unsync.m_Color.m_GroupID = new ColorGroupID(-1);
						}
					}
					num++;
				}
				Random random2 = random;
				random2.state += colors.m_SeedOffset;
				random2.state = math.select(random2.state, random.state, random2.state == 0);
				CalculateMeshColor(ref meshColor, ref syncValue, ref random2, ref searchData, ref colors, entity, colorIndex, flag2);
				if (num3 != 0f)
				{
					random2 = random;
					random2.state += colors2.m_SeedOffset;
					random2.state = math.select(random2.state, random.state, random2.state == 0);
					MeshColor meshColor2 = default(MeshColor);
					SyncData syncValue2 = new SyncData
					{
						m_GroupID = new ColorGroupID(-1),
						m_RandomSeed = 0u,
						m_ColorIndex = -1
					};
					CalculateMeshColor(ref meshColor2, ref syncValue2, ref random2, ref searchData, ref colors2, entity, colorIndex, flag2);
					if (colors2.m_Match.m_Probability > 0)
					{
						if (colors.m_Match.m_Probability > 0)
						{
							meshColor.m_ColorSet.m_Channel0 = Color.Lerp(meshColor.m_ColorSet.m_Channel0, meshColor2.m_ColorSet.m_Channel0, num3);
							meshColor.m_ColorSet.m_Channel1 = Color.Lerp(meshColor.m_ColorSet.m_Channel1, meshColor2.m_ColorSet.m_Channel1, num3);
							meshColor.m_ColorSet.m_Channel2 = Color.Lerp(meshColor.m_ColorSet.m_Channel2, meshColor2.m_ColorSet.m_Channel2, num3);
						}
						else
						{
							meshColor.m_ColorSet = meshColor2.m_ColorSet;
						}
						syncData[syncIndex++] = syncValue2;
					}
				}
			}
			meshColors[colorIndex] = meshColor;
			syncData[syncIndex++] = syncValue;
		}

		private void CalculateMeshColor(ref MeshColor meshColor, ref SyncData syncValue, ref Random random, ref SearchData searchData, ref ColorDatas colors, Entity entity, int colorIndex, bool anyGroupUsed)
		{
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			colors.m_Match.m_Probability += colors.m_Unmatch.m_Probability;
			if (!anyGroupUsed && ((Random)(ref random)).NextInt(colors.m_Match.m_Probability) < colors.m_Unmatch.m_Probability)
			{
				colors.m_Match.m_Color = colors.m_Unmatch.m_Color;
				colors.m_Match.m_Index = colors.m_Unmatch.m_Index;
				colors.m_Match.m_RandomSeed = colors.m_Unmatch.m_RandomSeed;
			}
			colors.m_Match.m_Probability += colors.m_Unsync.m_Probability;
			if (((Random)(ref random)).NextInt(colors.m_Match.m_Probability) < colors.m_Unsync.m_Probability)
			{
				colors.m_Match.m_Color = colors.m_Unsync.m_Color;
				colors.m_Match.m_Index = -1;
				colors.m_Match.m_RandomSeed = 0u;
			}
			if (colors.m_Match.m_Probability <= 0)
			{
				return;
			}
			meshColor.m_ColorSet = colors.m_Match.m_Color.m_ColorSet;
			syncValue.m_GroupID = colors.m_Match.m_Color.m_GroupID;
			syncValue.m_RandomSeed = random.state;
			syncValue.m_ColorIndex = colors.m_Match.m_Index;
			if ((colors.m_Match.m_Color.m_SyncFlags & ColorSyncFlags.SyncRangeVariation) != ColorSyncFlags.None && colors.m_Match.m_RandomSeed != 0)
			{
				syncValue.m_RandomSeed = colors.m_Match.m_RandomSeed;
			}
			if (colors.m_Match.m_Color.hasExternalChannels)
			{
				if (!searchData.m_ExternalSearched)
				{
					searchData.m_ColorSource = FindExternalSource(entity, colors.m_Match.m_Color.m_ColorSourceType);
					searchData.m_ExternalSearched = true;
				}
				BrandData brandData = default(BrandData);
				Game.Routes.Color color = default(Game.Routes.Color);
				if (colors.m_Match.m_Color.m_ColorSourceType == ColorSourceType.Parent)
				{
					DynamicBuffer<MeshColor> val = default(DynamicBuffer<MeshColor>);
					if (m_Stage == UpdateStage.Default || m_OwnerData.HasComponent(searchData.m_ColorSource) == (m_Stage == UpdateStage.IgnoreOwners))
					{
						if (searchData.m_ColorSource != Entity.Null)
						{
							m_CopyColors.Enqueue(new CopyColorData
							{
								m_Source = searchData.m_ColorSource,
								m_Target = entity,
								m_RandomSeed = syncValue.m_RandomSeed,
								m_ColorIndex = colorIndex,
								m_ExternalChannel0 = colors.m_Match.m_Color.m_ExternalChannel0,
								m_ExternalChannel1 = colors.m_Match.m_Color.m_ExternalChannel1,
								m_ExternalChannel2 = colors.m_Match.m_Color.m_ExternalChannel2,
								m_HueRange = colors.m_Match.m_Color.m_HueRange,
								m_SaturationRange = colors.m_Match.m_Color.m_SaturationRange,
								m_ValueRange = colors.m_Match.m_Color.m_ValueRange,
								m_AlphaRange0 = colors.m_Match.m_Color.m_AlphaRange0,
								m_AlphaRange1 = colors.m_Match.m_Color.m_AlphaRange1,
								m_AlphaRange2 = colors.m_Match.m_Color.m_AlphaRange2
							});
						}
					}
					else if (m_MeshColors.TryGetBuffer(searchData.m_ColorSource, ref val) && val.Length != 0)
					{
						MeshColor meshColor2 = val[math.min(colorIndex, val.Length - 1)];
						for (int i = 0; i < 3; i++)
						{
							int externalChannelIndex = colors.m_Match.m_Color.GetExternalChannelIndex(i);
							if (externalChannelIndex >= 0)
							{
								meshColor.m_ColorSet[externalChannelIndex] = meshColor2.m_ColorSet[i];
							}
						}
					}
				}
				else if (m_BrandData.TryGetComponent(searchData.m_ColorSource, ref brandData))
				{
					for (int j = 0; j < 3; j++)
					{
						int externalChannelIndex2 = colors.m_Match.m_Color.GetExternalChannelIndex(j);
						if (externalChannelIndex2 >= 0)
						{
							meshColor.m_ColorSet[externalChannelIndex2] = brandData.m_ColorSet[j];
						}
					}
				}
				else if (m_RouteColorData.TryGetComponent(searchData.m_ColorSource, ref color))
				{
					for (int k = 0; k < 3; k++)
					{
						int externalChannelIndex3 = colors.m_Match.m_Color.GetExternalChannelIndex(k);
						if (externalChannelIndex3 >= 0)
						{
							meshColor.m_ColorSet[externalChannelIndex3] = Color32.op_Implicit(color.m_Color);
						}
					}
				}
			}
			Random random2 = new Random
			{
				state = syncValue.m_RandomSeed
			};
			if (colors.m_Match.m_Color.hasVariationRanges)
			{
				float3 val2 = new float3((float)(int)colors.m_Match.m_Color.m_HueRange, (float)(int)colors.m_Match.m_Color.m_SaturationRange, (float)(int)colors.m_Match.m_Color.m_ValueRange) * 0.01f;
				float3 min = 1f - val2;
				float3 max = 1f + val2;
				RandomizeColor(ref meshColor.m_ColorSet.m_Channel0, ref random2, min, max);
				RandomizeColor(ref meshColor.m_ColorSet.m_Channel1, ref random2, min, max);
				RandomizeColor(ref meshColor.m_ColorSet.m_Channel2, ref random2, min, max);
			}
			if (colors.m_Match.m_Color.hasAlphaRanges)
			{
				float3 val3 = new float3((float)(int)colors.m_Match.m_Color.m_AlphaRange0, (float)(int)colors.m_Match.m_Color.m_AlphaRange1, (float)(int)colors.m_Match.m_Color.m_AlphaRange2) * 0.01f;
				float3 min2 = -val3;
				float3 max2 = val3;
				RandomizeAlphas(ref meshColor.m_ColorSet, ref random2, min2, max2);
			}
			if ((colors.m_Match.m_Color.m_SyncFlags & ColorSyncFlags.SyncRangeVariation) == 0 || colors.m_Match.m_RandomSeed == 0)
			{
				random = random2;
			}
		}

		private void FindFilters(Entity entity, ref Game.Prefabs.AgeMask age, ref GenderMask gender)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[entity];
			CreatureData creatureData = default(CreatureData);
			if (m_CreatureData.TryGetComponent(prefabRef.m_Prefab, ref creatureData))
			{
				gender = creatureData.m_Gender;
			}
			else
			{
				gender = GenderMask.Any;
			}
			ResidentData residentData = default(ResidentData);
			if (m_ResidentData.TryGetComponent(prefabRef.m_Prefab, ref residentData))
			{
				age = residentData.m_Age;
			}
			else
			{
				age = Game.Prefabs.AgeMask.Any;
			}
		}

		private Entity FindExternalSource(Entity entity, ColorSourceType colorSourceType)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = false;
			if (m_PlantData.HasComponent(entity))
			{
				return Entity.Null;
			}
			Temp temp = default(Temp);
			if (m_TempData.TryGetComponent(entity, ref temp))
			{
				if (temp.m_Original != Entity.Null)
				{
					entity = temp.m_Original;
				}
				else
				{
					flag = true;
				}
			}
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(entity, ref controller) && controller.m_Controller != Entity.Null)
			{
				entity = controller.m_Controller;
			}
			switch (colorSourceType)
			{
			case ColorSourceType.Brand:
			{
				DynamicBuffer<Renter> renters = default(DynamicBuffer<Renter>);
				if (m_Renters.TryGetBuffer(entity, ref renters))
				{
					if (FindBrand(renters, out var brand))
					{
						return brand;
					}
					flag2 = true;
				}
				CurrentRoute currentRoute = default(CurrentRoute);
				if (m_CurrentRouteData.TryGetComponent(entity, ref currentRoute) && m_RouteColorData.HasComponent(currentRoute.m_Route))
				{
					return currentRoute.m_Route;
				}
				if (m_RouteColorData.HasComponent(entity))
				{
					return entity;
				}
				Owner owner2 = default(Owner);
				while (m_OwnerData.TryGetComponent(entity, ref owner2))
				{
					entity = owner2.m_Owner;
					if (flag && m_TempData.TryGetComponent(entity, ref temp) && temp.m_Original != Entity.Null)
					{
						entity = temp.m_Original;
						flag = false;
					}
					if (m_Renters.TryGetBuffer(entity, ref renters))
					{
						if (FindBrand(renters, out var brand2))
						{
							return brand2;
						}
						flag2 = true;
					}
					if (m_CurrentRouteData.TryGetComponent(entity, ref currentRoute) && m_RouteColorData.HasComponent(currentRoute.m_Route))
					{
						return currentRoute.m_Route;
					}
					if (m_RouteColorData.HasComponent(entity))
					{
						return entity;
					}
				}
				if (flag2)
				{
					return m_DefaultBrand;
				}
				return Entity.Null;
			}
			case ColorSourceType.Parent:
			{
				Entity result = Entity.Null;
				Owner owner = default(Owner);
				while (m_OwnerData.TryGetComponent(entity, ref owner))
				{
					entity = owner.m_Owner;
					if (flag && m_TempData.TryGetComponent(entity, ref temp) && temp.m_Original != Entity.Null)
					{
						entity = temp.m_Original;
						flag = false;
					}
					if (m_MeshColors.HasBuffer(entity))
					{
						result = entity;
					}
				}
				return result;
			}
			default:
				return Entity.Null;
			}
		}

		private bool FindBrand(DynamicBuffer<Renter> renters, out Entity brand)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < renters.Length; i++)
			{
				Entity renter = renters[i].m_Renter;
				if (m_CompanyData.HasComponent(renter))
				{
					CompanyData companyData = m_CompanyData[renter];
					if (companyData.m_Brand != Entity.Null)
					{
						brand = companyData.m_Brand;
						return true;
					}
				}
			}
			brand = Entity.Null;
			return false;
		}
	}

	[BurstCompile]
	private struct CopyMeshColorsJob : IJob
	{
		public BufferLookup<MeshColor> m_MeshColors;

		public NativeQueue<CopyColorData> m_CopyColors;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			CopyColorData copyColorData = default(CopyColorData);
			while (m_CopyColors.TryDequeue(ref copyColorData))
			{
				DynamicBuffer<MeshColor> val = m_MeshColors[copyColorData.m_Source];
				DynamicBuffer<MeshColor> val2 = m_MeshColors[copyColorData.m_Target];
				if (val.Length == 0)
				{
					continue;
				}
				MeshColor meshColor = val[math.min(copyColorData.m_ColorIndex, val.Length - 1)];
				ref MeshColor reference = ref val2.ElementAt(copyColorData.m_ColorIndex);
				Random random = new Random
				{
					state = copyColorData.m_RandomSeed
				};
				if (copyColorData.hasVariationRanges)
				{
					float3 val3 = new float3((float)(int)copyColorData.m_HueRange, (float)(int)copyColorData.m_SaturationRange, (float)(int)copyColorData.m_ValueRange) * 0.01f;
					float3 min = 1f - val3;
					float3 max = 1f + val3;
					RandomizeColor(ref meshColor.m_ColorSet.m_Channel0, ref random, min, max);
					RandomizeColor(ref meshColor.m_ColorSet.m_Channel1, ref random, min, max);
					RandomizeColor(ref meshColor.m_ColorSet.m_Channel2, ref random, min, max);
				}
				if (copyColorData.hasAlphaRanges)
				{
					float3 val4 = new float3((float)(int)copyColorData.m_AlphaRange0, (float)(int)copyColorData.m_AlphaRange1, (float)(int)copyColorData.m_AlphaRange2) * 0.01f;
					float3 min2 = -val4;
					float3 max2 = val4;
					RandomizeAlphas(ref meshColor.m_ColorSet, ref random, min2, max2);
				}
				for (int i = 0; i < 3; i++)
				{
					int externalChannelIndex = copyColorData.GetExternalChannelIndex(i);
					if (externalChannelIndex >= 0)
					{
						reference.m_ColorSet[externalChannelIndex] = meshColor.m_ColorSet[i];
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> __Game_Buildings_RentersUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ColorUpdated> __Game_Routes_ColorUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> __Game_Routes_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BrandData> __Game_Prefabs_BrandData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResidentData> __Game_Prefabs_ResidentData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ColorVariation> __Game_Prefabs_ColorVariation_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ColorFilter> __Game_Prefabs_ColorFilter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OverlayElement> __Game_Prefabs_OverlayElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RentersUpdated>(true);
			__Game_Routes_ColorUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ColorUpdated>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Routes_CurrentRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentRoute>(true);
			__Game_Routes_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.Color>(true);
			__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BrandData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BrandData>(true);
			__Game_Prefabs_CreatureData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CreatureData>(true);
			__Game_Prefabs_ResidentData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResidentData>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_ColorVariation_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ColorVariation>(true);
			__Game_Prefabs_ColorFilter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ColorFilter>(true);
			__Game_Prefabs_OverlayElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OverlayElement>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Rendering_MeshColor_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(false);
		}
	}

	private ClimateSystem m_ClimateSystem;

	private SimulationSystem m_SimulationSystem;

	private PrefabSystem m_PrefabSystem;

	private RenderPrefabBase m_OverridePrefab;

	private Dictionary<string, int> m_GroupIDs;

	private EntityQuery m_UpdateQuery;

	private EntityQuery m_AllQuery;

	private EntityQuery m_PlantQuery;

	private EntityQuery m_BuildingSettingsQuery;

	private Entity m_LastSeason1;

	private Entity m_LastSeason2;

	private Entity m_OverrideEntity;

	private uint m_LastUpdateGroup;

	private uint m_UpdateGroupCount;

	private int m_OverrideIndex;

	private float m_LastSeasonBlend;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	public bool smoothColorsUpdated { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshColor>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Event>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RentersUpdated>(),
			ComponentType.ReadOnly<ColorUpdated>()
		};
		array[1] = val;
		m_UpdateQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshColor>() });
		m_PlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MeshColor>(),
			ComponentType.ReadOnly<Plant>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		m_BuildingSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		m_GroupIDs = new Dictionary<string, int>();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	public void SetOverride(Entity entity, RenderPrefabBase prefab, int variationIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (m_OverrideEntity != entity && m_OverrideEntity != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(m_OverrideEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(m_OverrideEntity))
				{
					EntityCommandBuffer val = ((ComponentSystemBase)this).World.GetExistingSystemManaged<EndFrameBarrier>().CreateCommandBuffer();
					((EntityCommandBuffer)(ref val)).AddComponent<BatchesUpdated>(m_OverrideEntity);
				}
			}
		}
		m_OverrideEntity = entity;
		m_OverridePrefab = prefab;
		m_OverrideIndex = variationIndex;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		bool flag = GetLoaded() && !((EntityQuery)(ref m_AllQuery)).IsEmptyIgnoreFilter;
		bool flag2 = !flag && !((EntityQuery)(ref m_UpdateQuery)).IsEmptyIgnoreFilter;
		uint num = (m_SimulationSystem.frameIndex >> 9) & 0xF;
		Entity currentClimate = m_ClimateSystem.currentClimate;
		smoothColorsUpdated = !flag && !((EntityQuery)(ref m_PlantQuery)).IsEmptyIgnoreFilter;
		if (currentClimate != Entity.Null)
		{
			ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(m_ClimateSystem.currentClimate);
			float num2 = m_ClimateSystem.currentDate;
			var (seasonInfo, num3, num4) = prefab.FindSeasonByTime(num2);
			if (num2 < num3)
			{
				num2 += 1f;
			}
			float num5 = (num3 + num4) * 0.5f;
			ClimateSystem.SeasonInfo seasonInfo2;
			float num6;
			float num7;
			if (num2 < num5)
			{
				num6 = num3 - 0.001f;
				if (num6 < 0f)
				{
					num6 += 1f;
				}
				(seasonInfo2, num6, num7) = prefab.FindSeasonByTime(num6);
				if (num6 > num3)
				{
					num5 += 1f;
					num2 += 1f;
				}
			}
			else
			{
				num7 = num4 + 0.001f;
				if (num7 >= 1f)
				{
					num7 -= 1f;
				}
				(seasonInfo2, num6, num7) = prefab.FindSeasonByTime(num7);
				if (num6 < num3)
				{
					num6 += 1f;
					num7 += 1f;
				}
			}
			float num8 = (num6 + num7) * 0.5f;
			float num9 = math.round(math.smoothstep(num5, num8, num2) * 1600f);
			Entity val = ((seasonInfo != null) ? m_PrefabSystem.GetEntity(seasonInfo.m_Prefab) : Entity.Null);
			Entity val2 = ((seasonInfo2 != null) ? m_PrefabSystem.GetEntity(seasonInfo2.m_Prefab) : Entity.Null);
			if (val != m_LastSeason1 || val2 != m_LastSeason2 || num9 != m_LastSeasonBlend)
			{
				m_LastSeason1 = val;
				m_LastSeason2 = val2;
				m_LastSeasonBlend = num9;
				m_UpdateGroupCount = 16u;
			}
			if (m_UpdateGroupCount != 0 && m_LastUpdateGroup != num)
			{
				m_UpdateGroupCount--;
			}
			else
			{
				smoothColorsUpdated = false;
			}
		}
		m_LastUpdateGroup = num;
		if (flag || flag2 || smoothColorsUpdated)
		{
			NativeList<Entity> val3 = default(NativeList<Entity>);
			JobHandle val4 = default(JobHandle);
			if (flag)
			{
				val3 = ((EntityQuery)(ref m_AllQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			}
			else if (smoothColorsUpdated)
			{
				((EntityQuery)(ref m_PlantQuery)).ResetFilter();
				((EntityQuery)(ref m_PlantQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame
				{
					m_Index = num
				});
				val3 = ((EntityQuery)(ref m_PlantQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			}
			else
			{
				val3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				val4 = default(JobHandle);
			}
			if (flag2)
			{
				NativeQueue<Entity> queue = default(NativeQueue<Entity>);
				queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				FindUpdatedMeshColorsJob findUpdatedMeshColorsJob = new FindUpdatedMeshColorsJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_RentersUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RentersUpdated>(ref __TypeHandle.__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ColorUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<ColorUpdated>(ref __TypeHandle.__Game_Routes_ColorUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Queue = queue.AsParallelWriter()
				};
				ListUpdatedMeshColorsJob obj = new ListUpdatedMeshColorsJob
				{
					m_Queue = queue,
					m_List = val3
				};
				JobHandle val5 = JobChunkExtensions.ScheduleParallel<FindUpdatedMeshColorsJob>(findUpdatedMeshColorsJob, m_UpdateQuery, ((SystemBase)this).Dependency);
				JobHandle val6 = IJobExtensions.Schedule<ListUpdatedMeshColorsJob>(obj, JobHandle.CombineDependencies(val4, val5));
				val4 = val6;
				queue.Dispose(val6);
			}
			else
			{
				val4 = JobHandle.CombineDependencies(val4, ((SystemBase)this).Dependency);
			}
			Entity entity = Entity.Null;
			if ((Object)(object)m_OverridePrefab != (Object)null)
			{
				m_PrefabSystem.TryGetEntity(m_OverridePrefab, out entity);
			}
			NativeQueue<CopyColorData> copyColors = default(NativeQueue<CopyColorData>);
			copyColors._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			SetMeshColorsJob setMeshColorsJob = new SetMeshColorsJob
			{
				m_RandomSeed = RandomSeed.Next(),
				m_DefaultBrand = ((EntityQuery)(ref m_BuildingSettingsQuery)).GetSingleton<BuildingConfigurationData>().m_DefaultRenterBrand,
				m_Season1 = m_LastSeason1,
				m_Season2 = m_LastSeason2,
				m_SeasonBlend = m_LastSeasonBlend,
				m_OverrideEntity = m_OverrideEntity,
				m_OverrideMesh = entity,
				m_OverrideIndex = m_OverrideIndex,
				m_Stage = (flag ? UpdateStage.IgnoreSubs : UpdateStage.Default),
				m_Entities = val3.AsDeferredJobArray(),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentRouteData = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteColorData = InternalCompilerInterface.GetComponentLookup<Game.Routes.Color>(ref __TypeHandle.__Game_Routes_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompanyData = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BrandData = InternalCompilerInterface.GetComponentLookup<BrandData>(ref __TypeHandle.__Game_Prefabs_BrandData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CreatureData = InternalCompilerInterface.GetComponentLookup<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResidentData = InternalCompilerInterface.GetComponentLookup<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ColorVariations = InternalCompilerInterface.GetBufferLookup<ColorVariation>(ref __TypeHandle.__Game_Prefabs_ColorVariation_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ColorFilters = InternalCompilerInterface.GetBufferLookup<ColorFilter>(ref __TypeHandle.__Game_Prefabs_ColorFilter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OverlayElements = InternalCompilerInterface.GetBufferLookup<OverlayElement>(ref __TypeHandle.__Game_Prefabs_OverlayElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CopyColors = copyColors.AsParallelWriter()
			};
			CopyMeshColorsJob obj2 = new CopyMeshColorsJob
			{
				m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CopyColors = copyColors
			};
			JobHandle val7 = IJobParallelForDeferExtensions.Schedule<SetMeshColorsJob, Entity>(setMeshColorsJob, val3, 4, val4);
			if (flag)
			{
				setMeshColorsJob.m_Stage = UpdateStage.IgnoreOwners;
				val7 = IJobParallelForDeferExtensions.Schedule<SetMeshColorsJob, Entity>(setMeshColorsJob, val3, 4, val7);
			}
			JobHandle val8 = IJobExtensions.Schedule<CopyMeshColorsJob>(obj2, val7);
			val3.Dispose(val7);
			copyColors.Dispose(val8);
			((SystemBase)this).Dependency = val8;
		}
	}

	public ColorGroupID GetColorGroupID(string name)
	{
		int value = -1;
		if (!string.IsNullOrEmpty(name) && !m_GroupIDs.TryGetValue(name, out value))
		{
			value = m_GroupIDs.Count;
			m_GroupIDs.Add(name, value);
		}
		return new ColorGroupID(value);
	}

	private static void RandomizeColor(ref Color color, ref Random random, float3 min, float3 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		Color.RGBToHSV(color, ref val.x, ref val.y, ref val.z);
		float a = color.a;
		float3 val2 = ((Random)(ref random)).NextFloat3(min, max);
		val.x = math.frac(val.x + val2.x);
		((float3)(ref val)).yz = math.saturate(((float3)(ref val)).yz * ((float3)(ref val2)).yz);
		color = Color.HSVToRGB(val.x, val.y, val.z);
		color.a = a;
	}

	private static void RandomizeAlphas(ref ColorSet colorSet, ref Random random, float3 min, float3 max)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		((float3)(ref val))._002Ector(colorSet.m_Channel0.a, colorSet.m_Channel1.a, colorSet.m_Channel2.a);
		float3 val2 = ((Random)(ref random)).NextFloat3(min, max);
		val = math.saturate(val + val2);
		colorSet.m_Channel0.a = val.x;
		colorSet.m_Channel1.a = val.y;
		colorSet.m_Channel2.a = val.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public MeshColorSystem()
	{
	}
}
