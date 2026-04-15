$ErrorActionPreference = "Stop"

param(
    [string]$BasePath = ""
)

if ([string]::IsNullOrWhiteSpace($BasePath)) {
    $BasePath = Split-Path -Parent $MyInvocation.MyCommand.Path
}

$filmsFolder = Join-Path $BasePath "KasomaFlix.Presentation\bin\Debug\net8.0-windows\FilmsLocaux"
New-Item -ItemType Directory -Path $filmsFolder -Force | Out-Null

$expected = @(
    "les_petites_victoires.mp4",
    "mayday.mp4",
    "remi_sans_famille.mp4",
    "smile.mp4",
    "blockbusters_2025.mp4",
    "balle_perdue.mp4",
    "freres.mp4",
    "joker_folie_a_deux.mp4",
    "oppenheimer.mp4"
)

$readmePath = Join-Path $filmsFolder "LIRE-MOI_FILMS.txt"
$content = @(
    "Dossier de films locaux pour KasomaFlix",
    "",
    "Copiez vos fichiers video ici avec les noms suivants :",
    ""
) + ($expected | ForEach-Object { "- $_" }) + @(
    "",
    "Astuce: vous pouvez utiliser n'importe quelle video de demo et la renommer.",
    "Format conseille: .mp4 (H.264)."
)

Set-Content -Path $readmePath -Value $content -Encoding UTF8

Write-Output "Dossier cree: $filmsFolder"
Write-Output "Fichier guide cree: $readmePath"
Write-Output "Placez vos videos puis lancez l'application."
