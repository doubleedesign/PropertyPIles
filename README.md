# PropertyPiles

Retrieve and display property listings in a format you can share with friends and family when you are house-hunting (to purchase).

PropertyPiles uses [RealtyAPI](https://www.realtyapi.io) to retrieve property listings. It has only been tested with Domain.com.au but should theoretically work with other APIs offered through this service.

_PropertyPiles is not affiliated with or endorsed by RealtyAPI, Domain, or any other property industry entity._

## Setup and Deployment

### Prerequisites

- [RealtyAPI](https://www.realtyapi.io/) account and API key

### Environment variables

To develop and test the app locally, create a `.env` file in the project root with your API key, your name to put above the site title (optional), the base URL for the RealtyAPI endpoints you are using, and the base URL for the source site (to be used for links to the real listings), e.g.:
   ```dotenv
   APP_USER_NAME="Leesa and Michael"
   REALTY_API_KEY=your_api_key_here
   REALTY_API_BASE_URL=https://domain.realtyapi.io/
   SOURCE_SITE_BASE_URL=https://www.domain.com.au/
   ```

For production deployment, you will need to set the same environment variables in your hosting environment.

### Fonts

>[!IMPORTANT]
>[Neue Montreal](https://pangrampangram.com/products/neue-montreal) is free for personal use. If you use this codebase for any commercial project, you must either purchase a licence or replace the font with something you have the rights to use commercially.

## Development

### Prerequisites

PropertyPiles is a [Blazor Web App](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor), so you will need:

- [RealtyAPI](https://www.realtyapi.io/) account and API key
- .NET 10 SDK (`choco install dotnet-sdk` or [download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))
- [Sass](https://sass-lang.com/install) installed globally (`choco install sass`)
- A .NET IDE such as [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/)

And you will also need to:
1. Create a .env file in the project root as per the setup instructions above.
2. Configure your IDE to compile Sass files when they are edited. In Rider, do this by creating a file watcher with the default settings (it will prompt you the first time you open a .scss file) and enabling it to run on save.

>[!TIP]
> Restart the app after changing environment variables to make sure the new value takes effect.

### Running locally

To run with hot reloading in Rider, use the CLI instead of the IDE's Run configurations.

From the solution directory:

```powershell
cd PropertyPiles && dotnet watch run
```

Create a file in the project root called `data.json` and add the IDs of your shortlisted properties like so:

```json
{
  "priority": [1, 2, 3],
  "maybe": [4, 5, 6],
  "dismissed": [7, 8, 9]
}
```

You do not need to manually mark when a property you shorlisted gets sold. The app will automatically account for this when processing the data.