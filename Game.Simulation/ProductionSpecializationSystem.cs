using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Economy;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ProductionSpecializationSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	public struct ProducedResource
	{
		public Resource m_Resource;

		public int m_Amount;
	}

	[BurstCompile]
	private struct SpecializationJob : IJob
	{
		public BufferLookup<SpecializationBonus> m_Bonuses;

		public NativeQueue<ProducedResource> m_Queue;

		public Entity m_City;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SpecializationBonus> val = m_Bonuses[m_City];
			ProducedResource producedResource = default(ProducedResource);
			while (m_Queue.TryDequeue(ref producedResource))
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(producedResource.m_Resource);
				while (val.Length <= resourceIndex)
				{
					val.Add(new SpecializationBonus
					{
						m_Value = 0
					});
				}
				SpecializationBonus specializationBonus = val[resourceIndex];
				specializationBonus.m_Value += producedResource.m_Amount;
				val[resourceIndex] = specializationBonus;
			}
			for (int i = 0; i < val.Length; i++)
			{
				SpecializationBonus specializationBonus2 = val[i];
				specializationBonus2.m_Value = Mathf.FloorToInt(0.999f * (float)specializationBonus2.m_Value);
				val[i] = specializationBonus2;
			}
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<SpecializationBonus> __Game_City_SpecializationBonus_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_City_SpecializationBonus_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpecializationBonus>(false);
		}
	}

	public static readonly int kUpdatesPerDay = 512;

	private CitySystem m_CitySystem;

	private EntityQuery m_BonusQuery;

	private NativeQueue<ProducedResource> m_ProductionQueue;

	private JobHandle m_QueueWriters;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public NativeQueue<ProducedResource> GetQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_QueueWriters;
		return m_ProductionQueue;
	}

	public void AddQueueWriter(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_QueueWriters = JobHandle.CombineDependencies(m_QueueWriters, handle);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_BonusQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SpecializationBonus>() });
		m_ProductionQueue = new NativeQueue<ProducedResource>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_ProductionQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		SpecializationJob specializationJob = new SpecializationJob
		{
			m_Bonuses = InternalCompilerInterface.GetBufferLookup<SpecializationBonus>(ref __TypeHandle.__Game_City_SpecializationBonus_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Queue = m_ProductionQueue,
			m_City = m_CitySystem.City
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<SpecializationJob>(specializationJob, JobHandle.CombineDependencies(m_QueueWriters, ((SystemBase)this).Dependency));
		m_QueueWriters = ((SystemBase)this).Dependency;
	}

	public void SetDefaults(Context context)
	{
		m_ProductionQueue.Clear();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((JobHandle)(ref m_QueueWriters)).Complete();
		NativeArray<int> val = default(NativeArray<int>);
		val._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
		int num = -1;
		for (int i = 0; i < m_ProductionQueue.Count; i++)
		{
			ProducedResource producedResource = m_ProductionQueue.Dequeue();
			int resourceIndex = EconomyUtils.GetResourceIndex(producedResource.m_Resource);
			int num2 = resourceIndex;
			val[num2] += producedResource.m_Amount;
			num = math.max(num, resourceIndex);
		}
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num + 1);
		for (int j = 0; j <= num; j++)
		{
			int num3 = val[j];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		m_ProductionQueue.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		int num2 = default(int);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			if (num2 > 0)
			{
				m_ProductionQueue.Enqueue(new ProducedResource
				{
					m_Resource = EconomyUtils.GetResource(i),
					m_Amount = num2
				});
			}
		}
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((int)((Context)(ref context)).purpose == 1 || (int)((Context)(ref context)).purpose == 2) && ((EntityQuery)(ref m_BonusQuery)).IsEmptyIgnoreFilter)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddBuffer<SpecializationBonus>(m_CitySystem.City);
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
	public ProductionSpecializationSystem()
	{
	}
}
