using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ServiceFeeSystem : GameSystemBase, IServiceFeeSystem, IDefaultSerializable, ISerializable, IPostDeserialize
{
	public struct FeeEvent : ISerializable
	{
		public PlayerResource m_Resource;

		public float m_Amount;

		public float m_Cost;

		public bool m_Outside;

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			PlayerResource num = m_Resource;
			((IWriter)writer/*cast due to .constrained prefix*/).Write((int)num);
			float num2 = m_Amount;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
			float num3 = m_Cost;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
			bool num4 = m_Outside;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			int resource = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref resource);
			m_Resource = (PlayerResource)resource;
			ref float reference = ref m_Amount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref float reference2 = ref m_Cost;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			ref bool reference3 = ref m_Outside;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		}
	}

	[BurstCompile]
	private struct PayFeeJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<Patient> m_PatientType;

		[ReadOnly]
		public BufferTypeHandle<Game.Buildings.Student> m_StudentType;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> m_HouseholdMembers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public BufferLookup<ServiceFee> m_Fees;

		public BufferLookup<Resources> m_Resources;

		public NativeQueue<FeeEvent> m_FeeEvents;

		public Entity m_City;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Patient> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Patient>(ref m_PatientType);
			BufferAccessor<Game.Buildings.Student> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Buildings.Student>(ref m_StudentType);
			if (bufferAccessor.Length != 0)
			{
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					DynamicBuffer<Patient> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						PayFee(val[j].m_Patient, PlayerResource.Healthcare);
					}
				}
			}
			if (bufferAccessor2.Length == 0)
			{
				return;
			}
			Game.Citizens.Student student2 = default(Game.Citizens.Student);
			for (int k = 0; k < ((ArchetypeChunk)(ref chunk)).Count; k++)
			{
				DynamicBuffer<Game.Buildings.Student> val2 = bufferAccessor2[k];
				for (int l = 0; l < val2.Length; l++)
				{
					Entity student = val2[l].m_Student;
					if (m_Students.TryGetComponent(student, ref student2))
					{
						PayFee(student, GetEducationResource(student2.m_Level));
					}
				}
			}
		}

		private void PayFee(Entity citizen, PlayerResource resource)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			HouseholdMember householdMember = default(HouseholdMember);
			DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
			if (m_HouseholdMembers.TryGetComponent(citizen, ref householdMember) && m_Resources.TryGetBuffer(householdMember.m_Household, ref resources))
			{
				float num = GetFee(resource, m_Fees[m_City]) / 128f;
				float amount = 1f / 128f;
				EconomyUtils.AddResources(Resource.Money, (int)(0f - math.round(num)), resources);
				m_FeeEvents.Enqueue(new FeeEvent
				{
					m_Resource = resource,
					m_Amount = amount,
					m_Cost = num,
					m_Outside = false
				});
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FeeToCityJob : IJob
	{
		public BufferLookup<CollectedCityServiceFeeData> m_FeeDatas;

		[ReadOnly]
		public NativeList<Entity> m_FeeDataEntities;

		public NativeQueue<FeeEvent> m_FeeEvents;

		public Entity m_City;

		public void Execute()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_FeeDataEntities.Length; i++)
			{
				DynamicBuffer<CollectedCityServiceFeeData> val = m_FeeDatas[m_FeeDataEntities[i]];
				for (int j = 0; j < val.Length; j++)
				{
					CollectedCityServiceFeeData collectedCityServiceFeeData = val[j];
					collectedCityServiceFeeData.m_Export = 0f;
					collectedCityServiceFeeData.m_ExportCount = 0f;
					collectedCityServiceFeeData.m_Import = 0f;
					collectedCityServiceFeeData.m_ImportCount = 0f;
					collectedCityServiceFeeData.m_Internal = 0f;
					collectedCityServiceFeeData.m_InternalCount = 0f;
					val[j] = collectedCityServiceFeeData;
				}
			}
			FeeEvent feeEvent = default(FeeEvent);
			while (m_FeeEvents.TryDequeue(ref feeEvent))
			{
				for (int k = 0; k < m_FeeDataEntities.Length; k++)
				{
					DynamicBuffer<CollectedCityServiceFeeData> val2 = m_FeeDatas[m_FeeDataEntities[k]];
					for (int l = 0; l < val2.Length; l++)
					{
						if (val2[l].m_PlayerResource != (int)feeEvent.m_Resource)
						{
							continue;
						}
						CollectedCityServiceFeeData collectedCityServiceFeeData2 = val2[l];
						if (feeEvent.m_Amount > 0f)
						{
							if (feeEvent.m_Outside)
							{
								collectedCityServiceFeeData2.m_Export += feeEvent.m_Cost * 128f;
								collectedCityServiceFeeData2.m_ExportCount += feeEvent.m_Amount * 128f;
							}
							else
							{
								collectedCityServiceFeeData2.m_Internal += feeEvent.m_Cost * 128f;
								collectedCityServiceFeeData2.m_InternalCount += feeEvent.m_Amount * 128f;
							}
						}
						else
						{
							collectedCityServiceFeeData2.m_Import += feeEvent.m_Cost * 128f;
							collectedCityServiceFeeData2.m_ImportCount += (0f - feeEvent.m_Amount) * 128f;
						}
						val2[l] = collectedCityServiceFeeData2;
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct TriggerJob : IJob
	{
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public BufferLookup<CollectedCityServiceFeeData> m_FeeDatas;

		public NativeQueue<TriggerAction> m_ActionQueue;

		public void Execute()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			NativeArray<float> val = default(NativeArray<float>);
			val._002Ector(13, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < m_Entities.Length; i++)
			{
				DynamicBuffer<CollectedCityServiceFeeData> val2 = m_FeeDatas[m_Entities[i]];
				for (int j = 0; j < val2.Length; j++)
				{
					num += val2[j].m_Export - val2[j].m_Import;
					int playerResource = val2[j].m_PlayerResource;
					val[playerResource] += val2[j].m_Export - val2[j].m_Import;
				}
			}
			for (int k = 0; k < 13; k++)
			{
				SendTradeResourceTrigger((PlayerResource)k, val[k]);
			}
			m_ActionQueue.Enqueue(new TriggerAction(TriggerType.ServiceTradeBalance, Entity.Null, num));
		}

		private void SendTradeResourceTrigger(PlayerResource resource, float total)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			TriggerType triggerType = TriggerType.NewNotification;
			switch (resource)
			{
			case PlayerResource.Electricity:
				triggerType = TriggerType.CityServiceElectricity;
				break;
			case PlayerResource.Healthcare:
				triggerType = TriggerType.CityServiceHealthcare;
				break;
			case PlayerResource.BasicEducation:
			case PlayerResource.SecondaryEducation:
			case PlayerResource.HigherEducation:
				triggerType = TriggerType.CityServiceEducation;
				break;
			case PlayerResource.Garbage:
				triggerType = TriggerType.CityServiceGarbage;
				break;
			case PlayerResource.Water:
			case PlayerResource.Sewage:
				triggerType = TriggerType.CityServiceWaterSewage;
				break;
			case PlayerResource.Mail:
				triggerType = TriggerType.CityServicePost;
				break;
			case PlayerResource.FireResponse:
				triggerType = TriggerType.CityServiceFireAndRescue;
				break;
			case PlayerResource.Police:
				triggerType = TriggerType.CityServicePolice;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				m_ActionQueue.Enqueue(new TriggerAction(triggerType, Entity.Null, total));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Patient> __Game_Buildings_Patient_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Buildings.Student> __Game_Buildings_Student_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		public BufferLookup<Resources> __Game_Economy_Resources_RW_BufferLookup;

		public BufferLookup<CollectedCityServiceFeeData> __Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup;

		[ReadOnly]
		public BufferLookup<CollectedCityServiceFeeData> __Game_Simulation_CollectedCityServiceFeeData_RO_BufferLookup;

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
			__Game_Buildings_Patient_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Patient>(true);
			__Game_Buildings_Student_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Buildings.Student>(true);
			__Game_Citizens_HouseholdMember_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdMember>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_Economy_Resources_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(false);
			__Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceFeeData>(false);
			__Game_Simulation_CollectedCityServiceFeeData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceFeeData>(true);
		}
	}

	private const int kUpdatesPerDay = 128;

	private CitySystem m_CitySystem;

	private TriggerSystem m_TriggerSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_FeeCollectorGroup;

	private EntityQuery m_CollectedFeeGroup;

	private NativeQueue<FeeEvent> m_FeeQueue;

	private NativeList<CollectedCityServiceFeeData> m_CityServiceFees;

	private JobHandle m_Writers;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 2048;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CollectedFeeGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CollectedCityServiceFeeData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.City.ServiceFeeCollector>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Patient>(),
			ComponentType.ReadOnly<Game.Buildings.Student>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_FeeCollectorGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_CollectedFeeGroup);
		m_FeeQueue = new NativeQueue<FeeEvent>(AllocatorHandle.op_Implicit((Allocator)4));
		m_CityServiceFees = new NativeList<CollectedCityServiceFeeData>(13, AllocatorHandle.op_Implicit((Allocator)4));
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)((Context)(ref context)).purpose == 1)
		{
			CacheFees(reset: true);
		}
		else
		{
			CacheFees();
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_Writers)).Complete();
		m_FeeQueue.Dispose();
		m_CityServiceFees.Dispose();
		base.OnDestroy();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Writers)).Complete();
		NativeArray<FeeEvent> val = m_FeeQueue.ToArray(AllocatorHandle.op_Implicit((Allocator)2));
		int length = val.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		Enumerator<FeeEvent> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				FeeEvent current = enumerator.Current;
				((IWriter)writer/*cast due to .constrained prefix*/).Write<FeeEvent>(current);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((JobHandle)(ref m_Writers)).Complete();
		m_FeeQueue.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		FeeEvent feeEvent = default(FeeEvent);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read<FeeEvent>(ref feeEvent);
			m_FeeQueue.Enqueue(feeEvent);
		}
	}

	public void SetDefaults(Context context)
	{
		((JobHandle)(ref m_Writers)).Complete();
		m_FeeQueue.Clear();
	}

	public NativeList<CollectedCityServiceFeeData> GetServiceFees()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_CityServiceFees;
	}

	public NativeQueue<FeeEvent> GetFeeQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_Writers;
		return m_FeeQueue;
	}

	public void AddQueueWriter(JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Writers = JobHandle.CombineDependencies(m_Writers, deps);
	}

	public int3 GetServiceFees(PlayerResource resource)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetServiceFees(resource, m_CityServiceFees);
	}

	public int GetServiceFeeIncomeEstimate(PlayerResource resource, float fee)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetServiceFeeIncomeEstimate(resource, fee, m_CityServiceFees);
	}

	public static PlayerResource GetEducationResource(int level)
	{
		switch (level)
		{
		case 1:
			return PlayerResource.BasicEducation;
		case 2:
			return PlayerResource.SecondaryEducation;
		case 3:
		case 4:
			return PlayerResource.HigherEducation;
		default:
			return PlayerResource.Count;
		}
	}

	public static float GetFee(PlayerResource resource, DynamicBuffer<ServiceFee> fees)
	{
		for (int i = 0; i < fees.Length; i++)
		{
			ServiceFee serviceFee = fees[i];
			if (serviceFee.m_Resource == resource)
			{
				return serviceFee.m_Fee;
			}
		}
		return 0f;
	}

	public static bool TryGetFee(PlayerResource resource, DynamicBuffer<ServiceFee> fees, out float fee)
	{
		for (int i = 0; i < fees.Length; i++)
		{
			ServiceFee serviceFee = fees[i];
			if (serviceFee.m_Resource == resource)
			{
				fee = serviceFee.m_Fee;
				return true;
			}
		}
		fee = 0f;
		return false;
	}

	public static void SetFee(PlayerResource resource, DynamicBuffer<ServiceFee> fees, float value)
	{
		for (int i = 0; i < fees.Length; i++)
		{
			ServiceFee serviceFee = fees[i];
			if (serviceFee.m_Resource == resource)
			{
				serviceFee.m_Fee = value;
				fees[i] = serviceFee;
				return;
			}
		}
		fees.Add(new ServiceFee
		{
			m_Fee = value,
			m_Resource = resource
		});
	}

	public static float GetConsumptionMultiplier(PlayerResource resource, float relativeFee, in ServiceFeeParameterData feeParameters)
	{
		return resource switch
		{
			PlayerResource.Electricity => AdjustElectricityConsumptionSystem.GetFeeConsumptionMultiplier(relativeFee, in feeParameters), 
			PlayerResource.Water => AdjustWaterConsumptionSystem.GetFeeConsumptionMultiplier(relativeFee, in feeParameters), 
			_ => 1f, 
		};
	}

	public static float GetEfficiencyMultiplier(PlayerResource resource, float relativeFee, in BuildingEfficiencyParameterData efficiencyParameters)
	{
		return resource switch
		{
			PlayerResource.Electricity => AdjustElectricityConsumptionSystem.GetFeeEfficiencyFactor(relativeFee, in efficiencyParameters), 
			PlayerResource.Water => AdjustWaterConsumptionSystem.GetFeeEfficiencyFactor(relativeFee, in efficiencyParameters), 
			_ => 1f, 
		};
	}

	public static int GetHappinessEffect(PlayerResource resource, float relativeFee, in CitizenHappinessParameterData happinessParameters)
	{
		return resource switch
		{
			PlayerResource.Electricity => CitizenHappinessSystem.GetElectricityFeeHappinessEffect(relativeFee, in happinessParameters), 
			PlayerResource.Water => CitizenHappinessSystem.GetWaterFeeHappinessEffect(relativeFee, in happinessParameters), 
			_ => 1, 
		};
	}

	public static int3 GetServiceFees(PlayerResource resource, NativeList<CollectedCityServiceFeeData> fees)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float3 val = default(float3);
		Enumerator<CollectedCityServiceFeeData> enumerator = fees.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CollectedCityServiceFeeData current = enumerator.Current;
				if (current.m_PlayerResource == (int)resource)
				{
					val += new float3(current.m_Internal, current.m_Export, current.m_Import);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return new int3(math.round(val));
	}

	public static int GetServiceFeeIncomeEstimate(PlayerResource resource, float fee, NativeList<CollectedCityServiceFeeData> fees)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Enumerator<CollectedCityServiceFeeData> enumerator = fees.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CollectedCityServiceFeeData current = enumerator.Current;
				if (current.m_PlayerResource == (int)resource)
				{
					num += current.m_InternalCount * fee;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		return (int)math.round(num);
	}

	private void CacheFees(bool reset = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_CollectedFeeGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		m_CityServiceFees.Clear();
		for (int i = 0; i < val.Length; i++)
		{
			Entity val2 = val[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<CollectedCityServiceFeeData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CollectedCityServiceFeeData>(val2, !reset);
			for (int j = 0; j < buffer.Length; j++)
			{
				if (reset)
				{
					CollectedCityServiceFeeData collectedCityServiceFeeData = new CollectedCityServiceFeeData
					{
						m_PlayerResource = buffer[j].m_PlayerResource
					};
					buffer[j] = collectedCityServiceFeeData;
				}
				ref NativeList<CollectedCityServiceFeeData> reference = ref m_CityServiceFees;
				CollectedCityServiceFeeData collectedCityServiceFeeData2 = buffer[j];
				reference.Add(ref collectedCityServiceFeeData2);
			}
		}
		val.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		CacheFees();
		PayFeeJob payFeeJob = new PayFeeJob
		{
			m_PatientType = InternalCompilerInterface.GetBufferTypeHandle<Patient>(ref __TypeHandle.__Game_Buildings_Patient_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StudentType = InternalCompilerInterface.GetBufferTypeHandle<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMembers = InternalCompilerInterface.GetComponentLookup<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FeeEvents = m_FeeQueue,
			m_City = m_CitySystem.City
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<PayFeeJob>(payFeeJob, m_FeeCollectorGroup, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		JobHandle val = default(JobHandle);
		FeeToCityJob feeToCityJob = new FeeToCityJob
		{
			m_FeeDatas = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceFeeData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FeeDataEntities = ((EntityQuery)(ref m_CollectedFeeGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_FeeEvents = m_FeeQueue,
			m_City = m_CitySystem.City
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<FeeToCityJob>(feeToCityJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, m_Writers));
		m_Writers = ((SystemBase)this).Dependency;
		TriggerJob triggerJob = new TriggerJob
		{
			m_Entities = ((EntityQuery)(ref m_CollectedFeeGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3)),
			m_FeeDatas = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceFeeData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceFeeData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = m_TriggerSystem.CreateActionBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<TriggerJob>(triggerJob, ((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
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
	public ServiceFeeSystem()
	{
	}
}
