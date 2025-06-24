using System.IO;
using System.Threading.Tasks;
using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class SaveGameSystem : GameSystemBase
{
	private TaskCompletionSource<bool> m_TaskCompletionSource;

	private UpdateSystem m_UpdateSystem;

	private WriteSystem m_WriteSystem;

	private bool m_Writing;

	private Context m_Context;

	public Stream stream { get; set; }

	public Context context
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Context;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			((Context)(ref m_Context)).Dispose();
			m_Context = value;
		}
	}

	public NativeArray<Entity> referencedContent { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_WriteSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WriteSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((Context)(ref m_Context)).Dispose();
		if (referencedContent.IsCreated)
		{
			referencedContent.Dispose();
		}
		base.OnDestroy();
	}

	public async Task RunOnce()
	{
		m_TaskCompletionSource = new TaskCompletionSource<bool>();
		((ComponentSystemBase)this).Enabled = true;
		await m_TaskCompletionSource.Task;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (m_Writing)
		{
			JobHandle writeDependency = m_WriteSystem.writeDependency;
			if (((JobHandle)(ref writeDependency)).IsCompleted)
			{
				writeDependency = m_WriteSystem.writeDependency;
				((JobHandle)(ref writeDependency)).Complete();
				m_Writing = false;
				((ComponentSystemBase)this).Enabled = false;
				m_TaskCompletionSource?.SetResult(result: true);
			}
		}
		else
		{
			m_Writing = true;
			m_UpdateSystem.Update(SystemUpdatePhase.Serialize);
		}
	}

	[Preserve]
	public SaveGameSystem()
	{
	}
}
