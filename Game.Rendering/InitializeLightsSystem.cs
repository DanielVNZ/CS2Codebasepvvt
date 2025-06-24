using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class InitializeLightsSystem : GameSystemBase
{
	[BurstCompile]
	private struct ProceduralInitializeJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<ProceduralLight> m_ProceduralLights;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		public BufferLookup<Emissive> m_Emissives;

		public BufferLookup<LightState> m_Lights;

		[ReadOnly]
		public int m_CurrentTime;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		public NativeHeapAllocator m_HeapAllocator;

		public NativeReference<ProceduralEmissiveSystem.AllocationInfo> m_AllocationInfo;

		public NativeQueue<ProceduralEmissiveSystem.AllocationRemove> m_AllocationRemoves;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ref ProceduralEmissiveSystem.AllocationInfo allocationInfo = ref CollectionUtils.ValueAsRef<ProceduralEmissiveSystem.AllocationInfo>(m_AllocationInfo);
			for (int i = 0; i < m_CullingData.Length; i++)
			{
				PreCullingData cullingData = m_CullingData[i];
				if ((cullingData.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated)) != 0 && (cullingData.m_Flags & PreCullingFlags.Emissive) != 0)
				{
					if ((cullingData.m_Flags & PreCullingFlags.NearCamera) == 0)
					{
						Remove(cullingData);
					}
					else
					{
						Update(cullingData, ref allocationInfo);
					}
				}
			}
		}

		private void Remove(PreCullingData cullingData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Emissive> emissives = m_Emissives[cullingData.m_Entity];
			DynamicBuffer<LightState> val = m_Lights[cullingData.m_Entity];
			Deallocate(emissives);
			emissives.Clear();
			val.Clear();
		}

		private void Update(PreCullingData cullingData, ref ProceduralEmissiveSystem.AllocationInfo allocationInfo)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[cullingData.m_Entity];
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			if (m_SubMeshes.TryGetBuffer(prefabRef.m_Prefab, ref val))
			{
				DynamicBuffer<Emissive> emissives = m_Emissives[cullingData.m_Entity];
				DynamicBuffer<LightState> val2 = m_Lights[cullingData.m_Entity];
				int num = 0;
				for (int i = 0; i < val.Length; i++)
				{
					SubMesh subMesh = val[i];
					if (m_ProceduralLights.HasBuffer(subMesh.m_SubMesh))
					{
						num += m_ProceduralLights[subMesh.m_SubMesh].Length;
					}
				}
				if (emissives.Length == val.Length && val2.Length == num)
				{
					return;
				}
				Deallocate(emissives);
				emissives.ResizeUninitialized(val.Length);
				val2.ResizeUninitialized(num);
				num = 0;
				for (int j = 0; j < val.Length; j++)
				{
					SubMesh subMesh2 = val[j];
					if (m_ProceduralLights.HasBuffer(subMesh2.m_SubMesh))
					{
						DynamicBuffer<ProceduralLight> val3 = m_ProceduralLights[subMesh2.m_SubMesh];
						NativeHeapBlock bufferAllocation = ((NativeHeapAllocator)(ref m_HeapAllocator)).Allocate((uint)(val3.Length + 1), 1u);
						if (((NativeHeapBlock)(ref bufferAllocation)).Empty)
						{
							((NativeHeapAllocator)(ref m_HeapAllocator)).Resize(((NativeHeapAllocator)(ref m_HeapAllocator)).Size + 1048576u / (uint)System.Runtime.CompilerServices.Unsafe.SizeOf<float4>());
							bufferAllocation = ((NativeHeapAllocator)(ref m_HeapAllocator)).Allocate((uint)(val3.Length + 1), 1u);
						}
						allocationInfo.m_AllocationCount++;
						emissives[j] = new Emissive
						{
							m_BufferAllocation = bufferAllocation,
							m_LightOffset = num,
							m_Updated = true
						};
						for (int k = 0; k < val3.Length; k++)
						{
							val2[num++] = new LightState
							{
								m_Intensity = 0f,
								m_Color = 0f
							};
						}
					}
					else
					{
						emissives[j] = new Emissive
						{
							m_LightOffset = -1
						};
					}
				}
			}
			else
			{
				Remove(cullingData);
			}
		}

		private void Deallocate(DynamicBuffer<Emissive> emissives)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < emissives.Length; i++)
			{
				Emissive emissive = emissives[i];
				if (!((NativeHeapBlock)(ref emissive.m_BufferAllocation)).Empty)
				{
					m_AllocationRemoves.Enqueue(new ProceduralEmissiveSystem.AllocationRemove
					{
						m_Allocation = emissive.m_BufferAllocation,
						m_RemoveTime = m_CurrentTime
					});
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ProceduralLight> __Game_Prefabs_ProceduralLight_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		public BufferLookup<Emissive> __Game_Rendering_Emissive_RW_BufferLookup;

		public BufferLookup<LightState> __Game_Rendering_LightState_RW_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ProceduralLight_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ProceduralLight>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
			__Game_Rendering_Emissive_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(false);
			__Game_Rendering_LightState_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LightState>(false);
		}
	}

	private ProceduralEmissiveSystem m_ProceduralEmissiveSystem;

	private PreCullingSystem m_PreCullingSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ProceduralEmissiveSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralEmissiveSystem>();
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		NativeReference<ProceduralEmissiveSystem.AllocationInfo> allocationInfo;
		NativeQueue<ProceduralEmissiveSystem.AllocationRemove> allocationRemoves;
		int currentTime;
		JobHandle dependencies;
		NativeHeapAllocator heapAllocator = m_ProceduralEmissiveSystem.GetHeapAllocator(out allocationInfo, out allocationRemoves, out currentTime, out dependencies);
		JobHandle dependencies2;
		JobHandle val = IJobExtensions.Schedule<ProceduralInitializeJob>(new ProceduralInitializeJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProceduralLights = InternalCompilerInterface.GetBufferLookup<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Emissives = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lights = InternalCompilerInterface.GetBufferLookup<LightState>(ref __TypeHandle.__Game_Rendering_LightState_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTime = currentTime,
			m_CullingData = m_PreCullingSystem.GetUpdatedData(readOnly: true, out dependencies2),
			m_HeapAllocator = heapAllocator,
			m_AllocationInfo = allocationInfo,
			m_AllocationRemoves = allocationRemoves
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2, dependencies));
		m_ProceduralEmissiveSystem.AddHeapWriter(val);
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
	public InitializeLightsSystem()
	{
	}
}
