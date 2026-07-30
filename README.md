# PropertyPiles

Retrieve and display property listings in a format you can share with friends and family when you are house-hunting (to purchase).

PropertyPiles uses [RealtyAPI](https://www.realtyapi.io) to retrieve property listings. It has only been tested with Domain.com.au but should theoretically work with other APIs offered through this service.

_PropertyPiles is not affiliated with or endorsed by RealtyAPI, Domain, or any other property industry entity._

## Usage

### Shortlist input

The most obvious evidence of the short-term nature of this project is the data input. It's just a JSON file. No interface to edit it, just...JSON.

For privacy, it's not committed to Git. Leaving a public record of houses one is considering buying doesn't pass the vibe check, so it's Gitignored locally and expected to be in Azure Blob Storage in production (or alternatively you can fork the project and modify it to put the file somewhere else).

The format is as follows:

```json
[
	{
		"path": "14-dreamer-circuit-mount-duneed-vic-3217-2020738365",
		"notes": [],
		"priority": true
	},
]
```

(And no, that's not the house I bought. That listing got taken down before my old place was even on the market.)

Valid fields are "notes" (array of strings), "dismissedReasons" (array of strings), and "priority" (boolean).

You do not need to manually mark when a property you shorlisted gets sold. The app will automatically account for this when processing the data.

### Authentication

Auth is rudimentary by design. Do you want to deal with your boomer family members asking why they need to sign into Google or whatever (or worse, create an account on your random app) just to view your property shortlist, and provide the associated tech support? No, I didn't think so. But, as I mentioned above, privacy is a consideration.

The app is essentially a single page, with a single password protecting it. This is just so that you can have some basic control over who you share your list with, and it shouldn't show up on the Wayback machine or anything like that. The password is set as an environment variable.

## Setup and Deployment

### Prerequisites

- [RealtyAPI](https://www.realtyapi.io/) account and API key
- Azure App Service instance for a .NET 10 app
- Azure Blob Storage account
- Azure Key Vault (optional but recommended)

### Environment variables

To develop and test the app locally, create a `.env` file in the project root with your API key, your name to put above the site title (optional), the base URL for the RealtyAPI endpoints you are using, the base URL for the source site (to be used for links to the real listings), and Azure blob storage credentials. For example:
   ```dotenv
	APP_USER_NAME="Leesa and Michael"
	REALTY_API_KEY=your_api_key_here
	REALTY_API_BASE_URL=https://domain.realtyapi.io/
	MAX_CACHE_AGE=3600
	SOURCE_SITE_BASE_URL=https://www.domain.com.au/
	BLOB_STORAGE_ACCOUNT_NAME=propertypilesfiles
	BLOB_STORAGE_CONTAINER_NAME=propertypilescontainer
	BLOB_STORAGE_ACCESS_KEY=your_key_here
	FRONT_END_PASSWORD=some_password_here
   ```

For production deployment, you will need to set the same environment variables in your hosting environment. For sensitive credenitals, you can have these refer to Azure Key Vault secrets instead of hardcoding the values in the environment variables.

### Build step

From the solution or project root:

```powershell
dotnet publish
```

The compiled files will be in `./PropertyPiles/bin/Release/net10.0/publish`.

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