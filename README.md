# Marketio Desktop

WPF-desktopapplicatie voor het beheer van het Marketio-platform: producten, klanten, bestellingen en gebruikers.
De solution bestaat uit twee projecten: een Model-Library (`Marketio_Shared`) en een WPF-toepassing (`MarketioDesktop`), met een eigen SQL Server-databank (standaard LocalDB).

**Repository:** [github.com/SoufianeAbk/MarketioDesktop](https://github.com/SoufianeAbk/MarketioDesktop)

---

## Inhoud

- [Overzicht](#overzicht)
- [Projectstructuur](#projectstructuur)
- [Technologieën](#technologieën)
- [Aan de slag](#aan-de-slag)
- [Databank](#databank)
- [Functionaliteit](#functionaliteit)
- [Gebruikersrollen](#gebruikersrollen)

---

## Overzicht

Marketio Desktop is een administratieve WPF-applicatie waarmee medewerkers en beheerders van een fictieve webshop:

- het productaanbod beheren (CRUD, categoriefilter, voorraadindicator);
- bestellingen aanmaken, opvolgen en van status wijzigen;
- klanten beheren en opzoeken;
- (enkel als Admin) gebruikersaccounts, rollen en blokkeringen beheren.

Bij het opstarten migreert en seedt de applicatie automatisch haar eigen databank, waarna een aanmeldvenster verschijnt. Na een geldige aanmelding opent het hoofdvenster met een navigatiemenu dat zich aanpast aan de rol van de ingelogde gebruiker.

---

## Projectstructuur

```
MarketioDesktop.sln
│
├── Marketio_Shared/                        → Model-Library
│   ├── Data/
│   │   ├── MarketioDbContext.cs            → EF Core DbContext (IdentityDbContext<AppUser>)
│   │   ├── DesignTimeDbContextFactory.cs   → voor Add-Migration / Update-Database
│   │   └── DataSeeder.cs                   → seed-data: rollen, gebruikers, producten, klanten, orders
│   ├── Entities/                           → Customer, Order, OrderItem, Product, ProductTranslation, GdprAuditLog
│   ├── Enums/                              → OrderStatus, PaymentMethod, ProductCategory
│   ├── Models/
│   │   └── AppUser.cs                      → Identity-gebruiker (IdentityUser + extra velden)
│   ├── DTOs/ · Interfaces/
│   ├── Migrations/
│   └── Marketio_Shared.csproj
│
└── MarketioDesktop/                         → WPF-applicatie
    ├── Controls/
    │   ├── StatusBadge.xaml(.cs)           → custom control (orderstatus)
    │   └── StockIndicator.xaml(.cs)        → custom control (voorraadniveau)
    ├── Converters/                          → ValueConverters voor zichtbaarheid en rollen
    ├── Services/                            → business logic + EF Core-queries
    ├── ViewModels/                          → MVVM-laag
    ├── Views/
    │   ├── Dialogs/                         → popup-vensters (aanmaken/bewerken/status)
    │   ├── LoginView.xaml
    │   ├── RegisterView.xaml
    │   ├── ProductsView.xaml
    │   ├── OrdersView.xaml
    │   ├── CustomersView.xaml
    │   └── AdminView.xaml
    ├── App.xaml(.cs)                        → DI-configuratie, opstart, migreren + seeden
    ├── MainWindow.xaml(.cs)                 → shell: navigatiemenu + ContentControl
    └── MarketioDesktop.csproj
```

---

## Technologieën

| Onderdeel | Technologie |
|---|---|
| Taal / runtime | C# · .NET 9 |
| UI-framework | WPF (`net9.0-windows`) |
| Architectuur | MVVM — eigen `BaseViewModel` + `RelayCommand`/`AsyncRelayCommand` uit CommunityToolkit.Mvvm |
| ORM | Entity Framework Core 9.0 (Code-First + Migrations) |
| Databank | SQL Server (standaard LocalDB) |
| Authenticatie & rollen | ASP.NET Core Identity |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |
| Logging | Microsoft.Extensions.Logging (Console + Debug) |
| Configuratie/geheimen | Microsoft.Extensions.Configuration.UserSecrets |

---

## Aan de slag

### Vereisten

- Visual Studio 2022 (17.12+) met de workload **".NET desktop development"**
- .NET 9 SDK
- SQL Server LocalDB (standaard inbegrepen bij Visual Studio)

### Installatie

```bash
git clone https://github.com/SoufianeAbk/MarketioDesktop.git
cd MarketioDesktop
```

Open `MarketioDesktop.sln` in Visual Studio. `MarketioDesktop` staat al ingesteld als opstartproject — gewoon op **Start** klikken volstaat. De applicatie migreert en seedt de databank zelf bij de eerste opstart.

### Databank manueel bijwerken (optioneel)

**Package Manager Console** (Default project: `Marketio_Shared`)
```powershell
Update-Database
```

**.NET CLI**
```bash
dotnet ef database update --project Marketio_Shared --startup-project MarketioDesktop
```

### Testaccounts

| Rol | E-mail | Wachtwoord |
|---|---|---|
| Admin | `admin@marketio.be` | `Admin@12345` |
| Manager | `manager@marketio.be` | `Manager@12345` |
| Customer | `user@marketio.be` | `User@12345` |

---

## Databank

Naast de tabellen van ASP.NET Core Identity bevat de databank vijf eigen tabellen:

| Tabel | Beschrijving |
|---|---|
| `Products` | Productcatalogus (naam, prijs, voorraad, categorie) |
| `ProductTranslations` | Nederlandse en Franse vertalingen per product |
| `Customers` | Klantgegevens |
| `Orders` | Bestellingen (status, betaalmethode, totaalbedrag) |
| `OrderItems` | Bestelregels, gekoppeld aan een order en een product |

Bij het opstarten worden automatisch enkele rollen, testaccounts, 10 producten, 3 klanten en 3 voorbeeldbestellingen aangemaakt, zodat de applicatie meteen bruikbaar is.

---

## Functionaliteit

- **Producten** — overzicht met categoriefilter en voorraadindicator, aanmaken/bewerken via een dialoogvenster, soft-delete.
- **Bestellingen** — overzicht met statusfilter en kleurgecodeerde statusbadge, nieuwe bestelling aanmaken via een klant- en productselectie, status wijzigen.
- **Klanten** — overzicht met zoekfunctie, CRUD via dialoogvensters.
- **Gebruikersbeheer** (Admin) — rollen toewijzen/verwijderen, accounts blokkeren/deblokkeren, nieuwe gebruikers registreren, accounts verwijderen.
- **Aanmelden/afmelden/registreren** — met wachtwoordvalidatie, lockout bij te veel mislukte pogingen en een aparte registratiestroom.

De interface is opgebouwd met herbruikbare XAML-stijlen (centraal in `App.xaml`), twee zelf ontworpen controls (`StatusBadge`, `StockIndicator`) en consistente visuele feedback (laadindicatoren, foutmeldingen, bevestigingsdialogen).

---

## Gebruikersrollen

| Rol | Toegang |
|---|---|
| Customer | Producten, Bestellingen |
| Manager | + Klanten |
| Admin | + Gebruikersbeheer, gebruikers registreren |

Het navigatiemenu in het hoofdvenster past zich automatisch aan op basis van de rol van de ingelogde gebruiker.
