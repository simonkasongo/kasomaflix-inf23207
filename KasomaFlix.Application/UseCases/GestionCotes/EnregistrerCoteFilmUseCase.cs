using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionCotes
{
    /// <summary>
    /// Enregistre ou met a jour la note d'un membre sur un film (contrainte unique membre+film).
    /// </summary>
    public class EnregistrerCoteFilmUseCase
    {
        private readonly ICoteRepository _cotes;
        private readonly IFilmRepository _films;

        public EnregistrerCoteFilmUseCase(ICoteRepository cotes, IFilmRepository films)
        {
            _cotes = cotes;
            _films = films;
        }

        public async Task ExecuteAsync(int membreId, int filmId, int note, string? commentaire)
        {
            if (note < 1 || note > 5)
            {
                throw new ArgumentException("La note doit etre entre 1 et 5.");
            }

            var film = await _films.GetByIdAsync(filmId);
            if (film == null)
            {
                throw new InvalidOperationException("Film introuvable.");
            }

            var existante = await _cotes.GetByMembreAndFilmAsync(membreId, filmId);
            if (existante != null)
            {
                existante.Note = note;
                existante.Commentaire = string.IsNullOrWhiteSpace(commentaire) ? null : commentaire.Trim();
                existante.DateCote = DateTime.Now;
                await _cotes.UpdateAsync(existante);
            }
            else
            {
                var c = new Cote
                {
                    MembreId = membreId,
                    FilmId = filmId,
                    Note = note,
                    Commentaire = string.IsNullOrWhiteSpace(commentaire) ? null : commentaire.Trim(),
                    DateCote = DateTime.Now
                };
                await _cotes.AddAsync(c);
            }

            await RafraichirMoyenneFilmAsync(filmId);
        }

        private async Task RafraichirMoyenneFilmAsync(int filmId)
        {
            var liste = (await _cotes.GetByFilmIdAsync(filmId)).ToList();
            if (liste.Count == 0)
            {
                return;
            }

            var film = await _films.GetByIdAsync(filmId);
            if (film == null)
            {
                return;
            }

            var moy = liste.Average(x => (decimal)x.Note);
            film.NoteMoyenne = Math.Round(moy, 2);
            film.NombreVotes = liste.Count;
            await _films.UpdateAsync(film);
        }
    }
}
