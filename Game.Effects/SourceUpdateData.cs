using Game.Objects;
using Unity.Collections;
using Unity.Entities;

namespace Game.Effects;

public struct SourceUpdateData
{
	private ParallelWriter<SourceUpdateInfo> m_SourceUpdateQueue;

	public SourceUpdateData(ParallelWriter<SourceUpdateInfo> sourceUpdateQueue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateQueue = sourceUpdateQueue;
	}

	public void Add(Entity entity, Transform transform)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = new SourceInfo(entity, -1),
			m_Type = SourceUpdateType.Add,
			m_Transform = transform
		});
	}

	public void Add(SourceInfo sourceInfo)
	{
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = sourceInfo,
			m_Type = SourceUpdateType.Add
		});
	}

	public void AddTemp(Entity prefab, Transform transform)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = new SourceInfo(prefab, -1),
			m_Type = SourceUpdateType.Temp,
			m_Transform = transform
		});
	}

	public void AddSnap()
	{
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_Type = SourceUpdateType.Snap
		});
	}

	public void Remove(Entity entity)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = new SourceInfo(entity, -1),
			m_Type = SourceUpdateType.Remove
		});
	}

	public void Remove(SourceInfo sourceInfo)
	{
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = sourceInfo,
			m_Type = SourceUpdateType.Remove
		});
	}

	public void WrongPrefab(SourceInfo sourceInfo)
	{
		m_SourceUpdateQueue.Enqueue(new SourceUpdateInfo
		{
			m_SourceInfo = sourceInfo,
			m_Type = SourceUpdateType.WrongPrefab
		});
	}
}
