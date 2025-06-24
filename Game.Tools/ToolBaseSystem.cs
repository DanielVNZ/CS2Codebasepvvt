using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Entities;
using Game.Areas;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Notifications;
using Game.Prefabs;
using Game.Routes;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public abstract class ToolBaseSystem : GameSystemBase, IEquatable<ToolBaseSystem>
{
	[BurstCompile]
	private struct DestroyDefinitionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				((ParallelWriter)(ref m_CommandBuffer)).DestroyEntity(unfilteredChunkIndex, nativeArray[i]);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct InvertBrushesJob : IJobChunk
	{
		public ComponentTypeHandle<Brush> m_BrushType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Brush> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Brush>(ref m_BrushType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Brush brush = nativeArray[i];
				brush.m_Strength = 0f - brush.m_Strength;
				nativeArray[i] = brush;
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

		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BrushData> __Game_Prefabs_BrushData_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Brush_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(false);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_BrushData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BrushData>(true);
		}
	}

	public const Snap kSnapAllIgnoredMask = Snap.AutoParent | Snap.PrefabType | Snap.ContourLines;

	protected ToolSystem m_ToolSystem;

	protected PrefabSystem m_PrefabSystem;

	protected DefaultToolSystem m_DefaultToolSystem;

	protected ToolRaycastSystem m_ToolRaycastSystem;

	protected OriginalDeletedSystem m_OriginalDeletedSystem;

	protected EntityQuery m_ErrorQuery;

	protected Snap m_SnapOnMask;

	protected Snap m_SnapOffMask;

	protected bool m_HasFocus;

	protected bool m_FocusChanged;

	protected bool m_ForceUpdate;

	private IProxyAction m_ApplyAction;

	private IProxyAction m_SecondaryApplyAction;

	private IProxyAction m_CancelAction;

	private protected IProxyAction m_DefaultApply;

	private protected IProxyAction m_DefaultSecondaryApply;

	private protected IProxyAction m_DefaultCancel;

	private protected IProxyAction m_MouseApply;

	private protected IProxyAction m_MouseCancel;

	private TypeHandle __TypeHandle;

	public abstract string toolID { get; }

	public virtual int uiModeIndex => 0;

	public virtual Color32 color { get; set; }

	public BrushPrefab brushType { get; set; }

	public float brushSize { get; set; }

	public float brushAngle { get; set; }

	public float brushStrength { get; set; }

	public bool requireZones { get; protected set; }

	public bool requireUnderground { get; protected set; }

	public bool requirePipelines { get; protected set; }

	public bool requireNetArrows { get; protected set; }

	public bool requireStopIcons { get; protected set; }

	public AreaTypeMask requireAreas { get; protected set; }

	public RouteType requireRoutes { get; protected set; }

	public TransportType requireStops { get; protected set; }

	public Layer requireNet { get; protected set; }

	public InfoviewPrefab infoview { get; private set; }

	public List<InfomodePrefab> infomodes { get; private set; }

	public virtual Snap selectedSnap { get; set; }

	public ApplyMode applyMode { get; protected set; }

	public virtual bool allowUnderground { get; protected set; }

	public virtual bool brushing => false;

	protected IProxyAction applyAction => m_ApplyAction ?? (m_ApplyAction = m_DefaultApply);

	protected IProxyAction secondaryApplyAction => m_SecondaryApplyAction ?? (m_SecondaryApplyAction = m_DefaultSecondaryApply);

	protected IProxyAction cancelAction => m_CancelAction ?? (m_CancelAction = m_DefaultCancel);

	protected IProxyAction applyActionOverride
	{
		get
		{
			if (m_ApplyAction == m_DefaultApply)
			{
				return null;
			}
			return m_ApplyAction;
		}
		set
		{
			SetAction(ref m_ApplyAction, value ?? m_DefaultApply);
		}
	}

	protected IProxyAction secondaryApplyActionOverride
	{
		get
		{
			if (m_SecondaryApplyAction == m_DefaultSecondaryApply)
			{
				return null;
			}
			return m_SecondaryApplyAction;
		}
		set
		{
			SetAction(ref m_SecondaryApplyAction, value ?? m_DefaultSecondaryApply);
		}
	}

	protected IProxyAction cancelActionOverride
	{
		get
		{
			if (m_CancelAction == m_DefaultCancel)
			{
				return null;
			}
			return m_CancelAction;
		}
		set
		{
			SetAction(ref m_CancelAction, value ?? m_DefaultCancel);
		}
	}

	private protected virtual IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield break;
		}
	}

	private IEnumerable<IProxyAction> baseToolActions
	{
		get
		{
			yield return m_DefaultApply;
			yield return m_DefaultSecondaryApply;
			yield return m_DefaultCancel;
			yield return m_MouseCancel;
			yield return m_MouseApply;
		}
	}

	internal IEnumerable<IProxyAction> actions => baseToolActions.Concat(toolActions);

	private protected bool actionsEnabled { get; private set; } = true;

	public static event Action<ProxyAction> EventToolActionPerformed;

	public virtual void GetUIModes(List<ToolMode> modes)
	{
	}

	public bool Equals(ToolBaseSystem other)
	{
		return this == other;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_OriginalDeletedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<OriginalDeletedSystem>();
		string name = ((object)this).GetType().Name;
		m_DefaultApply = InputManager.instance.toolActionCollection.GetActionState("Apply", name);
		m_DefaultSecondaryApply = InputManager.instance.toolActionCollection.GetActionState("Secondary Apply", name);
		m_DefaultCancel = InputManager.instance.toolActionCollection.GetActionState("Cancel", name);
		m_MouseApply = InputManager.instance.toolActionCollection.GetActionState("Mouse Apply", name);
		m_MouseCancel = InputManager.instance.toolActionCollection.GetActionState("Mouse Cancel", name);
		requireAreas = AreaTypeMask.None;
		requireRoutes = RouteType.None;
		requireStops = TransportType.None;
		selectedSnap = Snap.All;
		((ComponentSystemBase)this).Enabled = false;
		m_HasFocus = true;
		m_ErrorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Error>() });
		infomodes = new List<InfomodePrefab>();
		m_ToolSystem.tools.Add(this);
	}

	protected override void OnFocusChanged(bool hasfocus)
	{
		m_FocusChanged = hasfocus != m_HasFocus;
		m_HasFocus = hasfocus;
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		m_ForceUpdate = true;
		SetActions();
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		infoview = null;
		infomodes.Clear();
		ResetActions();
		((COSystemBase)this).OnStopRunning();
	}

	private protected virtual void SetActions()
	{
		UpdateActions();
		SetInteraction(set: true);
	}

	private protected virtual void ResetActions()
	{
		SetInteraction(set: false);
		using (ProxyAction.DeferStateUpdating())
		{
			applyAction.enabled = false;
			secondaryApplyAction.enabled = false;
			cancelAction.enabled = false;
			applyActionOverride = null;
			secondaryApplyActionOverride = null;
			cancelActionOverride = null;
			actionsEnabled = true;
		}
	}

	private protected virtual void UpdateActions()
	{
	}

	private void SetInteraction(bool set)
	{
		HashSet<ProxyAction> hashSet = new HashSet<ProxyAction>();
		foreach (IProxyAction action in actions)
		{
			if (!(action is UIBaseInputAction.IState state))
			{
				if (action is ProxyAction item)
				{
					hashSet.Add(item);
				}
				continue;
			}
			foreach (ProxyAction action2 in state.actions)
			{
				hashSet.Add(action2);
			}
		}
		foreach (ProxyAction item2 in hashSet)
		{
			if (set)
			{
				item2.onInteraction += OnActionInteraction;
			}
			else
			{
				item2.onInteraction -= OnActionInteraction;
			}
		}
	}

	public void ToggleToolOptions(bool enabled)
	{
		actionsEnabled = !enabled;
		UpdateActions();
	}

	private void OnActionInteraction(ProxyAction action, InputActionPhase phase)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)phase == 3)
		{
			ToolBaseSystem.EventToolActionPerformed?.Invoke(action);
		}
	}

	[Preserve]
	protected sealed override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).Dependency = OnUpdate(((SystemBase)this).Dependency);
		m_FocusChanged = false;
		m_ForceUpdate = false;
	}

	[Preserve]
	protected virtual JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return inputDeps;
	}

	[CanBeNull]
	public abstract PrefabBase GetPrefab();

	public abstract bool TrySetPrefab(PrefabBase prefab);

	public virtual void InitializeRaycast()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		m_ToolRaycastSystem.raycastFlags &= ~(RaycastFlags.ElevateOffset | RaycastFlags.SubElements | RaycastFlags.Placeholders | RaycastFlags.Markers | RaycastFlags.NoMainElements | RaycastFlags.UpgradeIsMain | RaycastFlags.OutsideConnections | RaycastFlags.Outside | RaycastFlags.Cargo | RaycastFlags.Passenger | RaycastFlags.Decals | RaycastFlags.EditorContainers | RaycastFlags.SubBuildings | RaycastFlags.PartialSurface | RaycastFlags.BuildingLots | RaycastFlags.IgnoreSecondary);
		m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
		m_ToolRaycastSystem.typeMask = TypeMask.None;
		m_ToolRaycastSystem.netLayerMask = Layer.None;
		m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.None;
		m_ToolRaycastSystem.routeType = RouteType.None;
		m_ToolRaycastSystem.transportType = TransportType.None;
		m_ToolRaycastSystem.iconLayerMask = IconLayerMask.None;
		m_ToolRaycastSystem.utilityTypeMask = UtilityTypes.None;
		m_ToolRaycastSystem.rayOffset = default(float3);
	}

	public virtual void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		onMask = Snap.None;
		offMask = Snap.None;
	}

	public virtual void SetUnderground(bool underground)
	{
	}

	public virtual void ElevationUp()
	{
	}

	public virtual void ElevationDown()
	{
	}

	public virtual void ElevationScroll()
	{
	}

	public static Snap GetActualSnap(Snap selectedSnap, Snap onMask, Snap offMask)
	{
		return (selectedSnap | ~offMask) & onMask;
	}

	protected Snap GetActualSnap()
	{
		return GetActualSnap(selectedSnap, m_SnapOnMask, m_SnapOffMask);
	}

	protected void UpdateInfoview(Entity prefab)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		infomodes.Clear();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<SubObject> val = default(DynamicBuffer<SubObject>);
		if (((EntityManager)(ref entityManager)).HasComponent<NetData>(prefab) && EntitiesExtensions.TryGetBuffer<SubObject>(((ComponentSystemBase)this).EntityManager, prefab, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				SubObject subObject = val[i];
				if ((subObject.m_Flags & SubObjectFlags.MakeOwner) != 0)
				{
					prefab = subObject.m_Prefab;
					break;
				}
			}
		}
		DynamicBuffer<PlaceableInfoviewItem> val2 = default(DynamicBuffer<PlaceableInfoviewItem>);
		if (EntitiesExtensions.TryGetBuffer<PlaceableInfoviewItem>(((ComponentSystemBase)this).EntityManager, prefab, true, ref val2) && val2.Length != 0)
		{
			infoview = m_PrefabSystem.GetPrefab<InfoviewPrefab>(val2[0].m_Item);
			for (int j = 1; j < val2.Length; j++)
			{
				infomodes.Add(m_PrefabSystem.GetPrefab<InfomodePrefab>(val2[j].m_Item));
			}
		}
		else
		{
			infoview = null;
		}
	}

	protected JobHandle DestroyDefinitions(EntityQuery group, ToolOutputBarrier barrier, JobHandle inputDeps)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref group)).IsEmptyIgnoreFilter)
		{
			return inputDeps;
		}
		DestroyDefinitionsJob destroyDefinitionsJob = new DestroyDefinitionsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = barrier.CreateCommandBuffer();
		destroyDefinitionsJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<DestroyDefinitionsJob>(destroyDefinitionsJob, group, inputDeps);
		((EntityCommandBufferSystem)barrier).AddJobHandleForProducer(val2);
		return val2;
	}

	protected JobHandle InvertBrushes(EntityQuery group, JobHandle inputDeps)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref group)).IsEmptyIgnoreFilter)
		{
			return inputDeps;
		}
		return JobChunkExtensions.ScheduleParallel<InvertBrushesJob>(new InvertBrushesJob
		{
			m_BrushType = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, group, inputDeps);
	}

	protected virtual bool GetAllowApply()
	{
		if (m_ToolSystem.ignoreErrors || ((EntityQuery)(ref m_ErrorQuery)).IsEmptyIgnoreFilter)
		{
			return !m_OriginalDeletedSystem.GetOriginalDeletedResult(0);
		}
		return false;
	}

	protected bool GetRaycastResult(out Entity entity, out RaycastHit hit)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolRaycastSystem.GetRaycastResult(out var result))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(result.m_Owner))
			{
				entity = result.m_Owner;
				hit = result.m_Hit;
				return true;
			}
		}
		entity = Entity.Null;
		hit = default(RaycastHit);
		return false;
	}

	protected bool GetRaycastResult(out Entity entity, out RaycastHit hit, out bool forceUpdate)
	{
		forceUpdate = m_OriginalDeletedSystem.GetOriginalDeletedResult(1) || m_ForceUpdate;
		return GetRaycastResult(out entity, out hit);
	}

	protected virtual bool GetRaycastResult(out ControlPoint controlPoint)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out Entity entity, out RaycastHit hit))
		{
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	protected virtual bool GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var entity, out var hit, out forceUpdate))
		{
			controlPoint = new ControlPoint(entity, hit);
			return true;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	protected bool GetContainers(EntityQuery group, out Entity laneContainer, out Entity transformContainer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		laneContainer = Entity.Null;
		transformContainer = Entity.Null;
		if (((EntityQuery)(ref group)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref group)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			Entity val2 = val[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<NetData>(val2))
			{
				laneContainer = val2;
				continue;
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ObjectData>(val2))
			{
				transformContainer = val2;
			}
		}
		val.Dispose();
		return true;
	}

	protected BrushPrefab FindDefaultBrush(EntityQuery query)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		BrushPrefab result = null;
		int num = int.MaxValue;
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BrushData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<BrushData>(ref __TypeHandle.__Game_Prefabs_BrushData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref query)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				NativeArray<BrushData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BrushData>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					BrushData brushData = nativeArray2[j];
					if (brushData.m_Priority < num)
					{
						result = m_PrefabSystem.GetPrefab<BrushPrefab>(nativeArray[j]);
						num = brushData.m_Priority;
					}
				}
			}
			return result;
		}
		finally
		{
			val.Dispose();
		}
	}

	protected void EnsureCachedBrushData()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)brushType != (Object)null))
		{
			return;
		}
		Entity entity = m_PrefabSystem.GetEntity(brushType);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		BrushData componentData = ((EntityManager)(ref entityManager)).GetComponentData<BrushData>(entity);
		if (math.all(componentData.m_Resolution != 0) || (Object)(object)brushType.m_Texture == (Object)null || ((Texture)brushType.m_Texture).width == 0 || ((Texture)brushType.m_Texture).height == 0)
		{
			return;
		}
		int2 val = default(int2);
		((int2)(ref val))._002Ector(((Texture)brushType.m_Texture).width, ((Texture)brushType.m_Texture).height);
		componentData.m_Resolution = val;
		int num = 1;
		float num2 = 1f;
		while (math.any(componentData.m_Resolution > 128) && math.all(componentData.m_Resolution > 1))
		{
			ref int2 resolution = ref componentData.m_Resolution;
			resolution /= 2;
			num *= 2;
			num2 *= 0.25f;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<BrushData>(entity, componentData);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<BrushCell> buffer = ((EntityManager)(ref entityManager)).GetBuffer<BrushCell>(entity, false);
		Color[] pixels = brushType.m_Texture.GetPixels();
		buffer.ResizeUninitialized(componentData.m_Resolution.x * componentData.m_Resolution.y);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < componentData.m_Resolution.y; i++)
		{
			for (int j = 0; j < componentData.m_Resolution.x; j++)
			{
				BrushCell brushCell = default(BrushCell);
				int num5 = num3;
				for (int k = 0; k < num; k++)
				{
					for (int l = 0; l < num; l++)
					{
						brushCell.m_Opacity += pixels[num5++].a;
					}
					num5 += val.x - num;
				}
				brushCell.m_Opacity *= num2;
				buffer[num4++] = brushCell;
				num3 += num;
			}
			num3 += val.x * (num - 1);
		}
	}

	protected EntityQuery GetDefinitionQuery()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.Exclude<Updated>()
		});
	}

	protected EntityQuery GetContainerQuery()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EditorContainerData>() });
	}

	protected EntityQuery GetBrushQuery()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BrushData>() });
	}

	protected void SetAction(ref IProxyAction action, IProxyAction newAction)
	{
		if (newAction != action)
		{
			if (action == null)
			{
				action = newAction;
			}
			else if (newAction == null)
			{
				action.enabled = false;
				action = null;
			}
			else
			{
				newAction.enabled = action.enabled;
				action.enabled = false;
				action = newAction;
			}
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
	protected ToolBaseSystem()
	{
	}
}
