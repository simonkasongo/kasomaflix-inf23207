using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionSessions;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class LecteurVideo : Page
    {
        private readonly CreerSessionUseCase _creerSessionUseCase;
        private int _filmId;
        private string _fichierVideo;
        private int? _sessionId;
        private DateTime _debutLecture;
        private bool _lectureEnCours = false;
        private bool _sessionTerminee = false;

        public LecteurVideo(int filmId, string fichierVideo)
        {
            InitializeComponent();
            _filmId = filmId;
            _fichierVideo = fichierVideo ?? string.Empty;
            _creerSessionUseCase = ServiceLocator.GetService<CreerSessionUseCase>();
            Loaded += LecteurVideo_Loaded;
        }

        private async void LecteurVideo_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn() && UserSession.GetUserId().HasValue)
            {
                try
                {
                    _sessionId = await _creerSessionUseCase.ExecuteAsync(UserSession.GetUserId().Value, _filmId);
                    _debutLecture = DateTime.Now;
                    ChargerFichierVideoLocal();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la création de la session : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _lectureEnCours = true;
            VideoPlayer.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _lectureEnCours = false;
            VideoPlayer.Stop();
            TimelineSlider.Value = 0;
        }

        private async void FermerLecteur_Click(object sender, RoutedEventArgs e)
        {
            await TerminerSessionSiNecessaireAsync();
            NavigationService.Navigate(new DetailsFilm(_filmId));
        }

        private async void CoterFilm_Click(object sender, RoutedEventArgs e)
        {
            await TerminerSessionSiNecessaireAsync();
            NavigationService.Navigate(new DetailsFilm(_filmId));
        }

        private void ChargerFichierVideoLocal()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_fichierVideo))
                {
                    MessageBox.Show("Aucun fichier video n'est associe a ce film.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dossier = AppConfig.ObtenirDossierFilmsLocaux();
                var chemin = System.IO.Path.Combine(dossier, _fichierVideo);
                if (!System.IO.File.Exists(chemin))
                {
                    MessageBox.Show($"Fichier video introuvable: {chemin}", "Fichier manquant", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                VideoPlayer.Source = new Uri(chemin, UriKind.Absolute);
                VideoPlayer.MediaOpened += (_, __) =>
                {
                    if (VideoPlayer.NaturalDuration.HasTimeSpan)
                    {
                        TimelineSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    }
                };
                VideoPlayer.Play();
                _lectureEnCours = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement video: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task TerminerSessionSiNecessaireAsync()
        {
            if (_sessionTerminee || !_sessionId.HasValue)
            {
                return;
            }

            try
            {
                var tempsVisionne = (int)Math.Max(0, (DateTime.Now - _debutLecture).TotalSeconds);
                using var scope = ServiceLocator.CreateScope();
                var terminerSessionUseCase = scope.ServiceProvider.GetRequiredService<TerminerSessionUseCase>();
                await terminerSessionUseCase.ExecuteAsync(_sessionId.Value, tempsVisionne);
                _sessionTerminee = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la fermeture de la session : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
