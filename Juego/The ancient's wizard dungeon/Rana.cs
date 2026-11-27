
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;
using System.Collections.Generic;

namespace The_ancient_s_wizard_dungeon
{
    public class Rana
    {
        // Estado de la rana.
        public int Vida { get; private set; } = 3;
        public bool EstaViva { get; private set; } = true;

        // Posición y colisiones.
        public Vector2 Posicion { get; set; }
        public CollisionBody Cuerpo { get; private set; }

        // Animaciones.
        private Animation _animacionSalto;
        private Animation _animacionAterriza;

        // Estado del salto.
        public bool Saltando { get; set; } = false;
        public Vector2 InicioSalto { get; set; }
        public Vector2 Destino { get; set; }

        // Temporizadores.
        public float TiempoMovimiento { get; set; } = 0f;
        public float IntervaloMovimiento { get; set; } = 0.7f;
        public float TiempoAnimacion { get; set; } = 0f;
        public float DuracionSalto { get; set; } = 2f;
        public float TiempoSalto { get; set; } = 1.5f;

        public Rana(
            TextureRegion sprite,
            Animation animacionSalto,
            Animation animacionAterriza,
            Vector2 posicion)
        {
            Posicion = posicion;

            _animacionSalto = new Animation(
     new List<TextureRegion>(animacionSalto.Frames),
     animacionSalto.Delay
 );

            _animacionAterriza = new Animation(
                new List<TextureRegion>(animacionAterriza.Frames),
                animacionAterriza.Delay
            );

            Cuerpo = new CollisionBody(
                new Point(sprite.Width * 2, sprite.Height * 2),
                posicion
            );
        }
        public bool RecibirDaño(int cantidad)
        {
            if (!EstaViva)
                return false;

            Vida -= cantidad;

            if (Vida <= 0)
            {
                Vida = 0;
                EstaViva = false;
                return true;
            }

            return false;
        }
        public TextureRegion FrameActual
        {
            get
            {
                if (Saltando)
                {
                    return _animacionAterriza.CurrentFrame;

                }

                return _animacionSalto.CurrentFrame;
            }
        }
        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (!EstaViva)
            {
                return;

            }

            FrameActual.Draw(
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
        public void ActualizarAnimaciones(GameTime gameTime)
        {
            _animacionSalto.Update(gameTime);

            if (Saltando)
            {
                _animacionAterriza.Update(gameTime);
            }

            TiempoSalto += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void Actualizar(
    GameTime gameTime,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    IReadOnlyList<Erizo> erizos,
    IReadOnlyList<Rana> ranas,
    Rectangle limitesMapa)
        {
            if (!EstaViva)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TiempoMovimiento += deltaTime;

            // Buscamos al jugador vivo más cercano.
            Vector2 objetivo = Vector2.Zero;
            bool hayObjetivo = false;

            if (jugador1.estaVivo)
            {
                objetivo = jugador1.Cuerpo.Position;
                hayObjetivo = true;
            }

            if (numeroJugadores == 2 && jugador2.estaVivo)
            {
                if (!hayObjetivo ||
                    Vector2.Distance(Posicion, jugador2.Cuerpo.Position) <
                    Vector2.Distance(Posicion, objetivo))
                {
                    objetivo = jugador2.Cuerpo.Position;
                    hayObjetivo = true;
                }
            }

            // Decidimos hacia dónde saltar.
            if (TiempoMovimiento >= IntervaloMovimiento && !Saltando && hayObjetivo)
            {
                TiempoMovimiento = -2.2f;

                int baldosa = 32;

                Vector2[] movimientos =
                {
            new Vector2 (baldosa * 2, baldosa),
            new Vector2 (baldosa * 2, -baldosa),
            new Vector2(-baldosa * 2, baldosa),
            new Vector2(-baldosa * 2, -baldosa),
            new Vector2 (baldosa, baldosa * 2),
            new Vector2 (baldosa, -baldosa * 2),
            new Vector2 (-baldosa, baldosa * 2),
            new Vector2 (-baldosa, -baldosa * 2)
        };

                Vector2 mejorPosicion = Posicion;
                float mejorDistancia = Vector2.Distance(Posicion, objetivo);

                foreach (Vector2 movimiento in movimientos)
                {
                    Vector2 posiblePosicion = Posicion + movimiento;

                    Rectangle posibleBounds = new Rectangle(
                        (int)posiblePosicion.X,
                        (int)posiblePosicion.Y,
                        Cuerpo.Bounds.Width,
                        Cuerpo.Bounds.Height
                    );

                    float distancia = Vector2.Distance(posiblePosicion, objetivo);

                    bool chocaHeroe1 = jugador1.estaVivo &&
                        posibleBounds.Intersects(jugador1.Cuerpo.Bounds);

                    bool chocaHeroe2 = numeroJugadores == 2 &&
                        jugador2.estaVivo &&
                        posibleBounds.Intersects(jugador2.Cuerpo.Bounds);

                    bool chocaErizo = false;

                    foreach (Erizo erizo in erizos)
                    {
                        if (erizo.EstaVivo &&
                            posibleBounds.Intersects(erizo.Cuerpo.Bounds))
                        {
                            chocaErizo = true;
                            break;
                        }
                    }
                    bool chocaRana = false;

                    foreach (Rana otraRana in ranas)
                    {
                        if (otraRana == this || !otraRana.EstaViva)
                            continue;

                        Rectangle boundsOtraRana;

                        if (otraRana.Saltando)
                        {
                            // Reservamos el lugar al que se dirige.
                            boundsOtraRana = new Rectangle(
                                (int)otraRana.Destino.X,
                                (int)otraRana.Destino.Y,
                                otraRana.Cuerpo.Bounds.Width,
                                otraRana.Cuerpo.Bounds.Height
                            );
                        }
                        else
                        {
                            boundsOtraRana = otraRana.Cuerpo.Bounds;
                        }

                        if (posibleBounds.Intersects(boundsOtraRana))
                        {
                            chocaRana = true;
                            break;
                        }
                    }

                    if (limitesMapa.Contains(posibleBounds) &&
                        !chocaHeroe1 &&
                        !chocaHeroe2 &&
                        !chocaErizo &&
                        !chocaRana &&
                        distancia < mejorDistancia)
                    {
                        mejorPosicion = posiblePosicion;
                        mejorDistancia = distancia;
                    }
                }

                if (mejorPosicion != Posicion)
                {
                    InicioSalto = Posicion;
                    Destino = mejorPosicion;
                    TiempoAnimacion = 0f;
                    Saltando = true;
                }
            }

            // ejecutamos el salto
            if (Saltando)
            {
                TiempoAnimacion += deltaTime;

                float progreso = MathHelper.Clamp(
                    TiempoAnimacion / DuracionSalto,
                    0f,
                    1f
                );

                Posicion = Vector2.Lerp(InicioSalto, Destino, progreso);
                Cuerpo.SetPosition(Posicion);

                if (TiempoAnimacion >= DuracionSalto)
                {
                    // termina el salto
                    if (PosicionLibre(
       Destino,
       jugador1,
       jugador2,
       numeroJugadores,
       erizos,
       ranas,
       limitesMapa))
                    {
                        Posicion = Destino;
                        Cuerpo.SetPosition(Posicion);
                        Saltando = false;
                    }
                    else
                    {
                        // El destino quedó ocupado durante el salto.
                        Saltando = false;

                        Reubicar(
                            jugador1,
                            jugador2,
                            numeroJugadores,
                            erizos,
                            ranas,
                            limitesMapa
                        );
                    }

                    bool recibioDaño = false;

                    if (ComprobarContacto(jugador1))
                    {
                        DañarJugador(jugador1);
                        recibioDaño = true;
                    }

                    if (numeroJugadores == 2 && ComprobarContacto(jugador2))
                    {
                        DañarJugador(jugador2);
                        recibioDaño = true;
                    }

                    if (recibioDaño)
                    {
                        Reubicar(
                            jugador1,
                            jugador2,
                            numeroJugadores,
                            erizos,
                            ranas,
                            limitesMapa
                        );
                    }

                    TiempoMovimiento = -2.2f;
                }
            }
        }
        private bool PosicionLibre(
    Vector2 posicion,
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    IReadOnlyList<Erizo> erizos,
    IReadOnlyList<Rana> ranas,
    Rectangle limitesMapa)
        {
            Rectangle posiblesBounds = new Rectangle(
                (int)posicion.X,
                (int)posicion.Y,
                Cuerpo.Bounds.Width,
                Cuerpo.Bounds.Height
            );

            if (!limitesMapa.Contains(posiblesBounds))
                return false;

           
            foreach (Erizo erizo in erizos)
            {
                if (erizo.EstaVivo &&
                    posiblesBounds.Intersects(erizo.Cuerpo.Bounds))
                    return false;
            }

            foreach (Rana otraRana in ranas)
            {
                if (otraRana != this &&
                    otraRana.EstaViva &&
                    posiblesBounds.Intersects(otraRana.Cuerpo.Bounds))
                    return false;
            }

            return true;
        }
        public bool ComprobarContacto(Jugador jugador)
        {
            if (!EstaViva || Saltando || !jugador.estaVivo)
            { return false; }

            return Cuerpo.Bounds.Intersects(jugador.Cuerpo.Bounds);
        }
        public void DañarJugador(Jugador jugador)
        {
            if (ComprobarContacto(jugador))
            {
                jugador._vidaJugadores.QuitarVida(2);
            }
        }
        private void Reubicar(
    Jugador jugador1,
    Jugador jugador2,
    int numeroJugadores,
    IReadOnlyList<Erizo> erizos,
    IReadOnlyList<Rana> ranas,
    Rectangle limitesMapa)
        {
            Vector2[] opciones =
            {
        new Vector2(64, 32),
        new Vector2(64, -32),
        new Vector2(-64, 32),
        new Vector2(-64, -32),
        new Vector2(32, 64),
        new Vector2(32, -64),
        new Vector2(-32, 64),
        new Vector2(-32, -64)
    };

            foreach (Vector2 opcion in opciones)
            {
                Vector2 posiblePosicion = Posicion + opcion;

                Rectangle posibleBounds = new Rectangle(
                    (int)posiblePosicion.X,
                    (int)posiblePosicion.Y,
                    Cuerpo.Bounds.Width,
                    Cuerpo.Bounds.Height
                );

                bool libre = limitesMapa.Contains(posibleBounds);

                if (jugador1.estaVivo &&
                    posibleBounds.Intersects(jugador1.Cuerpo.Bounds))
                {
                    libre = false;
                }

                if (numeroJugadores == 2 &&
                    jugador2.estaVivo &&
                    posibleBounds.Intersects(jugador2.Cuerpo.Bounds))
                {
                    libre = false;
                }

                foreach (Erizo erizo in erizos)
                {
                    if (erizo.EstaVivo &&
                        posibleBounds.Intersects(erizo.Cuerpo.Bounds))
                    {
                        libre = false;
                        break;
                    }
                }

                foreach (Rana otraRana in ranas)
                {
                    if (otraRana != this &&
                        otraRana.EstaViva &&
                        posibleBounds.Intersects(otraRana.Cuerpo.Bounds))
                    {
                        libre = false;
                        break;
                    }
                }

                if (libre)
                {
                    Posicion = posiblePosicion;
                    Cuerpo.SetPosition(Posicion);
                    return;
                }
            }
        }
    }
}
