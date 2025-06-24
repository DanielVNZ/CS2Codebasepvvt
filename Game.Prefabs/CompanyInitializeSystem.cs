using System.Runtime.CompilerServices;
using Game.Common;
using Game.Economy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class CompanyInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeAffiliatedBrandsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentTypeHandle<CommercialCompanyData> m_CommercialCompanyDataType;

		[ReadOnly]
		public ComponentTypeHandle<StorageCompanyData> m_StorageCompanyDataType;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProcessData> m_IndustrialProcessDataType;

		public BufferTypeHandle<CompanyBrandElement> m_CompanyBrandElementType;

		public BufferTypeHandle<AffiliatedBrandElement> m_AffiliatedBrandElementType;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMap<sbyte, Entity> val = default(NativeParallelMultiHashMap<sbyte, Entity>);
			val._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			NativeParallelMultiHashMap<sbyte, Entity> val2 = default(NativeParallelMultiHashMap<sbyte, Entity>);
			val2._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val3 = m_Chunks[i];
				bool flag = ((ArchetypeChunk)(ref val3)).Has<CommercialCompanyData>(ref m_CommercialCompanyDataType);
				bool flag2 = ((ArchetypeChunk)(ref val3)).Has<StorageCompanyData>(ref m_StorageCompanyDataType);
				NativeArray<IndustrialProcessData> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<IndustrialProcessData>(ref m_IndustrialProcessDataType);
				BufferAccessor<CompanyBrandElement> bufferAccessor = ((ArchetypeChunk)(ref val3)).GetBufferAccessor<CompanyBrandElement>(ref m_CompanyBrandElementType);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<CompanyBrandElement> val4 = bufferAccessor[j];
					for (int k = 0; k < val4.Length; k++)
					{
						if (m_DeletedData.HasComponent(val4[k].m_Brand))
						{
							val4.RemoveAtSwapBack(k--);
						}
					}
				}
				for (int l = 0; l < nativeArray.Length; l++)
				{
					IndustrialProcessData industrialProcessData = nativeArray[l];
					if (!flag2 && industrialProcessData.m_Input1.m_Resource != Resource.NoResource)
					{
						int resourceIndex = EconomyUtils.GetResourceIndex(industrialProcessData.m_Input1.m_Resource);
						DynamicBuffer<CompanyBrandElement> val5 = bufferAccessor[l];
						for (int m = 0; m < val5.Length; m++)
						{
							val2.Add((sbyte)resourceIndex, val5[m].m_Brand);
						}
					}
					if (!flag2 && industrialProcessData.m_Input2.m_Resource != Resource.NoResource)
					{
						int resourceIndex2 = EconomyUtils.GetResourceIndex(industrialProcessData.m_Input2.m_Resource);
						DynamicBuffer<CompanyBrandElement> val6 = bufferAccessor[l];
						for (int n = 0; n < val6.Length; n++)
						{
							val2.Add((sbyte)resourceIndex2, val6[n].m_Brand);
						}
					}
					if (!flag && !flag2 && industrialProcessData.m_Output.m_Resource != Resource.NoResource)
					{
						int resourceIndex3 = EconomyUtils.GetResourceIndex(industrialProcessData.m_Output.m_Resource);
						DynamicBuffer<CompanyBrandElement> val7 = bufferAccessor[l];
						for (int num = 0; num < val7.Length; num++)
						{
							val.Add((sbyte)resourceIndex3, val7[num].m_Brand);
						}
					}
				}
			}
			Entity brand = default(Entity);
			NativeParallelMultiHashMapIterator<sbyte> val10 = default(NativeParallelMultiHashMapIterator<sbyte>);
			Entity brand2 = default(Entity);
			NativeParallelMultiHashMapIterator<sbyte> val11 = default(NativeParallelMultiHashMapIterator<sbyte>);
			Entity brand3 = default(Entity);
			NativeParallelMultiHashMapIterator<sbyte> val12 = default(NativeParallelMultiHashMapIterator<sbyte>);
			for (int num2 = 0; num2 < m_Chunks.Length; num2++)
			{
				ArchetypeChunk val8 = m_Chunks[num2];
				bool flag3 = ((ArchetypeChunk)(ref val8)).Has<CommercialCompanyData>(ref m_CommercialCompanyDataType);
				bool flag4 = ((ArchetypeChunk)(ref val8)).Has<StorageCompanyData>(ref m_StorageCompanyDataType);
				NativeArray<IndustrialProcessData> nativeArray2 = ((ArchetypeChunk)(ref val8)).GetNativeArray<IndustrialProcessData>(ref m_IndustrialProcessDataType);
				BufferAccessor<AffiliatedBrandElement> bufferAccessor2 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<AffiliatedBrandElement>(ref m_AffiliatedBrandElementType);
				for (int num3 = 0; num3 < bufferAccessor2.Length; num3++)
				{
					IndustrialProcessData industrialProcessData2 = nativeArray2[num3];
					DynamicBuffer<AffiliatedBrandElement> val9 = bufferAccessor2[num3];
					val9.Clear();
					if (!flag4 && industrialProcessData2.m_Input1.m_Resource != Resource.NoResource)
					{
						int resourceIndex4 = EconomyUtils.GetResourceIndex(industrialProcessData2.m_Input1.m_Resource);
						if (val.TryGetFirstValue((sbyte)resourceIndex4, ref brand, ref val10))
						{
							do
							{
								val9.Add(new AffiliatedBrandElement
								{
									m_Brand = brand
								});
							}
							while (val.TryGetNextValue(ref brand, ref val10));
						}
					}
					if (!flag4 && industrialProcessData2.m_Input2.m_Resource != Resource.NoResource)
					{
						int resourceIndex5 = EconomyUtils.GetResourceIndex(industrialProcessData2.m_Input2.m_Resource);
						if (val.TryGetFirstValue((sbyte)resourceIndex5, ref brand2, ref val11))
						{
							do
							{
								val9.Add(new AffiliatedBrandElement
								{
									m_Brand = brand2
								});
							}
							while (val.TryGetNextValue(ref brand2, ref val11));
						}
					}
					if (!flag3 && industrialProcessData2.m_Output.m_Resource != Resource.NoResource)
					{
						int resourceIndex6 = EconomyUtils.GetResourceIndex(industrialProcessData2.m_Output.m_Resource);
						if (val2.TryGetFirstValue((sbyte)resourceIndex6, ref brand3, ref val12))
						{
							do
							{
								val9.Add(new AffiliatedBrandElement
								{
									m_Brand = brand3
								});
							}
							while (val2.TryGetNextValue(ref brand3, ref val12));
						}
						if (flag4 && val.TryGetFirstValue((sbyte)resourceIndex6, ref brand3, ref val12))
						{
							do
							{
								val9.Add(new AffiliatedBrandElement
								{
									m_Brand = brand3
								});
							}
							while (val.TryGetNextValue(ref brand3, ref val12));
						}
					}
					if (val9.Length >= 3)
					{
						NativeSortExtension.Sort<AffiliatedBrandElement>(val9.AsNativeArray());
					}
					int num4 = 0;
					AffiliatedBrandElement affiliatedBrandElement = default(AffiliatedBrandElement);
					for (int num5 = 0; num5 < val9.Length; num5++)
					{
						AffiliatedBrandElement affiliatedBrandElement2 = val9[num5];
						if (affiliatedBrandElement2.m_Brand != affiliatedBrandElement.m_Brand)
						{
							if (affiliatedBrandElement.m_Brand != Entity.Null)
							{
								val9[num4++] = affiliatedBrandElement;
							}
							affiliatedBrandElement = affiliatedBrandElement2;
						}
					}
					if (affiliatedBrandElement.m_Brand != Entity.Null)
					{
						val9[num4++] = affiliatedBrandElement;
					}
					if (num4 < val9.Length)
					{
						val9.RemoveRange(num4, val9.Length - num4);
					}
					val9.TrimExcess();
				}
			}
			val.Dispose();
			val2.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<BrandData> __Game_Prefabs_BrandData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<CommercialCompanyData> __Game_Prefabs_CommercialCompanyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle;

		public BufferTypeHandle<CompanyBrandElement> __Game_Prefabs_CompanyBrandElement_RW_BufferTypeHandle;

		public BufferTypeHandle<AffiliatedBrandElement> __Game_Prefabs_AffiliatedBrandElement_RW_BufferTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_BrandData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BrandData>(false);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_CommercialCompanyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CommercialCompanyData>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StorageCompanyData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialProcessData>(true);
			__Game_Prefabs_CompanyBrandElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CompanyBrandElement>(false);
			__Game_Prefabs_AffiliatedBrandElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AffiliatedBrandElement>(false);
		}
	}

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_CompanyQuery;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<BrandData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_CompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<CompanyBrandElement>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BrandData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<BrandData>(ref __TypeHandle.__Game_Prefabs_BrandData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			if (((ArchetypeChunk)(ref val2)).Has<Deleted>(ref componentTypeHandle))
			{
				continue;
			}
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
			if (!((ArchetypeChunk)(ref val2)).Has<BrandData>(ref componentTypeHandle3))
			{
				continue;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				Entity brand = nativeArray2[j];
				BrandPrefab prefab = m_PrefabSystem.GetPrefab<BrandPrefab>(nativeArray[j]);
				for (int k = 0; k < prefab.m_Companies.Length; k++)
				{
					CompanyPrefab prefab2 = prefab.m_Companies[k];
					m_PrefabSystem.GetBuffer<CompanyBrandElement>((PrefabBase)prefab2, isReadOnly: false).Add(new CompanyBrandElement(brand));
				}
			}
		}
		val.Dispose();
		JobHandle val3 = default(JobHandle);
		InitializeAffiliatedBrandsJob initializeAffiliatedBrandsJob = new InitializeAffiliatedBrandsJob
		{
			m_Chunks = ((EntityQuery)(ref m_CompanyQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommercialCompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<CommercialCompanyData>(ref __TypeHandle.__Game_Prefabs_CommercialCompanyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StorageCompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDataType = InternalCompilerInterface.GetComponentTypeHandle<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyBrandElementType = InternalCompilerInterface.GetBufferTypeHandle<CompanyBrandElement>(ref __TypeHandle.__Game_Prefabs_CompanyBrandElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AffiliatedBrandElementType = InternalCompilerInterface.GetBufferTypeHandle<AffiliatedBrandElement>(ref __TypeHandle.__Game_Prefabs_AffiliatedBrandElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<InitializeAffiliatedBrandsJob>(initializeAffiliatedBrandsJob, val3);
		initializeAffiliatedBrandsJob.m_Chunks.Dispose(((SystemBase)this).Dependency);
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
	public CompanyInitializeSystem()
	{
	}
}
