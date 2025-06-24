using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Buildings;

[CompilerGenerated]
public class LocalEffectSystem : GameSystemBase
{
	public struct EffectItem : IEquatable<EffectItem>
	{
		public Entity m_Provider;

		public LocalModifierType m_Type;

		public EffectItem(Entity provider, LocalModifierType type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Provider = provider;
			m_Type = type;
		}

		public bool Equals(EffectItem other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Entity)(ref m_Provider)).Equals(other.m_Provider))
			{
				return m_Type == other.m_Type;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Provider)/*cast due to .constrained prefix*/).GetHashCode() ^ (int)m_Type;
		}
	}

	public struct EffectBounds : IEquatable<EffectBounds>, IBounds2<EffectBounds>
	{
		public Bounds2 m_Bounds;

		public uint m_TypeMask;

		public float2 m_Delta;

		public EffectBounds(Bounds2 bounds, uint typeMask, float2 delta)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = bounds;
			m_TypeMask = typeMask;
			m_Delta = delta;
		}

		public bool Equals(EffectBounds other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (((Bounds2)(ref m_Bounds)).Equals(other.m_Bounds) && m_TypeMask == other.m_TypeMask)
			{
				return ((float2)(ref m_Delta)).Equals(other.m_Delta);
			}
			return false;
		}

		public void Reset()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			((Bounds2)(ref m_Bounds)).Reset();
			m_TypeMask = 0u;
			m_Delta = default(float2);
		}

		public float2 Center()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Bounds2)(ref m_Bounds)).Center();
		}

		public float2 Size()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Bounds2)(ref m_Bounds)).Size();
		}

		public EffectBounds Merge(EffectBounds other)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return new EffectBounds
			{
				m_Bounds = ((Bounds2)(ref m_Bounds)).Merge(other.m_Bounds),
				m_TypeMask = (m_TypeMask | other.m_TypeMask)
			};
		}

		public bool Intersect(EffectBounds other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (((Bounds2)(ref m_Bounds)).Intersect(other.m_Bounds))
			{
				return (m_TypeMask & other.m_TypeMask) != 0;
			}
			return false;
		}
	}

	public struct ReadData
	{
		private struct Iterator : INativeQuadTreeIterator<EffectItem, EffectBounds>, IUnsafeQuadTreeIterator<EffectItem, EffectBounds>
		{
			public float2 m_Position;

			public float2 m_Delta;

			public uint m_TypeMask;

			public bool Intersect(EffectBounds bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Position))
				{
					return (bounds.m_TypeMask & m_TypeMask) != 0;
				}
				return false;
			}

			public void Iterate(EffectBounds bounds, EffectItem entity2)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_Position) && (bounds.m_TypeMask & m_TypeMask) != 0)
				{
					float2 val = MathUtils.Center(bounds.m_Bounds);
					float num = (bounds.m_Bounds.max.x - bounds.m_Bounds.min.x) * 0.5f;
					float num2 = 1f - math.distancesq(val, m_Position) / (num * num);
					if (num2 > 0f)
					{
						float2 val2 = bounds.m_Delta * num2;
						m_Delta.y *= 1f + val2.y;
						m_Delta += val2;
					}
				}
			}
		}

		private NativeQuadTree<EffectItem, EffectBounds> m_SearchTree;

		public ReadData(NativeQuadTree<EffectItem, EffectBounds> searchTree)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_SearchTree = searchTree;
		}

		public void ApplyModifier(ref float value, float3 position, LocalModifierType type)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			Iterator iterator = new Iterator
			{
				m_Position = ((float3)(ref position)).xz,
				m_TypeMask = (uint)(1 << (int)type)
			};
			m_SearchTree.Iterate<Iterator>(ref iterator, 0);
			value += iterator.m_Delta.x;
			value += value * iterator.m_Delta.y;
		}
	}

	[BurstCompile]
	private struct UpdateLocalEffectsJob : IJob
	{
		[ReadOnly]
		public bool m_Loaded;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public NativeQuadTree<EffectItem, EffectBounds> m_SearchTree;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentTypeHandle<Signature> m_SignatureType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<LocalModifierData> m_LocalModifierData;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			NativeList<LocalModifierData> tempModifierList = default(NativeList<LocalModifierData>);
			tempModifierList._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType) || ((ArchetypeChunk)(ref val)).Has<Destroyed>(ref m_DestroyedType))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity provider = nativeArray[j];
						PrefabRef prefabRef = nativeArray2[j];
						InitializeTempList(tempModifierList, prefabRef.m_Prefab);
						if (bufferAccessor.Length != 0)
						{
							AddToTempList(tempModifierList, bufferAccessor[j]);
						}
						for (int k = 0; k < tempModifierList.Length; k++)
						{
							LocalModifierData localModifierData = tempModifierList[k];
							m_SearchTree.TryRemove(new EffectItem(provider, localModifierData.m_Type));
						}
					}
					continue;
				}
				if (m_Loaded || ((ArchetypeChunk)(ref val)).Has<Created>(ref m_CreatedType))
				{
					bool num = ((ArchetypeChunk)(ref val)).Has<Signature>(ref m_SignatureType);
					NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
					BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
					NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
					NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
					if (!num && bufferAccessor2.Length != 0)
					{
						for (int l = 0; l < nativeArray3.Length; l++)
						{
							Entity provider2 = nativeArray3[l];
							Transform transform = nativeArray4[l];
							PrefabRef prefabRef2 = nativeArray5[l];
							float efficiency = BuildingUtils.GetEfficiency(bufferAccessor2[l]);
							InitializeTempList(tempModifierList, prefabRef2.m_Prefab);
							if (bufferAccessor3.Length != 0)
							{
								AddToTempList(tempModifierList, bufferAccessor3[l]);
							}
							for (int m = 0; m < tempModifierList.Length; m++)
							{
								LocalModifierData localModifier = tempModifierList[m];
								if (GetEffectBounds(transform, efficiency, localModifier, out var effectBounds))
								{
									m_SearchTree.Add(new EffectItem(provider2, localModifier.m_Type), effectBounds);
								}
							}
						}
						continue;
					}
					for (int n = 0; n < nativeArray3.Length; n++)
					{
						Entity provider3 = nativeArray3[n];
						Transform transform2 = nativeArray4[n];
						PrefabRef prefabRef3 = nativeArray5[n];
						InitializeTempList(tempModifierList, prefabRef3.m_Prefab);
						if (bufferAccessor3.Length != 0)
						{
							AddToTempList(tempModifierList, bufferAccessor3[n]);
						}
						for (int num2 = 0; num2 < tempModifierList.Length; num2++)
						{
							LocalModifierData localModifier2 = tempModifierList[num2];
							if (GetEffectBounds(transform2, localModifier2, out var effectBounds2))
							{
								m_SearchTree.Add(new EffectItem(provider3, localModifier2.m_Type), effectBounds2);
							}
						}
					}
					continue;
				}
				bool num3 = ((ArchetypeChunk)(ref val)).Has<Signature>(ref m_SignatureType);
				NativeArray<Entity> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				BufferAccessor<Efficiency> bufferAccessor4 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
				NativeArray<Transform> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<InstalledUpgrade> bufferAccessor5 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
				if (!num3 && bufferAccessor4.Length != 0)
				{
					for (int num4 = 0; num4 < nativeArray6.Length; num4++)
					{
						Entity provider4 = nativeArray6[num4];
						Transform transform3 = nativeArray7[num4];
						PrefabRef prefabRef4 = nativeArray8[num4];
						float efficiency2 = BuildingUtils.GetEfficiency(bufferAccessor4[num4]);
						InitializeTempList(tempModifierList, prefabRef4.m_Prefab);
						if (bufferAccessor5.Length != 0)
						{
							AddToTempList(tempModifierList, bufferAccessor5[num4]);
						}
						for (int num5 = 0; num5 < tempModifierList.Length; num5++)
						{
							LocalModifierData localModifier3 = tempModifierList[num5];
							if (GetEffectBounds(transform3, efficiency2, localModifier3, out var effectBounds3))
							{
								m_SearchTree.AddOrUpdate(new EffectItem(provider4, localModifier3.m_Type), effectBounds3);
							}
							else
							{
								m_SearchTree.TryRemove(new EffectItem(provider4, localModifier3.m_Type));
							}
						}
					}
					continue;
				}
				for (int num6 = 0; num6 < nativeArray6.Length; num6++)
				{
					Entity provider5 = nativeArray6[num6];
					Transform transform4 = nativeArray7[num6];
					PrefabRef prefabRef5 = nativeArray8[num6];
					InitializeTempList(tempModifierList, prefabRef5.m_Prefab);
					if (bufferAccessor5.Length != 0)
					{
						AddToTempList(tempModifierList, bufferAccessor5[num6]);
					}
					for (int num7 = 0; num7 < tempModifierList.Length; num7++)
					{
						LocalModifierData localModifier4 = tempModifierList[num7];
						if (GetEffectBounds(transform4, localModifier4, out var effectBounds4))
						{
							m_SearchTree.AddOrUpdate(new EffectItem(provider5, localModifier4.m_Type), effectBounds4);
						}
						else
						{
							m_SearchTree.TryRemove(new EffectItem(provider5, localModifier4.m_Type));
						}
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
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Signature> __Game_Buildings_Signature_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LocalModifierData> __Game_Prefabs_LocalModifierData_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Signature_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Signature>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_LocalModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalModifierData>(true);
		}
	}

	private EntityQuery m_UpdatedProvidersQuery;

	private EntityQuery m_AllProvidersQuery;

	private NativeQuadTree<EffectItem, EffectBounds> m_SearchTree;

	private JobHandle m_ReadDependencies;

	private JobHandle m_WriteDependencies;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LocalEffectProvider>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedProvidersQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllProvidersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LocalEffectProvider>(),
			ComponentType.Exclude<Temp>()
		});
		m_SearchTree = new NativeQuadTree<EffectItem, EffectBounds>(1f, (Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_SearchTree.Dispose();
		base.OnDestroy();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllProvidersQuery : m_UpdatedProvidersQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle val2 = default(JobHandle);
			NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			JobHandle dependencies;
			JobHandle val3 = IJobExtensions.Schedule<UpdateLocalEffectsJob>(new UpdateLocalEffectsJob
			{
				m_Loaded = loaded,
				m_Chunks = chunks,
				m_SearchTree = GetSearchTree(readOnly: false, out dependencies),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SignatureType = InternalCompilerInterface.GetComponentTypeHandle<Signature>(ref __TypeHandle.__Game_Buildings_Signature_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LocalModifierData = InternalCompilerInterface.GetBufferLookup<LocalModifierData>(ref __TypeHandle.__Game_Prefabs_LocalModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2, dependencies));
			chunks.Dispose(val3);
			AddLocalEffectWriter(val3);
			((SystemBase)this).Dependency = val3;
		}
	}

	public ReadData GetReadData(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_WriteDependencies;
		return new ReadData(m_SearchTree);
	}

	public NativeQuadTree<EffectItem, EffectBounds> GetSearchTree(bool readOnly, out JobHandle dependencies)
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

	public void AddLocalEffectReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, jobHandle);
	}

	public void AddLocalEffectWriter(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = JobHandle.CombineDependencies(m_WriteDependencies, jobHandle);
	}

	public static void InitializeTempList(NativeList<LocalModifierData> tempModifierList, DynamicBuffer<LocalModifierData> localModifiers)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		tempModifierList.Clear();
		tempModifierList.AddRange(localModifiers.AsNativeArray());
	}

	public static void AddToTempList(NativeList<LocalModifierData> tempModifierList, DynamicBuffer<LocalModifierData> localModifiers, bool disabled)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < localModifiers.Length; i++)
		{
			LocalModifierData localModifierData = localModifiers[i];
			if (disabled)
			{
				localModifierData.m_Delta = default(Bounds1);
				localModifierData.m_Radius = default(Bounds1);
			}
			int num = 0;
			while (true)
			{
				if (num < tempModifierList.Length)
				{
					LocalModifierData localModifierData2 = tempModifierList[num];
					if (localModifierData2.m_Type == localModifierData.m_Type)
					{
						if (localModifierData2.m_Mode != localModifierData.m_Mode)
						{
							throw new Exception($"Modifier mode mismatch (type: {localModifierData.m_Type})");
						}
						localModifierData2.m_Delta.min += localModifierData.m_Delta.min;
						localModifierData2.m_Delta.max += localModifierData.m_Delta.max;
						switch (localModifierData2.m_RadiusCombineMode)
						{
						case ModifierRadiusCombineMode.Additive:
							localModifierData2.m_Radius.min += localModifierData.m_Radius.min;
							localModifierData2.m_Radius.max += localModifierData.m_Radius.max;
							break;
						case ModifierRadiusCombineMode.Maximal:
							localModifierData2.m_Radius.min = math.max(localModifierData2.m_Radius.min, localModifierData.m_Radius.min);
							localModifierData2.m_Radius.max = math.max(localModifierData2.m_Radius.max, localModifierData.m_Radius.max);
							break;
						}
						tempModifierList[num] = localModifierData2;
						break;
					}
					num++;
					continue;
				}
				tempModifierList.Add(ref localModifierData);
				break;
			}
		}
	}

	public static bool GetEffectBounds(Transform transform, LocalModifierData localModifier, out EffectBounds effectBounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		float max = localModifier.m_Radius.max;
		float max2 = localModifier.m_Delta.max;
		Bounds2 bounds = default(Bounds2);
		((Bounds2)(ref bounds))._002Ector(((float3)(ref transform.m_Position)).xz - max, ((float3)(ref transform.m_Position)).xz + max);
		uint typeMask = (uint)(1 << (int)localModifier.m_Type);
		max2 = math.select(max2, 1f / math.max(0.001f, 1f + max2) - 1f, localModifier.m_Mode == ModifierValueMode.InverseRelative);
		float2 delta = math.select(new float2(0f, max2), new float2(max2, 0f), localModifier.m_Mode == ModifierValueMode.Absolute);
		effectBounds = new EffectBounds(bounds, typeMask, delta);
		if (max >= 1f)
		{
			return max2 != 0f;
		}
		return false;
	}

	public static bool GetEffectBounds(Transform transform, float efficiency, LocalModifierData localModifier, out EffectBounds effectBounds)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		efficiency = math.sqrt(efficiency);
		float num = math.lerp(localModifier.m_Radius.min, localModifier.m_Radius.max, math.sqrt(efficiency));
		float num2 = math.lerp(localModifier.m_Delta.min, localModifier.m_Delta.max, efficiency);
		Bounds2 bounds = default(Bounds2);
		((Bounds2)(ref bounds))._002Ector(((float3)(ref transform.m_Position)).xz - num, ((float3)(ref transform.m_Position)).xz + num);
		uint typeMask = (uint)(1 << (int)localModifier.m_Type);
		num2 = math.select(num2, 1f / math.max(0.001f, 1f + num2) - 1f, localModifier.m_Mode == ModifierValueMode.InverseRelative);
		float2 delta = math.select(new float2(0f, num2), new float2(num2, 0f), localModifier.m_Mode == ModifierValueMode.Absolute);
		effectBounds = new EffectBounds(bounds, typeMask, delta);
		if (num >= 1f)
		{
			return num2 != 0f;
		}
		return false;
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
	public LocalEffectSystem()
	{
	}
}
