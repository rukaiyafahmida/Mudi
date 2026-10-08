# Mudi

An online grocery shop built with ASP.NET Core MVC, Identity and Entity Framework Core. The solution contains the web app (`Mudi`), models (`Mudi_Model`), repositories (`Mudi_DataAccess`) and shared utilities (`Mudi_Utility`).

## Run locally

Requires the **.NET 10 SDK** ([Microsoft download](https://dotnet.microsoft.com/download/dotnet/10.0)). Linux also needs its system SQLite library; on Ubuntu it is provided by `libsqlite3-0`.

From this repository:

```bash
./scripts/run-local.sh
```

Open **http://localhost:5000**. On this machine the script automatically finds the workspace SDK at `../.dotnet` and uses the workspace NuGet cache. Alternatively, with .NET 10 on your PATH:

```bash
dotnet run --project Mudi/Mudi.csproj --launch-profile Mudi
```

The first start creates `Mudi/mudi-local.db`, both user roles, website details and 16 demo grocery products across 7 categories. Subsequent starts keep your data. No SQL Server instance, HTTPS certificate, Facebook app or email service is required for development.

Development admin login:

- Email: `admin@mudi.local`
- Password: `MudiLocal7!`

These credentials are seeded **only in Development**. Register another account to try the customer cart, wishlist and checkout. Admins can manage categories, products and orders. Set `LocalDevelopment__SeedDemoData=false` to skip the demo catalogue, or override `LocalDevelopment__AdminEmail` and `LocalDevelopment__AdminPassword` before the first start to choose your own local administrator.

Emails are saved as HTML files in `Mudi/App_Data/mail/`; open them to follow confirmation and password-reset links. Development mode never needs to send real emails. Generated mail, databases, signing keys and build outputs are ignored by Git. To reset the disposable development database, stop the app and remove only `Mudi/mudi-local.db` and any `mudi-local.db-wal` / `mudi-local.db-shm` sidecars. SQLite uses `EnsureCreated` rather than the historical SQL Server migrations, so reset this development database when the model changes.

## Verify

```bash
dotnet build Mudi.sln
python3 scripts/smoke-test.py
```

On this workspace, where the SDK is not on PATH:

```bash
DOTNET_CLI_HOME=../.cli NUGET_PACKAGES=../.nuget ../.dotnet/dotnet build Mudi.sln -m:1 -nr:false -p:UseSharedCompilation=false
python3 scripts/smoke-test.py
```

The smoke test needs Python 3.8+ and a previously built Debug app. It starts its own localhost server with a temporary database and mail folder, checks registration, authorization, wishlist/cart, checkout prices, admin pages, category changes, website edits, order completion, image upload/update/delete and contact email, then stops its server and removes the test data. It does not use your local database.

## External integrations and SQL Server

The original SQL Server migrations are retained. SQL Server and real Facebook/Mailjet integrations have **not been verified by the local smoke test**. Before running an existing SQL Server database on EF Core 10, review/generate a migration that reconciles the EF Core 5 model snapshot with the upgraded model and test it against a backup; EF Core 10 detects pending model changes.

Configure integrations through environment variables or ASP.NET user secrets; credentials are no longer stored in source files:

- `Database__Provider=SqlServer` and `ConnectionStrings__DefaultConnection` select SQL Server. SQLite is restricted to Development.
- `Authentication__Facebook__AppId` and `Authentication__Facebook__AppSecret` enable Facebook login when both are set.
- `Email__Delivery=Mailjet`, `MailJet__ApiKey`, `MailJet__SecretKey`, `Email__FromEmail` and optionally `Email__FromName` configure real email delivery. Use a verified Mailjet sender.

The local launcher always selects Development and binds localhost. For deployment, configure Production separately, supply your own administrator and integration credentials, and configure HTTPS. Previously committed integration credentials remain in Git history; replace them before enabling those integrations.

## Frontend dependencies

Bootstrap, jQuery, Font Awesome, validation scripts, Summernote 0.8.18 and SweetAlert2 9 are served from `Mudi/wwwroot` so local pages work without CDNs. Summernote and SweetAlert2 retain their upstream MIT licenses under their `lib` directories. The old Syncfusion order grid is replaced by server-rendered tables; the admin search form remains available. Demo product photos come from the repository's existing assets. The 16 demo products use sample BDT prices and stock; restarting seeds missing demo names without duplicating existing products. The exact old local placeholder receives a matching tomato image and name while its ID, stock and existing order links are preserved. An already-seeded tomato item can coexist with the original demo pack.

## Interface and themes

The customer shop uses category shelves with product search, scroll controls and keyboard access. Admin pages share the responsive navigation, labelled forms, product image previews and mobile table layouts. Use the theme button in the sidebar or mobile header to switch between light and dark modes; the browser remembers your choice. Bebas Neue and Inter are self-hosted with their OFL licenses in `Mudi/wwwroot/fonts`. Design rules are recorded in `DESIGN.md`.
