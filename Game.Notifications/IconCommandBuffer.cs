using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Notifications;

public struct IconCommandBuffer
{
	public enum CommandFlags : byte
	{
		Add = 1,
		Remove = 2,
		Update = 4,
		Temp = 8,
		Hidden = 0x10,
		DisallowCluster = 0x20,
		All = 0x40
	}

	public struct Command : IComparable<Command>
	{
		public Entity m_Owner;

		public Entity m_Prefab;

		public Entity m_Target;

		public float3 m_Location;

		public CommandFlags m_CommandFlags;

		public IconPriority m_Priority;

		public IconClusterLayer m_ClusterLayer;

		public IconFlags m_Flags;

		public int m_BufferIndex;

		public float m_Delay;

		public int CompareTo(Command other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			int2 val = math.select(int2.op_Implicit(0), int2.op_Implicit(1), new bool2(m_Prefab != Entity.Null, other.m_Prefab != Entity.Null));
			return math.select(math.select(m_BufferIndex - other.m_BufferIndex, val.x - val.y, val.x != val.y), m_Owner.Index - other.m_Owner.Index, m_Owner.Index != other.m_Owner.Index);
		}
	}

	private ParallelWriter<Command> m_Commands;

	private int m_BufferIndex;

	public IconCommandBuffer(ParallelWriter<Command> commands, int bufferIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Commands = commands;
		m_BufferIndex = bufferIndex;
	}

	public void Add(Entity owner, Entity prefab, IconPriority priority = IconPriority.Info, IconClusterLayer clusterLayer = IconClusterLayer.Default, IconFlags flags = (IconFlags)0, Entity target = default(Entity), bool isTemp = false, bool isHidden = false, bool disallowCluster = false, float delay = 0f)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Commands.Enqueue(new Command
		{
			m_Owner = owner,
			m_Prefab = prefab,
			m_Target = target,
			m_CommandFlags = (CommandFlags)(1 | (isTemp ? 8 : 0) | (isHidden ? 16 : 0) | (disallowCluster ? 32 : 0)),
			m_Priority = priority,
			m_ClusterLayer = clusterLayer,
			m_Flags = (flags & ~IconFlags.CustomLocation),
			m_BufferIndex = m_BufferIndex,
			m_Delay = delay
		});
	}

	public void Add(Entity owner, Entity prefab, float3 location, IconPriority priority = IconPriority.Info, IconClusterLayer clusterLayer = IconClusterLayer.Default, IconFlags flags = IconFlags.IgnoreTarget, Entity target = default(Entity), bool isTemp = false, bool isHidden = false, bool disallowCluster = false, float delay = 0f)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_Commands.Enqueue(new Command
		{
			m_Owner = owner,
			m_Prefab = prefab,
			m_Target = target,
			m_Location = location,
			m_CommandFlags = (CommandFlags)(1 | (isTemp ? 8 : 0) | (isHidden ? 16 : 0) | (disallowCluster ? 32 : 0)),
			m_Priority = priority,
			m_ClusterLayer = clusterLayer,
			m_Flags = (flags | IconFlags.CustomLocation),
			m_BufferIndex = m_BufferIndex,
			m_Delay = delay
		});
	}

	public void Remove(Entity owner, Entity prefab, Entity target = default(Entity), IconFlags flags = (IconFlags)0)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		m_Commands.Enqueue(new Command
		{
			m_Owner = owner,
			m_Prefab = prefab,
			m_Target = target,
			m_CommandFlags = CommandFlags.Remove,
			m_Flags = flags,
			m_BufferIndex = m_BufferIndex
		});
	}

	public void Remove(Entity owner, IconPriority priority)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_Commands.Enqueue(new Command
		{
			m_Owner = owner,
			m_CommandFlags = (CommandFlags)66,
			m_Priority = priority,
			m_BufferIndex = m_BufferIndex
		});
	}

	public void Update(Entity owner)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_Commands.Enqueue(new Command
		{
			m_Owner = owner,
			m_CommandFlags = CommandFlags.Update,
			m_BufferIndex = m_BufferIndex
		});
	}
}
