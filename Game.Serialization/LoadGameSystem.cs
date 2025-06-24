using System.Threading.Tasks;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class LoadGameSystem : GameSystemBase
{
	public delegate void EventGameLoaded(Context serializationContext);

	public EventGameLoaded onOnSaveGameLoaded;

	private TaskCompletionSource<bool> m_TaskCompletionSource;

	private UpdateSystem m_UpdateSystem;

	private Context m_Context;

	public AsyncReadDescriptor dataDescriptor { get; set; }

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

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		((ComponentSystemBase)this).Enabled = false;
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		m_UpdateSystem.Update(SystemUpdatePhase.Deserialize);
		((ComponentSystemBase)this).Enabled = false;
		onOnSaveGameLoaded?.Invoke(context);
		m_TaskCompletionSource?.SetResult(result: true);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((Context)(ref m_Context)).Dispose();
		onOnSaveGameLoaded = null;
		base.OnDestroy();
	}

	[Preserve]
	public LoadGameSystem()
	{
	}
}
