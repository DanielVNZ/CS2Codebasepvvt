using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Effects;

[CompilerGenerated]
public class EffectControlSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct EffectControlJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Object> m_ObjectType;

		[ReadOnly]
		public ComponentTypeHandle<Static> m_StaticType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Events.Event> m_EventType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<EnabledEffect> m_EffectOwnerType;

		[ReadOnly]
		public ComponentLookup<EffectData> m_PrefabEffectData;

		[ReadOnly]
		public ComponentLookup<TrafficLights> m_TrafficLightsData;

		[ReadOnly]
		public ComponentLookup<LightEffectData> m_PrefabLightEffectData;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> m_PrefabAudioEffectData;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledEffectData;

		[NativeDisableContainerSafetyRestriction]
		public Writer<EnabledAction> m_ActionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<EnabledEffect> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<EnabledEffect>(ref m_EffectOwnerType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity owner = nativeArray[i];
					DynamicBuffer<EnabledEffect> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						EnabledEffect enabledEffect = val[j];
						m_ActionQueue.Enqueue(new EnabledAction
						{
							m_Owner = owner,
							m_EffectIndex = enabledEffect.m_EffectIndex,
							m_Flags = ActionFlags.Deleted
						});
					}
				}
				return;
			}
			bool flag = !((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType) && (((ArchetypeChunk)(ref chunk)).Has<Static>(ref m_StaticType) || (!((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Object>(ref m_ObjectType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Events.Event>(ref m_EventType)));
			NativeArray<CullingInfo> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			NativeArray<Game.Tools.EditorContainer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.EditorContainer>(ref m_EditorContainerType);
			NativeArray<Transform> transforms = default(NativeArray<Transform>);
			NativeArray<Curve> curves = default(NativeArray<Curve>);
			NativeArray<PrefabRef> val2 = default(NativeArray<PrefabRef>);
			if (flag)
			{
				transforms = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				curves = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			}
			if (nativeArray3.Length == 0)
			{
				val2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			}
			TrafficLights trafficLights = default(TrafficLights);
			Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
			EffectData effectData = default(EffectData);
			DynamicBuffer<Effect> val5 = default(DynamicBuffer<Effect>);
			EffectData effectData2 = default(EffectData);
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity val3 = nativeArray[k];
				DynamicBuffer<EnabledEffect> val4 = bufferAccessor[k];
				if (flag && m_TrafficLightsData.TryGetComponent(val3, ref trafficLights) && (trafficLights.m_Flags & TrafficLightFlags.MoveableBridge) != 0)
				{
					flag = false;
				}
				if (CollectionUtils.TryGet<Game.Tools.EditorContainer>(nativeArray3, k, ref editorContainer))
				{
					for (int l = 0; l < val4.Length; l++)
					{
						EnabledEffect enabledEffect2 = val4[l];
						EnabledEffectData enabledEffectData = m_EnabledEffectData[enabledEffect2.m_EnabledIndex];
						if (editorContainer.m_Prefab != enabledEffectData.m_Prefab)
						{
							m_ActionQueue.Enqueue(new EnabledAction
							{
								m_Owner = val3,
								m_EffectIndex = enabledEffect2.m_EffectIndex,
								m_Flags = ActionFlags.WrongPrefab
							});
						}
					}
					if (!m_PrefabEffectData.TryGetComponent(editorContainer.m_Prefab, ref effectData))
					{
						continue;
					}
					ActionFlags flags = (flag ? (ActionFlags.CheckEnabled | ActionFlags.IsStatic | ActionFlags.OwnerUpdated) : (ActionFlags.CheckEnabled | ActionFlags.OwnerUpdated));
					if (effectData.m_OwnerCulling)
					{
						if (!IsNearCamera(nativeArray2, k))
						{
							continue;
						}
					}
					else if (flag)
					{
						Effect effect = new Effect
						{
							m_Effect = editorContainer.m_Prefab
						};
						if (!IsNearCamera(transforms, curves, k, effect))
						{
							flags = (ActionFlags)0;
						}
					}
					m_ActionQueue.Enqueue(new EnabledAction
					{
						m_Owner = val3,
						m_EffectIndex = 0,
						m_Flags = flags
					});
					continue;
				}
				PrefabRef prefabRef = val2[k];
				m_PrefabEffects.TryGetBuffer(prefabRef.m_Prefab, ref val5);
				for (int m = 0; m < val4.Length; m++)
				{
					EnabledEffect enabledEffect3 = val4[m];
					EnabledEffectData enabledEffectData2 = m_EnabledEffectData[enabledEffect3.m_EnabledIndex];
					if (!val5.IsCreated || val5.Length <= enabledEffect3.m_EffectIndex || val5[enabledEffect3.m_EffectIndex].m_Effect != enabledEffectData2.m_Prefab)
					{
						m_ActionQueue.Enqueue(new EnabledAction
						{
							m_Owner = val3,
							m_EffectIndex = enabledEffect3.m_EffectIndex,
							m_Flags = ActionFlags.WrongPrefab
						});
					}
				}
				if (!val5.IsCreated)
				{
					continue;
				}
				bool flag2 = IsNearCamera(nativeArray2, k);
				for (int n = 0; n < val5.Length; n++)
				{
					Effect effect2 = val5[n];
					if (!m_PrefabEffectData.TryGetComponent(effect2.m_Effect, ref effectData2))
					{
						continue;
					}
					ActionFlags flags2 = (flag ? (ActionFlags.CheckEnabled | ActionFlags.IsStatic | ActionFlags.OwnerUpdated) : (ActionFlags.CheckEnabled | ActionFlags.OwnerUpdated));
					if (effectData2.m_OwnerCulling)
					{
						bool flag3 = m_PrefabAudioEffectData.HasComponent(effect2.m_Effect);
						if (!flag2 && !flag3)
						{
							continue;
						}
					}
					if (flag && !IsNearCamera(transforms, curves, k, effect2))
					{
						flags2 = (ActionFlags)0;
					}
					m_ActionQueue.Enqueue(new EnabledAction
					{
						m_Owner = val3,
						m_EffectIndex = n,
						m_Flags = flags2
					});
				}
			}
		}

		private bool IsNearCamera(NativeArray<Transform> transforms, NativeArray<Curve> curves, int index, Effect effect)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			QuadTreeBoundsXZ bounds = SearchSystem.GetBounds(transforms, curves, index, effect, ref m_PrefabLightEffectData, ref m_PrefabAudioEffectData);
			float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
			return RenderingUtils.CalculateLod(num * num, m_LodParameters) >= bounds.m_MinLod;
		}

		private bool IsNearCamera(NativeArray<CullingInfo> cullingInfos, int index)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			if (CollectionUtils.TryGet<CullingInfo>(cullingInfos, index, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
			{
				return (m_CullingData[cullingInfo.m_CullingIndex].m_Flags & PreCullingFlags.NearCamera) != 0;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct EffectCullingJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Static> m_StaticData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<EffectData> m_PrefabEffectData;

		[ReadOnly]
		public BufferLookup<EnabledEffect> m_EffectOwners;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledEffectData;

		[NativeDisableContainerSafetyRestriction]
		public Writer<EnabledAction> m_ActionQueue;

		public void Execute(int index)
		{
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData preCullingData = m_CullingData[index];
			if ((preCullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Deleted | PreCullingFlags.Created | PreCullingFlags.EffectInstances)) != (PreCullingFlags.NearCameraUpdated | PreCullingFlags.EffectInstances))
			{
				return;
			}
			if ((preCullingData.m_Flags & PreCullingFlags.NearCamera) != 0)
			{
				PrefabRef prefabRef = m_PrefabRefData[preCullingData.m_Entity];
				bool flag = ((preCullingData.m_Flags & PreCullingFlags.Temp) == 0 && (preCullingData.m_Flags & PreCullingFlags.Object) == 0) || m_StaticData.HasComponent(preCullingData.m_Entity);
				DynamicBuffer<Effect> val = default(DynamicBuffer<Effect>);
				Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
				EffectData effectData2 = default(EffectData);
				if (m_PrefabEffects.TryGetBuffer(prefabRef.m_Prefab, ref val))
				{
					EffectData effectData = default(EffectData);
					for (int i = 0; i < val.Length; i++)
					{
						Effect effect = val[i];
						if (m_PrefabEffectData.TryGetComponent(effect.m_Effect, ref effectData) && effectData.m_OwnerCulling)
						{
							m_ActionQueue.Enqueue(new EnabledAction
							{
								m_Owner = preCullingData.m_Entity,
								m_EffectIndex = i,
								m_Flags = ((!flag) ? ActionFlags.CheckEnabled : (ActionFlags.CheckEnabled | ActionFlags.IsStatic))
							});
						}
					}
				}
				else if (m_EditorContainerData.TryGetComponent(preCullingData.m_Entity, ref editorContainer) && m_PrefabEffectData.TryGetComponent(editorContainer.m_Prefab, ref effectData2) && effectData2.m_OwnerCulling)
				{
					m_ActionQueue.Enqueue(new EnabledAction
					{
						m_Owner = preCullingData.m_Entity,
						m_EffectIndex = 0,
						m_Flags = ((!flag) ? ActionFlags.CheckEnabled : (ActionFlags.CheckEnabled | ActionFlags.IsStatic))
					});
				}
				return;
			}
			DynamicBuffer<EnabledEffect> val2 = m_EffectOwners[preCullingData.m_Entity];
			for (int j = 0; j < val2.Length; j++)
			{
				EnabledEffect enabledEffect = val2[j];
				EnabledEffectData enabledEffectData = m_EnabledEffectData[enabledEffect.m_EnabledIndex];
				if (m_PrefabEffectData[enabledEffectData.m_Prefab].m_OwnerCulling)
				{
					m_ActionQueue.Enqueue(new EnabledAction
					{
						m_Owner = preCullingData.m_Entity,
						m_EffectIndex = enabledEffect.m_EffectIndex,
						m_Flags = (ActionFlags)0
					});
				}
			}
		}
	}

	[BurstCompile]
	private struct TreeCullingJob1 : IJob
	{
		[ReadOnly]
		public NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> m_EffectSearchTree;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float4 m_PrevLodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_PrevCameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float3 m_PrevCameraDirection;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> m_NodeBuffer;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> m_SubDataBuffer;

		[NativeDisableContainerSafetyRestriction]
		public Writer<EnabledAction> m_ActionQueue;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			TreeCullingIterator treeCullingIterator = new TreeCullingIterator
			{
				m_LodParameters = m_LodParameters,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = m_CameraPosition,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = m_CameraDirection,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_ActionQueue = m_ActionQueue
			};
			m_EffectSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, 3, m_NodeBuffer, m_SubDataBuffer);
		}
	}

	[BurstCompile]
	private struct TreeCullingJob2 : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> m_EffectSearchTree;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float4 m_PrevLodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_PrevCameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float3 m_PrevCameraDirection;

		[ReadOnly]
		public NativeArray<int> m_NodeBuffer;

		[ReadOnly]
		public NativeArray<int> m_SubDataBuffer;

		[NativeDisableContainerSafetyRestriction]
		public Writer<EnabledAction> m_ActionQueue;

		public void Execute(int index)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			TreeCullingIterator treeCullingIterator = new TreeCullingIterator
			{
				m_LodParameters = m_LodParameters,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = m_CameraPosition,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = m_CameraDirection,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_ActionQueue = m_ActionQueue
			};
			m_EffectSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, m_SubDataBuffer[index], m_NodeBuffer[index]);
		}
	}

	private struct TreeCullingIterator : INativeQuadTreeIteratorWithSubData<SourceInfo, QuadTreeBoundsXZ, int>, IUnsafeQuadTreeIteratorWithSubData<SourceInfo, QuadTreeBoundsXZ, int>
	{
		public float4 m_LodParameters;

		public float3 m_CameraPosition;

		public float3 m_CameraDirection;

		public float3 m_PrevCameraPosition;

		public float4 m_PrevLodParameters;

		public float3 m_PrevCameraDirection;

		public Writer<EnabledAction> m_ActionQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds, ref int subData)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			switch (subData)
			{
			case 1:
			{
				float num13 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num14 = RenderingUtils.CalculateLod(num13 * num13, m_LodParameters);
				if (num14 < bounds.m_MinLod)
				{
					return false;
				}
				float num15 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num16 = RenderingUtils.CalculateLod(num15 * num15, m_PrevLodParameters);
				if (num16 < bounds.m_MaxLod)
				{
					return num14 > num16;
				}
				return false;
			}
			case 2:
			{
				float num9 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num10 = RenderingUtils.CalculateLod(num9 * num9, m_PrevLodParameters);
				if (num10 < bounds.m_MinLod)
				{
					return false;
				}
				float num11 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num12 = RenderingUtils.CalculateLod(num11 * num11, m_LodParameters);
				if (num12 < bounds.m_MaxLod)
				{
					return num10 > num12;
				}
				return false;
			}
			default:
			{
				float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				float num2 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num3 = RenderingUtils.CalculateLod(num * num, m_LodParameters);
				int num4 = RenderingUtils.CalculateLod(num2 * num2, m_PrevLodParameters);
				subData = 0;
				if (num3 >= bounds.m_MinLod)
				{
					float num5 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
					int num6 = RenderingUtils.CalculateLod(num5 * num5, m_PrevLodParameters);
					subData |= math.select(0, 1, num6 < bounds.m_MaxLod && num3 > num6);
				}
				if (num4 >= bounds.m_MinLod)
				{
					float num7 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					int num8 = RenderingUtils.CalculateLod(num7 * num7, m_LodParameters);
					subData |= math.select(0, 2, num8 < bounds.m_MaxLod && num4 > num8);
				}
				return subData != 0;
			}
			}
		}

		public void Iterate(QuadTreeBoundsXZ bounds, int subData, SourceInfo sourceInfo)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			switch (subData)
			{
			case 1:
			{
				float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				if (RenderingUtils.CalculateLod(num * num, m_LodParameters) >= bounds.m_MinLod)
				{
					float num2 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
					if (RenderingUtils.CalculateLod(num2 * num2, m_PrevLodParameters) < bounds.m_MaxLod)
					{
						m_ActionQueue.Enqueue(new EnabledAction
						{
							m_Owner = sourceInfo.m_Entity,
							m_EffectIndex = sourceInfo.m_EffectIndex,
							m_Flags = (ActionFlags.SkipEnabled | ActionFlags.IsStatic)
						});
					}
				}
				return;
			}
			case 2:
			{
				float num3 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				if (RenderingUtils.CalculateLod(num3 * num3, m_PrevLodParameters) >= bounds.m_MinLod)
				{
					float num4 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					if (RenderingUtils.CalculateLod(num4 * num4, m_LodParameters) < bounds.m_MaxLod)
					{
						m_ActionQueue.Enqueue(new EnabledAction
						{
							m_Owner = sourceInfo.m_Entity,
							m_EffectIndex = sourceInfo.m_EffectIndex,
							m_Flags = (ActionFlags)0
						});
					}
				}
				return;
			}
			}
			float num5 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
			float num6 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
			int num7 = RenderingUtils.CalculateLod(num5 * num5, m_LodParameters);
			int num8 = RenderingUtils.CalculateLod(num6 * num6, m_PrevLodParameters);
			bool flag = num7 >= bounds.m_MinLod;
			bool flag2 = num8 >= bounds.m_MaxLod;
			if (flag != flag2)
			{
				m_ActionQueue.Enqueue(new EnabledAction
				{
					m_Owner = sourceInfo.m_Entity,
					m_EffectIndex = sourceInfo.m_EffectIndex,
					m_Flags = (flag ? (ActionFlags.SkipEnabled | ActionFlags.IsStatic) : ((ActionFlags)0))
				});
			}
		}
	}

	[Flags]
	public enum ActionFlags : byte
	{
		CheckEnabled = 1,
		Deleted = 2,
		SkipEnabled = 4,
		IsStatic = 8,
		OwnerUpdated = 0x10,
		WrongPrefab = 0x20
	}

	private struct EnabledAction
	{
		public Entity m_Owner;

		public int m_EffectIndex;

		public ActionFlags m_Flags;

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Owner)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct OverflowAction
	{
		public Entity m_Owner;

		public Entity m_Prefab;

		public int m_DataIndex;

		public int m_EffectIndex;

		public EnabledEffectFlags m_Flags;
	}

	[BurstCompile]
	private struct EnabledActionJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<VFXData> m_VFXDatas;

		[ReadOnly]
		public ComponentLookup<RandomTransformData> m_RandomTransformDatas;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryDatas;

		[ReadOnly]
		public BufferLookup<AudioSourceData> m_AudioSourceDatas;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[NativeDisableParallelForRestriction]
		public BufferLookup<EnabledEffect> m_EffectOwners;

		[ReadOnly]
		public Reader<EnabledAction> m_CullingActions;

		public ParallelWriter<OverflowAction> m_OverflowActions;

		public ParallelWriter<VFXUpdateInfo> m_VFXUpdateQueue;

		[NativeDisableParallelForRestriction]
		public NativeList<EnabledEffectData> m_EnabledData;

		[NativeDisableParallelForRestriction]
		public NativeReference<int> m_EnabledDataIndex;

		public EffectControlData m_EffectControlData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<EnabledAction> enumerator = m_CullingActions.GetEnumerator(index);
			DynamicBuffer<Effect> val2 = default(DynamicBuffer<Effect>);
			Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
			while (enumerator.MoveNext())
			{
				EnabledAction current = enumerator.Current;
				if ((current.m_Flags & (ActionFlags.CheckEnabled | ActionFlags.SkipEnabled)) != 0)
				{
					PrefabRef prefabRef = m_EffectControlData.m_Prefabs[current.m_Owner];
					Entity val = Entity.Null;
					bool isAnimated = false;
					bool isEditorContainer = false;
					if (m_PrefabEffects.TryGetBuffer(prefabRef.m_Prefab, ref val2))
					{
						Effect effect = val2[current.m_EffectIndex];
						val = effect.m_Effect;
						isAnimated = effect.m_BoneIndex.x >= 0 || effect.m_AnimationIndex >= 0;
					}
					else if (m_EditorContainerData.TryGetComponent(current.m_Owner, ref editorContainer))
					{
						val = editorContainer.m_Prefab;
						isAnimated = editorContainer.m_GroupIndex >= 0;
						isEditorContainer = true;
					}
					bool checkEnabled = (current.m_Flags & ActionFlags.CheckEnabled) != 0;
					if (m_EffectControlData.ShouldBeEnabled(current.m_Owner, val, checkEnabled, isEditorContainer))
					{
						Enable(current, val, isAnimated, isEditorContainer);
						continue;
					}
				}
				Disable(current);
			}
			enumerator.Dispose();
		}

		private unsafe void Enable(EnabledAction enabledAction, Entity effectPrefab, bool isAnimated, bool isEditorContainer)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<EnabledEffect> val = m_EffectOwners[enabledAction.m_Owner];
			for (int i = 0; i < val.Length; i++)
			{
				ref EnabledEffect reference = ref val.ElementAt(i);
				if (reference.m_EffectIndex != enabledAction.m_EffectIndex)
				{
					continue;
				}
				if (reference.m_EnabledIndex >= m_EnabledData.Length)
				{
					return;
				}
				ref EnabledEffectData reference2 = ref UnsafeUtility.ArrayElementAsRef<EnabledEffectData>((void*)NativeListUnsafeUtility.GetUnsafePtr<EnabledEffectData>(m_EnabledData), reference.m_EnabledIndex);
				if (reference2.m_Prefab != effectPrefab)
				{
					continue;
				}
				if ((enabledAction.m_Flags & ActionFlags.OwnerUpdated) != 0)
				{
					if ((enabledAction.m_Flags & ActionFlags.IsStatic) == 0 || isAnimated || m_InterpolatedTransformData.HasComponent(enabledAction.m_Owner))
					{
						reference2.m_Flags |= EnabledEffectFlags.DynamicTransform;
					}
					else
					{
						reference2.m_Flags &= ~EnabledEffectFlags.DynamicTransform;
					}
					if (OwnerCollapsed(enabledAction.m_Owner))
					{
						reference2.m_Flags |= EnabledEffectFlags.OwnerCollapsed;
					}
					else
					{
						reference2.m_Flags &= ~EnabledEffectFlags.OwnerCollapsed;
					}
				}
				if ((reference2.m_Flags & EnabledEffectFlags.IsEnabled) == 0)
				{
					reference2.m_Flags |= EnabledEffectFlags.IsEnabled | EnabledEffectFlags.EnabledUpdated;
					if ((reference2.m_Flags & EnabledEffectFlags.IsVFX) != 0)
					{
						m_VFXUpdateQueue.Enqueue(new VFXUpdateInfo
						{
							m_Type = VFXUpdateType.Add,
							m_EnabledIndex = int2.op_Implicit(reference.m_EnabledIndex)
						});
					}
				}
				else if ((enabledAction.m_Flags & ActionFlags.OwnerUpdated) != 0)
				{
					reference2.m_Flags |= EnabledEffectFlags.OwnerUpdated;
				}
				return;
			}
			int num = Interlocked.Increment(ref UnsafeUtility.AsRef<int>((void*)NativeReferenceUnsafeUtility.GetUnsafePtr<int>(m_EnabledDataIndex))) - 1;
			val.Add(new EnabledEffect
			{
				m_EffectIndex = enabledAction.m_EffectIndex,
				m_EnabledIndex = num
			});
			EnabledEffectFlags enabledEffectFlags = EnabledEffectFlags.IsEnabled | EnabledEffectFlags.EnabledUpdated;
			if (isEditorContainer)
			{
				enabledEffectFlags |= EnabledEffectFlags.EditorContainer;
			}
			if (m_EffectControlData.m_LightEffectDatas.HasComponent(effectPrefab))
			{
				enabledEffectFlags |= EnabledEffectFlags.IsLight;
			}
			if (m_VFXDatas.HasComponent(effectPrefab))
			{
				enabledEffectFlags |= EnabledEffectFlags.IsVFX;
				m_VFXUpdateQueue.Enqueue(new VFXUpdateInfo
				{
					m_Type = VFXUpdateType.Add,
					m_EnabledIndex = int2.op_Implicit(num)
				});
			}
			if (m_AudioSourceDatas.HasBuffer(effectPrefab))
			{
				enabledEffectFlags |= EnabledEffectFlags.IsAudio;
			}
			if (m_RandomTransformDatas.HasComponent(effectPrefab))
			{
				enabledEffectFlags |= EnabledEffectFlags.RandomTransform;
			}
			if (m_EffectControlData.m_Temps.HasComponent(enabledAction.m_Owner))
			{
				enabledEffectFlags |= EnabledEffectFlags.TempOwner;
			}
			if ((enabledAction.m_Flags & ActionFlags.IsStatic) == 0 || isAnimated || m_InterpolatedTransformData.HasComponent(enabledAction.m_Owner))
			{
				enabledEffectFlags |= EnabledEffectFlags.DynamicTransform;
			}
			if (OwnerCollapsed(enabledAction.m_Owner))
			{
				enabledEffectFlags |= EnabledEffectFlags.OwnerCollapsed;
			}
			if (num >= m_EnabledData.Capacity)
			{
				m_OverflowActions.Enqueue(new OverflowAction
				{
					m_Owner = enabledAction.m_Owner,
					m_Prefab = effectPrefab,
					m_DataIndex = num,
					m_EffectIndex = enabledAction.m_EffectIndex,
					m_Flags = enabledEffectFlags
				});
			}
			else
			{
				ref EnabledEffectData reference3 = ref UnsafeUtility.ArrayElementAsRef<EnabledEffectData>((void*)NativeListUnsafeUtility.GetUnsafePtr<EnabledEffectData>(m_EnabledData), num);
				reference3 = default(EnabledEffectData);
				reference3.m_Owner = enabledAction.m_Owner;
				reference3.m_Prefab = effectPrefab;
				reference3.m_EffectIndex = enabledAction.m_EffectIndex;
				reference3.m_Flags = enabledEffectFlags;
			}
		}

		private bool OwnerCollapsed(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = default(PrefabRef);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (m_DestroyedData.HasComponent(owner) && m_PrefabRefs.TryGetComponent(owner, ref prefabRef) && m_ObjectGeometryDatas.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
			{
				return (objectGeometryData.m_Flags & (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot)) == (Game.Objects.GeometryFlags.Physical | Game.Objects.GeometryFlags.HasLot);
			}
			return false;
		}

		private unsafe void Disable(EnabledAction enabledAction)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<EnabledEffect> val = m_EffectOwners[enabledAction.m_Owner];
			for (int i = 0; i < val.Length; i++)
			{
				ref EnabledEffect reference = ref val.ElementAt(i);
				if (reference.m_EffectIndex != enabledAction.m_EffectIndex)
				{
					continue;
				}
				if (reference.m_EnabledIndex >= m_EnabledData.Length)
				{
					break;
				}
				ref EnabledEffectData reference2 = ref UnsafeUtility.ArrayElementAsRef<EnabledEffectData>((void*)NativeListUnsafeUtility.GetUnsafePtr<EnabledEffectData>(m_EnabledData), reference.m_EnabledIndex);
				if ((reference2.m_Flags & EnabledEffectFlags.IsEnabled) != 0)
				{
					reference2.m_Flags &= ~EnabledEffectFlags.IsEnabled;
					reference2.m_Flags |= EnabledEffectFlags.EnabledUpdated;
					if ((reference2.m_Flags & EnabledEffectFlags.IsVFX) != 0)
					{
						m_VFXUpdateQueue.Enqueue(new VFXUpdateInfo
						{
							m_Type = VFXUpdateType.Remove,
							m_EnabledIndex = int2.op_Implicit(reference.m_EnabledIndex)
						});
					}
				}
				if ((enabledAction.m_Flags & ActionFlags.Deleted) != 0)
				{
					reference2.m_Flags |= EnabledEffectFlags.Deleted;
				}
				if ((enabledAction.m_Flags & ActionFlags.WrongPrefab) != 0)
				{
					reference2.m_Flags |= EnabledEffectFlags.WrongPrefab;
				}
				break;
			}
		}
	}

	[BurstCompile]
	private struct ResizeEnabledDataJob : IJob
	{
		[ReadOnly]
		public NativeReference<int> m_EnabledDataIndex;

		public NativeList<EnabledEffectData> m_EnabledData;

		public NativeQueue<OverflowAction> m_OverflowActions;

		public void Execute()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			m_EnabledData.Resize(math.min(m_EnabledDataIndex.Value, m_EnabledData.Capacity), (NativeArrayOptions)0);
			m_EnabledData.Resize(m_EnabledDataIndex.Value, (NativeArrayOptions)0);
			OverflowAction overflowAction = default(OverflowAction);
			while (m_OverflowActions.TryDequeue(ref overflowAction))
			{
				ref EnabledEffectData reference = ref m_EnabledData.ElementAt(overflowAction.m_DataIndex);
				reference = default(EnabledEffectData);
				reference.m_Owner = overflowAction.m_Owner;
				reference.m_Prefab = overflowAction.m_Prefab;
				reference.m_EffectIndex = overflowAction.m_EffectIndex;
				reference.m_Flags = overflowAction.m_Flags;
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Object> __Game_Objects_Object_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Static> __Game_Objects_Static_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Events.Event> __Game_Events_Event_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficLights> __Game_Net_TrafficLights_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LightEffectData> __Game_Prefabs_LightEffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> __Game_Prefabs_AudioEffectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Static> __Game_Objects_Static_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VFXData> __Game_Prefabs_VFXData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RandomTransformData> __Game_Prefabs_RandomTransformData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AudioSourceData> __Game_Prefabs_AudioSourceData_RO_BufferLookup;

		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RW_BufferLookup;

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
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Rendering_CullingInfo_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.EditorContainer>(true);
			__Game_Objects_Object_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Object>(true);
			__Game_Objects_Static_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Static>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Events_Event_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.Event>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Effects_EnabledEffect_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<EnabledEffect>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Net_TrafficLights_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficLights>(true);
			__Game_Prefabs_LightEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LightEffectData>(true);
			__Game_Prefabs_AudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioEffectData>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Tools.EditorContainer>(true);
			__Game_Objects_Static_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Static>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Effects_EnabledEffect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Prefabs_VFXData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VFXData>(true);
			__Game_Prefabs_RandomTransformData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RandomTransformData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_AudioSourceData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AudioSourceData>(true);
			__Game_Effects_EnabledEffect_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(false);
		}
	}

	private VFXSystem m_VFXSystem;

	private SearchSystem m_SearchSystem;

	private EffectFlagSystem m_EffectFlagSystem;

	private SimulationSystem m_SimulationSystem;

	private PreCullingSystem m_PreCullingSystem;

	private ToolSystem m_ToolSystem;

	private RenderingSystem m_RenderingSystem;

	private BatchDataSystem m_BatchDataSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private EffectControlData m_EffectControlData;

	private NativeList<EnabledEffectData> m_EnabledData;

	private EntityQuery m_UpdatedEffectsQuery;

	private EntityQuery m_AllEffectsQuery;

	private JobHandle m_EnabledWriteDependencies;

	private JobHandle m_EnabledReadDependencies;

	private float3 m_PrevCameraPosition;

	private float3 m_PrevCameraDirection;

	private float4 m_PrevLodParameters;

	private bool m_Loaded;

	private bool m_ResetPrevious;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_VFXSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VFXSystem>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_EffectFlagSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectFlagSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_EffectControlData = new EffectControlData((SystemBase)(object)this);
		m_EnabledData = new NativeList<EnabledEffectData>(AllocatorHandle.op_Implicit((Allocator)4));
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EnabledEffect>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<EffectsUpdated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		array[0] = val;
		m_UpdatedEffectsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllEffectsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EnabledEffect>() });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_EnabledData.Dispose();
		base.OnDestroy();
	}

	public void PostDeserialize(Context context)
	{
		((JobHandle)(ref m_EnabledWriteDependencies)).Complete();
		((JobHandle)(ref m_EnabledReadDependencies)).Complete();
		m_EnabledData.Clear();
		m_ResetPrevious = true;
		m_Loaded = true;
	}

	public NativeList<EnabledEffectData> GetEnabledData(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_EnabledWriteDependencies : JobHandle.CombineDependencies(m_EnabledWriteDependencies, m_EnabledReadDependencies));
		return m_EnabledData;
	}

	public void AddEnabledDataReader(JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_EnabledReadDependencies = JobHandle.CombineDependencies(m_EnabledReadDependencies, dependencies);
	}

	public void AddEnabledDataWriter(JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_EnabledWriteDependencies = dependencies;
	}

	public void GetLodParameters(out float4 lodParameters, out float3 cameraPosition, out float3 cameraDirection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		lodParameters = m_PrevLodParameters;
		cameraPosition = m_PrevCameraPosition;
		cameraDirection = m_PrevCameraDirection;
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

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllEffectsQuery : m_UpdatedEffectsQuery);
		m_EffectControlData.Update((SystemBase)(object)this, m_EffectFlagSystem.GetData(), m_SimulationSystem.frameIndex, m_ToolSystem.selected);
		((JobHandle)(ref m_EnabledWriteDependencies)).Complete();
		((JobHandle)(ref m_EnabledReadDependencies)).Complete();
		int length = m_EnabledData.Length;
		NativeParallelQueue<EnabledAction> val2 = default(NativeParallelQueue<EnabledAction>);
		val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<OverflowAction> overflowActions = default(NativeQueue<OverflowAction>);
		overflowActions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeReference<int> enabledDataIndex = default(NativeReference<int>);
		enabledDataIndex._002Ector(length, AllocatorHandle.op_Implicit((Allocator)3));
		float3 val3 = m_PrevCameraPosition;
		float3 val4 = m_PrevCameraDirection;
		float4 val5 = m_PrevLodParameters;
		if (m_CameraUpdateSystem.TryGetLODParameters(out var lodParameters))
		{
			val3 = float3.op_Implicit(((LODParameters)(ref lodParameters)).cameraPosition);
			IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
			val5 = RenderingUtils.CalculateLodParameters(m_BatchDataSystem.GetLevelOfDetail(m_RenderingSystem.frameLod, activeCameraController), lodParameters);
			val4 = m_CameraUpdateSystem.activeViewer.forward;
		}
		if (m_ResetPrevious)
		{
			m_PrevCameraPosition = val3;
			m_PrevCameraDirection = val4;
			m_PrevLodParameters = val5;
		}
		JobHandle dependencies;
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<EffectControlJob>(new EffectControlJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StaticType = InternalCompilerInterface.GetComponentTypeHandle<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EventType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.Event>(ref __TypeHandle.__Game_Events_Event_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EffectOwnerType = InternalCompilerInterface.GetBufferTypeHandle<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficLightsData = InternalCompilerInterface.GetComponentLookup<TrafficLights>(ref __TypeHandle.__Game_Net_TrafficLights_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLightEffectData = InternalCompilerInterface.GetComponentLookup<LightEffectData>(ref __TypeHandle.__Game_Prefabs_LightEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAudioEffectData = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodParameters = val5,
			m_CameraPosition = val3,
			m_CameraDirection = val4,
			m_CullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies),
			m_EnabledEffectData = m_EnabledData,
			m_ActionQueue = val2.AsWriter()
		}, val, JobHandle.CombineDependencies(dependencies, ((SystemBase)this).Dependency));
		if (!loaded)
		{
			JobHandle dependencies2;
			NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> searchTree = m_SearchSystem.GetSearchTree(readOnly: true, out dependencies2);
			NativeArray<int> nodeBuffer = default(NativeArray<int>);
			nodeBuffer._002Ector(256, (Allocator)3, (NativeArrayOptions)0);
			NativeArray<int> subDataBuffer = default(NativeArray<int>);
			subDataBuffer._002Ector(256, (Allocator)3, (NativeArrayOptions)0);
			TreeCullingJob1 treeCullingJob = new TreeCullingJob1
			{
				m_EffectSearchTree = searchTree,
				m_LodParameters = val5,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = val3,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = val4,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_NodeBuffer = nodeBuffer,
				m_SubDataBuffer = subDataBuffer,
				m_ActionQueue = val2.AsWriter()
			};
			TreeCullingJob2 treeCullingJob2 = new TreeCullingJob2
			{
				m_EffectSearchTree = searchTree,
				m_LodParameters = val5,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = val3,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = val4,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_NodeBuffer = nodeBuffer,
				m_SubDataBuffer = subDataBuffer,
				m_ActionQueue = val2.AsWriter()
			};
			JobHandle dependencies3;
			EffectCullingJob obj = new EffectCullingJob
			{
				m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StaticData = InternalCompilerInterface.GetComponentLookup<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EffectOwners = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies3),
				m_EnabledEffectData = m_EnabledData,
				m_ActionQueue = val2.AsWriter()
			};
			JobHandle val7 = IJobExtensions.Schedule<TreeCullingJob1>(treeCullingJob, dependencies2);
			JobHandle val8 = IJobParallelForExtensions.Schedule<TreeCullingJob2>(treeCullingJob2, nodeBuffer.Length, 1, val7);
			JobHandle val9 = IJobParallelForDeferExtensions.Schedule<EffectCullingJob, PreCullingData>(obj, obj.m_CullingData, 16, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies3));
			nodeBuffer.Dispose(val8);
			subDataBuffer.Dispose(val8);
			m_SearchSystem.AddSearchTreeReader(val8);
			val6 = JobHandle.CombineDependencies(val6, val8, val9);
		}
		EnabledActionJob enabledActionJob = new EnabledActionJob
		{
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VFXDatas = InternalCompilerInterface.GetComponentLookup<VFXData>(ref __TypeHandle.__Game_Prefabs_VFXData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomTransformDatas = InternalCompilerInterface.GetComponentLookup<RandomTransformData>(ref __TypeHandle.__Game_Prefabs_RandomTransformData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryDatas = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AudioSourceDatas = InternalCompilerInterface.GetBufferLookup<AudioSourceData>(ref __TypeHandle.__Game_Prefabs_AudioSourceData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectOwners = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingActions = val2.AsReader(),
			m_OverflowActions = overflowActions.AsParallelWriter(),
			m_VFXUpdateQueue = m_VFXSystem.GetSourceUpdateData().AsParallelWriter(),
			m_EnabledData = m_EnabledData,
			m_EnabledDataIndex = enabledDataIndex,
			m_EffectControlData = m_EffectControlData
		};
		ResizeEnabledDataJob obj2 = new ResizeEnabledDataJob
		{
			m_EnabledDataIndex = enabledDataIndex,
			m_EnabledData = m_EnabledData,
			m_OverflowActions = overflowActions
		};
		JobHandle val10 = IJobParallelForExtensions.Schedule<EnabledActionJob>(enabledActionJob, val2.HashRange, 1, val6);
		JobHandle val11 = (m_EnabledWriteDependencies = IJobExtensions.Schedule<ResizeEnabledDataJob>(obj2, val10));
		val2.Dispose(val10);
		overflowActions.Dispose(val11);
		enabledDataIndex.Dispose(val11);
		m_VFXSystem.AddSourceUpdateWriter(val10);
		m_PreCullingSystem.AddCullingDataReader(val6);
		m_PrevCameraPosition = val3;
		m_PrevCameraDirection = val4;
		m_PrevLodParameters = val5;
		m_ResetPrevious = false;
		((SystemBase)this).Dependency = val10;
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
	public EffectControlSystem()
	{
	}
}
