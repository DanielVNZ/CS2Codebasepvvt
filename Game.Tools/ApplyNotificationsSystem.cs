using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyNotificationsSystem : GameSystemBase
{
	[BurstCompile]
	private struct ApplyTempIconsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Icon> m_IconType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<IconElement> m_IconElementType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Icon> m_IconData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ToolErrorData> m_ToolErrorData;

		[ReadOnly]
		public BufferLookup<IconElement> m_IconElements;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelMultiHashMap<Entity, Entity> val2 = default(NativeParallelMultiHashMap<Entity, Entity>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashMap<Entity, Entity> val3 = default(NativeHashMap<Entity, Entity>);
			val3._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val4 = m_Chunks[j];
				if (((ArchetypeChunk)(ref val4)).Has<Icon>(ref m_IconType))
				{
					NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray(m_EntityType);
					NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Owner>(ref m_OwnerType);
					NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						Entity val5 = nativeArray[k];
						Owner owner = nativeArray2[k];
						PrefabRef prefabRef = nativeArray3[k];
						bool flag = false;
						if (m_ToolErrorData.HasComponent(prefabRef.m_Prefab))
						{
							flag = (m_ToolErrorData[prefabRef.m_Prefab].m_Flags & ToolErrorFlags.TemporaryOnly) != 0;
						}
						if (m_TempData.HasComponent(owner.m_Owner))
						{
							Temp temp = m_TempData[owner.m_Owner];
							if (temp.m_Original != Entity.Null)
							{
								if (flag)
								{
									((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val5, default(Deleted));
								}
								else
								{
									val2.Add(temp.m_Original, val5);
								}
								val3.TryAdd(temp.m_Original, Entity.Null);
							}
							else if (flag)
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val5, default(Deleted));
							}
							else
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Temp>(val5);
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(val5, ref m_AppliedTypes);
							}
						}
						else
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val5, default(Deleted));
						}
					}
					continue;
				}
				NativeArray<Entity> nativeArray4 = ((ArchetypeChunk)(ref val4)).GetNativeArray(m_EntityType);
				NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Temp>(ref m_TempType);
				bool flag2 = ((ArchetypeChunk)(ref val4)).Has<IconElement>(ref m_IconElementType);
				for (int l = 0; l < nativeArray5.Length; l++)
				{
					Entity val6 = nativeArray4[l];
					Temp temp2 = nativeArray5[l];
					if (temp2.m_Original != Entity.Null)
					{
						if (m_IconElements.HasBuffer(temp2.m_Original) && !val3.TryAdd(temp2.m_Original, val6))
						{
							val3[temp2.m_Original] = val6;
						}
						if (flag2)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<IconElement>(val6);
						}
					}
				}
			}
			NativeArray<Entity> keyArray = val3.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<IconElement> val7 = default(NativeList<IconElement>);
			val7._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			Entity val11 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val12 = default(NativeParallelMultiHashMapIterator<Entity>);
			Target target = default(Target);
			Target target2 = default(Target);
			for (int m = 0; m < keyArray.Length; m++)
			{
				Entity val8 = keyArray[m];
				DynamicBuffer<IconElement> val9 = default(DynamicBuffer<IconElement>);
				DynamicBuffer<IconElement> val10 = default(DynamicBuffer<IconElement>);
				if (m_IconElements.HasBuffer(val8))
				{
					val10 = m_IconElements[val8];
					for (int n = 0; n < val10.Length; n++)
					{
						IconElement iconElement = val10[n];
						val7.Add(ref iconElement);
					}
				}
				if (val2.TryGetFirstValue(val8, ref val11, ref val12))
				{
					if (!val9.IsCreated)
					{
						val9 = ((!val10.IsCreated) ? ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<IconElement>(val8) : ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<IconElement>(val8));
					}
					do
					{
						PrefabRef prefabRef2 = m_PrefabRefData[val11];
						m_TargetData.TryGetComponent(val11, ref target);
						int num3;
						Entity icon;
						if (val10.IsCreated)
						{
							for (num3 = 0; num3 < val7.Length; num3++)
							{
								icon = val7[num3].m_Icon;
								if (m_PrefabRefData[icon].m_Prefab != prefabRef2.m_Prefab)
								{
									continue;
								}
								m_TargetData.TryGetComponent(icon, ref target2);
								if (!(target2.m_Target == target.m_Target))
								{
									continue;
								}
								goto IL_041d;
							}
						}
						val9.Add(new IconElement(val11));
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Owner>(val11, new Owner(val8));
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Temp>(val11);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(val11, ref m_AppliedTypes);
						continue;
						IL_041d:
						Icon icon2 = m_IconData[val11];
						icon2.m_ClusterIndex = m_IconData[icon].m_ClusterIndex;
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Icon>(icon, icon2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(icon, default(Updated));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val11, default(Deleted));
						val9.Add(new IconElement(icon));
						CollectionUtils.Remove<IconElement>(val7, num3);
					}
					while (val2.TryGetNextValue(ref val11, ref val12));
				}
				if (!val10.IsCreated)
				{
					continue;
				}
				Entity tempContainer = val3[val8];
				for (int num4 = 0; num4 < val7.Length; num4++)
				{
					Entity icon3 = val7[num4].m_Icon;
					if (ValidateOldIcon(icon3, val8, tempContainer))
					{
						if (!val9.IsCreated)
						{
							val9 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<IconElement>(val8);
						}
						val9.Add(new IconElement(icon3));
					}
					else
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(icon3, default(Deleted));
					}
				}
				if (!val9.IsCreated)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<IconElement>(val8);
				}
				val7.Clear();
			}
		}

		private bool ValidateOldIcon(Entity icon, Entity container, Entity tempContainer)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[icon];
			if (m_ToolErrorData.HasComponent(prefabRef.m_Prefab))
			{
				return false;
			}
			if (tempContainer != Entity.Null && (int)m_IconData[icon].m_Priority >= 250 && m_DestroyedData.HasComponent(container) && !m_DestroyedData.HasComponent(tempContainer))
			{
				return false;
			}
			return true;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Icon> __Game_Notifications_Icon_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<IconElement> __Game_Notifications_IconElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Icon> __Game_Notifications_Icon_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ToolErrorData> __Game_Prefabs_ToolErrorData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<IconElement> __Game_Notifications_IconElement_RO_BufferLookup;

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
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Notifications_Icon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Icon>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Notifications_IconElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<IconElement>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Notifications_Icon_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Icon>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ToolErrorData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ToolErrorData>(true);
			__Game_Notifications_IconElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<IconElement>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_TempQuery;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() });
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
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
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<ApplyTempIconsJob>(new ApplyTempIconsJob
		{
			m_Chunks = chunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconType = InternalCompilerInterface.GetComponentTypeHandle<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IconElementType = InternalCompilerInterface.GetBufferTypeHandle<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconData = InternalCompilerInterface.GetComponentLookup<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ToolErrorData = InternalCompilerInterface.GetComponentLookup<ToolErrorData>(ref __TypeHandle.__Game_Prefabs_ToolErrorData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IconElements = InternalCompilerInterface.GetBufferLookup<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AppliedTypes = m_AppliedTypes,
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
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
	public ApplyNotificationsSystem()
	{
	}
}
