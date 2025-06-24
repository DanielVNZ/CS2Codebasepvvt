using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Achievements;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Settings;
using Game.Tools;
using Game.UI.Localization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class EditorHierarchyUISystem : UISystemBase
{
	private enum CameraMode
	{
		Default,
		FirstPerson,
		Orbit
	}

	[BurstCompile]
	public struct ObjectHierarchyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferLookup<SubMesh> m_SubMeshes;

		[ReadOnly]
		public NativeParallelHashSet<ItemId> m_ExpandedIds;

		public NativeList<HierarchyItem> m_Hierarchy;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray2[i].m_Prefab;
				ItemId itemId = new ItemId
				{
					type = ItemType.Object,
					entity = nativeArray[i]
				};
				bool flag = m_SubMeshes.HasBuffer(prefab);
				bool flag2 = flag && m_ExpandedIds.Contains(itemId);
				ref NativeList<HierarchyItem> reference = ref m_Hierarchy;
				HierarchyItem hierarchyItem = new HierarchyItem
				{
					id = itemId,
					level = 1,
					expandable = flag,
					expanded = flag2,
					selectable = true
				};
				reference.Add(ref hierarchyItem);
				if (flag2 && m_SubMeshes.TryGetBuffer(prefab, ref val))
				{
					for (int j = 0; j < val.Length; j++)
					{
						ref NativeList<HierarchyItem> reference2 = ref m_Hierarchy;
						hierarchyItem = new HierarchyItem
						{
							id = new ItemId
							{
								type = ItemType.SubMesh,
								entity = nativeArray[i],
								subIndex = j
							},
							level = 2,
							selectable = true
						};
						reference2.Add(ref hierarchyItem);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public class PanelItem
	{
		public ItemType type;

		public byte level;

		public IEditorPanel panel;

		public PanelItem(ItemType type, byte level, IEditorPanel panel)
		{
			this.type = type;
			this.level = level;
			this.panel = panel;
		}
	}

	public class Viewport : IJsonWritable
	{
		public int startIndex;

		public List<ViewportItem> items = new List<ViewportItem>();

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("startIndex");
			writer.Write(startIndex);
			writer.PropertyName("items");
			JsonWriterExtensions.Write<ViewportItem>(writer, (IList<ViewportItem>)items);
			writer.TypeEnd();
		}
	}

	public struct ViewportItem : IJsonWritable
	{
		public ItemId id;

		public byte level;

		public bool expandable;

		public bool expanded;

		public LocalizedString name;

		public bool selectable;

		public bool saveable;

		public LocalizedString? tooltip;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			JsonWriterExtensions.Write<ItemId>(writer, id);
			writer.PropertyName("level");
			writer.Write((int)level);
			writer.PropertyName("expandable");
			writer.Write(expandable);
			writer.PropertyName("expanded");
			writer.Write(expanded);
			writer.PropertyName("name");
			JsonWriterExtensions.Write<LocalizedString>(writer, name);
			writer.PropertyName("selectable");
			writer.Write(selectable);
			writer.PropertyName("saveable");
			writer.Write(saveable);
			writer.PropertyName("tooltip");
			JsonWriterExtensions.Write<LocalizedString>(writer, tooltip);
			writer.TypeEnd();
		}

		public bool EqualsHierarchy(HierarchyItem other)
		{
			if (id == other.id && level == other.level && expandable == other.expandable && expanded == other.expanded)
			{
				return selectable == other.selectable;
			}
			return false;
		}
	}

	public struct HierarchyItem : IComparable<HierarchyItem>
	{
		public ItemId id;

		public byte level;

		public bool expandable;

		public bool expanded;

		public bool selectable;

		public int CompareTo(HierarchyItem other)
		{
			return id.CompareTo(other.id);
		}
	}

	public struct ItemId : IJsonWritable, IJsonReadable, IEquatable<ItemId>, IComparable<ItemId>
	{
		public ItemType type;

		public Entity entity;

		public int subIndex;

		public bool isContainer => type == ItemType.ObjectContainer;

		public ItemId(ItemType type, Entity entity = default(Entity), int subIndex = 0)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.type = type;
			this.entity = entity;
			this.subIndex = subIndex;
		}

		public void Write(IJsonWriter writer)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("type");
			writer.Write((int)type);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, entity);
			writer.PropertyName("subIndex");
			writer.Write(subIndex);
			writer.TypeEnd();
		}

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("type");
			int num = default(int);
			reader.Read(ref num);
			type = (ItemType)num;
			reader.ReadProperty("entity");
			UnityReaders.Read(reader, ref entity);
			reader.ReadProperty("subIndex");
			reader.Read(ref subIndex);
			reader.ReadMapEnd();
		}

		public bool Equals(ItemId other)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (type == other.type && ((Entity)(ref entity)).Equals(other.entity))
			{
				return subIndex == other.subIndex;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is ItemId other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((((int)type * 397) ^ ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref entity)/*cast due to .constrained prefix*/).GetHashCode()) * 397) ^ subIndex;
		}

		public static bool operator ==(ItemId left, ItemId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ItemId left, ItemId right)
		{
			return !left.Equals(right);
		}

		public int CompareTo(ItemId other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			int num = ((Entity)(ref entity)).CompareTo(other.entity);
			if (num != 0)
			{
				return num;
			}
			byte b = (byte)type;
			int num2 = b.CompareTo((byte)other.type);
			if (num2 != 0)
			{
				return num2;
			}
			return subIndex.CompareTo(other.subIndex);
		}
	}

	public enum ItemType : byte
	{
		None,
		Map,
		Climate,
		Water,
		Resources,
		ObjectContainer,
		Object,
		SubMesh
	}

	[NoAlias]
	[BurstCompile]
	private struct EditorHierarchyUISystem_4E004959_LambdaJob_0_Job : IJob
	{
		public NativeList<HierarchyItem> hierarchy;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OriginalLambdaBody()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			NativeSortExtension.Sort<HierarchyItem>(hierarchy);
		}

		public void Execute()
		{
			OriginalLambdaBody();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<SubMesh> __Game_Prefabs_SubMesh_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_SubMesh_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubMesh>(true);
		}
	}

	private const string kGroup = "editorHierarchy";

	public Action<Entity> onSave;

	public Action<Entity> onBulldoze;

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private EditorToolUISystem m_EditorToolUISystem;

	private EditorPanelUISystem m_EditorPanelUISystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_ModifiedQuery;

	private GetterValueBinding<Viewport> m_ViewportBinding;

	private NativeList<HierarchyItem> m_Hierarchy;

	private NativeParallelHashSet<ItemId> m_ExpandedIds;

	private int m_TotalCount;

	private ItemId m_SelectedId;

	private Viewport m_Viewport;

	private int m_NextViewportStartIndex;

	private int m_NextViewportEndIndex;

	private bool m_Dirty;

	private ValueBinding<int> m_CameraMode;

	private TypeHandle __TypeHandle;

	public override GameMode gameMode => GameMode.Editor;

	public List<PanelItem> panelItems { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_EditorToolUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorToolUISystem>();
		m_EditorPanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorPanelUISystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_ModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("editorHierarchy", "width", (Func<int>)GetWidth, (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("editorHierarchy", "totalCount", (Func<int>)(() => m_TotalCount), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<ItemId>("editorHierarchy", "selectedId", (Func<ItemId>)(() => m_SelectedId), (IWriter<ItemId>)(object)new ValueWriter<ItemId>(), (EqualityComparer<ItemId>)null));
		AddBinding((IBinding)(object)(m_ViewportBinding = new GetterValueBinding<Viewport>("editorHierarchy", "viewport", (Func<Viewport>)(() => m_Viewport), (IWriter<Viewport>)(object)new ValueWriter<Viewport>(), (EqualityComparer<Viewport>)null)));
		AddBinding((IBinding)(object)(m_CameraMode = new ValueBinding<int>("editorHierarchy", "cameraMode", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<int>("editorHierarchy", "setWidth", (Action<int>)SetWidth, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int, int>("editorHierarchy", "setViewportRange", (Action<int, int>)SetViewportRange, (IReader<int>)null, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<ItemId>("editorHierarchy", "setSelectedId", (Action<ItemId>)SetSelectedId, (IReader<ItemId>)(object)new ValueReader<ItemId>()));
		AddBinding((IBinding)(object)new TriggerBinding<ItemId, bool>("editorHierarchy", "setExpanded", (Action<ItemId, bool>)SetExpanded, (IReader<ItemId>)(object)new ValueReader<ItemId>(), (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("editorHierarchy", "toggleCameraMode", (Action<int>)delegate(int mode)
		{
			ToggleCameraMode((CameraMode)mode);
		}, (IReader<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("editorHierarchy", "save", (Action<Entity>)OnSave, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("editorHierarchy", "bulldoze", (Action<Entity>)OnBulldoze, (IReader<Entity>)null));
		panelItems = new List<PanelItem>
		{
			new PanelItem(ItemType.Map, 0, ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapPanelSystem>()),
			new PanelItem(ItemType.Climate, 1, ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimatePanelSystem>()),
			new PanelItem(ItemType.Water, 1, ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPanelSystem>()),
			new PanelItem(ItemType.Resources, 1, ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourcePanelSystem>())
		};
		m_Hierarchy = new NativeList<HierarchyItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_ExpandedIds = new NativeParallelHashSet<ItemId>(128, AllocatorHandle.op_Implicit((Allocator)4));
		m_Viewport = new Viewport();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Dirty = true;
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		m_ExpandedIds.Clear();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Hierarchy.Dispose();
		m_ExpandedIds.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = m_Dirty || !((EntityQuery)(ref m_ModifiedQuery)).IsEmptyIgnoreFilter;
		m_Dirty = false;
		m_TotalCount = m_Hierarchy.Length;
		UpdateSelection();
		base.OnUpdate();
		UpdateViewport(flag);
		if (flag)
		{
			UpdateHierarchy(m_Hierarchy);
		}
	}

	private void UpdateSelection()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (m_EditorPanelUISystem.activePanel == null)
		{
			if (!m_SelectedId.isContainer)
			{
				m_SelectedId = default(ItemId);
			}
			return;
		}
		if (m_ToolSystem.selected != Entity.Null)
		{
			if (m_SelectedId.entity != m_ToolSystem.selected)
			{
				m_SelectedId = new ItemId
				{
					type = ItemType.Object,
					entity = m_ToolSystem.selected
				};
				RefreshCameraController((CameraMode)m_CameraMode.value);
			}
			return;
		}
		PanelItem panelItem = panelItems.FirstOrDefault((PanelItem p) => p.type == m_SelectedId.type);
		if (m_EditorPanelUISystem.activePanel != panelItem?.panel)
		{
			PanelItem panelItem2 = panelItems.FirstOrDefault((PanelItem p) => p.panel == m_EditorPanelUISystem.activePanel);
			m_SelectedId = ((panelItem2 != null) ? new ItemId(panelItem2.type) : default(ItemId));
		}
	}

	private void UpdateViewport(bool force)
	{
		m_NextViewportStartIndex = math.clamp(m_NextViewportStartIndex, 0, m_Hierarchy.Length);
		m_NextViewportEndIndex = math.clamp(m_NextViewportEndIndex, 0, m_Hierarchy.Length);
		if (force || ViewportChanged())
		{
			m_Viewport.startIndex = m_NextViewportStartIndex;
			m_Viewport.items.Clear();
			for (int i = m_NextViewportStartIndex; i < m_NextViewportEndIndex; i++)
			{
				m_Viewport.items.Add(BuildViewportItem(m_Hierarchy[i]));
			}
			m_ViewportBinding.TriggerUpdate();
		}
	}

	private bool ViewportChanged()
	{
		if (m_NextViewportStartIndex != m_Viewport.startIndex || m_NextViewportEndIndex != m_Viewport.startIndex + m_Viewport.items.Count)
		{
			return true;
		}
		for (int i = 0; i < m_Viewport.items.Count; i++)
		{
			ViewportItem viewportItem = m_Viewport.items[i];
			int num = m_Viewport.startIndex + i;
			if (num >= m_Hierarchy.Length)
			{
				return true;
			}
			if (!viewportItem.EqualsHierarchy(m_Hierarchy[num]))
			{
				return true;
			}
		}
		return false;
	}

	private ViewportItem BuildViewportItem(HierarchyItem item)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef refData = default(PrefabRef);
		PrefabBase prefab;
		return new ViewportItem
		{
			id = item.id,
			level = item.level,
			expandable = item.expandable,
			expanded = item.expanded,
			name = GetName(item.id),
			tooltip = GetTooltip(item.id),
			selectable = item.selectable,
			saveable = (item.id.type == ItemType.Object && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, item.id.entity, ref refData) && m_PrefabSystem.TryGetPrefab<PrefabBase>(refData, out prefab) && !prefab.builtin)
		};
	}

	private LocalizedString GetName(ItemId id)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<SubMesh> val = default(DynamicBuffer<SubMesh>);
		PrefabBase prefab2;
		if (id.type == ItemType.Object)
		{
			PrefabRef refData = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, id.entity, ref refData) && m_PrefabSystem.TryGetPrefab<PrefabBase>(refData, out var prefab))
			{
				return LocalizedString.Value(((Object)prefab).name);
			}
		}
		else if (id.type == ItemType.SubMesh && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, id.entity, ref prefabRef) && EntitiesExtensions.TryGetBuffer<SubMesh>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val) && id.subIndex < val.Length && m_PrefabSystem.TryGetPrefab<PrefabBase>(val[id.subIndex].m_SubMesh, out prefab2))
		{
			return LocalizedString.Value(((Object)prefab2).name);
		}
		return "Editor." + id.type.ToString().ToUpper();
	}

	private LocalizedString GetTooltip(ItemId id)
	{
		return "Editor." + id.type.ToString().ToUpper() + "_TOOLTIP";
	}

	private void UpdateHierarchy(NativeList<HierarchyItem> hierarchy)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		hierarchy.Clear();
		foreach (PanelItem panelItem in panelItems)
		{
			HierarchyItem hierarchyItem = new HierarchyItem
			{
				id = new ItemId(panelItem.type),
				level = panelItem.level,
				selectable = true
			};
			hierarchy.Add(ref hierarchyItem);
		}
		if (!((EntityQuery)(ref m_ObjectQuery)).IsEmptyIgnoreFilter)
		{
			ItemId itemId = new ItemId(ItemType.ObjectContainer);
			bool num = m_ExpandedIds.Contains(itemId);
			HierarchyItem hierarchyItem = new HierarchyItem
			{
				id = itemId,
				level = 0,
				expandable = true,
				expanded = m_ExpandedIds.Contains(itemId),
				selectable = false
			};
			hierarchy.Add(ref hierarchyItem);
			if (num)
			{
				ObjectHierarchyJob objectHierarchyJob = new ObjectHierarchyJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SubMeshes = InternalCompilerInterface.GetBufferLookup<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ExpandedIds = m_ExpandedIds,
					m_Hierarchy = hierarchy
				};
				((SystemBase)this).Dependency = JobChunkExtensions.Schedule<ObjectHierarchyJob>(objectHierarchyJob, m_ObjectQuery, ((SystemBase)this).Dependency);
				((SystemBase)this).Dependency = EditorHierarchyUISystem_4E004959_LambdaJob_0_Execute(hierarchy, ((SystemBase)this).Dependency);
			}
		}
	}

	private void SetViewportRange(int startIndex, int endIndex)
	{
		m_NextViewportStartIndex = startIndex;
		m_NextViewportEndIndex = endIndex;
	}

	public void SetSelectedId(ItemId id)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		m_SelectedId = id;
		switch (id.type)
		{
		case ItemType.Object:
			m_ToolSystem.selected = id.entity;
			m_EditorToolUISystem.SelectEntity(id.entity);
			break;
		case ItemType.SubMesh:
			m_ToolSystem.selected = id.entity;
			m_EditorToolUISystem.SelectEntitySubMesh(id.entity, id.subIndex);
			break;
		default:
			m_ToolSystem.selected = Entity.Null;
			m_EditorPanelUISystem.activePanel = panelItems.FirstOrDefault((PanelItem p) => p.type == id.type)?.panel;
			break;
		}
		RefreshCameraController((CameraMode)m_CameraMode.value);
	}

	private void SetExpanded(ItemId id, bool expanded)
	{
		m_Dirty = true;
		if (expanded)
		{
			m_ExpandedIds.Add(id);
		}
		else
		{
			m_ExpandedIds.Remove(id);
		}
	}

	private void ToggleCameraMode(CameraMode cameraMode)
	{
		m_CameraMode.Update((int)cameraMode);
		RefreshCameraController(cameraMode);
	}

	private void RefreshCameraController(CameraMode mode)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (mode == CameraMode.Default || (mode == CameraMode.Orbit && m_SelectedId.entity == Entity.Null))
		{
			if (m_CameraUpdateSystem.activeCameraController != m_CameraUpdateSystem.gamePlayController)
			{
				m_CameraUpdateSystem.gamePlayController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
				m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.gamePlayController;
			}
			return;
		}
		switch (mode)
		{
		case CameraMode.Orbit:
			m_CameraUpdateSystem.orbitCameraController.followedEntity = m_SelectedId.entity;
			if (m_CameraUpdateSystem.activeCameraController != m_CameraUpdateSystem.orbitCameraController)
			{
				m_CameraUpdateSystem.orbitCameraController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
				m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.orbitCameraController;
			}
			break;
		case CameraMode.FirstPerson:
			if (m_CameraUpdateSystem.activeCameraController != m_CameraUpdateSystem.cinematicCameraController)
			{
				m_CameraUpdateSystem.cinematicCameraController.TryMatchPosition(m_CameraUpdateSystem.activeCameraController);
				m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.cinematicCameraController;
			}
			break;
		}
	}

	private int GetWidth()
	{
		return (SharedSettings.instance?.editor)?.hierarchyWidth ?? 350;
	}

	private void SetWidth(int width)
	{
		EditorSettings editorSettings = SharedSettings.instance?.editor;
		if (editorSettings != null)
		{
			editorSettings.hierarchyWidth = width;
		}
	}

	private void OnSave(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity);
		EditorPrefabUtils.SavePrefab(m_PrefabSystem.GetPrefab<PrefabBase>(componentData));
		PlatformManager.instance.UnlockAchievement(Game.Achievements.Achievements.IMadeThis);
		onSave?.Invoke(entity);
	}

	private void OnBulldoze(Entity entity)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		onBulldoze?.Invoke(entity);
	}

	private JobHandle EditorHierarchyUISystem_4E004959_LambdaJob_0_Execute(NativeList<HierarchyItem> hierarchy, JobHandle __inputDependency)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return IJobExtensions.Schedule<EditorHierarchyUISystem_4E004959_LambdaJob_0_Job>(new EditorHierarchyUISystem_4E004959_LambdaJob_0_Job
		{
			hierarchy = hierarchy
		}, __inputDependency);
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
	public EditorHierarchyUISystem()
	{
	}
}
