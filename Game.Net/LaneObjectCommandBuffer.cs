using Colossal.Collections;
using Colossal.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct LaneObjectCommandBuffer
{
	private Writer<LaneObjectAction> m_LaneActionQueue;

	private ParallelWriter<TreeObjectAction> m_TreeActionQueue;

	public LaneObjectCommandBuffer(Writer<LaneObjectAction> laneActionQueue, ParallelWriter<TreeObjectAction> treeActionQueue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_LaneActionQueue = laneActionQueue;
		m_TreeActionQueue = treeActionQueue;
	}

	public void Remove(Entity lane, Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_LaneActionQueue.Enqueue(new LaneObjectAction(lane, entity));
	}

	public void Remove(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TreeActionQueue.Enqueue(new TreeObjectAction(entity));
	}

	public void Add(Entity lane, Entity entity, float2 curvePosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_LaneActionQueue.Enqueue(new LaneObjectAction(lane, entity, curvePosition));
	}

	public void Add(Entity entity, Bounds3 bounds)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_TreeActionQueue.Enqueue(new TreeObjectAction(entity, bounds));
	}

	public void Update(Entity lane, Entity entity, float2 curvePosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_LaneActionQueue.Enqueue(new LaneObjectAction(lane, entity, entity, curvePosition));
	}

	public void Update(Entity entity, Bounds3 bounds)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_TreeActionQueue.Enqueue(new TreeObjectAction(entity, entity, bounds));
	}
}
