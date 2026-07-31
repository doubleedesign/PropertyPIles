using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PropertyPilesTests;

public static class HttpResponseMocks {
	private static string _projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
	private static string _cacheDir = Path.Combine(_projectRoot, "cache");

	public static void MockListingDataResponse(WireMockServer server, string propertyPath) {
		string assumedId = propertyPath.Split('-').Last();
		string filePath = Path.Combine(_cacheDir, $"{assumedId}.json");
		string response = File.ReadAllText(filePath);
		
		server.Given(Request.Create()
			.WithPath($"/details/byurl")
			.WithParam("url", propertyPath)  
			.UsingGet()
		).RespondWith(Response.Create()
			.WithStatusCode(200)
			.WithBody(response)
		);
	}

	public static void MockIspPropertyIdResponse(WireMockServer server) {
		server.Given(Request.Create()
			.WithPath("/api/signup/address/search")
			.UsingGet()
		).RespondWith(Response.Create()
			.WithStatusCode(200)
			.WithBody("{\"LOC000083302541\": \"72 ROSSACK DRIVE, WAURN PONDS 3216\"}")
		);
	}
	
	public static void MockNbnDataResponse(WireMockServer server, string propertyId) {
		string filePath = Path.Combine(_cacheDir, $"{propertyId}_nbn.json");
		string response = File.ReadAllText(filePath);

		server.Given(Request.Create()
			.WithPath("/api/signup/service-qualification/LOC000083302541")
			.UsingGet()
		).RespondWith(Response.Create()
			.WithStatusCode(200)
			.WithBody(response)
		);
	}
}