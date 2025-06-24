using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colossal;
using Colossal.Annotations;
using Colossal.Logging;
using Colossal.Logging.Diagnostics;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine;

namespace Game.UI;

public static class ErrorDialogManager
{
	private static bool m_Initialized;

	private static bool m_Enabled = true;

	private static Queue<ErrorDialog> m_ErrorDialogs = new Queue<ErrorDialog>();

	private static float m_SimulationSpeed;

	public static bool enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			if (m_Enabled != value)
			{
				m_Enabled = value;
				if (!m_Enabled)
				{
					Clear();
				}
			}
		}
	}

	[CanBeNull]
	public static ErrorDialog currentErrorDialog
	{
		get
		{
			if (m_ErrorDialogs.Count == 0)
			{
				return null;
			}
			return m_ErrorDialogs.Peek();
		}
	}

	public static bool TryGetFirstError(out string error)
	{
		if (m_ErrorDialogs.Count > 0)
		{
			ErrorDialog errorDialog = m_ErrorDialogs.First();
			error = $"{errorDialog.localizedMessage}\n{errorDialog.errorDetails}";
			return true;
		}
		error = null;
		return false;
	}

	public static void DismissAllErrors()
	{
		while (m_ErrorDialogs.Count > 0)
		{
			DismissCurrentErrorDialog();
		}
	}

	public static void Initialize()
	{
		if (!m_Initialized)
		{
			UnityLogger.OnException += OnException;
			UnityLogger.OnWarnOrHigher += OnWarnOrHigher;
			m_Initialized = true;
		}
	}

	public static void Clear()
	{
		m_ErrorDialogs.Clear();
		m_SimulationSpeed = 0f;
	}

	public static void Dispose()
	{
		if (m_Initialized)
		{
			Clear();
			UnityLogger.OnException -= OnException;
			UnityLogger.OnWarnOrHigher -= OnWarnOrHigher;
			m_Initialized = false;
		}
	}

	private static void OnException(Exception e, Object context)
	{
		ShowErrorDialog(new ErrorDialog
		{
			severity = ErrorDialog.Severity.Error,
			localizedMessage = LocalizedString.Value(e.Message.Replace("\\", "\\\\")),
			errorDetails = GetErrorDetail(e, context),
			actions = GetActions()
		});
	}

	private static string GetErrorDetail(Exception e, Object context)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (context != (Object)null)
		{
			string text = $"{context.name} ({((object)context).GetType()})";
			string text2 = ((object)context).ToString();
			stringBuilder.AppendFormat("With object {0}", text);
			stringBuilder.AppendLine();
			if (text != text2)
			{
				stringBuilder.AppendFormat("Additional info: {0}", text2);
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
		}
		if (e != null)
		{
			StackTraceHelper.ExtractStackTraceFromException(e, stringBuilder);
			return StackTraceHelper.ExtractStackTrace(3, (Type)null, stringBuilder);
		}
		return StackTraceHelper.ExtractStackTrace(3, (Type)null, stringBuilder);
	}

	private static void OnWarnOrHigher(ILog log, Level level, string message, Exception e, Object context)
	{
		if ((log == null || log.showsErrorsInUI) && level >= Level.Error)
		{
			ShowErrorDialog(new ErrorDialog
			{
				severity = ((!(level == Level.Warn)) ? ErrorDialog.Severity.Error : ErrorDialog.Severity.Warning),
				localizedMessage = LocalizedString.Value(message.Replace("\\", "\\\\")),
				errorDetails = GetErrorDetail(e, context),
				actions = GetActions()
			});
		}
	}

	private static ErrorDialog.Actions GetActions()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GameManager.instance == (Object)null)
		{
			return ErrorDialog.Actions.Quit;
		}
		if (PlatformExt.IsPlatformSet((Platform)24, Application.platform, false))
		{
			if (!GameManager.instance.gameMode.IsGameOrEditor())
			{
				return ErrorDialog.Actions.None;
			}
			return ErrorDialog.Actions.SaveAndContinue;
		}
		if (!GameManager.instance.gameMode.IsGameOrEditor())
		{
			return ErrorDialog.Actions.Quit;
		}
		return ErrorDialog.Actions.Default;
	}

	public static void DismissCurrentErrorDialog()
	{
		if (m_ErrorDialogs.Count > 0)
		{
			m_ErrorDialogs.Dequeue();
		}
		RestorePause();
	}

	private static void HandlePause()
	{
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		SimulationSystem simulationSystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystem>() : null);
		if (simulationSystem != null)
		{
			if (m_SimulationSpeed == 0f)
			{
				m_SimulationSpeed = simulationSystem.selectedSpeed;
			}
			simulationSystem.selectedSpeed = 0f;
		}
	}

	private static void RestorePause()
	{
		if (m_ErrorDialogs.Count == 0)
		{
			World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
			SimulationSystem simulationSystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystem>() : null);
			if (simulationSystem != null)
			{
				simulationSystem.selectedSpeed = m_SimulationSpeed;
			}
		}
	}

	public static void DisplayDebugErrorDialog()
	{
		ShowErrorDialog(new ErrorDialog
		{
			severity = ErrorDialog.Severity.Error,
			localizedMessage = "Debug Error",
			errorDetails = "Debug details",
			actions = GetActions()
		});
	}

	public static void ShowErrorDialog(ErrorDialog e)
	{
		if (m_Enabled)
		{
			HandlePause();
			m_ErrorDialogs.Enqueue(e);
		}
	}
}
