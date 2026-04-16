## Projet : KasomaFlix (application WPF)

Ce dépôt contient une application de visionnement et de gestion de films développée en **C#** dans le cadre du cours **INF23207 — Génie logiciel II** à l’UQAR. Le travail met en œuvre une architecture en **couches** (Domain, Application, Infrastructure, Presentation), des **cas d’utilisation** côté Application, la persistance avec **Entity Framework Core** vers **SQL Server** (LocalDB par défaut), et une interface **WPF**.

L’application distingue notamment un profil **membre** (catalogue, détails, visionnement, transactions, abonnements, profil) et un profil **administrateur** (tableau de bord, gestion des films, membres, transactions).

### Structure du projet

```
VisionnementFilms/
├── KasomaFlix.Domain/              # entités, interfaces de dépôts
├── KasomaFlix.Application/         # cas d’utilisation, DTOs
├── KasomaFlix.Infrastructure/      # EF Core, implémentations des dépôts
│   └── Data/Scripts/
│       └── CreateDatabase.sql      # script de création / données de base
├── KasomaFlix.Presentation/        # WPF (vues, navigation)
│   ├── Images/                     # affiches copiées avec le build
│   └── appsettings.json          # chaîne de connexion
├── VisionnementFilms.sln
└── README.md
```

### Technologies

- C# / **.NET 8**, **WPF** (XAML)
- **Entity Framework Core**, **SQL Server** (LocalDB recommandé en local)
- **Microsoft.Extensions** (configuration, injection de dépendances)

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- **SQL Server LocalDB** (souvent installé avec Visual Studio) ou une instance SQL Server compatible avec la chaîne dans `appsettings.json`

### Base de données

1. Créer la base (par ex. avec **SQL Server Management Studio** ou **sqlcmd**) en exécutant le script :  
   `KasomaFlix.Infrastructure/Data/Scripts/CreateDatabase.sql`
2. Vérifier que la chaîne `ConnectionStrings:DefaultConnection` dans  
   `KasomaFlix.Presentation/appsettings.json`  
   pointe vers la même base (par défaut : `VisionnementFilmsDB` sur `(localdb)\mssqllocaldb`).

### Lancer le projet

**Visual Studio** : ouvrir `VisionnementFilms.sln`, définir le projet de démarrage sur **`KasomaFlix.Presentation`**, puis lancer le débogage (F5).

**Ligne de commande**, à la racine du dossier `VisionnementFilms` :

```bash
dotnet build VisionnementFilms.sln
dotnet run --project KasomaFlix.Presentation/KasomaFlix.Presentation.csproj
```

### Auteur

Simon Kasongo

Cours INF23207 — UQAR, Automne 2025
