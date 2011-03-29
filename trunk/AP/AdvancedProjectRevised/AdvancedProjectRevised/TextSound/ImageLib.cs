using OpenTK.Graphics;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Collections.Generic;
using Img = System.Drawing.Imaging;

namespace AP
{
    public class ImageHandler
    {
        TextureImage textImage;
        List<Bitmap> y;
        List<int> width;
        List<int> height;

        public ImageHandler()
        {
            textImage = new TextureImage();
            y = new List<Bitmap>(); // initialize list of Bitmaps
            width = new List<int>();
            height = new List<int>();
        }
        public int loadImage(String path)
        {
            Bitmap temp = new Bitmap(Bitmap.FromFile(path)); // create a temporary bitmap for the specified file
            y.Add(temp); // add this bitmap to the bitmap list

            height.Add(temp.Height);
            width.Add(temp.Width);

            int textemp = TexUtil.CreateTextureFromBitmap(temp); // create a GL texture for this bitmap, store its number

            textImage.addImage(textemp); // add this texture number to the textImage class
            return width.Count-1;
        }
        private float calculateCurrentWidth(int imageNum, float percentwid)
        {
            return width[imageNum] * percentwid;
        }
        public void drawImage(int imageNum, float x, float y, float size, float percentwid)
        {
            // Write something centered in the viewport, set color to white to draw in proper color
            GL.Color3(1.0f, 1.0f, 1.0f);
            textImage.drawImageAt(size, x, y, 0, calculateCurrentWidth(imageNum,percentwid), height[imageNum], imageNum);

        }
    }
  public static class TexUtil
  {
    #region Public

    /// <summary>
    /// Initialize OpenGL state to enable alpha-blended texturing.
    /// Disable again with GL.Disable(EnableCap.Texture2D).
    /// Call this before drawing any texture, when you boot your
    /// application, eg. in OnLoad() of GameWindow or Form_Load()
    /// if you're building a WinForm app.
    /// </summary>
      public static void InitTexturing()
      {
          GL.Disable(EnableCap.CullFace);
          GL.Enable(EnableCap.Texture2D);
          GL.Enable(EnableCap.Blend);
          GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
          GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
      }
      public static void DisableTexturing()
      {
          GL.Disable(EnableCap.Texture2D);
          GL.Disable(EnableCap.Blend);
      }

    /// <summary>
    /// Create an OpenGL texture (translucent or opaque) from a given Bitmap.
    /// 24- and 32-bit bitmaps supported.
    /// Make sure width and height is 1, 2, .., 32, 64, 128, 256 and so on in size since all
    /// 3d graphics cards support those dimensions. Not necessarily square. Don't forget
    /// to call GL.DeleteTexture(int) when you don't need the texture anymore (eg. when switching
    /// levels in your game).
    /// </summary>
    public static int CreateTextureFromBitmap(Bitmap bitmap)
    {

      Img.BitmapData data = bitmap.LockBits(
        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        Img.ImageLockMode.ReadOnly,
        Img.PixelFormat.Format32bppArgb);
      var tex = GiveMeATexture();
      GL.BindTexture(TextureTarget.Texture2D, tex);
      GL.TexImage2D(
        TextureTarget.Texture2D,
        0,
        PixelInternalFormat.Rgba,
        data.Width, data.Height,
        0,
        PixelFormat.Bgra,
        PixelType.UnsignedByte,
        data.Scan0);
      bitmap.UnlockBits(data);
      SetParameters();
      return tex;
    }

    #endregion

    private static int GiveMeATexture()
    {
      int tex = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, tex);
      return tex;
    }

    private static void SetParameters()
    {
      GL.TexParameter(
        TextureTarget.Texture2D,
        TextureParameterName.TextureMinFilter,
        (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D,
        TextureParameterName.TextureMagFilter,
        (int)TextureMagFilter.Linear);
    }

  }

  public class TextureImage
  {

    List<int> textureId;

    /// <summary>
    /// Create a TextureImage object. The sent-in textureId should refer to a
    /// texture bitmap containing a 16x16 grid of fixed-width characters,
    /// representing the ASCII table. A 32 bit texture is assumed, aswell as
    /// all GL state necessary to turn on texturing. The dimension of the
    /// texture bitmap may be anything from 128x128 to 512x256 or any other
    /// order-by-two-squared-dimensions.
    /// </summary>
    public TextureImage()
    {
        textureId = new List<int>();
    }

    /// <summary>
    /// This is a convenience function to write a text string using a simple coordinate system defined to be 0..100 in x and 0..100 in y.
    /// For example, writing the text at 50,50 means it will be centered onscreen. The height is given in percent of the height of the viewport.
    /// No GL state except the currently bound texture is modified. This method is not as flexible nor as fast
    /// as the WriteString() method, but it is easier to use.
    /// </summary>
    public void drawImageAt(
      double heightPercent,
      double xPercent,
      double yPercent,
      double degreesCounterClockwise, float width, float height,
      int imageNum)
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Ortho(0, 100, 0, 100, -1, 1);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Translate(xPercent, yPercent, 0);
      double aspectRatio = ComputeAspectRatio();
      GL.Scale(aspectRatio * heightPercent, heightPercent, heightPercent);
      GL.Rotate(degreesCounterClockwise, 0, 0, 1);
      GL.BindTexture(TextureTarget.Texture2D, textureId[imageNum]);
      GL.PushMatrix();
      GL.Begin(BeginMode.Polygon);

      drawImage(width, height);

      GL.End();
      GL.PopMatrix();
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Projection);
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Modelview);

    }

    private static double ComputeAspectRatio()
    {
      int[] viewport = new int[4];
      GL.GetInteger(GetPName.Viewport, viewport);
      int w = viewport[2];
      int h = viewport[3];
      double aspectRatio = (float)h / (float)w;
      return aspectRatio;
    }

    private void drawImage(float width, float height)
    {
      double left = 0;
      double right = 1;
      double top = 1;
      double bottom = 0;

      GL.TexCoord2(left, bottom); GL.Vertex2(0, (float)(height / 10));
      GL.TexCoord2(right, bottom); GL.Vertex2(0 + (float)(width / 10), (float)(height / 10));
      GL.TexCoord2(right, top); GL.Vertex2(0 + (float)(width / 10), 0);
      GL.TexCoord2(left, top); GL.Vertex2(0, 0);
    }

    // adds a textureID to the ID list
    public void addImage(int tex)
    {
        textureId.Add(tex);
    }

  }

}