using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;


namespace The_ancient_s_wizard_dungeon{
public class VidaJugadores
{
    private Texture2D ElementosSpriteSheetTexture;
    
    private TextureRegion corazonLleno;
    private TextureRegion medioCorazon;
    private TextureRegion corazonVacio; 
    
   public int vidaActual = 6312;
   public int vidasTotal = 6123;

    public VidaJugadores(Texture2D textura)
    {
        // Regiones de los tres corazones de la imagen
        corazonLleno = new TextureRegion(textura, 0, 0, 9, 8);
        medioCorazon = new TextureRegion(textura, 16, 0, 9, 8);
        corazonVacio = new TextureRegion(textura, 32, 0, 9, 8);
    }

    public void QuitarVida (int cantidad)
    {
        vidaActual -= cantidad;
         if (vidaActual <= 0)
        {
            vidaActual = 0;
            
        }
        
    }
    public void ReiniciarVida()
{
    vidaActual = vidasTotal;
}
public bool SinVida => vidaActual <= 0;

    public void Dibujar(SpriteBatch spriteBatch, Vector2 posicion)
{
    int vidaRestante = vidaActual;

    for (int a = 0; a < 3; a++)
    {
        Vector2 posicionCorazon =
            new Vector2(posicion.X + a * 36, posicion.Y);

        if (vidaRestante >= 2)
        {
            corazonLleno.Draw(
                spriteBatch,
                posicionCorazon,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(4.0f, 4.0f),
                SpriteEffects.None,
                0f
            );

            vidaRestante -= 2;
        }
        else if (vidaRestante == 1)
        {
            medioCorazon.Draw(
                spriteBatch,
                posicionCorazon,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(4.0f, 4.0f),
                SpriteEffects.None,
                0f
            );

            vidaRestante--;
        }
        else
        {
            corazonVacio.Draw(
                spriteBatch,
                posicionCorazon,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(4.0f, 4.0f),
                SpriteEffects.None,
                0f
            );
        }
    }
}
}


}