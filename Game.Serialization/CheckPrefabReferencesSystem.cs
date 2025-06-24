using System.Runtime.CompilerServices;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class CheckPrefabReferencesSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckPrefabReferencesJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<Entity> m_PrefabArray;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PrefabData> m_PrefabData;

		public UnsafeList<bool> m_ReferencedPrefabs;

		public void Execute(int index)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (m_ReferencedPrefabs[index])
			{
				m_PrefabData.SetComponentEnabled(m_PrefabArray[index], true);
				m_ReferencedPrefabs[index] = false;
			}
		}
	}

	private NativeArray<Entity> m_PrefabArray;

	private UnsafeList<bool> m_ReferencedPrefabs;

	private JobHandle m_DataDeps;

	private JobHandle m_UserDeps;

	private bool m_IsLoading;

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).Dependency = (m_UserDeps = (m_DataDeps = IJobParallelForExtensions.Schedule<CheckPrefabReferencesJob>(new CheckPrefabReferencesJob
		{
			m_PrefabArray = m_PrefabArray,
			m_PrefabData = ((SystemBase)this).GetComponentLookup<PrefabData>(false),
			m_ReferencedPrefabs = m_ReferencedPrefabs
		}, m_PrefabArray.Length, 64, JobHandle.CombineDependencies(m_DataDeps, m_UserDeps, ((SystemBase)this).Dependency))));
	}

	public void BeginPrefabCheck(NativeArray<Entity> array, bool isLoading, JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabArray = array;
		m_ReferencedPrefabs = new UnsafeList<bool>(0, AllocatorHandle.op_Implicit((Allocator)3), (NativeArrayOptions)0);
		m_ReferencedPrefabs.Resize(array.Length, (NativeArrayOptions)1);
		m_DataDeps = dependencies;
		m_IsLoading = isLoading;
	}

	public void EndPrefabCheck(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		dependencies = JobHandle.CombineDependencies(m_DataDeps, m_UserDeps);
		m_ReferencedPrefabs.Dispose(dependencies);
		m_PrefabArray = default(NativeArray<Entity>);
		m_ReferencedPrefabs = default(UnsafeList<bool>);
		m_DataDeps = default(JobHandle);
		m_UserDeps = default(JobHandle);
	}

	public PrefabReferences GetPrefabReferences(SystemBase system, out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_DataDeps;
		return new PrefabReferences(m_PrefabArray, m_ReferencedPrefabs, system.GetComponentLookup<PrefabData>(true), m_IsLoading);
	}

	public void AddPrefabReferencesUser(JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_UserDeps = JobHandle.CombineDependencies(m_UserDeps, dependencies);
	}

	[Preserve]
	public CheckPrefabReferencesSystem()
	{
	}
}
