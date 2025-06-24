using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Agents;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class PathDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct PathGizmoJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PersonalCar> m_PersonalCarType;

		[ReadOnly]
		public ComponentTypeHandle<DeliveryTruck> m_DeliveryTruckType;

		[ReadOnly]
		public ComponentTypeHandle<Creature> m_CreatureType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<JobSeeker> m_JobSeekerType;

		[ReadOnly]
		public ComponentTypeHandle<SchoolSeeker> m_SchoolSeekerType;

		[ReadOnly]
		public ComponentTypeHandle<Household> m_HouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<CompanyData> m_CompanyType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Segment> m_RouteSegmentType;

		[ReadOnly]
		public ComponentTypeHandle<GoodsDeliveryRequest> m_GoodsDeliveryRequestType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public bool m_PersonalCarOption;

		[ReadOnly]
		public bool m_DeliveryTruckOption;

		[ReadOnly]
		public bool m_ServiceVehicleOption;

		[ReadOnly]
		public bool m_ResidentOption;

		[ReadOnly]
		public bool m_CitizenOption;

		[ReadOnly]
		public bool m_JobSeekerOption;

		[ReadOnly]
		public bool m_SchoolSeekerOption;

		[ReadOnly]
		public bool m_HouseholdOption;

		[ReadOnly]
		public bool m_CompanyOption;

		[ReadOnly]
		public bool m_RouteOption;

		[ReadOnly]
		public bool m_DeliveryRequestOption;

		[ReadOnly]
		public bool m_ServiceRequestOption;

		[ReadOnly]
		public float m_TimeOffset;

		[ReadOnly]
		public Entity m_Selected;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			int num;
			int num2;
			if (m_Selected != Entity.Null)
			{
				num = (num2 = -1);
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					if (nativeArray[i] == m_Selected)
					{
						num = i;
						num2 = i + 1;
						break;
					}
				}
				if (num == -1)
				{
					return;
				}
			}
			else
			{
				num = 0;
				num2 = ((ArchetypeChunk)(ref chunk)).Count;
				if (((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType))
				{
					if (((ArchetypeChunk)(ref chunk)).Has<PersonalCar>(ref m_PersonalCarType))
					{
						if (!m_PersonalCarOption)
						{
							return;
						}
					}
					else if (((ArchetypeChunk)(ref chunk)).Has<DeliveryTruck>(ref m_DeliveryTruckType))
					{
						if (!m_DeliveryTruckOption)
						{
							return;
						}
					}
					else if (!m_ServiceVehicleOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Creature>(ref m_CreatureType))
				{
					if (!m_ResidentOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Citizen>(ref m_CitizenType))
				{
					if (!m_CitizenOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<JobSeeker>(ref m_JobSeekerType))
				{
					if (!m_JobSeekerOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<SchoolSeeker>(ref m_SchoolSeekerType))
				{
					if (!m_SchoolSeekerOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Household>(ref m_HouseholdType))
				{
					if (!m_HouseholdOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<CompanyData>(ref m_CompanyType))
				{
					if (!m_CompanyOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<Game.Routes.Segment>(ref m_RouteSegmentType))
				{
					if (!m_RouteOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<GoodsDeliveryRequest>(ref m_GoodsDeliveryRequestType))
				{
					if (!m_DeliveryRequestOption)
					{
						return;
					}
				}
				else if (((ArchetypeChunk)(ref chunk)).Has<ServiceRequest>(ref m_ServiceRequestType) && !m_ServiceRequestOption)
				{
					return;
				}
			}
			NativeArray<PathOwner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			float timeOffset = m_TimeOffset * 10f;
			Color val = Color.white;
			Color val2 = Color.magenta;
			Color val3 = Color.red;
			Color val4 = Color.green;
			if (nativeArray2.Length == 0)
			{
				val *= 0.5f;
				val2 *= 0.5f;
				val3 *= 0.5f;
				val4 *= 0.5f;
			}
			for (int j = num; j < num2; j++)
			{
				int num3 = 0;
				if (nativeArray2.Length != 0)
				{
					num3 = nativeArray2[j].m_ElementIndex;
				}
				DynamicBuffer<PathElement> val5 = bufferAccessor[j];
				float3 val6 = default(float3);
				bool flag = false;
				bool flag2 = false;
				for (int k = num3; k < val5.Length; k++)
				{
					PathElement pathElement = val5[k];
					if (m_CurveData.HasComponent(pathElement.m_Target))
					{
						Curve curve = m_CurveData[pathElement.m_Target];
						Bezier4x3 val7 = MathUtils.Cut(curve.m_Bezier, pathElement.m_TargetDelta);
						float length = curve.m_Length * math.abs(pathElement.m_TargetDelta.y - pathElement.m_TargetDelta.x);
						if (flag && math.lengthsq(val7.a - val6) > 1E-06f)
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6, val7.a, val2);
						}
						DrawPathCurve(val7, length, timeOffset, val);
						if (k == num3)
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val7.a, 1f, val3);
						}
						if (k == val5.Length - 1)
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val7.d, 1f, val4);
						}
						val6 = val7.d;
						flag = true;
						flag2 = false;
						continue;
					}
					float3 position;
					if (m_PositionData.HasComponent(pathElement.m_Target))
					{
						position = m_PositionData[pathElement.m_Target].m_Position;
					}
					else
					{
						if (!m_TransformData.HasComponent(pathElement.m_Target))
						{
							continue;
						}
						position = m_TransformData[pathElement.m_Target].m_Position;
					}
					if (flag && math.lengthsq(position - val6) > 1E-06f)
					{
						if (flag2)
						{
							Bezier4x3 curve2 = NetUtils.StraightCurve(val6, position);
							DrawPathCurve(curve2, math.distance(val6, position), timeOffset, val);
						}
						else
						{
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(val6, position, val2);
						}
					}
					if (k == num3)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(position, 1f, val3);
					}
					else if (k == val5.Length - 1)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(position, 1f, val4);
					}
					val6 = position;
					flag = true;
					flag2 = true;
				}
			}
		}

		private void DrawPathCurve(Bezier4x3 curve, float length, float timeOffset, Color color)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (length >= 1f)
			{
				int num = (int)math.ceil(length * 0.01f);
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawFlowCurve(curve, length, color, timeOffset, false, num, -1, 1f, 25f, 16);
			}
			else
			{
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawCurve(curve, length, color, -1);
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<JobSeeker> __Game_Agents_JobSeeker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SchoolSeeker> __Game_Citizens_SchoolSeeker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Household> __Game_Citizens_Household_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Segment> __Game_Routes_Segment_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GoodsDeliveryRequest> __Game_Simulation_GoodsDeliveryRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

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
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Pathfind_PathOwner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PersonalCar>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DeliveryTruck>(true);
			__Game_Creatures_Creature_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(true);
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Agents_JobSeeker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<JobSeeker>(true);
			__Game_Citizens_SchoolSeeker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SchoolSeeker>(true);
			__Game_Citizens_Household_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Household>(true);
			__Game_Companies_CompanyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(true);
			__Game_Routes_Segment_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Routes.Segment>(true);
			__Game_Simulation_GoodsDeliveryRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GoodsDeliveryRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceRequest>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
		}
	}

	private EntityQuery m_PathGroup;

	private GizmosSystem m_GizmosSystem;

	private ToolSystem m_ToolSystem;

	private Option m_PersonalCarOption;

	private Option m_DeliveryTruckOption;

	private Option m_ServiceVehicleOption;

	private Option m_ResidentOption;

	private Option m_CitizenOption;

	private Option m_CompanyOption;

	private Option m_RouteOption;

	private Option m_DeliveryRequestOption;

	private Option m_ServiceRequestOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PathGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PathElement>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PersonalCarOption = AddOption("Personal cars", defaultEnabled: true);
		m_DeliveryTruckOption = AddOption("Delivery trucks", defaultEnabled: true);
		m_ServiceVehicleOption = AddOption("Service vehicles", defaultEnabled: true);
		m_ResidentOption = AddOption("Citizens (instance)", defaultEnabled: true);
		m_CitizenOption = AddOption("Citizens (agent)", defaultEnabled: false);
		m_CompanyOption = AddOption("Companies", defaultEnabled: false);
		m_RouteOption = AddOption("Transport routes", defaultEnabled: false);
		m_DeliveryRequestOption = AddOption("Delivery requests", defaultEnabled: false);
		m_ServiceRequestOption = AddOption("Service requests", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_PathGroup)).IsEmptyIgnoreFilter)
		{
			((SystemBase)this).Dependency = DrawPathGizmos(m_PathGroup, ((SystemBase)this).Dependency);
		}
	}

	private JobHandle DrawPathGizmos(EntityQuery group, JobHandle inputDeps)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<PathGizmoJob>(new PathGizmoJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarType = InternalCompilerInterface.GetComponentTypeHandle<PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeliveryTruckType = InternalCompilerInterface.GetComponentTypeHandle<DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_JobSeekerType = InternalCompilerInterface.GetComponentTypeHandle<JobSeeker>(ref __TypeHandle.__Game_Agents_JobSeeker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolSeekerType = InternalCompilerInterface.GetComponentTypeHandle<SchoolSeeker>(ref __TypeHandle.__Game_Citizens_SchoolSeeker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdType = InternalCompilerInterface.GetComponentTypeHandle<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegmentType = InternalCompilerInterface.GetComponentTypeHandle<Game.Routes.Segment>(ref __TypeHandle.__Game_Routes_Segment_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GoodsDeliveryRequestType = InternalCompilerInterface.GetComponentTypeHandle<GoodsDeliveryRequest>(ref __TypeHandle.__Game_Simulation_GoodsDeliveryRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestType = InternalCompilerInterface.GetComponentTypeHandle<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarOption = m_PersonalCarOption.enabled,
			m_DeliveryTruckOption = m_DeliveryTruckOption.enabled,
			m_ServiceVehicleOption = m_ServiceVehicleOption.enabled,
			m_ResidentOption = m_ResidentOption.enabled,
			m_CitizenOption = m_CitizenOption.enabled,
			m_CompanyOption = m_CompanyOption.enabled,
			m_RouteOption = m_RouteOption.enabled,
			m_DeliveryRequestOption = m_DeliveryRequestOption.enabled,
			m_ServiceRequestOption = m_ServiceRequestOption.enabled,
			m_TimeOffset = Time.realtimeSinceStartup,
			m_Selected = m_ToolSystem.selected,
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, group, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
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
	public PathDebugSystem()
	{
	}
}
