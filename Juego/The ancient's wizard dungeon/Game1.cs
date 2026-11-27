//using System.Numerics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using MonoGameLibrary.Physics;
//using System.Numerics;

namespace The_ancient_s_wizard_dungeon;
//cd '.\Juego\The ancient''s wizard dungeon\'
//despues
//dotnet mgcb-editor ./Content/Content.mgcb

public class Game1 : Core
{

    //estados del juego
    private enum EstadoJuego
    {
        MenuPrincipal,
        SeleccionJugadores,
        Jugando,
        Pausado,
        Opciones,
        Victoria,
        Derrota
    }
    private SpriteFont _fuente;
    private EstadoJuego _estadoAnteriorOpciones;
    private Tilemap _tileMap;
    private UIManager _uiManager;
    private Rectangle limitesMapa = new Rectangle(32, 160, 736, 416);

    private EstadoJuego _estadoActual = EstadoJuego.MenuPrincipal;
    private Texture2D pantallaInicial;
    private Texture2D principe1_spritesheetTexture;
    private Texture2D principe2_spritesheetTexture;
    private Texture2D _ErizoSpriteSheetTexture;
    private Texture2D _ranaSpriteSheetTexture;
    private Texture2D _esqueletoSpriteSheetTexture;
    private Texture2D _texturaElementos;
    private Texture2D _texturaBarra;

    //regiones
    private TextureRegion _regionErizoFrente;
    private TextureRegion principe1frente;
    private TextureRegion principe2frente;
    private TextureRegion _regionRanaFrente;

    //animaciones
    private Dictionary<string, Animation> _animacionesHeroe1;
    private Dictionary<string, Animation> _animacionesHeroe2;
    private string _estadoAnimacionHeroe1 = "Frente";
    private string _estadoAnimacionHeroe2 = "Frente";

    private Animation _animacionRanaSalto;
    private Animation _animacionRanaAterriza;

    private Vector2 _posicionRana = new Vector2(400, 300);

    //botones
    private Texture2D _botonesTexture;

    private TextureRegion _regionBotonIniciar;
    private TextureRegion _regionBoton1Jugador;
    private TextureRegion _regionBoton2Jugadores;
    private TextureRegion _regionBotonSalir;
    private TextureRegion _regionBotonOpciones;
    private TextureRegion _regionBotonVacio;




    private int _numeroJugadores = 1;

    //musica / sonidos
    private Song _musicaFondo;
    private float _volumenMusica = 0.5f;
    private KeyboardState _tecladoAnterior;


    //donde aparecen los jugadores
    private Jugador _jugador1;
    private Jugador _jugador2;

    //aparicion de enemigos
    private Enemigos _enemigos;
    private Oleadas _oleadas;


    private Vector2 _posicionHeroe1;
    private Vector2 _posicionHeroe2;

    private Rectangle rectanguloHeroe1;
    private Rectangle rectanguloHeroe2;

    private CollisionBody _cuerpoHeroe1;
    private CollisionBody _cuerpoHeroe2;

    private CollisionBody _cuerpoRana;


    private float velocidad = 2.0f;





    float tiempoTranscurrido = 0f;
    bool mostrarEnemigo = false;


    private int cantidadRanas = 3;

    private bool ranaViva = true;

    private int dañoAtaque = 1;

    // Dirección inicial: hacia abajo.
    private Vector2 direccionHeroe1 = new Vector2(0, 1);
    private Vector2 direccionHeroe2 = new Vector2(0, 1);

    //boton para iniciar
    private Rectangle _botonIniciarRect = new Rectangle(250, 420, 300, 60);

    // seleccionar jugadores
    private Rectangle _boton1JugadorRect = new Rectangle(250, 220, 300, 60);
    private Rectangle _boton2JugadoresRect = new Rectangle(250, 310, 300, 60);

    // Textura de 1x1 píxel para pintar formas y botones sólidos

    public Game1() : base("The ancien's wizard dungeon", 800, 600, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }



    protected override void LoadContent()
    {
        // Ejemplo de carga apuntando a la ruta relativa dentro de Content
        //TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        // Y luego extraes tus sprites individuales usando los nombres del XML
        //var jugadorSprite = atlas.CreateSprite("jugador_idle");
        // TODO: use this.Content to load your game content here

        base.LoadContent();

        //Barra progreso
        _texturaBarra = new Texture2D(((Game)this).GraphicsDevice, 1, 1);
        _texturaBarra.SetData(new[] { Color.White });
        //musica
        _musicaFondo = Content.Load<Song>("Audio/MusicaFondo");

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = _volumenMusica;
        MediaPlayer.Play(_musicaFondo);

        //cargamos las texturas del content
        _fuente = Content.Load<SpriteFont>("Fonts/Fuente");
        pantallaInicial = Content.Load<Texture2D>("PantallaInicial");
        principe1_spritesheetTexture = Content.Load<Texture2D>("principe1_spritesheet");
        principe2_spritesheetTexture = Content.Load<Texture2D>("principe2_spritesheet");
        _ErizoSpriteSheetTexture = Content.Load<Texture2D>("ErizoSpriteSheet");
        _botonesTexture = Content.Load<Texture2D>("botones");
        _ranaSpriteSheetTexture = Content.Load<Texture2D>("ranaSpriteSheet");
        _esqueletoSpriteSheetTexture = Content.Load<Texture2D>("esqueletoSpriteSheet");
        _texturaElementos = Content.Load<Texture2D>("ElementosSpriteSheet");


        // definimos regiones (TextureRegion toma: Texture, X, Y, Ancho, Alto)
        int anchoBoton = 57;
        int altoBoton = 11;
        int anchoerizo = 34;
        int altoerizo = 32;


        _regionBotonIniciar = new TextureRegion(
     _botonesTexture, 0, 0, anchoBoton, altoBoton);

        _regionBoton1Jugador = new TextureRegion(
            _botonesTexture, 0, 13, anchoBoton, altoBoton);

        _regionBoton2Jugadores = new TextureRegion(
            _botonesTexture, 0, 26, anchoBoton, altoBoton);

        _regionBotonSalir = new TextureRegion(
            _botonesTexture, 0, 39, anchoBoton, altoBoton);

        _regionBotonOpciones = new TextureRegion(
            _botonesTexture, 0, 52, anchoBoton, altoBoton);

        _regionBotonVacio = new TextureRegion(
            _botonesTexture, 0, 65, anchoBoton, altoBoton);

        // UIManager controla la navegación y el dibujo de los menús.
        _uiManager = new UIManager(
            _regionBotonIniciar,
            _regionBoton1Jugador,
              _regionBoton2Jugadores,
    _regionBotonSalir,
    _regionBotonOpciones,
    _regionBotonVacio
        );
        _regionErizoFrente = new TextureRegion(_ErizoSpriteSheetTexture, 34, 35, anchoerizo, altoerizo);
        principe1frente = new TextureRegion(principe1_spritesheetTexture, 31, 0, 22, 27);
        principe2frente = new TextureRegion(principe2_spritesheetTexture, 31, 0, 22, 27);
        _regionRanaFrente = new TextureRegion(_ranaSpriteSheetTexture, 0, 0, 15, 15);

        _enemigos = new Enemigos();
        _oleadas = new Oleadas();

        TextureAtlas atlasP1 = new TextureAtlas(principe1_spritesheetTexture);
        atlasP1.AddRegion("frente", 32, 0, 20, 25);
        atlasP1.AddRegion("frente_0", 0, 0, 20, 25);
        atlasP1.AddRegion("frente_1", 64, 0, 20, 25);
        atlasP1.AddRegion("der", 32, 33, 13, 26);
        atlasP1.AddRegion("der_0", 0, 32, 20, 28);
        atlasP1.AddRegion("der_1", 64, 32, 20, 28);
        atlasP1.AddRegion("izq", 32, 65, 13, 26);
        atlasP1.AddRegion("izq_0", 0, 64, 20, 28);
        atlasP1.AddRegion("izq_1", 64, 64, 20, 28);
        atlasP1.AddRegion("atras", 32, 96, 20, 25);
        atlasP1.AddRegion("atras_0", 0, 96, 20, 25);
        atlasP1.AddRegion("atras_1", 64, 96, 20, 25);
        atlasP1.AddRegion("ataqueFrente", 0, 128, 23, 32);
        atlasP1.AddRegion("ataqueDer", 0, 160, 31, 28);
        atlasP1.AddRegion("ataqueIzq", 0, 192, 31, 28);
        atlasP1.AddRegion("ataqueAtras", 0, 224, 23, 24);


        _animacionesHeroe1 = new Dictionary<string, Animation>
        {
            { "Frente", new Animation(new List<TextureRegion> { atlasP1.GetRegion("frente_0"),atlasP1.GetRegion("frente"), atlasP1.GetRegion("frente_1"),atlasP1.GetRegion("frente") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Izquierda", new Animation(new List<TextureRegion> { atlasP1.GetRegion("izq_0"),atlasP1.GetRegion("izq"), atlasP1.GetRegion("izq_1"),atlasP1.GetRegion("izq") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Derecha", new Animation(new List<TextureRegion> { atlasP1.GetRegion("der_0"),atlasP1.GetRegion("der"), atlasP1.GetRegion("der_1"),atlasP1.GetRegion("der") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Atras", new Animation(new List<TextureRegion> { atlasP1.GetRegion("atras_0"),atlasP1.GetRegion("atras"), atlasP1.GetRegion("atras_1"),atlasP1.GetRegion("atras") }, System.TimeSpan.FromMilliseconds(200)) },
            { "quietoFrente", new Animation(new List<TextureRegion> { atlasP1.GetRegion("frente") }, System.TimeSpan.FromMilliseconds(200)) },
             { "quietoIzquierda", new Animation(new List<TextureRegion> { atlasP1.GetRegion("izq") }, System.TimeSpan.FromMilliseconds(200)) },
              { "quietoDerecha", new Animation(new List<TextureRegion> { atlasP1.GetRegion("der") }, System.TimeSpan.FromMilliseconds(200)) },
               { "quietoAtras", new Animation(new List<TextureRegion> { atlasP1.GetRegion("atras") }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueFrente", new Animation(new List<TextureRegion> { atlasP1.GetRegion("ataqueFrente") }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueDer", new Animation(new List<TextureRegion> { atlasP1.GetRegion("ataqueDer") }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueIzq", new Animation(new List<TextureRegion> { atlasP1.GetRegion("ataqueIzq") }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueAtras", new Animation(new List<TextureRegion> { atlasP1.GetRegion("ataqueAtras") }, System.TimeSpan.FromMilliseconds(200)) }

        };

        TextureAtlas atlasP2 = new TextureAtlas(principe2_spritesheetTexture);
        atlasP2.AddRegion("frente", 32, 0, 20, 25);
        atlasP2.AddRegion("frente_0", 0, 0, 20, 25);
        atlasP2.AddRegion("frente_1", 64, 0, 20, 25);
        atlasP2.AddRegion("der", 32, 33, 13, 26);
        atlasP2.AddRegion("der_0", 0, 32, 20, 28);
        atlasP2.AddRegion("der_1", 64, 32, 20, 28);
        atlasP2.AddRegion("izq", 32, 65, 13, 26);
        atlasP2.AddRegion("izq_0", 0, 64, 20, 28);
        atlasP2.AddRegion("izq_1", 64, 64, 20, 28);
        atlasP2.AddRegion("atras", 32, 96, 20, 25);
        atlasP2.AddRegion("atras_0", 0, 96, 20, 25);
        atlasP2.AddRegion("atras_1", 64, 96, 20, 25);
        atlasP2.AddRegion("ataqueFrente", 0, 128, 23, 32);
        atlasP2.AddRegion("ataqueDer", 0, 160, 31, 28);
        atlasP2.AddRegion("ataqueIzq", 0, 192, 31, 28);
        atlasP2.AddRegion("ataqueAtras", 0, 224, 23, 24);
        _animacionesHeroe2 = new Dictionary<string, Animation>
        {
          { "Frente", new Animation(new List<TextureRegion> { atlasP2.GetRegion("frente_0"),atlasP2.GetRegion("frente"), atlasP2.GetRegion("frente_1"),atlasP2.GetRegion("frente") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Izquierda", new Animation(new List<TextureRegion> { atlasP2.GetRegion("izq_0"),atlasP2.GetRegion("izq"), atlasP2.GetRegion("izq_1"),atlasP2.GetRegion("izq") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Derecha", new Animation(new List<TextureRegion> { atlasP2.GetRegion("der_0"),atlasP2.GetRegion("der"), atlasP2.GetRegion("der_1"),atlasP2.GetRegion("der") }, System.TimeSpan.FromMilliseconds(200)) },
            { "Atras", new Animation(new List<TextureRegion> { atlasP2.GetRegion("atras_0"),atlasP2.GetRegion("atras"), atlasP2.GetRegion("atras_1"),atlasP2.GetRegion("atras") }, System.TimeSpan.FromMilliseconds(200)) },
            { "quietoFrente", new Animation(new List<TextureRegion> { atlasP2.GetRegion("frente") }, System.TimeSpan.FromMilliseconds(200)) },
             { "quietoIzquierda", new Animation(new List<TextureRegion> { atlasP2.GetRegion("izq") }, System.TimeSpan.FromMilliseconds(200)) },
              { "quietoDerecha", new Animation(new List<TextureRegion> { atlasP2.GetRegion("der") }, System.TimeSpan.FromMilliseconds(200)) },
               { "quietoAtras", new Animation(new List<TextureRegion> { atlasP2.GetRegion("atras") }, System.TimeSpan.FromMilliseconds(200)) },
                 { "AtaqueFrente", new Animation(new List<TextureRegion> { atlasP2.GetRegion("ataqueFrente"),atlasP2.GetRegion("frente") }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueDer", new Animation(new List<TextureRegion> { atlasP2.GetRegion("ataqueDer"),atlasP2.GetRegion("der"), }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueIzq", new Animation(new List<TextureRegion> { atlasP2.GetRegion("ataqueIzq"),atlasP2.GetRegion("izq"), }, System.TimeSpan.FromMilliseconds(200)) },
               { "AtaqueAtras", new Animation(new List<TextureRegion> { atlasP2.GetRegion("ataqueAtras"),atlasP2.GetRegion("atras"), }, System.TimeSpan.FromMilliseconds(200)) }
        };

        TextureAtlas atlasRana = new TextureAtlas(_ranaSpriteSheetTexture);
        atlasRana.AddRegion("base", 0, 0, 16, 16);
        atlasRana.AddRegion("salto", 16, 0, 16, 16);
        atlasRana.AddRegion("aterriza_0", 0, 16, 16, 16);
        atlasRana.AddRegion("aterriza_1", 16, 16, 16, 16);
        _animacionRanaSalto = new Animation(new List<TextureRegion> { atlasRana.GetRegion("base"), atlasRana.GetRegion("salto") }, System.TimeSpan.FromMilliseconds(500));
        _animacionRanaAterriza = new Animation(new List<TextureRegion> { atlasRana.GetRegion("aterriza_0"), atlasRana.GetRegion("aterriza_1"), atlasRana.GetRegion("aterriza_0"), atlasRana.GetRegion("aterriza_1") }, System.TimeSpan.FromMilliseconds(500));





        //cargamos todos los mapas xml
        _tileMap = Tilemap.FromFile(Content, "Levels/Level1.xml");

        // posicion heroes
        _posicionHeroe1 = new Vector2(100, 160);
        _posicionHeroe2 = new Vector2(100, 300);
        rectanguloHeroe1 = new Rectangle(100, 100, principe1frente.Width, principe1frente.Height);
        rectanguloHeroe2 = new Rectangle(180, 100, principe2frente.Width, principe2frente.Height);



        _cuerpoHeroe1 = new CollisionBody(
           new Point(20 * 3, 25 * 3),
         _posicionHeroe1
         );

        _cuerpoHeroe2 = new CollisionBody(
         new Point(20 * 3, 25 * 3),
        _posicionHeroe2
    );


        _cuerpoRana = new CollisionBody(
             new Point(_regionRanaFrente.Width * 2, _regionRanaFrente.Height * 2),
        _posicionRana
        );


        // escalamos el mapa para que sea mas grande ya que sino tienen que ser muchas baldosas
        _tileMap.Scale = new Vector2(2f, 2f);

        _jugador1 = new Jugador(_texturaElementos, _cuerpoHeroe1);
        _jugador2 = new Jugador(_texturaElementos, _cuerpoHeroe2);



    }

    protected override void Update(GameTime gameTime)
    {






        var tecladoActual = Keyboard.GetState();

        if (tecladoActual.IsKeyDown(Keys.Escape) &&
     _tecladoAnterior.IsKeyUp(Keys.Escape))
        {
            if (_estadoActual == EstadoJuego.Jugando)
            {
                _estadoActual = EstadoJuego.Pausado;
                _uiManager.ReiniciarSeleccion();
            }
            else if (_estadoActual == EstadoJuego.Pausado)
            {
                _estadoActual = EstadoJuego.Jugando;
            }
        }
        switch (_estadoActual)
        {

            case EstadoJuego.MenuPrincipal:

                _uiManager.Update(gameTime, 3);

                if (_uiManager.Confirmar)
                {
                    switch (_uiManager.SelectedIndex)
                    {
                        case 0:
                            // JUGAR
                            _estadoActual = EstadoJuego.SeleccionJugadores;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 1:
                            // OPCIONES
                            _estadoAnteriorOpciones = EstadoJuego.MenuPrincipal;
                            _estadoActual = EstadoJuego.Opciones;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 2:
                            // SALIR
                            Exit();
                            break;
                    }
                }
                break;

            case EstadoJuego.SeleccionJugadores:

                _uiManager.Update(gameTime, 2);

                if (_uiManager.Confirmar)
                {
                    _numeroJugadores = _uiManager.SelectedIndex + 1;
                    _estadoActual = EstadoJuego.Jugando;

                    _oleadas.Reiniciar();

                    cantidadRanas = 3;
                    ranaViva = true;
                    _jugador1._vidaJugadores.vidaActual = _jugador1._vidaJugadores.vidasTotal;
                    _jugador2._vidaJugadores.vidaActual = _jugador2._vidaJugadores.vidasTotal;
                    _jugador1.estaVivo = true;
                    _jugador2.estaVivo = true;
                    tiempoTranscurrido = 0f;

                    mostrarEnemigo = false;

                    _cuerpoHeroe1.SetPosition(_posicionHeroe1);
                    _cuerpoHeroe2.SetPosition(_posicionHeroe2);

                    _enemigos.PrepararOleada(
                        _oleadas.OleadaActual,
                        _regionErizoFrente,
                        _regionRanaFrente,
                        _animacionRanaSalto,
                        _animacionRanaAterriza,
                        limitesMapa,
                        _jugador1,
                        _jugador2,
                        _numeroJugadores
                    );
                }

                break;

            case EstadoJuego.Jugando:
                _jugador1.ActualizarAtaque(gameTime);
                _jugador2.ActualizarAtaque(gameTime);

                tiempoTranscurrido +=
                    (float)gameTime.ElapsedGameTime.TotalSeconds;


                Vector2 movimientoH1 = Vector2.Zero;
                Vector2 movimientoH2 = Vector2.Zero;

                bool moviendoH1 = false;
                bool moviendoH2 = false;

                if (tecladoActual.IsKeyDown(Keys.Space) && _tecladoAnterior.IsKeyUp(Keys.Space))
                {
                    _jugador1.IniciarAtaque();
                }
                if (tecladoActual.IsKeyDown(Keys.Left))
                {
                    direccionHeroe1 = new Vector2(-1, 0);
                    movimientoH1.X -= velocidad;
                    _estadoAnimacionHeroe1 = "Izquierda";
                    moviendoH1 = true;

                }
                else if (tecladoActual.IsKeyDown(Keys.Right))
                {
                    direccionHeroe1 = new Vector2(1, 0);
                    movimientoH1.X += velocidad;
                    _estadoAnimacionHeroe1 = "Derecha";
                    moviendoH1 = true;

                }
                if (tecladoActual.IsKeyDown(Keys.Down))
                {
                    direccionHeroe1 = new Vector2(0, 1);
                    movimientoH1.Y += velocidad;
                    if (!moviendoH1) _estadoAnimacionHeroe1 = "Frente";
                    moviendoH1 = true;

                }
                else if (tecladoActual.IsKeyDown(Keys.Up))
                {
                    direccionHeroe1 = new Vector2(0, -1);
                    movimientoH1.Y -= velocidad;
                    if (!moviendoH1) _estadoAnimacionHeroe1 = "Atras";
                    moviendoH1 = true;

                }
                // ANIMACION DEL JUGADOR 1
                if (_jugador1.AtaqueActivo)
                {
                    if (direccionHeroe1 == new Vector2(0, -1))
                        _estadoAnimacionHeroe1 = "AtaqueAtras";
                    else if (direccionHeroe1 == new Vector2(0, 1))
                        _estadoAnimacionHeroe1 = "AtaqueFrente";
                    else if (direccionHeroe1 == new Vector2(-1, 0))
                        _estadoAnimacionHeroe1 = "AtaqueIzq";
                    else
                        _estadoAnimacionHeroe1 = "AtaqueDer";

                    _animacionesHeroe1[_estadoAnimacionHeroe1].Update(gameTime);
                }
                else if (moviendoH1)
                {
                    _animacionesHeroe1[_estadoAnimacionHeroe1].Update(gameTime);
                }

                else
                {
                    if (direccionHeroe1 == new Vector2(0, -1))
                        _estadoAnimacionHeroe1 = "quietoAtras";
                    else if (direccionHeroe1 == new Vector2(0, 1))
                        _estadoAnimacionHeroe1 = "quietoFrente";
                    else if (direccionHeroe1 == new Vector2(-1, 0))
                        _estadoAnimacionHeroe1 = "quietoIzquierda";
                    else
                        _estadoAnimacionHeroe1 = "quietoDerecha";
                }
                //mov heroe 2


                if (_numeroJugadores == 2 && tecladoActual.IsKeyDown(Keys.F) && _tecladoAnterior.IsKeyUp(Keys.F))
                {
                    _jugador2.IniciarAtaque();
                }
                if (tecladoActual.IsKeyDown(Keys.A))
                {

                    direccionHeroe2 = new Vector2(-1, 0);
                    movimientoH2.X -= velocidad;
                    _estadoAnimacionHeroe2 = "Izquierda";
                    moviendoH2 = true;
                }
                else if (tecladoActual.IsKeyDown(Keys.D))
                {
                    direccionHeroe2 = new Vector2(1, 0);
                    movimientoH2.X += velocidad;
                    _estadoAnimacionHeroe2 = "Derecha";
                    moviendoH2 = true;
                }

                if (tecladoActual.IsKeyDown(Keys.S))
                {
                    direccionHeroe2 = new Vector2(0, 1);
                    movimientoH2.Y += velocidad;
                    if (!moviendoH2) _estadoAnimacionHeroe2 = "Frente";
                    moviendoH2 = true;
                }
                else if (tecladoActual.IsKeyDown(Keys.W))
                {
                    direccionHeroe2 = new Vector2(0, -1);
                    movimientoH2.Y -= velocidad;
                    if (!moviendoH2) _estadoAnimacionHeroe2 = "Atras";
                    moviendoH2 = true;
                }
                // ANIMACION DEL JUGADOR 2
                if (_numeroJugadores == 2)
                {
                    if (_jugador2.AtaqueActivo)
                    {
                        if (direccionHeroe2 == new Vector2(0, -1))
                            _estadoAnimacionHeroe2 = "AtaqueAtras";
                        else if (direccionHeroe2 == new Vector2(0, 1))
                            _estadoAnimacionHeroe2 = "AtaqueFrente";
                        else if (direccionHeroe2 == new Vector2(-1, 0))
                            _estadoAnimacionHeroe2 = "AtaqueIzq";
                        else
                            _estadoAnimacionHeroe2 = "AtaqueDer";

                        _animacionesHeroe2[_estadoAnimacionHeroe2].Update(gameTime);
                    }
                    else if (moviendoH2)
                    {
                        _animacionesHeroe2[_estadoAnimacionHeroe2].Update(gameTime);
                    }
                    else
                    {
                        if (direccionHeroe2 == new Vector2(0, -1))
                            _estadoAnimacionHeroe2 = "quietoAtras";
                        else if (direccionHeroe2 == new Vector2(0, 1))
                            _estadoAnimacionHeroe2 = "quietoFrente";
                        else if (direccionHeroe2 == new Vector2(-1, 0))
                            _estadoAnimacionHeroe2 = "quietoIzquierda";
                        else
                            _estadoAnimacionHeroe2 = "quietoDerecha";
                    }
                }


                _jugador1.ActualizarDireccion(direccionHeroe1);
                _jugador2.ActualizarDireccion(direccionHeroe2);
                // posicion hitbox
                _cuerpoHeroe1.SetPosition(_posicionHeroe1);
                _cuerpoHeroe2.SetPosition(_posicionHeroe2);

                _cuerpoRana.SetPosition(_posicionRana);

                // Movimiento del heroe 1
                List<CollisionBody> cuerposColision = new List<CollisionBody>();

                // Agregamos al jugador 2 si está jugando.
                if (_numeroJugadores == 2 && _jugador2.estaVivo)
                {
                    cuerposColision.Add(_cuerpoHeroe2);
                }



                // Agregamos los erizos vivos.
                if (mostrarEnemigo)
                {
                    _enemigos.AgregarCuerposVivos(cuerposColision);
                }

                // Movemos al jugador 1.
                _cuerpoHeroe1.Move(movimientoH1, cuerposColision.ToArray());

                // Movimiento del heroe 2
                if (_numeroJugadores == 2)
                {
                    List<CollisionBody> cuerposColisionH2 = new List<CollisionBody>();

                    // Agregamos al jugador 1 si esta vivo.
                    if (_jugador1.estaVivo)
                    {
                        cuerposColisionH2.Add(_cuerpoHeroe1);
                    }

                    // Agregamos los erizos vivos y ranas.
                    if (mostrarEnemigo)
                    {
                        _enemigos.AgregarCuerposVivos(cuerposColisionH2);
                    }

                    _cuerpoHeroe2.Move(movimientoH2, cuerposColisionH2.ToArray());
                }

                _cuerpoHeroe1.KeepInside(limitesMapa);

                if (_numeroJugadores == 2)
                {
                    _cuerpoHeroe2.KeepInside(limitesMapa);
                }


                _posicionHeroe1 = _cuerpoHeroe1.Position;
                _posicionHeroe2 = _cuerpoHeroe2.Position;

                rectanguloHeroe1.X = (int)_posicionHeroe1.X;
                rectanguloHeroe1.Y = (int)_posicionHeroe1.Y;

                rectanguloHeroe2.X = (int)_posicionHeroe2.X;
                rectanguloHeroe2.Y = (int)_posicionHeroe2.Y;

                if (!limitesMapa.Contains(rectanguloHeroe1))
                {
                    _posicionHeroe1.X = MathHelper.Clamp(_posicionHeroe1.X, 0, limitesMapa.Width - rectanguloHeroe1.Width);
                    _posicionHeroe1.Y = MathHelper.Clamp(_posicionHeroe1.Y, 0, limitesMapa.Height - rectanguloHeroe1.Height);
                }

                if (!limitesMapa.Contains(rectanguloHeroe2))
                {
                    _posicionHeroe2.X = MathHelper.Clamp(_posicionHeroe2.X, 0, limitesMapa.Width - rectanguloHeroe2.Width);
                    _posicionHeroe2.Y = MathHelper.Clamp(_posicionHeroe2.Y, 0, limitesMapa.Height - rectanguloHeroe2.Height);
                }

                mostrarEnemigo = false;
                if (tiempoTranscurrido >= 3f)
                {
                    mostrarEnemigo = true;
                }

                if (mostrarEnemigo)
                {
                    // Actualizamos la posición de los cuerpos de los jugadores.
                    _cuerpoHeroe1.SetPosition(_posicionHeroe1);
                    _cuerpoHeroe2.SetPosition(_posicionHeroe2);

                    // Actualizamos todos los erizos mediante el administrador.
                    _enemigos.ActualizarErizos(
     gameTime,
     _jugador1,
     _jugador2,
     _numeroJugadores,
     limitesMapa
 );
                    _enemigos.ActualizarAnimacionesRanas(gameTime);

                    _enemigos.ActualizarRanas(
                        gameTime,
                        _jugador1,
                        _jugador2,
                        _numeroJugadores,
                        limitesMapa
                    );

                    // Comprobamos si todos los jugadores fueron derrotados.
                    if (!_jugador1.estaVivo &&
                        (_numeroJugadores == 1 || !_jugador2.estaVivo))
                    {
                        _estadoActual = EstadoJuego.Derrota;
                    }
                }


                rectanguloHeroe1.X = (int)_posicionHeroe1.X;
                rectanguloHeroe1.Y = (int)_posicionHeroe1.Y;

                rectanguloHeroe2.X = (int)_posicionHeroe2.X;
                rectanguloHeroe2.Y = (int)_posicionHeroe2.Y;

                _cuerpoHeroe1.SetPosition(_posicionHeroe1);
                _cuerpoHeroe2.SetPosition(_posicionHeroe2);

                _cuerpoRana.SetPosition(_posicionRana);

                // ATAQUE DEL JUGADOR 1
                if (_jugador1.AtaqueActivo && mostrarEnemigo)
                {
                    Rectangle hitbox = _jugador1.CrearHitboxAtaque();


                    //erizo
                    foreach (Erizo erizo in _enemigos.Erizos)
                    {
                        if (erizo.EstaVivo &&
                            _jugador1.PuedeGolpear(erizo.Id) &&
                            CollisionBody.Intersectan(hitbox, erizo.Cuerpo))
                        {
                            erizo.RecibirDaño(dañoAtaque);

                            _jugador1.RegistrarGolpe(erizo.Id);


                        }
                    }

                    // RANA
                    foreach (Rana rana in _enemigos.Ranas)
                    {
                        if (rana.EstaViva &&
                            _jugador1.PuedeGolpear(rana.GetHashCode()) &&
                            CollisionBody.Intersectan(hitbox, rana.Cuerpo))
                        {
                            bool murio = rana.RecibirDaño(dañoAtaque);

                            _jugador1.RegistrarGolpe(rana.GetHashCode());

                            if (murio)
                            {
                                cantidadRanas--;
                            }
                        }
                    }
                }

                // ATAQUE DEL JUGADOR 2
                if (_numeroJugadores == 2 &&
                    _jugador2.AtaqueActivo &&
                    mostrarEnemigo)
                {
                    Rectangle hitbox = _jugador2.CrearHitboxAtaque();

                    foreach (Erizo erizo in _enemigos.Erizos)
                    {
                        if (erizo.EstaVivo &&
                            _jugador2.PuedeGolpear(erizo.Id) &&
                            CollisionBody.Intersectan(hitbox, erizo.Cuerpo))
                        {
                            erizo.RecibirDaño(dañoAtaque);

                            _jugador2.RegistrarGolpe(erizo.Id);


                        }
                    }

                    // RANA
                    foreach (Rana rana in _enemigos.Ranas)
                    {
                        if (rana.EstaViva &&
                            _jugador2.PuedeGolpear(rana.GetHashCode()) &&
                            CollisionBody.Intersectan(hitbox, rana.Cuerpo))
                        {
                            bool murio = rana.RecibirDaño(dañoAtaque);

                            _jugador2.RegistrarGolpe(rana.GetHashCode());

                            if (murio)
                            {
                                cantidadRanas--;
                            }
                        }
                    }
                }
                if (_jugador1._vidaJugadores.SinVida)
                {
                    _jugador1.muere();
                }

                if (_numeroJugadores == 2 && _jugador2._vidaJugadores.SinVida)
                {
                    _jugador2.muere();
                }

                // cambia a derrota si todos los jugadores están muertos.
                if (_numeroJugadores == 1 && !_jugador1.estaVivo)
                {
                    _estadoActual = EstadoJuego.Derrota;
                }
                else if (_numeroJugadores == 2 &&
                         !_jugador1.estaVivo &&
                         !_jugador2.estaVivo)
                {
                    _estadoActual = EstadoJuego.Derrota;
                }

                //REAPARECEMOS ENEMIGOS SI NO PUDIERON APARECER
                _enemigos.IntentarAparicionesPendientes(
                    gameTime,
                    _regionErizoFrente,
                    _regionRanaFrente,
                    _animacionRanaSalto,
                    _animacionRanaAterriza,
                    limitesMapa,
                    _jugador1,
                    _jugador2,
                    _numeroJugadores
                );
                _enemigos.ActualizarProgreso();

                if (!ranaViva && cantidadRanas > 0)
                {
                    ranaViva = true;

                    _posicionRana = new Vector2(500, 400);
                    _cuerpoRana.SetPosition(_posicionRana);



                }
                if (_oleadas.TodosLosEnemigosDerrotados(_enemigos))
                {
                    _oleadas.CompletarOleada();
                    _oleadas.AvanzarOleada();

                    if (_oleadas.PartidaGanada)
                    {
                        _estadoActual = EstadoJuego.Victoria;
                    }
                    else
                    {
                        _enemigos.PrepararOleada(
                            _oleadas.OleadaActual,
                            _regionErizoFrente,
                            _regionRanaFrente,
                            _animacionRanaSalto,
                            _animacionRanaAterriza,
                            limitesMapa,
                            _jugador1,
                            _jugador2,
                            _numeroJugadores
                        );
                    }
                }

                break;

            //PAUSAR EL JUEGO
            case EstadoJuego.Pausado:

                _uiManager.Update(gameTime, 3);

                if (_uiManager.Confirmar)
                {
                    switch (_uiManager.SelectedIndex)
                    {
                        case 0:
                            // CONTINUAR
                            _estadoActual = EstadoJuego.Jugando;
                            break;

                        case 1:
                            // OPCIONES
                            _estadoAnteriorOpciones = EstadoJuego.Pausado;
                            _estadoActual = EstadoJuego.Opciones;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 2:
                            // VOLVER AL MENU
                            _estadoActual = EstadoJuego.MenuPrincipal;
                            _uiManager.ReiniciarSeleccion();
                            break;
                    }
                }

                break;
            case EstadoJuego.Opciones:

                _uiManager.Update(gameTime, 1);

                // Bajar volumen
                if (tecladoActual.IsKeyDown(Keys.Left) &&
                    _tecladoAnterior.IsKeyUp(Keys.Left))
                {
                    _volumenMusica = MathHelper.Clamp(
                        _volumenMusica - 0.1f, 0f, 1f
                    );

                    MediaPlayer.Volume = _volumenMusica;
                }

                // Subir volumen
                if (tecladoActual.IsKeyDown(Keys.Right) &&
                    _tecladoAnterior.IsKeyUp(Keys.Right))
                {
                    _volumenMusica = MathHelper.Clamp(
                        _volumenMusica + 0.1f, 0f, 1f
                    );

                    MediaPlayer.Volume = _volumenMusica;
                }

                if (_uiManager.Confirmar)
                {
                    _estadoActual = _estadoAnteriorOpciones;
                    _uiManager.ReiniciarSeleccion();
                }

                break;
            case EstadoJuego.Derrota:

                _uiManager.Update(gameTime, 3);

                if (_uiManager.Confirmar)
                {
                    switch (_uiManager.SelectedIndex)
                    {
                        case 0:
                            // JUGAR DE NUEVO
                            _estadoActual = EstadoJuego.SeleccionJugadores;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 1:
                            // VOLVER AL MENU PRINCIPAL
                            _estadoActual = EstadoJuego.MenuPrincipal;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 2:
                            // SALIR
                            Exit();
                            break;
                    }
                }

                break;

            case EstadoJuego.Victoria:

                _uiManager.Update(gameTime, 3);

                if (_uiManager.Confirmar)
                {
                    switch (_uiManager.SelectedIndex)
                    {
                        case 0:
                            // JUGAR DE NUEVO
                            _estadoActual = EstadoJuego.SeleccionJugadores;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 1:
                            // VOLVER AL MENU PRINCIPAL
                            _estadoActual = EstadoJuego.MenuPrincipal;
                            _uiManager.ReiniciarSeleccion();
                            break;

                        case 2:
                            // SALIR
                            Exit();
                            break;
                    }
                }

                break;
        }
        // TODO: Add your update logic here


        _tecladoAnterior = tecladoActual;

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointWrap);

        if (_estadoActual == EstadoJuego.MenuPrincipal)
        {


            //pantalla inicio
            float escalaXTitulo = (float)Window.ClientBounds.Width / pantallaInicial.Width;
            float escalaYTitulo = (float)Window.ClientBounds.Height / pantallaInicial.Height;
            SpriteBatch.Draw(
                            pantallaInicial,
                            Vector2.Zero,
                            null,
                            Color.White,
                            0,
                            Vector2.Zero,
                            new Vector2(escalaXTitulo, escalaYTitulo),
                            SpriteEffects.None,
                            0.0f
                        );


            _uiManager.DrawMenuPrincipal(SpriteBatch);
        }

        else if (_estadoActual == EstadoJuego.SeleccionJugadores)
        {

            GraphicsDevice.Clear(Color.DarkSlateGray);

            _uiManager.DrawSeleccionJugadores(SpriteBatch);

        }
        else if (_estadoActual == EstadoJuego.Opciones &&
            _estadoAnteriorOpciones == EstadoJuego.MenuPrincipal)
        {
            float escalaXTitulo = (float)Window.ClientBounds.Width / pantallaInicial.Width;
            float escalaYTitulo = (float)Window.ClientBounds.Height / pantallaInicial.Height;

            SpriteBatch.Draw(
                pantallaInicial,
                Vector2.Zero,
                null,
                Color.White,
                0,
                Vector2.Zero,
                new Vector2(escalaXTitulo, escalaYTitulo),
                SpriteEffects.None,
                0.0f
            );

            _uiManager.DrawOpciones(SpriteBatch, _volumenMusica);
        }
        else if (
     _estadoActual == EstadoJuego.Jugando || _estadoActual == EstadoJuego.Pausado || (_estadoActual == EstadoJuego.Opciones && _estadoAnteriorOpciones == EstadoJuego.Pausado))
        {
            _tileMap.Draw(SpriteBatch);
            if (_jugador1.estaVivo)
            {
                TextureRegion frameHeroe1 = _animacionesHeroe1[_estadoAnimacionHeroe1].CurrentFrame;

                frameHeroe1.Draw(
                    SpriteBatch,
                    new Vector2(_posicionHeroe1.X, _posicionHeroe1.Y),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(3f, 3f),
                    SpriteEffects.None,
                    0f);
            }
            if (_numeroJugadores == 2 && _jugador2.estaVivo)
            {

                TextureRegion frameHeroe2 = _animacionesHeroe2[_estadoAnimacionHeroe2].CurrentFrame;
                frameHeroe2.Draw(
                   SpriteBatch,
                   new Vector2(_posicionHeroe2.X, _posicionHeroe2.Y),
                   Color.White,
                   0f,
                   Vector2.Zero,
                   new Vector2(3f, 3f),
                   SpriteEffects.None,
                   0f);
            }
            // Dibujar vidas del jugador 1
            _jugador1.DibujarVida(
                SpriteBatch,
                new Vector2(20, 15)
            );


            // Dibujar vidas del jugador 2 solamente si está jugando
            // una partida de dos jugadores
            if (_numeroJugadores == 2)
            {
                _jugador2.DibujarVida(
                    SpriteBatch,
                    new Vector2(672, 15)
                );

            }

            string textoOleada = $"OLEADA {_oleadas.OleadaActual} / 5";

            Vector2 posicionTexto = new Vector2(300, 2);

            SpriteBatch.DrawString(
                _fuente,
                textoOleada,
                posicionTexto,
                Color.White
            );

            // Barra de de la oleada
            Rectangle fondoBarra = new Rectangle(300, 25, 200, 12);

            SpriteBatch.Draw(
                _texturaBarra,
                fondoBarra,
                Color.DarkGray
            );
            //progreso de la barra de la oleada
            int anchoRelleno = (int)(200 * _enemigos.ProgresoOleada);

            Rectangle rellenoBarra = new Rectangle(300, 25, anchoRelleno, 12);

            SpriteBatch.Draw(
                _texturaBarra,
                rellenoBarra,
                Color.Gold
            );
            //dibujar enemigos
            if (mostrarEnemigo == true)
            {
                _enemigos.DibujarErizos(SpriteBatch);

                _enemigos.DibujarRanas(SpriteBatch);



            }
            if (_estadoActual == EstadoJuego.Pausado)
            {
                _uiManager.DrawMenuPausa(SpriteBatch);
            }
            else if (_estadoActual == EstadoJuego.Opciones &&
                     _estadoAnteriorOpciones == EstadoJuego.Pausado)
            {
                _uiManager.DrawOpciones(SpriteBatch, _volumenMusica);
            }

        }

        if (_estadoActual == EstadoJuego.Derrota)
        {
            SpriteBatch.DrawString(
                _fuente,
                "Game Over",
                new Vector2(300, 100),
                Color.White
            );

            _uiManager.DrawMenuPausa(SpriteBatch);
        }
        if (_estadoActual == EstadoJuego.Victoria)
        {
            SpriteBatch.DrawString(
                _fuente,
                "¡GANASTE!",
                new Vector2(300, 200),
                Color.White
            );

            _uiManager.DrawMenuPausa(SpriteBatch);
        }


        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
