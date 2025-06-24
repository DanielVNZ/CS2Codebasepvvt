using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Effects;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Audio;

[CompilerGenerated]
public class SFXCullingSystem : GameSystemBase
{
	[BurstCompile]
	private struct SFXCullingJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<CullingGroupData> m_CullingGroupData;

		[ReadOnly]
		public ComponentLookup<AudioSpotData> m_AudioSpotData;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> m_AudioEffectDatas;

		[ReadOnly]
		public BufferLookup<AudioSourceData> m_AudioSourceDatas;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public float m_DeltaTime;

		[NativeDisableParallelForRestriction]
		public NativeList<EnabledEffectData> m_EnabledData;

		public Writer<CullingGroupItem> m_CullingGroupItems;

		public SourceUpdateData m_SourceUpdateData;

		public void Execute(int index)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			ref EnabledEffectData reference = ref m_EnabledData.ElementAt(index);
			if ((reference.m_Flags & EnabledEffectFlags.IsAudio) == 0)
			{
				return;
			}
			if ((reference.m_Flags & EnabledEffectFlags.IsEnabled) == 0)
			{
				if ((reference.m_Flags & EnabledEffectFlags.WrongPrefab) != 0)
				{
					m_SourceUpdateData.WrongPrefab(new SourceInfo(reference.m_Owner, reference.m_EffectIndex));
				}
				else
				{
					m_SourceUpdateData.Remove(new SourceInfo(reference.m_Owner, reference.m_EffectIndex));
				}
				reference.m_NextTime = -1f;
				return;
			}
			CullingGroupData cullingGroupData = default(CullingGroupData);
			if (m_CullingGroupData.TryGetComponent(reference.m_Prefab, ref cullingGroupData))
			{
				m_CullingGroupItems.Enqueue(cullingGroupData.m_GroupIndex, new CullingGroupItem
				{
					m_EnabledIndex = index,
					m_DistanceSq = math.distancesq(reference.m_Position, m_CameraPosition)
				});
				return;
			}
			float3 val = reference.m_Position;
			float num = float.MaxValue;
			DynamicBuffer<AudioSourceData> val2 = default(DynamicBuffer<AudioSourceData>);
			AudioEffectData audioEffectData = default(AudioEffectData);
			if (m_AudioSourceDatas.TryGetBuffer(reference.m_Prefab, ref val2) && val2.Length > 0 && m_AudioEffectDatas.TryGetComponent(val2[0].m_SFXEntity, ref audioEffectData))
			{
				num = audioEffectData.m_MaxDistance;
				if (audioEffectData.m_SourceSize.x > 0f || audioEffectData.m_SourceSize.y > 0f || audioEffectData.m_SourceSize.z > 0f)
				{
					float3 sourceOffset = default(float3);
					if ((reference.m_Flags & EnabledEffectFlags.EditorContainer) == 0)
					{
						PrefabRef prefabRef = m_Prefabs[reference.m_Owner];
						sourceOffset = m_PrefabEffects[prefabRef.m_Prefab][reference.m_EffectIndex].m_Position;
					}
					val = AudioManager.GetClosestSourcePosition(sourceTransform: new Transform(reference.m_Position, reference.m_Rotation), targetPosition: m_CameraPosition, sourceOffset: sourceOffset, sourceSize: audioEffectData.m_SourceSize);
				}
			}
			if (math.distancesq(val, m_CameraPosition) >= num * num)
			{
				m_SourceUpdateData.Remove(new SourceInfo(reference.m_Owner, reference.m_EffectIndex));
				reference.m_NextTime = -1f;
				return;
			}
			AudioSpotData audioSpotData = default(AudioSpotData);
			if (m_AudioSpotData.TryGetComponent(reference.m_Prefab, ref audioSpotData))
			{
				if (reference.m_NextTime <= 0f)
				{
					Random random = m_RandomSeed.GetRandom(index);
					reference.m_NextTime = ((Random)(ref random)).NextFloat(audioSpotData.m_Interval.y);
					return;
				}
				reference.m_NextTime -= m_DeltaTime;
				if (!(reference.m_NextTime < 0f))
				{
					return;
				}
				Random random2 = m_RandomSeed.GetRandom(index);
				reference.m_NextTime = ((Random)(ref random2)).NextFloat(audioSpotData.m_Interval.x, audioSpotData.m_Interval.y);
			}
			m_SourceUpdateData.Add(new SourceInfo(reference.m_Owner, reference.m_EffectIndex));
		}
	}

	private struct CullingGroupItem : IComparable<CullingGroupItem>
	{
		public int m_EnabledIndex;

		public float m_DistanceSq;

		public int CompareTo(CullingGroupItem other)
		{
			return m_DistanceSq.CompareTo(other.m_DistanceSq);
		}
	}

	[BurstCompile]
	private struct SFXGroupCullingJob : IJobParallelFor
	{
		[ReadOnly]
		public int m_MaxAllowedAmount;

		[ReadOnly]
		public float m_MaxDistance;

		[ReadOnly]
		public Reader<CullingGroupItem> m_CullingGroupItems;

		[NativeDisableParallelForRestriction]
		public NativeList<EnabledEffectData> m_EnabledData;

		public SourceUpdateData m_SourceUpdateData;

		public void Execute(int groupIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CullingGroupItem> val = m_CullingGroupItems.ToArray(groupIndex, AllocatorHandle.op_Implicit((Allocator)2));
			NativeSortExtension.Sort<CullingGroupItem>(val);
			int i = 0;
			float num = m_MaxDistance * m_MaxDistance;
			for (; i < val.Length && i < m_MaxAllowedAmount; i++)
			{
				CullingGroupItem cullingGroupItem = val[i];
				if (cullingGroupItem.m_DistanceSq > num)
				{
					break;
				}
				ref EnabledEffectData reference = ref m_EnabledData.ElementAt(cullingGroupItem.m_EnabledIndex);
				if ((reference.m_Flags & (EnabledEffectFlags.EnabledUpdated | EnabledEffectFlags.AudioDisabled)) != 0)
				{
					reference.m_Flags &= ~EnabledEffectFlags.AudioDisabled;
					m_SourceUpdateData.Add(new SourceInfo(reference.m_Owner, reference.m_EffectIndex));
				}
			}
			for (; i < val.Length; i++)
			{
				CullingGroupItem cullingGroupItem2 = val[i];
				m_EnabledData.ElementAt(cullingGroupItem2.m_EnabledIndex).m_Flags |= EnabledEffectFlags.AudioDisabled;
			}
			val.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CullingGroupData> __Game_Prefabs_CullingGroupData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AudioSpotData> __Game_Prefabs_AudioSpotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AudioEffectData> __Game_Prefabs_AudioEffectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AudioSourceData> __Game_Prefabs_AudioSourceData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CullingGroupData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingGroupData>(true);
			__Game_Prefabs_AudioSpotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioSpotData>(true);
			__Game_Prefabs_AudioEffectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AudioEffectData>(true);
			__Game_Prefabs_AudioSourceData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AudioSourceData>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
		}
	}

	private AudioManager m_AudioManager;

	private EffectControlSystem m_EffectControlSystem;

	private EntityQuery m_CullingAudioSettingsQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_CullingAudioSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CullingAudioSettingsData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		if (!((Object)(object)main == (Object)null))
		{
			int num = 4;
			NativeParallelQueue<CullingGroupItem> val = default(NativeParallelQueue<CullingGroupItem>);
			val._002Ector(num, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle dependencies;
			NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: false, out dependencies);
			JobHandle deps;
			SourceUpdateData sourceUpdateData = m_AudioManager.GetSourceUpdateData(out deps);
			SFXCullingJob sFXCullingJob = new SFXCullingJob
			{
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CullingGroupData = InternalCompilerInterface.GetComponentLookup<CullingGroupData>(ref __TypeHandle.__Game_Prefabs_CullingGroupData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AudioSpotData = InternalCompilerInterface.GetComponentLookup<AudioSpotData>(ref __TypeHandle.__Game_Prefabs_AudioSpotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AudioEffectDatas = InternalCompilerInterface.GetComponentLookup<AudioEffectData>(ref __TypeHandle.__Game_Prefabs_AudioEffectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AudioSourceDatas = InternalCompilerInterface.GetBufferLookup<AudioSourceData>(ref __TypeHandle.__Game_Prefabs_AudioSourceData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CameraPosition = float3.op_Implicit(((Component)main).transform.position),
				m_RandomSeed = RandomSeed.Next(),
				m_DeltaTime = Time.deltaTime,
				m_EnabledData = enabledData,
				m_CullingGroupItems = val.AsWriter(),
				m_SourceUpdateData = sourceUpdateData
			};
			((SystemBase)this).Dependency = IJobParallelForDeferExtensions.Schedule<SFXCullingJob, EnabledEffectData>(sFXCullingJob, sFXCullingJob.m_EnabledData, 16, JobHandle.CombineDependencies(dependencies, deps, ((SystemBase)this).Dependency));
			JobHandle val2 = ((SystemBase)this).Dependency;
			if (!((EntityQuery)(ref m_CullingAudioSettingsQuery)).IsEmptyIgnoreFilter)
			{
				CullingAudioSettingsData singleton = ((EntityQuery)(ref m_CullingAudioSettingsQuery)).GetSingleton<CullingAudioSettingsData>();
				val2 = IJobParallelForExtensions.Schedule<SFXGroupCullingJob>(new SFXGroupCullingJob
				{
					m_MaxAllowedAmount = singleton.m_PublicTransCullMaxAmount,
					m_MaxDistance = singleton.m_PublicTransCullMaxDistance,
					m_CullingGroupItems = val.AsReader(),
					m_EnabledData = enabledData,
					m_SourceUpdateData = sourceUpdateData
				}, num, 1, val2);
			}
			val.Dispose(val2);
			m_EffectControlSystem.AddEnabledDataWriter(val2);
			m_AudioManager.AddSourceUpdateWriter(val2);
		}
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
	public SFXCullingSystem()
	{
	}
}
