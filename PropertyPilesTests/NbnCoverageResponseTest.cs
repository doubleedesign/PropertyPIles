using System.Text.Json;
using PropertyPiles.Utils;
using PropertyPiles.Services;
using PropertyPiles.Types;
using WireMock.Server;

namespace PropertyPilesTests;

public class NbnCoverageResponseTest {
	private ListingDataService _listingDataService;
	private WireMockServer _server;

	[SetUp]
	public void Setup() {
		this._server = WireMockServer.Start();
		
		Environment.SetEnvironmentVariable("REALTY_API_BASE_URL", this._server.Url);
		Environment.SetEnvironmentVariable("REALTY_API_KEY", "test-api-key");
		Environment.SetEnvironmentVariable("NBN_BASE_URL", $"{this._server.Url}/onesq/api/v1/sq");
		Environment.SetEnvironmentVariable("NBN_ISP_NAME", "IINET");
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
		HttpResponseMocks.MockNbnDataResponse(this._server, "2020754328");
		
		SavedItem item = JsonSerializer.Deserialize<SavedItem>("{\"path\": \"72-rossack-drive-waurn-ponds-vic-3216-2020754328\"}")!;
		var record = new PropertyRecord(item);
		await record.PopulateData(this._listingDataService, new NbnCoverageService());

		Logger.DebugObject(record);
	}
}