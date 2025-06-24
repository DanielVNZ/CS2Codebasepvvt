using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class WindTextureSystem : GameSystemBase
{
	[BurstCompile]
	private struct WindTextureJob : IJobFor
	{
		[ReadOnly]
		public NativeArray<Wind> m_WindMap;

		public NativeArray<float2> m_WindTexture;

		public void Execute(int index)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			m_WindTexture[index] = m_WindMap[index].m_Wind;
		}
	}

	private WindSystem m_WindSystem;

	private Texture2D m_WindTexture;

	private JobHandle m_UpdateHandle;

	private bool m_RequireUpdate;

	private bool m_RequireApply;

	public Texture2D WindTexture => m_WindTexture;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		base.OnCreate();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_WindTexture = new Texture2D(WindSystem.kTextureSize, WindSystem.kTextureSize, (TextureFormat)19, false, true)
		{
			name = "WindTexture",
			hideFlags = (HideFlags)61
		};
	}

	public void RequireUpdate()
	{
		m_RequireUpdate = true;
	}

	public void CompleteUpdate()
	{
		if (m_RequireApply)
		{
			m_RequireApply = false;
			((JobHandle)(ref m_UpdateHandle)).Complete();
			m_WindTexture.Apply();
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (m_RequireUpdate)
		{
			m_RequireUpdate = false;
			m_RequireApply = true;
			JobHandle dependencies;
			WindTextureJob windTextureJob = new WindTextureJob
			{
				m_WindMap = m_WindSystem.GetMap(readOnly: true, out dependencies),
				m_WindTexture = m_WindTexture.GetRawTextureData<float2>()
			};
			m_UpdateHandle = IJobForExtensions.Schedule<WindTextureJob>(windTextureJob, WindSystem.kTextureSize * WindSystem.kTextureSize, dependencies);
			m_WindSystem.AddReader(m_UpdateHandle);
		}
	}

	[Preserve]
	public WindTextureSystem()
	{
	}
}
