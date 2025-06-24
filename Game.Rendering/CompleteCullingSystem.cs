using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class CompleteCullingSystem : GameSystemBase
{
	[BurstCompile]
	private struct CullingCleanupJob : IJob
	{
		public ComponentLookup<CullingInfo> m_CullingInfo;

		public NativeList<PreCullingData> m_CullingData;

		public void Execute()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_CullingData.Length; i++)
			{
				ref PreCullingData reference = ref m_CullingData.ElementAt(i);
				if ((reference.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.Created | PreCullingFlags.Applied | PreCullingFlags.BatchesUpdated | PreCullingFlags.ColorsUpdated)) == 0)
				{
					continue;
				}
				if ((reference.m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					if ((reference.m_Flags & PreCullingFlags.Deleted) == 0)
					{
						m_CullingInfo.GetRefRW(reference.m_Entity).ValueRW.m_CullingIndex = 0;
					}
					m_CullingData.RemoveAtSwapBack(i);
					if (i < m_CullingData.Length)
					{
						ref PreCullingData reference2 = ref m_CullingData.ElementAt(i);
						if ((reference2.m_Flags & PreCullingFlags.Deleted) == 0)
						{
							m_CullingInfo.GetRefRW(reference2.m_Entity).ValueRW.m_CullingIndex = i;
						}
					}
					i--;
				}
				else
				{
					reference.m_Flags &= ~(PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.Created | PreCullingFlags.Applied | PreCullingFlags.BatchesUpdated | PreCullingFlags.ColorsUpdated);
				}
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Rendering_CullingInfo_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(false);
		}
	}

	private PreCullingSystem m_CullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_CullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = IJobExtensions.Schedule<CullingCleanupJob>(new CullingCleanupJob
		{
			m_CullingInfo = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingData = m_CullingSystem.GetCullingData(readOnly: false, out dependencies)
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_CullingSystem.AddCullingDataWriter(val);
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
	public CompleteCullingSystem()
	{
	}
}
