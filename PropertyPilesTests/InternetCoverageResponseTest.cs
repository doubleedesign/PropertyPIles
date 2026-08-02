using System.Text.Json;
using PropertyPiles.Utils;
using PropertyPiles.Services;
using PropertyPiles.Types;
using WireMock.Server;

namespace PropertyPilesTests;

public class InternetCoverageResponseTest {
	private ListingDataService _listingDataService;
	private WireMockServer _server;
	
	[SetUp]
	public void Setup() {
		this._server = WireMockServer.Start();
		
		Environment.SetEnvironmentVariable("REALTY_API_BASE_URL", this._server.Url);
		Environment.SetEnvironmentVariable("REALTY_API_KEY", "test-api-key");
		Environment.SetEnvironmentVariable("NBN_BASE_URL", $"{this._server.Url}");
		this._listingDataService = new ListingDataService();
	}

	[TearDown]
	public void TearDown() {
		this._server.Stop();
		this._server.Dispose();
	}

	[Test]
	public async Task HasNbnCoverage() {
		HttpResponseMocks.MockListingDataResponse(this._server, "72-rossack-drive-waurn-ponds-vic-3216-2020754328");
		HttpResponseMocks.MockIspPropertyIdResponse(this._server);
		HttpResponseMocks.MockNbnDataResponse(this._server, "2020754328");
		
		SavedItem item = JsonSerializer.Deserialize<SavedItem>("{\"path\": \"72-rossack-drive-waurn-ponds-vic-3216-2020754328\"}")!;
		var record = new PropertyRecord(item);
		await record.PopulateData(this._listingDataService, new InternetCoverageService());
		
		Assert.That(record.NbnCoverage, Is.Not.Null);
		Assert.That(record.NbnCoverage.Type, Is.EqualTo("nbn"));
		Assert.That(record.NbnCoverage.ServiceClass, Is.EqualTo(3));
		Assert.That(record.NbnCoverage.SpeedPotential?.DownloadSpeed, Is.EqualTo(2000));
		Assert.That(record.NbnCoverage.SpeedPotential?.UploadSpeed, Is.EqualTo(500));
	}
	
	[Test]
	public async Task HasOpticommCoverage() {
		HttpResponseMocks.MockListingDataResponse(this._server, "14-dreamer-circuit-mount-duneed-2020738365");
		HttpResponseMocks.MockIspPropertyIdResponse(this._server);
		HttpResponseMocks.MockNbnDataResponse(this._server, "2020738365");
		
		SavedItem item = JsonSerializer.Deserialize<SavedItem>("{\"path\": \"14-dreamer-circuit-mount-duneed-2020738365\"}")!;
		var record = new PropertyRecord(item);
		await record.PopulateData(this._listingDataService, new InternetCoverageService());

		Logger.DebugObject(record);
		
		Assert.That(record.NbnCoverage, Is.Not.Null);
		Assert.That(record.NbnCoverage.Type, Is.EqualTo("opticomm"));
		Assert.That(record.NbnCoverage.ServiceClass, Is.EqualTo(3));
		Assert.That(record.NbnCoverage.SpeedPotential?.DownloadSpeed, Is.EqualTo(1000));
		Assert.That(record.NbnCoverage.SpeedPotential?.UploadSpeed, Is.EqualTo(50));
	}
}