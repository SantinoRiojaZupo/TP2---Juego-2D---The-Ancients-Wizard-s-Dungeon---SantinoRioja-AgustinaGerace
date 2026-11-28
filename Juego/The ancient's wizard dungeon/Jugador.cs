using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Physics;

namespace The_ancient_s_wizard_dungeon
{



    public class Jugador
    {
        public VidaJugadores _vidaJugadores;

        public int _puntuacion;
        public bool invulnerable = false;

        public CollisionBody Cuerpo { get; private set; }
        private Vector2 _direccion = new Vector2(0, 1);
        private int _alcanceAtaque = 24;
        private float duracionInvulnerabilidad = 1.3f;
        private float tiempoInvulnerabilidad = 0f;

        private float _tiempoTitileo = 0f;
        private bool _mostrarJugador = true;
        public bool MostrarJugador => _mostrarJugador;

        private float _duracionAtaque = 0.2f;
        private float _tiempoAtaque = 0f;

        public bool AtaqueActivo { get; private set; }

        private HashSet<int> _enemigosGolpeados = new HashSet<int>();

        public bool estaVivo = true;



        public Jugador(Texture2D texturaVida, CollisionBody cuerpo)
        {
            _vidaJugadores = new VidaJugadores(texturaVida);
            _puntuacion = 0;
            Cuerpo = cuerpo;

        }
        public void DibujarVida(SpriteBatch spriteBatch, Vector2 posicion)
        {
            _vidaJugadores.Dibujar(spriteBatch, posicion);
        }
        public void muere()
        {
            estaVivo = false;

        }
        public void Revivir()
        {
            estaVivo = true;
            _vidaJugadores.ReiniciarVida();
        }
        public bool RecibirDaño(int cantidad)
        {
            if (!estaVivo || invulnerable)
                return false;

            _vidaJugadores.QuitarVida(cantidad);

            invulnerable = true;
            tiempoInvulnerabilidad = duracionInvulnerabilidad;

            if (_vidaJugadores.SinVida)
                muere();

            return true;
        }

        public void ActualizarInvulnerabilidad(GameTime gameTime)
        {
            if (!invulnerable)
            {
                return;
            }

            float deltaTime =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            tiempoInvulnerabilidad -= deltaTime;

            // aca vamos a controlar el titileo o efecto cuando atacan al jugador
            _tiempoTitileo += deltaTime;

            if (_tiempoTitileo >= 0.1f)
            {
                _tiempoTitileo = 0f;
                _mostrarJugador = !_mostrarJugador;
            }

            if (tiempoInvulnerabilidad <= 0f)
            {
                tiempoInvulnerabilidad = 0f;
                invulnerable = false;

                _mostrarJugador = true;
                _tiempoTitileo = 0f;
            }
        }
        public void ActualizarDireccion(Vector2 direccion)
        {
            // Solo actualizamos si hay una dirección válida.
            if (direccion != Vector2.Zero)
            {
                _direccion = direccion;
            }
        }
        public Rectangle CrearHitboxAtaque()
        {
            Rectangle cuerpo = Cuerpo.Bounds;

            if (_direccion == new Vector2(0, -1)) // Arriba
            {
                return new Rectangle(
                    cuerpo.X,
                    cuerpo.Y - _alcanceAtaque,
                    cuerpo.Width,
                    _alcanceAtaque
                );
            }
            else if (_direccion == new Vector2(0, 1)) // Abajo
            {
                return new Rectangle(
                    cuerpo.X,
                    cuerpo.Bottom,
                    cuerpo.Width,
                    _alcanceAtaque
                );
            }
            else if (_direccion == new Vector2(-1, 0)) // Izquierda
            {
                return new Rectangle(
                    cuerpo.X - _alcanceAtaque,
                    cuerpo.Y,
                    _alcanceAtaque,
                    cuerpo.Height
                );
            }
            else // Derecha
            {
                return new Rectangle(
                    cuerpo.Right,
                    cuerpo.Y,
                    _alcanceAtaque,
                    cuerpo.Height
                );
            }


        }

        public void IniciarAtaque()
        {
            if (AtaqueActivo)
                return;

            AtaqueActivo = true;
            _tiempoAtaque = _duracionAtaque;

            // Permitimos volver a golpear a los enemigos
            // cuando comience un nuevo ataque.
            _enemigosGolpeados.Clear();
        }

        public void ActualizarAtaque(GameTime gameTime)
        {
            if (!AtaqueActivo)
                return;

            _tiempoAtaque -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_tiempoAtaque <= 0)
            {
                AtaqueActivo = false;
            }
        }

        public bool PuedeGolpear(int idEnemigo)
        {
            return AtaqueActivo && !_enemigosGolpeados.Contains(idEnemigo);
        }

        public void RegistrarGolpe(int idEnemigo)
        {
            _enemigosGolpeados.Add(idEnemigo);
        }
    }

}
