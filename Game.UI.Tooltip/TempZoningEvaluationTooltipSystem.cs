using System.Collections.Generic;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

public class TempZoningEvaluationTooltipSystem : TooltipSystemBase
{
	private IZoningInfoSystem m_ZoningInfoSystem;

	private List<ZoningEvaluationTooltip> m_Tooltips;

	public int maxCount { get; set; } = 5;

	public float scoreThreshold { get; set; } = 10f;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ZoningInfoSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoningInfoSystem>();
		m_Tooltips = new List<ZoningEvaluationTooltip>(maxCount);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		NativeList<ZoneEvaluationUtils.ZoningEvaluationResult> evaluationResults = m_ZoningInfoSystem.evaluationResults;
		for (int i = 0; i < math.min(evaluationResults.Length, maxCount); i++)
		{
			if (m_Tooltips.Count <= i)
			{
				m_Tooltips.Add(new ZoningEvaluationTooltip
				{
					path = $"zoningEvaluation{i}"
				});
			}
			ZoneEvaluationUtils.ZoningEvaluationResult zoningEvaluationResult = evaluationResults[i];
			if (Mathf.Abs(zoningEvaluationResult.m_Score) > scoreThreshold)
			{
				ZoningEvaluationTooltip zoningEvaluationTooltip = m_Tooltips[i];
				zoningEvaluationTooltip.factor = zoningEvaluationResult.m_Factor;
				zoningEvaluationTooltip.score = zoningEvaluationResult.m_Score;
				AddMouseTooltip(zoningEvaluationTooltip);
			}
		}
	}

	[Preserve]
	public TempZoningEvaluationTooltipSystem()
	{
	}
}
