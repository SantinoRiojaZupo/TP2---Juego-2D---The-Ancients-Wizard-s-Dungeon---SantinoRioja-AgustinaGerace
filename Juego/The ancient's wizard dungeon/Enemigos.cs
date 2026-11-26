
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;
using System;

namespace The_ancient_s_wizard_dungeon
{
    public class Enemigos
    {
        private List<Erizo> _erizos = new List<Erizo>();
        private List<Rana> _ranas = new List<Rana>();

        public IReadOnlyList<Rana> Ranas => _ranas;

        public IReadOnlyList<Erizo> Erizos => _erizos;

        private int _erizosPendientes = 0;
        private int _ranasPendientes = 0;

        private float _tiempoIntentoAparicion = 0f;
        private const float INTERVALO_APARICION = 2f;

        private Random _random = new Random();

        private const float DISTANCIA_MINIMA_JUGADORES = 150f;
        private const int INTENTOS_MAXIMOS_APARICION = 100;

        private bool BuscarPosicionValida(
            Point tamaño,
            Rectangle limitesMapa,
            Jugador jugador1,
            Jugador jugador2,
            int numeroJugadores,
            out Vector2 posicion)
        {
            posicion = Vector2.Zero;

            // buscamos hasta 100 posiciones aleatorias.
            for (int i = 0; i < INTENTOS_MAXIMOS_APARICION; i++)
            {
                int x = _random.Next(
                    limitesMapa.Left,
                    limitesMapa.Right - tamaño.X + 1
                );

                int y = _random.Next(
                    limitesMapa.Top,
                    limitesMapa.Bottom - tamaño.Y + 1
                );

                Vector2 posiblePosicion = new Vector2(x, y);

                Rectangle posiblesBounds = new Rectangle(
                    x,
                    y,
                    tamaño.X,
                    tamaño.Y
                );

                // comprobamos la distancia con los jugadores.
                if (jugador1.estaVivo)
                {
                    Vector2 centroJugador1 =
                        jugador1.Cuerpo.Position +
                        new Vector2(
                            jugador1.Cuerpo.Bounds.Width / 2f,
                            jugador1.Cuerpo.Bounds.Height / 2f
                        );

                    Vector2 centroEnemigo =
                        posiblePosicion +
                        new Vector2(tamaño.X / 2f, tamaño.Y / 2f);

                    if (Vector2.Distance(centroEnemigo, centroJugador1)
                        < DISTANCIA_MINIMA_JUGADORES)
                        continue;
                }

                if (numeroJugadores == 2 && jugador2.estaVivo)
                {
                    Vector2 centroJugador2 =
                        jugador2.Cuerpo.Position +
                        new Vector2(
                            jugador2.Cuerpo.Bounds.Width / 2f,
                            jugador2.Cuerpo.Bounds.Height / 2f
                        );

                    Vector2 centroEnemigo =
                        posiblePosicion +
                        new Vector2(tamaño.X / 2f, tamaño.Y / 2f);

                    if (Vector2.Distance(centroEnemigo, centroJugador2)
                        < DISTANCIA_MINIMA_JUGADORES)
                        continue;
                }

                // Comprobamos que no se superponga con otros erizos.
                bool ocupado = false;

                foreach (Erizo erizo in _erizos)
                {
                    if (erizo.EstaVivo &&
                        posiblesBounds.Intersects(erizo.Cuerpo.Bounds))
                    {
                        ocupado = true;
                        break;
                    }
                }

                if (ocupado)
                    continue;

                // Comprobamos que no se superponga con otras ranas.
                foreach (Rana rana in _ranas)
                {
                    if (rana.EstaViva &&
                        posiblesBounds.Intersects(rana.Cuerpo.Bounds))
                    {
                        ocupado = true;
                        break;
                    }
                }

                if (ocupado)
                {
                    continue;
                }

                posicion = posiblePosicion;
                return true;
            }

            return false;
        }

        private int _siguienteId = 1;

        //agregar
        public void AgregarErizo(TextureRegion sprite, Vector2 posicion)
        {
            Erizo nuevoErizo = new Erizo(
                _siguienteId,
                sprite,
                posicion
            );

            _erizos.Add(nuevoErizo);

            _siguienteId++;
        }
        public void AgregarRana(
       TextureRegion sprite,
       Animation animacionSalto,
       Animation animacionAterriza,
       Vector2 posicion)
        {
            Rana nuevaRana = new Rana(
                sprite,
                animacionSalto,
                animacionAterriza,
                posicion
            );

            _ranas.Add(nuevaRana);
        }

        public void Limpiar()
        {
            _erizos.Clear();
            _ranas.Clear();
        }
        public void PrepararOleada(
         int numeroOleada,
         TextureRegion spriteErizo,
         TextureRegion spriteRana,
         Animation animacionSaltoRana,
         Animation animacionAterrizaRana,
         Rectangle limitesMapa,
         Jugador jugador1,
         Jugador jugador2,
         int numeroJugadores)
        {
            Limpiar();

            int cantidadErizos = numeroOleada + 3211;
            int cantidadRanas = numeroOleada -1;

            _erizosPendientes = cantidadErizos;
            _ranasPendientes = cantidadRanas;
            _tiempoIntentoAparicion = 0f;

            // Creamos los erizos.
            for (int i = 0; i < cantidadErizos; i++)
            {
                if (BuscarPosicionValida(
                    new Point(spriteErizo.Width * 2, spriteErizo.Height * 2),
                    limitesMapa,
                    jugador1,
                    jugador2,
                    numeroJugadores,
                    out Vector2 posicion))
                {
                    AgregarErizo(spriteErizo, posicion);
                    _erizosPendientes--;
                }
            }

            // Creamos las ranas.
            for (int i = 0; i < cantidadRanas; i++)
            {
                if (BuscarPosicionValida(
                    new Point(spriteRana.Width * 2, spriteRana.Height * 2),
                    limitesMapa,
                    jugador1,
                    jugador2,
                    numeroJugadores,
                    out Vector2 posicion))
                {
                    AgregarRana(
                        spriteRana,
                        animacionSaltoRana,
                        animacionAterrizaRana,
                        posicion
                    );
                    _ranasPendientes--;
                }
            }
        }

        //intenta aparecer enemigos que no pudieron antes
        public void IntentarAparicionesPendientes(
    GameTime gameTime,
    TextureRegion spriteErizo,
    TextureRegion spriteRana,
    Animation animacionSaltoRana,
    Animation animacionAterrizaRana,
    Rectangle limitesMapa,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores)
        {
            if (!jugador1.estaVivo &&
            (numeroJugadores == 1 || !jugador2.estaVivo))
            {
                return;
            }
            if (_erizosPendientes <= 0 && _ranasPendientes <= 0)
                return;

            _tiempoIntentoAparicion +=
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_tiempoIntentoAparicion < INTERVALO_APARICION)
                return;

            _tiempoIntentoAparicion = 0f;

            if (_erizosPendientes > 0 &&
                BuscarPosicionValida(
                    new Point(spriteErizo.Width * 2, spriteErizo.Height * 2),
                    limitesMapa, jugador1, jugador2, numeroJugadores,
                    out Vector2 posicionErizo))
            {
                AgregarErizo(spriteErizo, posicionErizo);
                _erizosPendientes--;
            }

            if (_ranasPendientes > 0 &&
                BuscarPosicionValida(
                    new Point(spriteRana.Width * 2, spriteRana.Height * 2),
                    limitesMapa, jugador1, jugador2, numeroJugadores,
                    out Vector2 posicionRana))
            {
                AgregarRana(
                    spriteRana,
                    animacionSaltoRana,
                    animacionAterrizaRana,
                    posicionRana
                );

                _ranasPendientes--;
            }
        }
        public bool TieneEnemigosPendientes()
        {
            return _erizosPendientes > 0 || _ranasPendientes > 0;
        }
        //dibujar
        public void DibujarErizos(SpriteBatch spriteBatch)
        {
            foreach (Erizo erizo in _erizos)
            {
                erizo.Dibujar(spriteBatch);
            }
        }
        public void DibujarRanas(SpriteBatch spriteBatch)
        {
            foreach (Rana rana in _ranas)
            {
                rana.Dibujar(spriteBatch);
            }
        }
        public void ActualizarAnimacionesRanas(GameTime gameTime)
        {
            foreach (Rana rana in _ranas)
            {
                rana.ActualizarAnimaciones(gameTime);
            }
        }
        //cuerpos
        public void AgregarCuerposVivos(List<CollisionBody> cuerpos)
        {
            foreach (Erizo erizo in _erizos)
            {
                if (erizo.EstaVivo)
                {
                    cuerpos.Add(erizo.Cuerpo);
                }
            }
            foreach (Rana rana in _ranas)
            {
                if (rana.EstaViva)
                {
                    cuerpos.Add(rana.Cuerpo);
                }
            }
        }

        //actualizar vivos
        public void ActualizarErizos(
    GameTime gameTime,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    Rectangle limitesMapa)
        {
            foreach (Erizo erizo in _erizos)
            {
                erizo.Actualizar(
                    gameTime,
                    jugador1,
                    jugador2,
                    numeroJugadores,
                    limitesMapa,
                    _ranas,
                    _erizos
                );
            }
        }
        public void ActualizarRanas(
    GameTime gameTime,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    Rectangle limitesMapa)
        {
            foreach (Rana rana in _ranas)
            {
                rana.Actualizar(
                    gameTime,
                    jugador1,
                    jugador2,
                    numeroJugadores,
                    _erizos,
                    _ranas,
                    limitesMapa
                );
            }
        }
    }
}
