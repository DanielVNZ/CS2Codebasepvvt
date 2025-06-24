using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class BrandPopularitySystem : GameSystemBase, IPreDeserialize
{
	public struct BrandPopularity : IComparable<BrandPopularity>
	{
		public Entity m_BrandPrefab;

		public int m_Popularity;

		public int CompareTo(BrandPopularity other)
		{
			return other.m_Popularity - m_Popularity;
		}
	}

	[BurstCompile]
	private struct UpdateBrandPopularityJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CompanyChunks;

		[ReadOnly]
		public ComponentTypeHandle<CompanyData> m_CompanyDataType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_CompanyRentPropertyType;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> m_UnderConstructions;

		public NativeList<BrandPopularity> m_BrandPopularity;

		public void Execute()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			m_BrandPopularity.Clear();
			if (m_CompanyChunks.Length == 0)
			{
				return;
			}
			NativeParallelHashMap<Entity, int> val = default(NativeParallelHashMap<Entity, int>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			int num = default(int);
			for (int i = 0; i < m_CompanyChunks.Length; i++)
			{
				ArchetypeChunk val2 = m_CompanyChunks[i];
				NativeArray<CompanyData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<CompanyData>(ref m_CompanyDataType);
				NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PropertyRenter>(ref m_CompanyRentPropertyType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					CompanyData companyData = nativeArray[j];
					PropertyRenter propertyRenter = nativeArray2[j];
					if (companyData.m_Brand != Entity.Null && propertyRenter.m_Property != Entity.Null && !m_UnderConstructions.HasComponent(propertyRenter.m_Property))
					{
						if (val.TryGetValue(companyData.m_Brand, ref num))
						{
							val[companyData.m_Brand] = num + 1;
							continue;
						}
						val.Add(companyData.m_Brand, 1);
						ref NativeList<BrandPopularity> reference = ref m_BrandPopularity;
						BrandPopularity brandPopularity = new BrandPopularity
						{
							m_BrandPrefab = companyData.m_Brand
						};
						reference.Add(ref brandPopularity);
					}
				}
			}
			for (int k = 0; k < m_BrandPopularity.Length; k++)
			{
				BrandPopularity brandPopularity2 = m_BrandPopularity[k];
				brandPopularity2.m_Popularity = val[brandPopularity2.m_BrandPrefab];
				m_BrandPopularity[k] = brandPopularity2;
			}
			val.Dispose();
			NativeSortExtension.Sort<BrandPopularity>(m_BrandPopularity);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Companies_CompanyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Objects_UnderConstruction_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UnderConstruction>(true);
		}
	}

	private EntityQuery m_ModifiedQuery;

	private NativeList<BrandPopularity> m_BrandPopularity;

	private JobHandle m_Readers;

	public const int kUpdatesPerDay = 128;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 2048;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.ReadOnly<PropertyRenter>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_BrandPopularity = new NativeList<BrandPopularity>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_ModifiedQuery);
	}

	public void PreDeserialize(Context context)
	{
		m_BrandPopularity.Clear();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_BrandPopularity.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> companyChunks = ((EntityQuery)(ref m_ModifiedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<UpdateBrandPopularityJob>(new UpdateBrandPopularityJob
		{
			m_CompanyChunks = companyChunks,
			m_CompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyRentPropertyType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructions = InternalCompilerInterface.GetComponentLookup<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BrandPopularity = m_BrandPopularity
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, m_Readers));
		companyChunks.Dispose(val2);
		((SystemBase)this).Dependency = val2;
		m_Readers = default(JobHandle);
	}

	public NativeList<BrandPopularity> ReadBrandPopularity(out JobHandle dependency)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependency = ((SystemBase)this).Dependency;
		return m_BrandPopularity;
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
	public BrandPopularitySystem()
	{
	}
}
