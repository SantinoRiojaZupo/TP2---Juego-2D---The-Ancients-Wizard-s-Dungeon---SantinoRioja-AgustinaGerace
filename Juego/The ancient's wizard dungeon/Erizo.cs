
using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace The_ancient_s_wizard_dungeon
{
    public class Erizo
    {
        // Identificador único de cada erizo.
        public int Id { get; private set; }

        // Sprite y cuerpo de colisión.
        public TextureRegion Sprite { get; private set; }
        public CollisionBody Cuerpo { get; private set; }

        // Estado del enemigo.
        public int Vida { get; private set; }
        public bool EstaVivo { get; private set; }

        // Movimiento.
        private float _velocidad = 1.5f;
        private Vector2 _direccionRetroceso;

        // Retroceso y pausa después de tocar a un jugador.
        private bool _enPausa = false;
        private float _tiempoPausa = 0f;
        private float _tiempoRetroceso = 0f;
        private float _velocidadRetroceso = 2f;

        // Evita aplicar daño continuamente por el mismo contacto.
        private bool _tocandoJugador1 = false;
        private bool _tocandoJugador2 = false;

        public Vector2 Posicion
        {
            get { return Cuerpo.Position; }
        }

        public Erizo(int id, TextureRegion sprite, Vector2 posicion)
        {
            Id = id;
            Sprite = sprite;

            Vida = 3;
            EstaVivo = true;

            Cuerpo = new CollisionBody(
                new Point(sprite.Width * 2, sprite.Height * 2),
                posicion
            );
        }

        public void RecibirDaño(int cantidad)
        {
            if (!EstaVivo)
                return;

            Vida -= cantidad;

            if (Vida <= 0)
            {
                Vida = 0;
                EstaVivo = false;
            }
        }

        public void Reiniciar(Vector2 posicion)
        {
            Vida = 3;
            EstaVivo = true;

            Cuerpo.SetPosition(posicion);

            _enPausa = false;
            _tiempoPausa = 0f;
            _tiempoRetroceso = 0f;

            _tocandoJugador1 = false;
            _tocandoJugador2 = false;
        }

        public void Actualizar(
    GameTime gameTime,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    Rectangle limitesMapa,
    IReadOnlyList<Rana> ranas,
     IReadOnlyList<Erizo> erizos)
        {
            if (!EstaVivo)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Actualizamos las posiciones de los jugadores.
            CollisionBody cuerpoHeroe1 = jugador1.Cuerpo;
            CollisionBody cuerpoHeroe2 = jugador2.Cuerpo;

            List<CollisionBody> cuerposBloqueo = new List<CollisionBody>();

            if (jugador1.estaVivo)
            {
                cuerposBloqueo.Add(cuerpoHeroe1);
            }

            if (numeroJugadores == 2 && jugador2.estaVivo)
            {
                cuerposBloqueo.Add(cuerpoHeroe2);
            }

            foreach (Rana rana in ranas)
            {
                if (rana.EstaViva && !rana.Saltando)
                {
                    cuerposBloqueo.Add(rana.Cuerpo);
                }
            }
            foreach (Erizo erizo in erizos)
            {
                if (erizo != this && erizo.EstaVivo)
                {
                    cuerposBloqueo.Add(erizo.Cuerpo);
                }
            }

            // Si el erizo está en pausa, controla el retroceso.
            if (_enPausa)
            {
                _tiempoPausa -= deltaTime;

                if (_tiempoRetroceso > 0f)
                {
                    Vector2 retroceso = _direccionRetroceso
                                      * _velocidadRetroceso
                                      * deltaTime * 60f;
                    Rectangle proximoRetroceso = new Rectangle(
(int)(Cuerpo.Position.X + retroceso.X),
(int)(Cuerpo.Position.Y + retroceso.Y),
Cuerpo.Bounds.Width,
Cuerpo.Bounds.Height
);
                    if (limitesMapa.Contains(proximoRetroceso))
                    {

                        Cuerpo.Move(
     retroceso,
     cuerposBloqueo.ToArray()
 );

                    }
                }
                _tiempoRetroceso -= deltaTime;

                if (_tiempoPausa <= 0f)
                {
                    _enPausa = false;
                }

                return;
            }

            // Buscamos al jugador vivo más cercano.
            Jugador objetivo = null;

            if (jugador1.estaVivo)
                objetivo = jugador1;

            if (numeroJugadores == 2 && jugador2.estaVivo)
            {
                if (objetivo == null ||
                    Vector2.Distance(Posicion, jugador2.Cuerpo.Position) <
                    Vector2.Distance(Posicion, objetivo.Cuerpo.Position))
                {
                    objetivo = jugador2;
                }
            }

            // Si no hay jugadores vivos, no perseguimos a nadie.
            if (objetivo == null)
                return;

            Vector2 direccion = objetivo.Cuerpo.Position - Posicion;

            if (direccion != Vector2.Zero)
                direccion.Normalize();

            Vector2 movimiento = direccion * _velocidad * deltaTime * 60f;

            // Comprobamos dónde quedaría el erizo después de moverse.
            Rectangle proximoBounds = new Rectangle(
                (int)(Cuerpo.Position.X + movimiento.X),
                (int)(Cuerpo.Position.Y + movimiento.Y),
                Cuerpo.Bounds.Width,
                Cuerpo.Bounds.Height
            );

            bool chocoH1 = jugador1.estaVivo &&
                proximoBounds.Intersects(cuerpoHeroe1.Bounds);

            bool chocoH2 = numeroJugadores == 2 &&
                jugador2.estaVivo &&
                proximoBounds.Intersects(cuerpoHeroe2.Bounds);

            bool chocoRana = false;

            foreach (Rana rana in ranas)
            {
                if (rana.EstaViva && !rana.Saltando && proximoBounds.Intersects(rana.Cuerpo.Bounds))
                {
                    chocoRana = true;
                    break;
                }
            }

            // Reiniciamos las marcas cuando dejamos de tocar a un jugador.
            if (!chocoH1)
                _tocandoJugador1 = false;

            if (!chocoH2)
                _tocandoJugador2 = false;

            // Aplicamos daño una sola vez por contacto.
            if (chocoH1 && !_tocandoJugador1)
            {
                jugador1.RecibirDaño(1);
                _tocandoJugador1 = true;
            }

            if (chocoH2 && !_tocandoJugador2)
            {
                jugador2.RecibirDaño(1);
                _tocandoJugador2 = true;
            }

            // Si chocamos contra un jugador, retrocedemos y hacemos una pausa.
            if (chocoH1 || chocoH2)
            {
                Vector2 posicionHeroe = chocoH1
                    ? cuerpoHeroe1.Position
                    : cuerpoHeroe2.Position;

                _direccionRetroceso = Posicion - posicionHeroe;

                if (_direccionRetroceso != Vector2.Zero)
                    _direccionRetroceso.Normalize();

                _enPausa = true;
                _tiempoPausa = 1.5f;

                // Duración del movimiento de retroceso.
                _tiempoRetroceso = 0.25f;

                return;
            }

            // Si chocamos contra la rana, buscamos otro movimiento.
            if (chocoRana)
            {
                float distanciaMovimiento = movimiento.Length();

                Vector2[] opciones =
                {
            new Vector2(distanciaMovimiento, 0),
            new Vector2(-distanciaMovimiento, 0),
            new Vector2(0, distanciaMovimiento),
            new Vector2(0, -distanciaMovimiento)
        };

                Vector2 mejorMovimiento = Vector2.Zero;
                float mejorDistancia = float.MaxValue;

                foreach (Vector2 opcion in opciones)
                {
                    Rectangle posibleBounds = new Rectangle(
                        (int)(Posicion.X + opcion.X),
                        (int)(Posicion.Y + opcion.Y),
                        Cuerpo.Bounds.Width,
                        Cuerpo.Bounds.Height
                    );

                    bool libre = limitesMapa.Contains(posibleBounds);

                    foreach (Rana rana in ranas)
                    {
                        if (rana.EstaViva && !rana.Saltando && posibleBounds.Intersects(rana.Cuerpo.Bounds))
                        {
                            libre = false;
                            break;
                        }
                    }

                    libre = libre &&
                            !posibleBounds.Intersects(cuerpoHeroe1.Bounds) &&
                            (numeroJugadores != 2 ||
                             !posibleBounds.Intersects(cuerpoHeroe2.Bounds));

                    if (libre)
                    {
                        Vector2 posiblePosicion = Posicion + opcion;

                        float distancia = Vector2.Distance(
                            posiblePosicion,
                            objetivo.Cuerpo.Position
                        );

                        if (distancia < mejorDistancia)
                        {
                            mejorDistancia = distancia;
                            mejorMovimiento = opcion;
                        }
                    }
                }
                if (mejorMovimiento != Vector2.Zero)
                {
                    Cuerpo.Move(
                        mejorMovimiento,
                        cuerposBloqueo.ToArray()
                    );
                }
            }
            else if (limitesMapa.Contains(proximoBounds))
            {
                //persegir al jugador evitando cuerpos de colision
                Cuerpo.Move(
      movimiento,
      cuerposBloqueo.ToArray()
  );
            }
        }
        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (!EstaVivo)
                return;

            Sprite.Draw(
                spriteBatch,
                Posicion,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(2f, 2f),
                SpriteEffects.None,
                0f
            );
        }
    }
}
