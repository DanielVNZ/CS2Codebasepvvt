using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Objects;
using Game.Routes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LinesSection : InfoSectionBase
{
	private enum Result
	{
		HasRoutes,
		HasPassengers
	}

	[BurstCompile]
	private struct LinesJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_Owners;

		[ReadOnly]
		public ComponentLookup<WaitingPassengers> m_WaitingPassengers;

		[ReadOnly]
		public BufferLookup<SubObject> m_SubObjectBuffers;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRouteBuffers;

		public NativeArray<bool> m_BoolResult;

		public NativeList<Entity> m_LinesResult;

		public NativeArray<int> m_PassengersResult;

		public void Execute()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			bool supportRoutes = false;
			bool supportPassengers = false;
			int passengerCount = 0;
			CheckEntity(m_SelectedEntity, ref supportRoutes, ref supportPassengers, ref passengerCount);
			m_PassengersResult[0] = passengerCount;
			m_BoolResult[0] = supportRoutes;
			m_BoolResult[1] = supportPassengers;
		}

		private void CheckEntity(Entity entity, ref bool supportRoutes, ref bool supportPassengers, ref int passengerCount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
			if (m_ConnectedRouteBuffers.TryGetBuffer(entity, ref val))
			{
				supportRoutes = true;
				WaitingPassengers waitingPassengers = default(WaitingPassengers);
				Owner owner = default(Owner);
				for (int i = 0; i < val.Length; i++)
				{
					if (m_WaitingPassengers.TryGetComponent(val[i].m_Waypoint, ref waitingPassengers))
					{
						supportPassengers = true;
						passengerCount += waitingPassengers.m_Count;
					}
					if (m_Owners.TryGetComponent(val[i].m_Waypoint, ref owner) && !NativeListExtensions.Contains<Entity, Entity>(m_LinesResult, owner.m_Owner))
					{
						m_LinesResult.Add(ref owner.m_Owner);
					}
				}
			}
			WaitingPassengers waitingPassengers2 = default(WaitingPassengers);
			if (m_WaitingPassengers.TryGetComponent(entity, ref waitingPassengers2))
			{
				supportPassengers = true;
				passengerCount += waitingPassengers2.m_Count;
			}
			DynamicBuffer<SubObject> val2 = default(DynamicBuffer<SubObject>);
			if (m_SubObjectBuffers.TryGetBuffer(entity, ref val2))
			{
				for (int j = 0; j < val2.Length; j++)
				{
					CheckEntity(val2[j].m_SubObject, ref supportRoutes, ref supportPassengers, ref passengerCount);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaitingPassengers> __Game_Routes_WaitingPassengers_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

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
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Routes_WaitingPassengers_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaitingPassengers>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubObject>(true);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
		}
	}

	private TransportationOverviewUISystem m_TransportationOverviewUISystem;

	private NativeArray<bool> m_BoolResult;

	private NativeArray<int> m_PassengersResult;

	private NativeList<Entity> m_LinesResult;

	private TypeHandle __TypeHandle;

	protected override string group => "LinesSection";

	protected override bool displayForOutsideConnections => true;

	private NativeList<Entity> lines { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		lines.Clear();
		m_LinesResult.Clear();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TransportationOverviewUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TransportationOverviewUISystem>();
		m_BoolResult = new NativeArray<bool>(2, (Allocator)4, (NativeArrayOptions)1);
		m_PassengersResult = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)1);
		m_LinesResult = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		lines = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool>(group, "toggle", (Action<Entity, bool>)OnToggle, (IReader<Entity>)null, (IReader<bool>)null));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		lines.Dispose();
		m_LinesResult.Dispose();
		m_PassengersResult.Dispose();
		m_BoolResult.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<LinesJob>(new LinesJob
		{
			m_SelectedEntity = selectedEntity,
			m_Owners = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaitingPassengers = InternalCompilerInterface.GetComponentLookup<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectBuffers = InternalCompilerInterface.GetBufferLookup<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRouteBuffers = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BoolResult = m_BoolResult,
			m_LinesResult = m_LinesResult,
			m_PassengersResult = m_PassengersResult
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_BoolResult[0] || m_BoolResult[1];
	}

	protected override void OnProcess()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		m_InfoUISystem.SetRoutesVisible();
		for (int i = 0; i < m_LinesResult.Length; i++)
		{
			NativeList<Entity> val = lines;
			Entity val2 = m_LinesResult[i];
			val.Add(ref val2);
		}
		base.tooltipTags.Add(TooltipTags.CargoRoute.ToString());
		base.tooltipTags.Add(TooltipTags.TransportLine.ToString());
		base.tooltipTags.Add(TooltipTags.TransportStop.ToString());
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("hasLines");
		writer.Write(m_BoolResult[0]);
		writer.PropertyName("lines");
		JsonWriterExtensions.ArrayBegin(writer, lines.Length);
		Game.Routes.Color color = default(Game.Routes.Color);
		for (int i = 0; i < lines.Length; i++)
		{
			writer.TypeBegin("Game.UI.LinesSection.Line");
			writer.PropertyName("name");
			m_NameSystem.BindName(writer, lines[i]);
			writer.PropertyName("color");
			if (EntitiesExtensions.TryGetComponent<Game.Routes.Color>(((ComponentSystemBase)this).EntityManager, lines[i], ref color))
			{
				UnityWriters.Write(writer, color.m_Color);
			}
			else
			{
				UnityWriters.Write(writer, Color.white);
			}
			writer.PropertyName("entity");
			UnityWriters.Write(writer, lines[i]);
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			bool flag = !RouteUtils.CheckOption(((EntityManager)(ref entityManager)).GetComponentData<Route>(lines[i]), RouteOption.Inactive);
			writer.PropertyName("active");
			writer.Write(flag);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		writer.PropertyName("hasPassengers");
		writer.Write(m_BoolResult[1]);
		writer.PropertyName("passengers");
		writer.Write(m_PassengersResult[0]);
	}

	private void OnToggle(Entity entity, bool state)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TransportationOverviewUISystem.SetLineState(entity, state);
		m_InfoUISystem.RequestUpdate();
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
	public LinesSection()
	{
	}
}
