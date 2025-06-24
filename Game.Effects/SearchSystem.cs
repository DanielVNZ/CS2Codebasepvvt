using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
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
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Effects;

[CompilerGenerated]
public class SearchSystem : GameSystemBase, IPreDeserialize
{
	private struct AddedSource
	{
		public Entity m_Prefab;

		public int m_EffectIndex;
	}

	[BurstCompile]
	private struct UpdateSearchTreeJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> m_EditorContainerType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[ReadOnly]
		public ComponentLookup<EffectData> m_PrefabEffectData;

		[ReadOnly]
		public ComponentLookup<LightEffectData> m_PrefabLightEffectData;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> m_PrefabAudioEffectData;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public bool m_Loaded;

		public NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> m_SearchTree;

		public NativeParallelMultiHashMap<Entity, AddedSource> m_AddedSources;

		public EffectControlData m_EffectControlData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Tools.EditorContainer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Tools.EditorContainer>(ref m_EditorContainerType);
			NativeArray<PrefabRef> val = default(NativeArray<PrefabRef>);
			if (nativeArray2.Length == 0)
			{
				val = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType) || ((ArchetypeChunk)(ref chunk)).Has<InterpolatedTransform>(ref m_InterpolatedTransformType))
			{
				AddedSource addedSource = default(AddedSource);
				NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity val2 = nativeArray[i];
					if (m_AddedSources.TryGetFirstValue(val2, ref addedSource, ref val3))
					{
						do
						{
							m_SearchTree.TryRemove(new SourceInfo(val2, addedSource.m_EffectIndex));
						}
						while (m_AddedSources.TryGetNextValue(ref addedSource, ref val3));
						m_AddedSources.Remove(val2);
					}
				}
				return;
			}
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			bool flag = m_Loaded || ((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType);
			Game.Tools.EditorContainer editorContainer = default(Game.Tools.EditorContainer);
			AddedSource addedSource2 = default(AddedSource);
			NativeParallelMultiHashMapIterator<Entity> val5 = default(NativeParallelMultiHashMapIterator<Entity>);
			EffectData effectData = default(EffectData);
			DynamicBuffer<Effect> val6 = default(DynamicBuffer<Effect>);
			AddedSource addedSource3 = default(AddedSource);
			NativeParallelMultiHashMapIterator<Entity> val7 = default(NativeParallelMultiHashMapIterator<Entity>);
			EffectData effectData2 = default(EffectData);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity val4 = nativeArray[j];
				if (CollectionUtils.TryGet<Game.Tools.EditorContainer>(nativeArray2, j, ref editorContainer))
				{
					if (m_AddedSources.TryGetFirstValue(val4, ref addedSource2, ref val5))
					{
						do
						{
							if (editorContainer.m_Prefab != addedSource2.m_Prefab)
							{
								m_SearchTree.TryRemove(new SourceInfo(val4, addedSource2.m_EffectIndex));
							}
						}
						while (m_AddedSources.TryGetNextValue(ref addedSource2, ref val5));
						m_AddedSources.Remove(val4);
					}
					if (m_PrefabEffectData.TryGetComponent(editorContainer.m_Prefab, ref effectData) && !effectData.m_OwnerCulling)
					{
						if (m_EffectControlData.ShouldBeEnabled(val4, editorContainer.m_Prefab, checkEnabled: true, isEditorContainer: true))
						{
							Effect effect = new Effect
							{
								m_Effect = editorContainer.m_Prefab
							};
							QuadTreeBoundsXZ bounds = GetBounds(nativeArray3, nativeArray4, j, effect);
							m_SearchTree.AddOrUpdate(new SourceInfo(val4, 0), bounds);
							m_AddedSources.Add(val4, new AddedSource
							{
								m_Prefab = editorContainer.m_Prefab,
								m_EffectIndex = 0
							});
						}
						else if (!flag)
						{
							m_SearchTree.TryRemove(new SourceInfo(val4, 0));
						}
					}
					continue;
				}
				PrefabRef prefabRef = val[j];
				m_PrefabEffects.TryGetBuffer(prefabRef.m_Prefab, ref val6);
				if (m_AddedSources.TryGetFirstValue(val4, ref addedSource3, ref val7))
				{
					do
					{
						if (!val6.IsCreated || val6.Length <= addedSource3.m_EffectIndex || val6[addedSource3.m_EffectIndex].m_Effect != addedSource3.m_Prefab)
						{
							m_SearchTree.TryRemove(new SourceInfo(val4, addedSource3.m_EffectIndex));
						}
					}
					while (m_AddedSources.TryGetNextValue(ref addedSource3, ref val7));
					m_AddedSources.Remove(val4);
				}
				if (!val6.IsCreated)
				{
					continue;
				}
				for (int k = 0; k < val6.Length; k++)
				{
					Effect effect2 = val6[k];
					if (m_PrefabEffectData.TryGetComponent(effect2.m_Effect, ref effectData2) && !effectData2.m_OwnerCulling)
					{
						if (m_EffectControlData.ShouldBeEnabled(val4, effect2.m_Effect, checkEnabled: true, isEditorContainer: false))
						{
							QuadTreeBoundsXZ bounds2 = GetBounds(nativeArray3, nativeArray4, j, effect2);
							m_SearchTree.AddOrUpdate(new SourceInfo(val4, k), bounds2);
							m_AddedSources.Add(val4, new AddedSource
							{
								m_Prefab = effect2.m_Effect,
								m_EffectIndex = k
							});
						}
						else if (!flag)
						{
							m_SearchTree.TryRemove(new SourceInfo(val4, k));
						}
					}
				}
			}
		}

		private QuadTreeBoundsXZ GetBounds(NativeArray<Transform> transforms, NativeArray<Curve> curves, int index, Effect effect)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return SearchSystem.GetBounds(transforms, curves, index, effect, ref m_PrefabLightEffectData, ref m_PrefabAudioEffectData);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Tools.EditorContainer> __Game_Tools_EditorContainer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<EffectData> __Game_Prefabs_EffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LightEffectData> __Game_Prefabs_LightEffectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> __Game_Prefabs_AudioEffectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Tools_EditorContainer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Tools.EditorContainer>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(true);
			__Game_Prefabs_EffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EffectData>(true);
			__Game_Prefabs_LightEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LightEffectData>(true);
			__Game_Prefabs_AudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioEffectData>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
		}
	}

	private EffectFlagSystem m_EffectFlagSystem;

	private SimulationSystem m_SimulationSystem;

	private ToolSystem m_ToolSystem;

	private EffectControlData m_EffectControlData;

	private NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> m_SearchTree;

	private NativeParallelMultiHashMap<Entity, AddedSource> m_AddedSources;

	private EntityQuery m_UpdatedEffectsQuery;

	private EntityQuery m_AllEffectsQuery;

	private JobHandle m_ReadDependencies;

	private JobHandle m_WriteDependencies;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EffectFlagSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectFlagSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_EffectControlData = new EffectControlData((SystemBase)(object)this);
		m_SearchTree = new NativeQuadTree<SourceInfo, QuadTreeBoundsXZ>(1f, (Allocator)4);
		m_AddedSources = new NativeParallelMultiHashMap<Entity, AddedSource>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EnabledEffect>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<EffectsUpdated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Game.Events.Event>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<EnabledEffect>(),
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Static>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<EffectsUpdated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val;
		m_UpdatedEffectsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EnabledEffect>() };
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Game.Events.Event>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<EnabledEffect>(),
			ComponentType.ReadOnly<Object>(),
			ComponentType.ReadOnly<Static>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[1] = val;
		m_AllEffectsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_SearchTree.Dispose();
		m_AddedSources.Dispose();
		base.OnDestroy();
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllEffectsQuery : m_UpdatedEffectsQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			m_EffectControlData.Update((SystemBase)(object)this, m_EffectFlagSystem.GetData(), m_SimulationSystem.frameIndex, m_ToolSystem.selected);
			JobHandle dependencies;
			JobHandle val2 = JobChunkExtensions.Schedule<UpdateSearchTreeJob>(new UpdateSearchTreeJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Tools.EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffectData = InternalCompilerInterface.GetComponentLookup<EffectData>(ref __TypeHandle.__Game_Prefabs_EffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabLightEffectData = InternalCompilerInterface.GetComponentLookup<LightEffectData>(ref __TypeHandle.__Game_Prefabs_LightEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAudioEffectData = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Loaded = loaded,
				m_SearchTree = GetSearchTree(readOnly: false, out dependencies),
				m_AddedSources = m_AddedSources,
				m_EffectControlData = m_EffectControlData
			}, val, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
			AddSearchTreeWriter(val2);
			((SystemBase)this).Dependency = val2;
		}
	}

	public NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> GetSearchTree(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_WriteDependencies : JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies));
		return m_SearchTree;
	}

	public void AddSearchTreeReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, jobHandle);
	}

	public void AddSearchTreeWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = jobHandle;
	}

	public void PreDeserialize(Context context)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeQuadTree<SourceInfo, QuadTreeBoundsXZ> searchTree = GetSearchTree(readOnly: false, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		searchTree.Clear();
		m_AddedSources.Clear();
		m_Loaded = true;
	}

	public static QuadTreeBoundsXZ GetBounds(NativeArray<Transform> transforms, NativeArray<Curve> curves, int index, Effect effect, ref ComponentLookup<LightEffectData> prefabLightEffectData, ref ComponentLookup<AudioEffectData> prefabAudioEffectData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		float3 val = effect.m_Position;
		quaternion rotation = effect.m_Rotation;
		Curve curve = default(Curve);
		Transform transform = default(Transform);
		if (CollectionUtils.TryGet<Curve>(curves, index, ref curve))
		{
			val = MathUtils.Position(curve.m_Bezier, 0.5f);
		}
		else if (CollectionUtils.TryGet<Transform>(transforms, index, ref transform))
		{
			Transform transform2 = ObjectUtils.LocalToWorld(transform, val, rotation);
			val = transform2.m_Position;
			rotation = transform2.m_Rotation;
		}
		Bounds3 val2 = default(Bounds3);
		((Bounds3)(ref val2))._002Ector(val - 1f, val + 1f);
		int num = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(1f)));
		LightEffectData lightEffectData = default(LightEffectData);
		if (prefabLightEffectData.TryGetComponent(effect.m_Effect, ref lightEffectData))
		{
			val2 |= new Bounds3(val - lightEffectData.m_Range, val + lightEffectData.m_Range);
			num = math.min(num, lightEffectData.m_MinLod);
		}
		AudioEffectData audioEffectData = default(AudioEffectData);
		if (prefabAudioEffectData.TryGetComponent(effect.m_Effect, ref audioEffectData) && math.any(audioEffectData.m_SourceSize > 0f))
		{
			Bounds3 bounds = default(Bounds3);
			((Bounds3)(ref bounds))._002Ector(-audioEffectData.m_SourceSize, audioEffectData.m_SourceSize);
			val2 |= ObjectUtils.CalculateBounds(val, rotation, bounds);
			num = math.min(num, RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(audioEffectData.m_SourceSize)));
		}
		return new QuadTreeBoundsXZ(val2, (BoundsMask)0, num);
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
	public SearchSystem()
	{
	}
}
