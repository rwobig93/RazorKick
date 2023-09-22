<h1 align="center"> RazorKick </h1>
<h3 align="center"> RazorKick is a starting boilerplate / template project to get a 'Kick' start on your Blazor applications </h3>

<hr/>

# Clone the repo
```shell
git clone https://github.com/rwobig93/RazorKick.git
```

## About
- TBA

## Features
- Full custom identity framework <Details>
  - Users, Roles & Permissions
  - Permissions can be applied to users or roles
  - Authorization is aggregated between user and role inheritance
  - Roles and Permissions CRUD functionality within admin pages
  - Dynamic roles based on entity types (check out [dynamic in the PermissionService](Infrastructure/Services/Identity/AppPermissionService.cs))
  - All built out and easily customizable
  - JWT (JSON Web Token) with proper expirations and refresh token validation
  - User API token generation for auth
  - Service account support
  - User registration
  - Built with strict security in mind
  - Profile image support from hosted url </Details>
- Oauth client to login using SSO for Google & Spotify
- MFA (Multi-Factor Authentication) support using OTP
- Local client timezone support
- Audit trails for all entities
- Database Provider Switching <Database Provider Switching>
  - Built out support for MSSql & Postgres (Coming Soon)
  - Dapper used as the ORM
  - All database scripts aren't code-first for efficiency and full control (see files in the [Infrastructure Database directory](Infrastructure/Database) for examples)
  - Database migrations aren't code-first for full control (see files in the [Infrastructure Database Migrations directory](Infrastructure/Database/MsSql/Migrations) for examples)
  - All database scripts are stored procedures, primarily for DBA manageability and no client side manipulation, only thing sent is the procedure name/path </Database Provider Switching>
- Health checks page and framework using .NET implementation
- API using minimal API framework <Details>
  - Support for multiple version methods (url/header/media/query)
  - Swagger documentation with enrichment </Details>
- Background and fire-and-forget jobs using Hangfire with dark theme
- Custom user-configurable theming <Details>
  - Native themes for Light, Dark & Hacker
  - User configurable themes with theme editor in account settings
  - 3 custom themes per user out of the box </Details>
- Export table data to Excel (xlsx) for selected or all data based on entity support
- Search & Filtering for admin pages
- Diverse settings configuration options via appsettings.json for lots of flexibility (see [appsettings.json](RazorKick/appsettings.json) for examples)

## Planned Updates
- Discord Oauth
