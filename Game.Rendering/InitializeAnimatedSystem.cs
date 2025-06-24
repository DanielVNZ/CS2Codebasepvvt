using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class InitializeAnimatedSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeAnimatedJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CharacterStyleData> m_CharacterStyleData;

		[ReadOnly]
		public BufferLookup<MeshGroup> m_MeshGroups;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColors;

		[ReadOnly]
		public BufferLookup<AnimationClip> m_AnimationClips;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> m_SubMeshGroups;

		[ReadOnly]
		public BufferLookup<CharacterElement> m_CharacterElements;

		[ReadOnly]
		public BufferLookup<OverlayElement> m_OverlayElements;

		public BufferLookup<Animated> m_Animateds;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public AnimatedSystem.AllocationData m_AllocationData;

		public void Execute()
		{
			for (int i = 0; i < m_CullingData.Length; i++)
			{
				PreCullingData cullingData = m_CullingData[i];
				if ((cullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated)) != 0 && (cullingData.m_Flags & PreCullingFlags.Animated) != 0)
				{
					if ((cullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
					{
						Remove(cullingData);
					}
					else
					{
						Update(cullingData);
					}
				}
			}
		}

		private void Remove(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Animated> animateds = m_Animateds[cullingData.m_Entity];
			Deallocate(animateds);
			animateds.Clear();
		}

		private unsafe void Update(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_SubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				DynamicBuffer<Animated> animateds = m_Animateds[cullingData.m_Entity];
				DynamicBuffer<MeshGroup> val2 = default(DynamicBuffer<MeshGroup>);
				DynamicBuffer<MeshColor> meshColors = default(DynamicBuffer<MeshColor>);
				DynamicBuffer<CharacterElement> val3 = default(DynamicBuffer<CharacterElement>);
				int num = val.Length;
				DynamicBuffer<SubMeshGroup> val4 = default(DynamicBuffer<SubMeshGroup>);
				if (m_SubMeshGroups.TryGetBuffer(prefabRef.m_Prefab, ref val4))
				{
					if (m_MeshGroups.TryGetBuffer(cullingData.m_Entity, ref val2))
					{
						num = val2.Length;
						m_MeshColors.TryGetBuffer(cullingData.m_Entity, ref meshColors);
					}
					else
					{
						num = 1;
					}
					m_CharacterElements.TryGetBuffer(prefabRef.m_Prefab, ref val3);
				}
				bool flag = animateds.Length != num;
				if (!flag && (cullingData.m_Flags & (PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated)) == 0)
				{
					return;
				}
				Deallocate(animateds);
				animateds.ResizeUninitialized(num);
				OverlayIndex* ptr = stackalloc OverlayIndex[8];
				MeshGroup meshGroup = default(MeshGroup);
				MeshGroup meshGroup2 = default(MeshGroup);
				DynamicBuffer<AnimationClip> val6 = default(DynamicBuffer<AnimationClip>);
				for (int i = 0; i < num; i++)
				{
					Animated animated = ((!flag) ? animateds[i] : new Animated
					{
						m_ClipIndexBody0 = -1,
						m_ClipIndexBody0I = -1,
						m_ClipIndexBody1 = -1,
						m_ClipIndexBody1I = -1,
						m_ClipIndexFace0 = -1,
						m_ClipIndexFace1 = -1
					});
					Entity val5;
					AnimationLayerMask animationLayerMask;
					if (val3.IsCreated)
					{
						CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup);
						SubMeshGroup subMeshGroup = val4[(int)meshGroup.m_SubMeshGroup];
						CharacterElement characterElement = val3[(int)meshGroup.m_SubMeshGroup];
						CharacterStyleData characterStyleData = m_CharacterStyleData[characterElement.m_Style];
						val5 = characterElement.m_Style;
						animationLayerMask = characterStyleData.m_AnimationLayerMask;
						animated.m_BoneAllocation = m_AllocationData.AllocateBones(characterStyleData.m_BoneCount);
						MetaBufferData metaBufferData = new MetaBufferData
						{
							m_BoneOffset = (int)((NativeHeapBlock)(ref animated.m_BoneAllocation)).Begin,
							m_BoneCount = characterStyleData.m_BoneCount,
							m_ShapeCount = characterStyleData.m_ShapeCount,
							m_ShapeWeights = characterElement.m_ShapeWeights,
							m_TextureWeights = characterElement.m_TextureWeights,
							m_OverlayWeights = characterElement.m_OverlayWeights,
							m_MaskWeights = characterElement.m_MaskWeights
						};
						DynamicBuffer<OverlayElement> overlayElements = default(DynamicBuffer<OverlayElement>);
						for (int j = subMeshGroup.m_SubMeshRange.x; j < subMeshGroup.m_SubMeshRange.y; j++)
						{
							SubMesh subMesh = val[j];
							if (m_OverlayElements.TryGetBuffer(subMesh.m_SubMesh, ref overlayElements))
							{
								break;
							}
						}
						AddOverlayIndex(ptr, 0, overlayElements, characterElement.m_OverlayWeights.m_Weight0);
						AddOverlayIndex(ptr, 1, overlayElements, characterElement.m_OverlayWeights.m_Weight1);
						AddOverlayIndex(ptr, 2, overlayElements, characterElement.m_OverlayWeights.m_Weight2);
						AddOverlayIndex(ptr, 3, overlayElements, characterElement.m_OverlayWeights.m_Weight3);
						AddOverlayIndex(ptr, 4, overlayElements, characterElement.m_OverlayWeights.m_Weight4);
						AddOverlayIndex(ptr, 5, overlayElements, characterElement.m_OverlayWeights.m_Weight5);
						AddOverlayIndex(ptr, 6, overlayElements, characterElement.m_OverlayWeights.m_Weight6);
						AddOverlayIndex(ptr, 7, overlayElements, characterElement.m_OverlayWeights.m_Weight7);
						NativeSortExtension.Sort<OverlayIndex>(ptr, 8);
						metaBufferData.m_OverlayWeights.m_Weight0 = ptr->m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight1 = ptr[1].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight2 = ptr[2].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight3 = ptr[3].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight4 = ptr[4].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight5 = ptr[5].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight6 = ptr[6].m_Weight;
						metaBufferData.m_OverlayWeights.m_Weight7 = ptr[7].m_Weight;
						int colorOffset = meshGroup.m_ColorOffset + (subMeshGroup.m_SubMeshRange.y - subMeshGroup.m_SubMeshRange.x);
						metaBufferData.m_OverlayColors1.m_Color0 = GetOverlayColor(ptr, 0, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color1 = GetOverlayColor(ptr, 1, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color2 = GetOverlayColor(ptr, 2, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color3 = GetOverlayColor(ptr, 3, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color4 = GetOverlayColor(ptr, 4, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color5 = GetOverlayColor(ptr, 5, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color6 = GetOverlayColor(ptr, 6, meshColors, colorOffset);
						metaBufferData.m_OverlayColors1.m_Color7 = GetOverlayColor(ptr, 7, meshColors, colorOffset);
						animated.m_MetaIndex = m_AllocationData.AddMetaBufferData(metaBufferData);
					}
					else
					{
						int num2 = i;
						if (val4.IsCreated)
						{
							CollectionUtils.TryGet<MeshGroup>(val2, i, ref meshGroup2);
							num2 = val4[(int)meshGroup2.m_SubMeshGroup].m_SubMeshRange.x;
						}
						val5 = val[num2].m_SubMesh;
						animationLayerMask = new AnimationLayerMask(AnimationLayer.Body);
					}
					if (flag && m_AnimationClips.TryGetBuffer(val5, ref val6) && val6.Length != 0)
					{
						if ((animationLayerMask.m_Mask & new AnimationLayerMask(AnimationLayer.Body).m_Mask) != 0)
						{
							animated.m_ClipIndexBody0 = 0;
						}
						if ((animationLayerMask.m_Mask & new AnimationLayerMask(AnimationLayer.Facial).m_Mask) != 0)
						{
							animated.m_ClipIndexFace0 = 0;
						}
					}
					animateds[i] = animated;
				}
			}
			else
			{
				Remove(cullingData);
			}
		}

		private void Deallocate(DynamicBuffer<Animated> animateds)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < animateds.Length; i++)
			{
				Animated animated = animateds[i];
				if (!((NativeHeapBlock)(ref animated.m_BoneAllocation)).Empty)
				{
					m_AllocationData.ReleaseBones(animated.m_BoneAllocation);
				}
				if (animated.m_MetaIndex != 0)
				{
					m_AllocationData.RemoveMetaBufferData(animated.m_MetaIndex);
				}
			}
		}

		private unsafe void AddOverlayIndex(OverlayIndex* overlayIndex, int index, DynamicBuffer<OverlayElement> overlayElements, BlendWeight weight)
		{
			int sortOrder = 0;
			if (overlayElements.IsCreated && weight.m_Index >= 0 && weight.m_Index < overlayElements.Length)
			{
				sortOrder = overlayElements[weight.m_Index].m_SortOrder;
			}
			overlayIndex[index] = new OverlayIndex
			{
				m_Weight = weight,
				m_OriginalIndex = index,
				m_SortOrder = sortOrder
			};
		}

		private unsafe Color GetOverlayColor(OverlayIndex* overlayIndex, int index, DynamicBuffer<MeshColor> meshColors, int colorOffset)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (meshColors.IsCreated && meshColors.Length >= colorOffset + 8)
			{
				MeshColor meshColor = meshColors[colorOffset + overlayIndex[index].m_OriginalIndex];
				return ((Color)(ref meshColor.m_ColorSet.m_Channel0)).linear;
			}
			return Color.white;
		}
	}

	private struct OverlayIndex : IComparable<OverlayIndex>
	{
		public BlendWeight m_Weight;

		public int m_OriginalIndex;

		public int m_SortOrder;

		public int CompareTo(OverlayIndex other)
		{
			return math.select(m_OriginalIndex - other.m_OriginalIndex, m_SortOrder - other.m_SortOrder, m_SortOrder != other.m_SortOrder);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CharacterStyleData> __Game_Prefabs_CharacterStyleData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<MeshGroup> __Game_Rendering_MeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AnimationClip> __Game_Prefabs_AnimationClip_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMeshGroup> __Game_Prefabs_SubMeshGroup_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CharacterElement> __Game_Prefabs_CharacterElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<OverlayElement> __Game_Prefabs_OverlayElement_RO_BufferLookup;

		public BufferLookup<Animated> __Game_Rendering_Animated_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CharacterStyleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CharacterStyleData>(true);
			__Game_Rendering_MeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshGroup>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Prefabs_AnimationClip_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AnimationClip>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Prefabs_SubMeshGroup_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMeshGroup>(true);
			__Game_Prefabs_CharacterElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CharacterElement>(true);
			__Game_Prefabs_OverlayElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OverlayElement>(true);
			__Game_Rendering_Animated_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Animated>(false);
		}
	}

	private AnimatedSystem m_AnimatedSystem;

	private PreCullingSystem m_PreCullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = IJobExtensions.Schedule<InitializeAnimatedJob>(new InitializeAnimatedJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterStyleData = InternalCompilerInterface.GetComponentLookup<CharacterStyleData>(ref __TypeHandle.__Game_Prefabs_CharacterStyleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshGroups = InternalCompilerInterface.GetBufferLookup<MeshGroup>(ref __TypeHandle.__Game_Rendering_MeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimationClips = InternalCompilerInterface.GetBufferLookup<AnimationClip>(ref __TypeHandle.__Game_Prefabs_AnimationClip_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshGroups = InternalCompilerInterface.GetBufferLookup<SubMeshGroup>(ref __TypeHandle.__Game_Prefabs_SubMeshGroup_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CharacterElements = InternalCompilerInterface.GetBufferLookup<CharacterElement>(ref __TypeHandle.__Game_Prefabs_CharacterElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OverlayElements = InternalCompilerInterface.GetBufferLookup<OverlayElement>(ref __TypeHandle.__Game_Prefabs_OverlayElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Animateds = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies),
			m_AllocationData = m_AnimatedSystem.GetAllocationData(out dependencies2)
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		m_AnimatedSystem.AddAllocationWriter(val);
		m_PreCullingSystem.AddCullingDataReader(val);
		((SystemBase)this).Dependency = val;
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
	public InitializeAnimatedSystem()
	{
	}
}
