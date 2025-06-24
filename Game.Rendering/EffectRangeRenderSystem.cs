using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class EffectRangeRenderSystem : GameSystemBase
{
	[BurstCompile]
	private struct EffectRangeRenderJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_InfomodeChunks;

		public OverlayRenderSystem.Buffer m_OverlayBuffer;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_BuildingEfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> m_FirewatchTowerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewLocalEffectData> m_InfoviewLocalEffectType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.FirewatchTower> m_FirewatchTowerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Efficiency> m_Efficiencies;

		[ReadOnly]
		public BufferLookup<LocalModifierData> m_LocalModifierData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			NativeList<LocalModifierData> tempModifierList = default(NativeList<LocalModifierData>);
			tempModifierList._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			uint modifierTypes = GetModifierTypes();
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Game.Buildings.FirewatchTower> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.FirewatchTower>(ref m_FirewatchTowerType);
			NativeArray<Temp> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
			Temp temp = default(Temp);
			Game.Buildings.FirewatchTower firewatchTower = default(Game.Buildings.FirewatchTower);
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			Game.Buildings.FirewatchTower firewatchTower2 = default(Game.Buildings.FirewatchTower);
			DynamicBuffer<Efficiency> buffer2 = default(DynamicBuffer<Efficiency>);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				PrefabRef prefabRef = nativeArray4[i];
				InitializeTempList(tempModifierList, prefabRef.m_Prefab);
				if (bufferAccessor.Length != 0)
				{
					AddToTempList(tempModifierList, bufferAccessor[i]);
				}
				for (int j = 0; j < tempModifierList.Length; j++)
				{
					LocalModifierData localModifier = tempModifierList[j];
					if (((uint)(1 << (int)localModifier.m_Type) & modifierTypes) == 0)
					{
						continue;
					}
					Transform transform = nativeArray[i];
					if (CollectionUtils.TryGet<Temp>(nativeArray3, i, ref temp))
					{
						if ((m_FirewatchTowerData.TryGetComponent(temp.m_Original, ref firewatchTower) && (firewatchTower.m_Flags & FirewatchTowerFlags.HasCoverage) == 0) || !m_Efficiencies.TryGetBuffer(temp.m_Original, ref buffer))
						{
							CheckModifier(localModifier, transform);
						}
						else
						{
							CheckModifier(localModifier, BuildingUtils.GetEfficiency(buffer), transform);
						}
					}
					else if ((CollectionUtils.TryGet<Game.Buildings.FirewatchTower>(nativeArray2, i, ref firewatchTower2) && (firewatchTower2.m_Flags & FirewatchTowerFlags.HasCoverage) == 0) || !CollectionUtils.TryGet<Efficiency>(bufferAccessor2, i, ref buffer2))
					{
						CheckModifier(localModifier, transform);
					}
					else
					{
						CheckModifier(localModifier, BuildingUtils.GetEfficiency(buffer2), transform);
					}
				}
			}
			tempModifierList.Dispose();
		}

		private void InitializeTempList(NativeList<LocalModifierData> tempModifierList, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LocalModifierData> localModifiers = default(DynamicBuffer<LocalModifierData>);
			if (m_LocalModifierData.TryGetBuffer(prefab, ref localModifiers))
			{
				LocalEffectSystem.InitializeTempList(tempModifierList, localModifiers);
			}
			else
			{
				tempModifierList.Clear();
			}
		}

		private void AddToTempList(NativeList<LocalModifierData> tempModifierList, DynamicBuffer<InstalledUpgrade> upgrades)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<LocalModifierData> localModifiers = default(DynamicBuffer<LocalModifierData>);
			for (int i = 0; i < upgrades.Length; i++)
			{
				InstalledUpgrade installedUpgrade = upgrades[i];
				if (m_LocalModifierData.TryGetBuffer(m_PrefabRefData[installedUpgrade.m_Upgrade].m_Prefab, ref localModifiers))
				{
					LocalEffectSystem.AddToTempList(tempModifierList, localModifiers, BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive));
				}
			}
		}

		private uint GetModifierTypes()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			uint num = 0u;
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewLocalEffectData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewLocalEffectData>(ref m_InfoviewLocalEffectType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					num |= (uint)(1 << (int)nativeArray[j].m_Type);
				}
			}
			return num;
		}

		private void CheckModifier(LocalModifierData localModifier, float efficiency, Transform transform)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewLocalEffectData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewLocalEffectData>(ref m_InfoviewLocalEffectType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfoviewLocalEffectData infoviewLocalEffectData = nativeArray[j];
					if (infoviewLocalEffectData.m_Type == localModifier.m_Type)
					{
						float3 val2 = math.forward(transform.m_Rotation);
						float num = math.lerp(localModifier.m_Radius.min, localModifier.m_Radius.max, math.sqrt(efficiency));
						Color val3 = RenderingUtils.ToColor(infoviewLocalEffectData.m_Color);
						Color fillColor = val3;
						fillColor.a = 0f;
						m_OverlayBuffer.DrawCircle(val3, fillColor, num * 0.02f, OverlayRenderSystem.StyleFlags.Projected, ((float3)(ref val2)).xz, transform.m_Position, num * 2f);
					}
				}
			}
		}

		private void CheckModifier(LocalModifierData localModifier, Transform transform)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_InfomodeChunks.Length; i++)
			{
				ArchetypeChunk val = m_InfomodeChunks[i];
				NativeArray<InfoviewLocalEffectData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<InfoviewLocalEffectData>(ref m_InfoviewLocalEffectType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfoviewLocalEffectData infoviewLocalEffectData = nativeArray[j];
					if (infoviewLocalEffectData.m_Type == localModifier.m_Type)
					{
						float3 val2 = math.forward(transform.m_Rotation);
						float max = localModifier.m_Radius.max;
						Color val3 = RenderingUtils.ToColor(infoviewLocalEffectData.m_Color);
						Color fillColor = val3;
						fillColor.a = 0f;
						m_OverlayBuffer.DrawCircle(val3, fillColor, max * 0.02f, OverlayRenderSystem.StyleFlags.Projected, ((float3)(ref val2)).xz, transform.m_Position, max * 2f);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FirewatchTower> __Game_Buildings_FirewatchTower_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfoviewLocalEffectData> __Game_Prefabs_InfoviewLocalEffectData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.FirewatchTower> __Game_Buildings_FirewatchTower_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalModifierData> __Game_Prefabs_LocalModifierData_RO_BufferLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.FirewatchTower>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_InfoviewLocalEffectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewLocalEffectData>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_FirewatchTower_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.FirewatchTower>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Prefabs_LocalModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalModifierData>(true);
		}
	}

	private EntityQuery m_ProviderQuery;

	private EntityQuery m_InfomodeQuery;

	private OverlayRenderSystem m_OverlayRenderSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_OverlayRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OverlayRenderSystem>();
		m_ProviderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<LocalEffectProvider>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>()
		});
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewLocalEffectData>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ProviderQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_InfomodeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> infomodeChunks = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle dependencies;
		JobHandle val2 = JobChunkExtensions.Schedule<EffectRangeRenderJob>(new EffectRangeRenderJob
		{
			m_InfomodeChunks = infomodeChunks,
			m_OverlayBuffer = m_OverlayRenderSystem.GetBuffer(out dependencies),
			m_BuildingEfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FirewatchTowerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.FirewatchTower>(ref __TypeHandle.__Game_Buildings_FirewatchTower_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InfoviewLocalEffectType = InternalCompilerInterface.GetComponentTypeHandle<InfoviewLocalEffectData>(ref __TypeHandle.__Game_Prefabs_InfoviewLocalEffectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FirewatchTowerData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.FirewatchTower>(ref __TypeHandle.__Game_Buildings_FirewatchTower_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Efficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalModifierData = InternalCompilerInterface.GetBufferLookup<LocalModifierData>(ref __TypeHandle.__Game_Prefabs_LocalModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		}, m_ProviderQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
		infomodeChunks.Dispose(val2);
		m_OverlayRenderSystem.AddBufferWriter(val2);
		((SystemBase)this).Dependency = val2;
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
	public EffectRangeRenderSystem()
	{
	}
}
