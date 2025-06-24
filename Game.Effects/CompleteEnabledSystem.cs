using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Effects;

[CompilerGenerated]
public class CompleteEnabledSystem : GameSystemBase
{
	[BurstCompile]
	private struct EffectCleanupJob : IJob
	{
		public BufferLookup<EnabledEffect> m_EffectOwners;

		public NativeList<EnabledEffectData> m_EnabledData;

		public NativeQueue<VFXUpdateInfo> m_VFXUpdateQueue;

		public void Execute()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_EnabledData.Length; i++)
			{
				ref EnabledEffectData reference = ref m_EnabledData.ElementAt(i);
				if ((reference.m_Flags & (EnabledEffectFlags.EnabledUpdated | EnabledEffectFlags.OwnerUpdated)) == 0)
				{
					continue;
				}
				if ((reference.m_Flags & EnabledEffectFlags.IsEnabled) == 0)
				{
					if ((reference.m_Flags & EnabledEffectFlags.Deleted) == 0)
					{
						DynamicBuffer<EnabledEffect> val = m_EffectOwners[reference.m_Owner];
						for (int j = 0; j < val.Length; j++)
						{
							if (val[j].m_EffectIndex == reference.m_EffectIndex)
							{
								val.RemoveAt(j);
								break;
							}
						}
					}
					m_EnabledData.RemoveAtSwapBack(i);
					if (i < m_EnabledData.Length)
					{
						ref EnabledEffectData reference2 = ref m_EnabledData.ElementAt(i);
						if ((reference2.m_Flags & EnabledEffectFlags.Deleted) == 0)
						{
							DynamicBuffer<EnabledEffect> val2 = m_EffectOwners[reference2.m_Owner];
							for (int k = 0; k < val2.Length; k++)
							{
								ref EnabledEffect reference3 = ref val2.ElementAt(k);
								if (reference3.m_EffectIndex == reference2.m_EffectIndex)
								{
									reference3.m_EnabledIndex = i;
									break;
								}
							}
						}
						if ((reference2.m_Flags & (EnabledEffectFlags.IsEnabled | EnabledEffectFlags.IsVFX)) == (EnabledEffectFlags.IsEnabled | EnabledEffectFlags.IsVFX))
						{
							m_VFXUpdateQueue.Enqueue(new VFXUpdateInfo
							{
								m_Type = VFXUpdateType.MoveIndex,
								m_EnabledIndex = new int2(i, m_EnabledData.Length)
							});
						}
					}
					i--;
				}
				else
				{
					reference.m_Flags &= ~(EnabledEffectFlags.EnabledUpdated | EnabledEffectFlags.OwnerUpdated);
				}
			}
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Effects_EnabledEffect_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(false);
		}
	}

	private EffectControlSystem m_EffectControlSystem;

	private VFXSystem m_VFXSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_VFXSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VFXSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = IJobExtensions.Schedule<EffectCleanupJob>(new EffectCleanupJob
		{
			m_EffectOwners = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EnabledData = m_EffectControlSystem.GetEnabledData(readOnly: false, out dependencies),
			m_VFXUpdateQueue = m_VFXSystem.GetSourceUpdateData()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_EffectControlSystem.AddEnabledDataWriter(val);
		m_VFXSystem.AddSourceUpdateWriter(val);
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
	public CompleteEnabledSystem()
	{
	}
}
