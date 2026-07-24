# Property Pile

Retrieve and display property listings in a format you can share with friends and family when you are house-hunting (to purchase).

## Setup and Deployment

### Prerequisites

- [RealtyAPI](https://www.realtyapi.io/) account and API key

## Development

### Prerequisites

PropertyPile is a [Blazor Web App](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor), so you will need:

- [RealtyAPI](https://www.realtyapi.io/) account and API key
- .NET 10 SDK (`choco install dotnet-sdk` or [download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))
- A .NET IDE such as [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/)

Create a .env file in the project root with your API key like so:

```dotenv
REALTY_API_KEY=your_api_key_here
```

### Running locally

To run with hot reloading in Rider, use the CLI instead of the IDE's Run configurations.

From the solution directory:

```powershell
cd PropertyPile && dotnet watch run
```