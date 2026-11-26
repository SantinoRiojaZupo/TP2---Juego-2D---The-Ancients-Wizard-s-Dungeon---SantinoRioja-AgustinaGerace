
using System;

namespace The_ancient_s_wizard_dungeon
{
    public class Oleadas
    {
        //oleadas maximas para indicar si gano      
        private const int MAX_OLEADAS = 5;


        public int OleadaActual { get; private set; } = 1;


        public bool PartidaGanada { get; private set; } = false;

        // indica si puede avanzar la oleada
        public bool PuedeAvanzar { get; private set; } = false;

        // reiniciar todo con una nueva partida
        public void Reiniciar()
        {
            OleadaActual = 1;
            PartidaGanada = false;
            PuedeAvanzar = false;
        }

        public bool TodosLosEnemigosDerrotados(Enemigos enemigos)
        {
            if (enemigos.TieneEnemigosPendientes())
            {
    return false;
                
            }
            foreach (Erizo erizo in enemigos.Erizos)
            {
                if (erizo.EstaVivo)
                    return false;
            }

            foreach (Rana rana in enemigos.Ranas)
            {
                if (rana.EstaViva)
                    return false;
            }

            return true;
        }

        public void CompletarOleada()
        {
            PuedeAvanzar = true;
        }

        // avanza de oleada o se declara la victoria para el jugador
        public void AvanzarOleada()
        {
            if (!PuedeAvanzar || PartidaGanada)
                return;

            PuedeAvanzar = false;

            if (OleadaActual >= MAX_OLEADAS)
            {
                PartidaGanada = true;
                return;
            }

            OleadaActual++;
        }
    }
}
