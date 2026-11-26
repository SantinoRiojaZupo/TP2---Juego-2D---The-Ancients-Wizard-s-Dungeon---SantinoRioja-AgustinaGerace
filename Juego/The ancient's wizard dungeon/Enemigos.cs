
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Physics;

namespace The_ancient_s_wizard_dungeon
{
    public class Enemigos
    {
        private List<Erizo> _erizos = new List<Erizo>();
        private List<Rana> _ranas = new List<Rana>();

        public IReadOnlyList<Rana> Ranas => _ranas;

        public IReadOnlyList<Erizo> Erizos => _erizos;

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
                    _ranas
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
