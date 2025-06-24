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

public class DotNetDependency : BaseDependency
{
	private const string kDependencyName = ".Net";

	private static readonly string sDotNetVersion = "8.0";

	public static readonly string sDotNetInstallerUrl = "https://aka.ms/dotnet/" + sDotNetVersion + "/dotnet-sdk-win-" + RuntimeInformation.OSArchitecture.ToString().ToLower() + ".exe";

	public static readonly string kDefaultInstallationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

	public static readonly string kInstallationFolder = "dotnet";

	private long? m_DownloadSize;

	private string m_InstallationDirectory;

	public override string name => ".Net SDK";

	public override string icon => "Media/Toolchain/DotNet.svg";

	public override bool confirmUninstallation => true;

	public string installerPath => Path.Combine(Path.GetFullPath(SharedSettings.instance.modding.downloadDirectory), Path.GetFileName(sDotNetInstallerUrl));

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

	public override LocalizedString installDescr => new LocalizedString("Options.WARN_TOOLCHAIN_INSTALL_DOTNET", null, new Dictionary<string, ILocElement>
	{
		{
			"DOTNET_VERSION",
			LocalizedString.Value(sDotNetVersion)
		},
		{
			"HOST",
			LocalizedString.Value(new Uri(sDotNetInstallerUrl).Host)
		}
	});

	public override LocalizedString uninstallMessage => new LocalizedString("Options.WARN_TOOLCHAIN_DOTNET_UNINSTALL", null, new Dictionary<string, ILocElement> { 
	{
		"DOTNET_VERSION",
		LocalizedString.Value(version)
	} });

	public override string version
	{
		get
		{
			if (base.version == null)
			{
				Task.Run(async () => await GetVersion(GameManager.instance.terminationToken)).Wait();
			}
			return base.version;
		}
		protected set
		{
			base.version = value;
		}
	}

	public DotNetDependency()
	{
		installationDirectory = kDefaultInstallationDirectory;
	}

	public override async Task<bool> IsInstalled(CancellationToken token)
	{
		return (await GetDotnetVersion(token).ConfigureAwait(continueOnCapturedContext: false)).Major >= 6;
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
			m_DownloadSize = await IToolchainDependency.GetDownloadSizeAsync(sDotNetInstallerUrl, token).ConfigureAwait(continueOnCapturedContext: false);
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
				m_Size = 1073741824L
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
		return BaseDependency.Download(this, token, sDotNetInstallerUrl, installerPath, "DownloadingDotNet");
	}

	public override async Task Install(CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		string path = installerPath;
		try
		{
			IToolchainDependency.log.DebugFormat("Installing {0}", (object)".Net");
			base.state = new IToolchainDependency.State(DependencyState.Installing, "InstallingDotNet");
			await Cli.Wrap(path).WithArguments("/install /quiet /norestart INSTALLDIR=\"" + Path.Combine(installationDirectory, kInstallationFolder) + "\" ").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
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
			IToolchainDependency.log.DebugFormat("Uninstalling {0}", (object)".Net");
			base.state = new IToolchainDependency.State(DependencyState.Removing, "RemovingDotNet");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("DisplayName", "Microsoft .NET SDK " + version + " (" + RuntimeInformation.OSArchitecture.ToString().ToLower() + ")");
			if (IToolchainDependency.GetUninstaller(dictionary, out var keyName) == null)
			{
				throw new ToolchainException(ToolchainError.Uninstall, this, "Uninstaller not found");
			}
			await Cli.Wrap(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Package Cache", keyName, "dotnet-sdk-" + version + "-win-" + RuntimeInformation.OSArchitecture.ToString().ToLower() + ".exe")).WithArguments("/uninstall /quiet").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
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

	private async Task<string> GetVersion(CancellationToken token)
	{
		System.Version version = await GetDotnetVersion(token).ConfigureAwait(continueOnCapturedContext: false);
		base.version = ((version.Major != 0) ? version.ToString() : string.Empty);
		return base.version;
	}

	public override LocalizedString GetLocalizedVersion()
	{
		if (string.IsNullOrEmpty(version))
		{
			return new LocalizedString("Options.WARN_TOOLCHAIN_MIN_VERSION", null, new Dictionary<string, ILocElement> { 
			{
				"MIN_VERSION",
				LocalizedString.Value("6.0")
			} });
		}
		return base.GetLocalizedVersion();
	}

	public static async Task<System.Version> GetDotnetVersion(CancellationToken token)
	{
		System.Version installedVersion = new System.Version();
		List<string> errorText = new List<string>();
		try
		{
			await Cli.Wrap("dotnet").WithArguments("--version").WithStandardOutputPipe(PipeTarget.ToDelegate((Action<string>)delegate(string l)
			{
				if (System.Version.TryParse(l, out var result))
				{
					installedVersion = result;
				}
				else
				{
					int num = l.IndexOf('-');
					if (num > 0 && System.Version.TryParse(l.Substring(0, num), out result))
					{
						installedVersion = result;
					}
					else
					{
						IToolchainDependency.log.ErrorFormat("Failed to parse {0} version number \"{1}\"", (object)".Net", (object)l);
					}
				}
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
				IToolchainDependency.log.ErrorFormat((Exception)ex, "Failed to get {0} version", (object)".Net");
			}
			if (errorText.Count > 0)
			{
				IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
			}
		}
		catch (Exception ex2)
		{
			IToolchainDependency.log.ErrorFormat(ex2, "Failed to get {0} version", (object)".Net");
			if (errorText.Count > 0)
			{
				IToolchainDependency.log.Warn((object)string.Join<string>('\n', (IEnumerable<string>)errorText));
			}
		}
		return installedVersion;
	}
}
