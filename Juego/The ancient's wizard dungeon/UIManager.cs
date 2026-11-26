
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;

namespace The_ancient_s_wizard_dungeon
{
    public class UIManager
    {
     private TextureRegion _botonIniciar;
private TextureRegion _boton1Jugador;
private TextureRegion _boton2Jugadores;
private TextureRegion _botonSalir;
private TextureRegion _botonOpciones;
private TextureRegion _botonVacio;

        private int _selectedIndex = 0;

        private KeyboardState _previousKeyboardState;

        // Indica si se acaba de confirmar una opción
        public bool Confirmar { get; private set; }

        // Indica qué opción está seleccionada
        public int SelectedIndex => _selectedIndex;

        public UIManager(
            TextureRegion botonIniciar,
            TextureRegion boton1Jugador,
             TextureRegion boton2Jugadores,
    TextureRegion botonSalir,
    TextureRegion botonOpciones,
    TextureRegion botonVacio)
        {
            _botonIniciar = botonIniciar;
            _boton1Jugador = boton1Jugador;
            _boton2Jugadores = boton2Jugadores;
             _botonSalir = botonSalir;
    _botonOpciones = botonOpciones;
    _botonVacio = botonVacio;

            _selectedIndex = 0;
        }

public void DrawMenuPausa(SpriteBatch spriteBatch)
{
    DibujarBoton(
        spriteBatch,
        _botonIniciar,
        new Vector2(250, 250),
        _selectedIndex == 0 ? Color.Gold : Color.White
    );

    DibujarBoton(
        spriteBatch,
        _botonOpciones,
        new Vector2(250, 340),
        _selectedIndex == 1 ? Color.Gold : Color.White
    );

    DibujarBoton(
        spriteBatch,
        _botonSalir,
        new Vector2(250, 430),
        _selectedIndex == 2 ? Color.Gold : Color.White
    );
}
public void DrawOpciones(SpriteBatch spriteBatch,float volumen)
{
  // Calculamos cuántos segmentos deben estar llenos.
    int segmentosLlenos = (int)(volumen * 10f + 0.5f);

    

    // Dibujamos la barra de volumen.
    for (int i = 0; i < 10; i++)
    {
        Color color = i < segmentosLlenos
            ? Color.Gold
            : Color.White;

        _botonVacio.Draw(
            spriteBatch,
            new Vector2(250 + i * 42, 270),
            color,
            0f,
            Vector2.Zero,
            new Vector2(0.3f, 0.3f),
            SpriteEffects.None,
            0f
        );
    }

    // Botón para regresar.
    DibujarBoton(
        spriteBatch,
        _botonSalir,
        new Vector2(250, 420),
        Color.Gold
    );
}
private void DibujarBoton(
    SpriteBatch spriteBatch,
    TextureRegion boton,
    Vector2 posicion,
    Color color)
{
    boton.Draw(
        spriteBatch,
        posicion,
        color,
        0f,
        Vector2.Zero,
        new Vector2(4f, 4f),
        SpriteEffects.None,
        0f
    );
}
        public void Update(GameTime gameTime, int cantidadOpciones)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            Confirmar = false;

        

            // Navegar hacia abajo
            if (
                (currentKeyboardState.IsKeyDown(Keys.Down) &&
                 _previousKeyboardState.IsKeyUp(Keys.Down))
                ||
                (currentKeyboardState.IsKeyDown(Keys.S) &&
                 _previousKeyboardState.IsKeyUp(Keys.S))
            )
            {
                _selectedIndex++;
            }

            // Navegar hacia arriba
            if (
                (currentKeyboardState.IsKeyDown(Keys.Up) &&
                 _previousKeyboardState.IsKeyUp(Keys.Up))
                ||
                (currentKeyboardState.IsKeyDown(Keys.W) &&
                 _previousKeyboardState.IsKeyUp(Keys.W))
            )
            {
                _selectedIndex--;
            }

if (cantidadOpciones > 0)
{
    _selectedIndex =
        (_selectedIndex + cantidadOpciones) % cantidadOpciones;
}
else
{
    _selectedIndex = 0;
}
            // Confirmar con Enter
            if (
                (currentKeyboardState.IsKeyDown(Keys.Enter) &&
                _previousKeyboardState.IsKeyUp(Keys.Enter))||(currentKeyboardState.IsKeyDown(Keys.Space) &&
                _previousKeyboardState.IsKeyUp(Keys.Space))
            )
            {
                Confirmar = true;
            }

            // Guardamos el estado del teclado
            _previousKeyboardState = currentKeyboardState;
        }

        // Reinicia la selección al entrar en una pantalla
        public void ReiniciarSeleccion()
        {
            _selectedIndex = 0;
    _previousKeyboardState = Keyboard.GetState();
        }

        // Dibuja el menú principal
        public void DrawMenuPrincipal(SpriteBatch spriteBatch)
{
    Color colorIniciar =
        (_selectedIndex == 0) ? Color.Gold : Color.White;

    Color colorOpciones =
        (_selectedIndex == 1) ? Color.Gold : Color.White;

    Color colorSalir =
        (_selectedIndex == 2) ? Color.Gold : Color.White;

    DibujarBoton(
        spriteBatch,
        _botonIniciar,
        new Vector2(250, 250),
        colorIniciar
    );

    DibujarBoton(
        spriteBatch,
        _botonOpciones,
        new Vector2(250, 340),
        colorOpciones
    );

    DibujarBoton(
        spriteBatch,
        _botonSalir,
        new Vector2(250, 430),
        colorSalir
    );
}


        // Dibuja la pantalla de selección de jugadores
        public void DrawSeleccionJugadores(SpriteBatch spriteBatch)
        {
            Color colorBoton1 =
                (_selectedIndex == 0) ? Color.Gold : Color.White;

            Color colorBoton2 =
                (_selectedIndex == 1) ? Color.Gold : Color.White;

            _boton1Jugador.Draw(
                spriteBatch,
                new Vector2(250, 220),
                colorBoton1,
                0f,
                Vector2.Zero,
                new Vector2(4f, 4f),
                SpriteEffects.None,
                0f
            );

            _boton2Jugadores.Draw(
                spriteBatch,
                new Vector2(250, 310),
                colorBoton2,
                0f,
                Vector2.Zero,
                new Vector2(4f, 4f),
                SpriteEffects.None,
                0f
            );
        }
    }
    
}
