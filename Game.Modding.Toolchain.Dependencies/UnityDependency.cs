using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Colossal;
using Game.Settings;
using Game.UI.Localization;
using Microsoft.Win32;
using UnityEngine;

namespace Game.Modding.Toolchain.Dependencies;

public class UnityDependency : BaseDependency
{
	public const string kUnityInstallerUrl = "https://download.unity3d.com/download_unity/5f63fdee6d95/Windows64EditorInstaller/UnitySetup64-2022.3.60f1.exe";

	public static readonly string sUnityVersion = Application.unityVersion;

	public static readonly string kDefaultInstallationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

	public static readonly string kInstallationFolder = "Unity";

	private static string sUnityPath;

	private long? m_DownloadSize;

	private string m_InstallationDirectory;

	public override string name => "Unity editor";

	public override string icon => "Media/Toolchain/Unity.svg";

	public override string version
	{
		get
		{
			return sUnityVersion;
		}
		protected set
		{
		}
	}

	public static string unityPath
	{
		get
		{
			string path = "SOFTWARE\\Unity Technologies\\Installer\\Unity " + sUnityVersion + "\\";
			string path2 = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Unity " + sUnityVersion + "\\";
			int depth;
			if (TryGetRegistryKeyValue(Registry.CurrentUser, path, "Location x64", out var value))
			{
				depth = 0;
			}
			else if (TryGetRegistryKeyValue(Registry.LocalMachine, path, "Location x64", out value))
			{
				depth = 0;
			}
			else if (TryGetRegistryKeyValue(Registry.LocalMachine, path2, "UninstallString", out value))
			{
				depth = 2;
			}
			else
			{
				if (!TryGetRegistryKeyValue(Registry.LocalMachine, path2, "DisplayIcon", out value))
				{
					return null;
				}
				depth = 2;
			}
			if (TryGetParentPath(value, depth, out var parentPath))
			{
				return parentPath;
			}
			return null;
		}
	}

	public static string unityExe
	{
		get
		{
			if (unityPath != null)
			{
				return Path.Combine(unityPath, "Editor", "Unity.exe");
			}
			return null;
		}
	}

	public static string unityUninstallerExe
	{
		get
		{
			if (sUnityPath != null)
			{
				return Path.Combine(sUnityPath, "Editor", "Uninstall.exe");
			}
			return null;
		}
	}

	public string installerPath => Path.Combine(Path.GetFullPath(SharedSettings.instance.modding.downloadDirectory), Path.GetFileName("https://download.unity3d.com/download_unity/5f63fdee6d95/Windows64EditorInstaller/UnitySetup64-2022.3.60f1.exe"));

	public override string installationDirectory
	{
		get
		{
			return m_InstallationDirectory;
		}
		set
		{
			m_InstallationDirectory = Path.GetFullPath(Path.Combine(value, kInstallationFolder));
		}
	}

	public override bool canChangeInstallationDirectory => true;

	public override bool confirmUninstallation => true;

	public override LocalizedString installDescr => new LocalizedString("Options.WARN_TOOLCHAIN_INSTALL_UNITY", null, new Dictionary<string, ILocElement>
	{
		{
			"UNITY_VERSION",
			LocalizedString.Value(sUnityVersion)
		},
		{
			"HOST",
			LocalizedString.Value(new Uri("https://download.unity3d.com/download_unity/5f63fdee6d95/Windows64EditorInstaller/UnitySetup64-2022.3.60f1.exe").Host)
		}
	});

	public override LocalizedString uninstallMessage => new LocalizedString("Options.WARN_TOOLCHAIN_UNITY_UNINSTALL", null, new Dictionary<string, ILocElement> { 
	{
		"UNITY_VERSION",
		LocalizedString.Value(sUnityVersion)
	} });

	public UnityDependency()
	{
		sUnityPath = null;
		installationDirectory = kDefaultInstallationDirectory;
	}

	public override Task<bool> IsInstalled(CancellationToken token)
	{
		return Task.FromResult(unityExe != null && LongFile.Exists(unityExe));
	}

	public override Task<bool> IsUpToDate(CancellationToken token)
	{
		return IsInstalled(token);
	}

	public override async Task<bool> NeedDownload(CancellationToken token)
	{
		FileInfo installerFile = new FileInfo(installerPath);
		if (!installerFile.Exists)
		{
			return true;
		}
		long num = await GetUnityInstallerSize(token).ConfigureAwait(continueOnCapturedContext: false);
		if (installerFile.Length != num)
		{
			await AsyncUtils.DeleteFileAsync(installerPath, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
			return true;
		}
		return false;
	}

	private async Task<long> GetUnityInstallerSize(CancellationToken token)
	{
		m_DownloadSize.GetValueOrDefault();
		if (!m_DownloadSize.HasValue)
		{
			m_DownloadSize = await IToolchainDependency.GetDownloadSizeAsync("https://download.unity3d.com/download_unity/5f63fdee6d95/Windows64EditorInstaller/UnitySetup64-2022.3.60f1.exe", token).ConfigureAwait(continueOnCapturedContext: false);
		}
		return m_DownloadSize.Value;
	}

	public override async Task<List<IToolchainDependency.DiskSpaceRequirements>> GetRequiredDiskSpace(CancellationToken token)
	{
		List<IToolchainDependency.DiskSpaceRequirements> requests = new List<IToolchainDependency.DiskSpaceRequirements>();
		if (!(await IsInstalled(token).ConfigureAwait(continueOnCapturedContext: false)))
		{
			requests.Add(new IToolchainDependency.DiskSpaceRequirements
			{
				m_Path = installationDirectory,
				m_Size = 6442450944L
			});
			if (await NeedDownload(token).ConfigureAwait(continueOnCapturedContext: false))
			{
				List<IToolchainDependency.DiskSpaceRequirements> list = requests;
				list.Add(new IToolchainDependency.DiskSpaceRequirements
				{
					m_Path = installerPath,
					m_Size = await GetUnityInstallerSize(token).ConfigureAwait(continueOnCapturedContext: false)
				});
			}
		}
		return requests;
	}

	public override Task Download(CancellationToken token)
	{
		return BaseDependency.Download(this, token, "https://download.unity3d.com/download_unity/5f63fdee6d95/Windows64EditorInstaller/UnitySetup64-2022.3.60f1.exe", installerPath, "DownloadingUnity");
	}

	public override async Task Install(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		string path = installerPath;
		try
		{
			IToolchainDependency.log.Debug((object)"Installing Unity");
			base.state = new IToolchainDependency.State(DependencyState.Installing, "InstallingUnity");
			await Cli.Wrap(path).WithArguments("/S").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
			{
				IToolchainDependency.log.Debug((object)l);
			}))
				.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					IToolchainDependency.log.Error((object)l);
				}))
				.WithUseShellExecute(true)
				.ExecuteAsync(token)
				.ConfigureAwait(false);
		}
		catch (ToolchainException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new ToolchainException(ToolchainError.Install, this, innerException);
		}
		finally
		{
			await AsyncUtils.DeleteFileAsync(path, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public override async Task Uninstall(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		try
		{
			IToolchainDependency.log.Debug((object)"Uninstalling Unity");
			base.state = new IToolchainDependency.State(DependencyState.Removing, "RemovingUnity");
			string text = unityUninstallerExe;
			if (text != null)
			{
				await Cli.Wrap(text).WithArguments("/S /D=" + Path.Combine(installationDirectory, kInstallationFolder)).WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					IToolchainDependency.log.Debug((object)l);
				}))
					.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
					{
						IToolchainDependency.log.Error((object)l);
					}))
					.WithUseShellExecute(true)
					.ExecuteAsync(token)
					.ConfigureAwait(false);
			}
			string unityPath = UnityDependency.unityPath;
			if (unityPath != null)
			{
				await AsyncUtils.WaitForAction((Func<bool>)(() => !LongDirectory.Exists(unityPath)), token, 100).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (ToolchainException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new ToolchainException(ToolchainError.Uninstall, this, innerException);
		}
	}

	private static bool TryGetRegistryKeyValue(RegistryKey registry, string path, string key, out string value)
	{
		value = null;
		RegistryKey registryKey = null;
		try
		{
			registryKey = registry.OpenSubKey(path);
			if (registryKey != null)
			{
				object value2 = registryKey.GetValue(key);
				if (value2 != null)
				{
					value = value2.ToString();
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			IToolchainDependency.log.Error(ex, (object)("Failed checking registry key " + registry.Name + "\\" + path + key));
		}
		finally
		{
			registryKey?.Dispose();
		}
		return false;
	}

	private static bool TryGetParentPath(string path, int depth, out string parentPath)
	{
		parentPath = null;
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		for (int i = 0; i < depth; i++)
		{
			directoryInfo = directoryInfo.Parent;
			if (directoryInfo == null)
			{
				return false;
			}
		}
		if (directoryInfo.Exists)
		{
			parentPath = directoryInfo.FullName;
			return true;
		}
		return false;
	}
}
