using Azure;
using Azure.Storage.Blobs;
using PropertyPiles.Utils;
namespace PropertyPiles.Services;

public class BlobService {
	private string? _accountName = "";
	private string? _containerName = "";
	private string? _accountKey = "";
	private BlobClient _client;

	public BlobService() {
		this.SetAccountNameFromEnv();
		this.SetContainerNameFromEnv();
		this.SetAccessKeyFromEnv();
		
		var connectionString = $"DefaultEndpointsProtocol=https;AccountName={this._accountName};AccountKey={this._accountKey};EndpointSuffix=core.windows.net";

		this._client = new(
			connectionString,
			this._containerName,
			"data.json"
		);
	}

	private void SetAccountNameFromEnv() {
		this._accountName = Environment.GetEnvironmentVariable("BLOB_STORAGE_ACCOUNT_NAME");
		
		if (string.IsNullOrEmpty(this._accountName)) {
			throw new ArgumentException("Environment variable BLOB_STORAGE_ACCOUNT_NAME is not set.");
		}
	}
	
	private void SetContainerNameFromEnv() {
		this._containerName = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONTAINER_NAME");
		
		if (string.IsNullOrEmpty(this._containerName)) {
			throw new ArgumentException("Environment variable BLOB_STORAGE_CONTAINER_NAME is not set.");
		}
	}

	private void SetAccessKeyFromEnv() {
		this._accountKey = Environment.GetEnvironmentVariable("BLOB_STORAGE_ACCESS_KEY");
		if (string.IsNullOrEmpty(this._accountKey)) {
			throw new ArgumentException("Environment variable BLOB_STORAGE_ACCESS_KEY is not set.");
		}
	}
	
	public async Task<string?> DownloadBlobContents() {
		try {
			var response = await this._client.DownloadAsync();
			using (var reader = new StreamReader(response.Value.Content)) {
				return await reader.ReadToEndAsync();
			}
		} 
		catch (RequestFailedException ex) {
			Logger.Error(ex.Message);
			return null;
		}
	}
}