namespace AwsCredentialsManager;

public class AwsAccount(string name)
{
	public string Name { get; } = name;
	public string AccessKeyId { get; private set; } = default!;
	public string SecretAccessKey { get; private set; } = default!;
	public string CaBundle { get; private set; } = default!;
	public string SessionToken { get; private set; } = default!;
	public string Output { get; private set; } = default!;
	public string Region { get; private set; } = default!;
	public string ToolArtifactGuid { get; private set; } = default!;
	public string RoleArn { get; private set; } = default!;
	public string SourceProfile { get; private set; } = default!;
	public string MfaSerial { get; private set; } = default!;

	public void SetProperties(string properties)
	{
		var arr = properties
			.ReplaceLineEndings()
			.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

		foreach (var property in arr)
		{
			SetProperty(property);
		}
	}

	public void SetProperty(string property)
	{
		if (string.IsNullOrWhiteSpace(property) || property.StartsWith('['))
		{
			return;
		}

		var kvp = RemoveWhiteSpace(property).Split('=', 2);

		if (kvp.Length != 2)
		{
			throw new($"Property string '{property}' contains unexpected format.");
		}

		switch (kvp[0])
		{
			case "aws_access_key_id":
				AccessKeyId = kvp[1];
				break;
			case "aws_secret_access_key":
				SecretAccessKey = kvp[1];
				break;
			case "aws_session_token":
				SessionToken = kvp[1];
				break;
			case "ca_bundle":
				CaBundle = kvp[1];
				break;
			case "output":
				Output = kvp[1];
				break;
			case "region":
				Region = kvp[1];
				break;
			case "toolkit_artifact_guid":
				ToolArtifactGuid = kvp[1];
				break;
			case "role_arn":
				RoleArn = kvp[1];
				break;
			case "source_profile":
				SourceProfile = kvp[1];
				break;
			case "mfa_serial":
				MfaSerial = kvp[1];
				break;
			default:
				throw new($"Property string contains unrecognized property '{kvp[0]}'.");
		}
	}

	private static string RemoveWhiteSpace(string s)
	{
		return new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
	}

	public List<string> AsLineList()
	{
		var lines = new List<string>
		{
			Name
		};

		TryAdd(lines, "aws_access_key_id", AccessKeyId);
		TryAdd(lines, "aws_secret_access_key", SecretAccessKey);
		TryAdd(lines, "aws_session_token", SessionToken);
		TryAdd(lines, "output", Output);
		TryAdd(lines, "region", Region);
		TryAdd(lines, "mfa_serial", MfaSerial);
		TryAdd(lines, "role_arn", RoleArn);
		TryAdd(lines, "source_profile", SourceProfile);
		TryAdd(lines, "toolkit_artifact_guid", ToolArtifactGuid);

		lines.Add("");

		return lines;
	}

	private static void TryAdd(List<string> lines, string key, string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return;
		}

		lines.Add($"{key} = {value}");
	}
}
