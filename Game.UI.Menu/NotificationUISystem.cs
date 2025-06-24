using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.PSI;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

[CompilerGenerated]
public class NotificationUISystem : UISystemBase
{
	private class DelayedNotificationInfo
	{
		private NotificationInfo m_Notification;

		private float m_Delay;

		public DelayedNotificationInfo(NotificationInfo notification, float delay)
		{
			m_Notification = notification;
			m_Delay = delay;
		}

		public void Reset(float delay)
		{
			m_Delay = delay;
		}

		public bool Update(float deltaTime, out NotificationInfo notification)
		{
			notification = null;
			m_Delay -= deltaTime;
			if (m_Delay <= 0f)
			{
				notification = m_Notification;
				return true;
			}
			return false;
		}
	}

	public class NotificationInfo : IJsonWritable
	{
		public readonly string id;

		[CanBeNull]
		public string thumbnail;

		[CanBeNull]
		public LocalizedString? title;

		[CanBeNull]
		public LocalizedString? text;

		public ProgressState progressState;

		public int progress;

		public Action onClicked;

		public NotificationInfo(string id)
		{
			this.id = id;
		}

		public void Write(IJsonWriter writer)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected I4, but got Unknown
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("thumbnail");
			writer.Write(thumbnail);
			writer.PropertyName("title");
			JsonWriterExtensions.Write<LocalizedString>(writer, title);
			writer.PropertyName("text");
			JsonWriterExtensions.Write<LocalizedString>(writer, text);
			writer.PropertyName("progressState");
			writer.Write((int)progressState);
			writer.PropertyName("progress");
			writer.Write(progress);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "notification";

	private const string kInstallation = "installation";

	private const string kDownloading = "downloading";

	private const float kDelay = 2f;

	private ValueBinding<List<NotificationInfo>> m_NotificationsBinding;

	private Dictionary<string, DelayedNotificationInfo> m_PendingRemoval;

	private Dictionary<string, NotificationInfo> m_NotificationsMap;

	private Dictionary<int, Mod> m_ModInfoCache;

	private bool m_Dirty;

	public static int width
	{
		get
		{
			float num = (float)Screen.width / 1920f;
			return Mathf.CeilToInt(48f * num);
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		NotificationSystem.BindUI(this);
		base.OnCreate();
		m_NotificationsMap = new Dictionary<string, NotificationInfo>();
		m_PendingRemoval = new Dictionary<string, DelayedNotificationInfo>();
		m_ModInfoCache = new Dictionary<int, Mod>();
		AddBinding((IBinding)(object)(m_NotificationsBinding = new ValueBinding<List<NotificationInfo>>("notification", "notifications", new List<NotificationInfo>(), (IWriter<List<NotificationInfo>>)(object)new ListWriter<NotificationInfo>((IWriter<NotificationInfo>)(object)new ValueWriter<NotificationInfo>()), (EqualityComparer<List<NotificationInfo>>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<string>("notification", "selectNotification", (Action<string>)SelectNotification, (IReader<string>)null));
		PlatformManager.instance.onModSubscriptionChanged += new ModSubscriptionEventHandler(HandleModSubscription);
		PlatformManager.instance.onModDownloadStarted += new ModEventHandler(HandleModDownloadStarted);
		PlatformManager.instance.onModDownloadCompleted += new ModEventHandler(HandleModDownloadCompleted);
		PlatformManager.instance.onModDownloadFailed += new ModEventHandler(HandleModDownloadFailed);
		PlatformManager.instance.onModInstallProgress += new ModInstallProgressEventHandler(HandleModInstallProgress);
		PlatformManager.instance.onTransferOnGoing += new TransferEventHandler(HandleTransferOnGoing);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		NotificationSystem.UnbindUI();
	}

	public static string GetTitle(string titleId)
	{
		return "Menu.NOTIFICATION_TITLE[" + titleId + "]";
	}

	public static string GetText(string textId)
	{
		return "Menu.NOTIFICATION_DESCRIPTION[" + textId + "]";
	}

	private void AddModNotification(Mod mod, string notificationId = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string identifier = notificationId ?? GetModNotificationId(mod);
		LocalizedString? title = LocalizedString.Value(mod.displayName);
		string thumbnail = $"{mod.thumbnailPath}?width={width})";
		Action onClick = mod.onClick;
		AddOrUpdateNotification(identifier, title, null, thumbnail, null, null, onClick);
	}

	private string GetModNotificationId(Mod mod, string suffix = null)
	{
		return GetModNotificationId(mod.id.ToString(), suffix);
	}

	private string GetModNotificationId(string modId, string suffix = null)
	{
		if (!string.IsNullOrEmpty(suffix))
		{
			return modId + "." + suffix;
		}
		return modId;
	}

	private unsafe void HandleModSubscription(IModSupport psi, Mod mod, ModSubscriptionStatus status)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)(*(ModSubscriptionStatus*)(&status))/*cast due to .constrained prefix*/).ToString();
		string modNotificationId = GetModNotificationId(mod, text);
		LocalizedString? title = LocalizedString.Value(mod.displayName);
		LocalizedString? text2 = GetText(text);
		string thumbnail = $"{mod.thumbnailPath}?width={width})";
		Action onClick = mod.onClick;
		RemoveNotification(modNotificationId, 2f, title, text2, thumbnail, null, null, onClick);
	}

	private void HandleModDownloadStarted(IModSupport psi, Mod mod)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_ModInfoCache[mod.id] = mod;
		AddModNotification(mod, GetModNotificationId(mod, "installation"));
		string modNotificationId = GetModNotificationId(mod, "installation");
		LocalizedString? text = GetText("DownloadPending");
		int? progress = 0;
		AddOrUpdateNotification(modNotificationId, null, text, null, null, progress);
	}

	private void HandleModDownloadCompleted(IModSupport psi, Mod mod)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_ModInfoCache.Remove(mod.id);
		string modNotificationId = GetModNotificationId(mod, "installation");
		LocalizedString? text = GetText("InstallComplete");
		ProgressState? progressState = (ProgressState)3;
		int? progress = 100;
		RemoveNotification(modNotificationId, 2f, null, text, null, progressState, progress);
	}

	private void HandleModDownloadFailed(IModSupport psi, Mod mod)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_ModInfoCache.Remove(mod.id);
		string modNotificationId = GetModNotificationId(mod, "installation");
		LocalizedString? text = GetText("InstallFailed");
		ProgressState? progressState = (ProgressState)4;
		int? progress = 100;
		RemoveNotification(modNotificationId, 2f, null, text, null, progressState, progress);
	}

	private void HandleModInstallProgress(IModSupport psi, int modId, TransferStatus status)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		ProgressState val = (ProgressState)(((int)status.type != 2) ? 1 : ((int)status.progressState));
		string modNotificationId = GetModNotificationId(status.id, "installation");
		LocalizedString? text = GetText($"{(object)(TransferType)2}{val}");
		ProgressState? progressState = val;
		int? progress = Mathf.CeilToInt(status.progress * 100f);
		AddOrUpdateNotification(modNotificationId, null, text, null, progressState, progress);
		if ((int)status.type != 0)
		{
			return;
		}
		if ((int)status.progressState == 1 && !NotificationExists(GetModNotificationId(status.id, "downloading")))
		{
			if (!m_ModInfoCache.TryGetValue(modId, out var value))
			{
				value = new Mod
				{
					id = modId
				};
			}
			AddModNotification(value, GetModNotificationId(value, "downloading"));
		}
		else if ((int)status.progressState == 3 && NotificationExists(GetModNotificationId(status.id, "downloading")))
		{
			string modNotificationId2 = GetModNotificationId(status.id, "downloading");
			text = GetText("DownloadComplete");
			progressState = (ProgressState)3;
			progress = 100;
			RemoveNotification(modNotificationId2, 2f, null, text, null, progressState, progress);
		}
	}

	private void HandleTransferOnGoing(ITransferSupport psi, TransferStatus status)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (NotificationExists(GetModNotificationId(status.id, "downloading")))
		{
			if ((int)status.progressState == 3)
			{
				string modNotificationId = GetModNotificationId(status.id, "downloading");
				LocalizedString? text = GetText("DownloadComplete");
				ProgressState? progressState = (ProgressState)3;
				int? progress = 100;
				RemoveNotification(modNotificationId, 2f, null, text, null, progressState, progress);
			}
			else if ((int)status.progressState == 4)
			{
				string modNotificationId2 = GetModNotificationId(status.id, "downloading");
				LocalizedString? text = GetText("DownloadFailed");
				ProgressState? progressState = (ProgressState)4;
				int? progress = 100;
				RemoveNotification(modNotificationId2, 2f, null, text, null, progressState, progress);
			}
			else
			{
				string modNotificationId3 = GetModNotificationId(status.id, "downloading");
				LocalizedString? text = GetText("DownloadProgressing");
				ProgressState? progressState = status.progressState;
				int? progress = Mathf.CeilToInt(status.progress * 100f);
				AddOrUpdateNotification(modNotificationId3, null, text, null, progressState, progress);
			}
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
		ProcessPendingRemovals(((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime);
		if (m_Dirty)
		{
			m_Dirty = false;
			m_NotificationsBinding.TriggerUpdate();
		}
	}

	private void SelectNotification(string notificationId)
	{
		if (m_NotificationsMap.TryGetValue(notificationId, out var value))
		{
			value.onClicked?.Invoke();
		}
	}

	private void UpdateNotification(NotificationInfo notificationInfo, LocalizedString? title, LocalizedString? text, string thumbnail, ProgressState? progressState, int? progress, Action onClicked)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (title.HasValue && !notificationInfo.title.HasValue)
		{
			notificationInfo.title = title;
		}
		if (text.HasValue)
		{
			notificationInfo.text = text;
		}
		if (thumbnail != null && notificationInfo.thumbnail == null)
		{
			notificationInfo.thumbnail = thumbnail;
		}
		if (progressState.HasValue)
		{
			notificationInfo.progressState = progressState.Value;
		}
		if (progress.HasValue)
		{
			notificationInfo.progress = progress.Value;
		}
		if (onClicked != null && notificationInfo.onClicked == null)
		{
			notificationInfo.onClicked = onClicked;
		}
	}

	public NotificationInfo AddOrUpdateNotification(string identifier, LocalizedString? title = null, LocalizedString? text = null, string thumbnail = null, ProgressState? progressState = null, int? progress = null, Action onClicked = null)
	{
		if (m_NotificationsMap.TryGetValue(identifier, out var value))
		{
			UpdateNotification(value, title, text, thumbnail, progressState, progress, onClicked);
		}
		else
		{
			value = new NotificationInfo(identifier);
			UpdateNotification(value, title, text, thumbnail, progressState, progress, onClicked);
			m_NotificationsMap.Add(identifier, value);
			m_NotificationsBinding.value.Add(value);
		}
		m_Dirty = true;
		return value;
	}

	public void RemoveNotification(string identifier, float delay = 0f, LocalizedString? title = null, LocalizedString? text = null, string thumbnail = null, ProgressState? progressState = null, int? progress = null, Action onClicked = null)
	{
		NotificationInfo notificationInfo = AddOrUpdateNotification(identifier, title, text, thumbnail, progressState, progress, onClicked);
		DelayedNotificationInfo value;
		if (delay == 0f)
		{
			m_NotificationsBinding.value.Remove(notificationInfo);
			m_NotificationsMap.Remove(notificationInfo.id);
		}
		else if (m_PendingRemoval.TryGetValue(identifier, out value))
		{
			value.Reset(delay);
		}
		else
		{
			m_PendingRemoval.Add(identifier, new DelayedNotificationInfo(notificationInfo, delay));
		}
		m_Dirty = true;
	}

	public bool NotificationExists(string identifier)
	{
		return m_NotificationsMap.ContainsKey(identifier);
	}

	private void ProcessPendingRemovals(float deltaTime)
	{
		List<NotificationInfo> list = null;
		foreach (KeyValuePair<string, DelayedNotificationInfo> item in m_PendingRemoval)
		{
			if (item.Value.Update(deltaTime, out var notification))
			{
				list = new List<NotificationInfo> { notification };
				m_Dirty = true;
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (NotificationInfo item2 in list)
		{
			m_NotificationsBinding.value.Remove(item2);
			m_NotificationsMap.Remove(item2.id);
			m_PendingRemoval.Remove(item2.id);
		}
	}

	[Preserve]
	public NotificationUISystem()
	{
	}
}
