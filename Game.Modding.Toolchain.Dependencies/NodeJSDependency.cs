using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Exceptions;
using Colossal;
using Game.SceneFlow;
using Game.Settings;
using Game.UI.Localization;

namespace Game.Modding.Toolchain.Dependencies;

public class NodeJSDependency : BaseDependency
{
	public static readonly string kNodeJSVersion = "20.11.0";

	public static readonly string kMinNodeJSVersion = "18.0";

	public static readonly string kNodeJSInstallerUrl = "https://nodejs.org/dist/v" + kNodeJSVersion + "/node-v" + kNodeJSVersion + "-" + RuntimeInformation.OSArchitecture.ToString().ToLower() + ".msi";

	public static readonly string kDefaultInstallationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

	public static readonly string kInstallationFolder = "nodejs";

	private long? m_DownloadSize;

	private string m_InstallationDirectory;

	public override string name => "Node.js";

	public override string icon => "Media/Toolchain/NodeJS.svg";

	public override bool confirmUninstallation => true;

	public string installerPath => Path.Combine(Path.GetFullPath(SharedSettings.instance.modding.downloadDirectory), Path.GetFileName(kNodeJSInstallerUrl));

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

	public override LocalizedString installDescr => new LocalizedString("Options.WARN_TOOLCHAIN_INSTALL_NODEJS", null, new Dictionary<string, ILocElement>
	{
		{
			"NODEJS_VERSION",
			LocalizedString.Value(kNodeJSVersion)
		},
		{
			"HOST",
			LocalizedString.Value(new Uri(kNodeJSInstallerUrl).Host)
		}
	});

	public override LocalizedString uninstallMessage => new LocalizedString("Options.WARN_TOOLCHAIN_NODEJS_UNINSTALL", null, new Dictionary<string, ILocElement> { 
	{
		"NODEJS_VERSION",
		LocalizedString.Value(kNodeJSVersion)
	} });

	public override string version
	{
		get
		{
			if (base.version == null)
			{
				Task.Run(async () => await GetNodeVersion(GameManager.instance.terminationToken)).Wait();
			}
			return base.version;
		}
		protected set
		{
			base.version = value;
		}
	}

	public NodeJSDependency()
	{
		installationDirectory = kDefaultInstallationDirectory;
	}

	public override async Task<bool> IsInstalled(CancellationToken token)
	{
		return !string.IsNullOrEmpty(await GetNodeVersion(token).ConfigureAwait(continueOnCapturedContext: false));
	}

	public override async Task<bool> IsUpToDate(CancellationToken token)
	{
		string input = await GetNodeVersion(token).ConfigureAwait(continueOnCapturedContext: false);
		if (System.Version.TryParse(kMinNodeJSVersion, out var result) && System.Version.TryParse(input, out var result2))
		{
			return result2 >= result;
		}
		return false;
	}

	public override async Task<bool> NeedDownload(CancellationToken token)
	{
		FileInfo installerFile = new FileInfo(installerPath);
		if (!installerFile.Exists)
		{
			return true;
		}
		long num = await GetDotNetInstallerSize(token).ConfigureAwait(continueOnCapturedContext: false);
		if (installerFile.Length != num)
		{
			await AsyncUtils.DeleteFileAsync(installerPath, token, 100, 5000).ConfigureAwait(continueOnCapturedContext: false);
			return true;
		}
		return false;
	}

	private async Task<long> GetDotNetInstallerSize(CancellationToken token)
	{
		m_DownloadSize.GetValueOrDefault();
		if (!m_DownloadSize.HasValue)
		{
			m_DownloadSize = await IToolchainDependency.GetDownloadSizeAsync(kNodeJSInstallerUrl, token).ConfigureAwait(continueOnCapturedContext: false);
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
				m_Size = 104857600L
			});
			if (await NeedDownload(token).ConfigureAwait(continueOnCapturedContext: false))
			{
				List<IToolchainDependency.DiskSpaceRequirements> list = requests;
				list.Add(new IToolchainDependency.DiskSpaceRequirements
				{
					m_Path = installerPath,
					m_Size = await GetDotNetInstallerSize(token).ConfigureAwait(continueOnCapturedContext: false)
				});
			}
		}
		return requests;
	}

	public override Task Download(CancellationToken token)
	{
		return BaseDependency.Download(this, token, kNodeJSInstallerUrl, installerPath, "DownloadingNodeJS");
	}

	public override async Task Install(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		string path = installerPath;
		try
		{
			IToolchainDependency.log.DebugFormat("Installing {0}", (object)name);
			base.state = new IToolchainDependency.State(DependencyState.Installing, "InstallingNodeJS");
			await Cli.Wrap("msiexec").WithArguments("/i \"" + path + "\" /passive /norestart INSTALLDIR=\"" + installationDirectory + "\" ").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
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
			IToolchainDependency.UpdateProcessEnvVarPathValue();
		}
		catch (ToolchainException)
		{
			throw;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (CommandExecutionException ex3)
		{
			CommandExecutionException ex4 = ex3;
			if (ex4.ExitCode == 1602 || ex4.ExitCode == 1603)
			{
				throw new ToolchainException(ToolchainError.Install, this, "Installation canceled by user");
			}
			throw new ToolchainException(ToolchainError.Install, this, (Exception)(object)ex4);
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
		try
		{
			IToolchainDependency.log.DebugFormat("Uninstalling {0}", (object)name);
			base.state = new IToolchainDependency.State(DependencyState.Removing, "RemovingNodeJS");
			if (IToolchainDependency.GetUninstaller(new Dictionary<string, string>
			{
				{ "DisplayName", "Node.js" },
				{ "DisplayVersion", version }
			}, out var keyName) == null)
			{
				throw new ToolchainException(ToolchainError.Uninstall, this, "Uninstaller not found");
			}
			await Cli.Wrap("msiexec").WithArguments("/x " + keyName + " /passive /norestart").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
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
		catch (CommandExecutionException ex3)
		{
			CommandExecutionException ex4 = ex3;
			if (ex4.ExitCode == 1602 || ex4.ExitCode == 1603)
			{
				throw new ToolchainException(ToolchainError.Install, this, "Uninstallation canceled by user");
			}
			throw new ToolchainException(ToolchainError.Install, this, (Exception)(object)ex4);
		}
		catch (Exception innerException)
		{
			throw new ToolchainException(ToolchainError.Uninstall, this, innerException);
		}
	}

	private async Task<string> GetNodeVersion(CancellationToken token)
	{
		string installedVersion = string.Empty;
		List<string> errorText = new List<string>();
		try
		{
			await Cli.Wrap("node").WithArguments("-v").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
			{
				installedVersion = l;
			}))
				.WithStandardErrorPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
				{
					errorText.Add(l);
				}))
				.WithValidation((CommandResultValidation)0)
				.ExecuteAsync(token)
				.ConfigureAwait(false);
		}
		catch (Win32Exception ex)
		{
			if (ex.ErrorCode != -2147467259)
			{
				IToolchainDependency.log.ErrorFormat((Exception)ex, "Failed to get {0} version", (object)name);
			}
		}
		catch (Exception ex2)
		{
			IToolchainDependency.log.ErrorFormat(ex2, "Failed to get {0} version", (object)name);
		}
		if (errorText.Count > 0)
		{
			IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
		}
		NodeJSDependency nodeJSDependency = this;
		string text;
		if (!installedVersion.StartsWith('v'))
		{
			text = installedVersion;
		}
		else
		{
			string text2 = installedVersion;
			text = text2.Substring(1, text2.Length - 1);
		}
		((BaseDependency)nodeJSDependency).version = text;
		return base.version;
	}

	public override LocalizedString GetLocalizedVersion()
	{
		if (string.IsNullOrEmpty(version))
		{
			return new LocalizedString("Options.WARN_TOOLCHAIN_MIN_VERSION", null, new Dictionary<string, ILocElement> { 
			{
				"MIN_VERSION",
				LocalizedString.Value(kMinNodeJSVersion)
			} });
		}
		return base.GetLocalizedVersion();
	}
}
