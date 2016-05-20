using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captcha
{
    class dCaptcha : PictureBox
    {
        private SecureRandom sRandom;
        private SecureString captchaCode;
        private Mode difficulty;
        public dCaptcha()
        {
            this.sRandom = new SecureRandom();
            this.captchaCode = new SecureString();
            this.Size = new Size(140, 50);
            this.BackColor = Color.White;
            GenerateCaptcha();
        }
        #region Properties
        public SecureString CaptchaCode
        {
            get
            {
                return captchaCode;
            }
        }
        public Mode Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
            }
        }
        #endregion

        public void GenerateNew()
        {
            this.captchaCode = new SecureString();
            GenerateCaptcha();
            base.Invalidate();
        }
        #region Enums
        public enum Mode
        {
            Easy,
            Medium,
            Hard
        }
        enum ColorType
        {
            Regular,
            Blob
        }
        #endregion
        public void drawRotatedText(Graphics g, int x, int y, float angle, string text, Font font, Brush brush)
        {
            g.TranslateTransform(x, y); // Set rotation point
            g.RotateTransform(angle); // Rotate text
            g.TranslateTransform(-x, -y); // Reset translate transform
            SizeF size = g.MeasureString(text, font); // Get size of rotated text (bounding box)
            g.DrawString(text, font, brush, new PointF(x - size.Width / 2.0f, y - size.Height / 2.0f)); // Draw string centered in x, y
            g.ResetTransform(); // Only needed if you reuse the Graphics object for multiple calls to DrawString
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Point[] pts = RandomPoints(5);
            // Should still be somewhat secure since stored as char array, rather than plain string.
            char[] code = SecureStringToString(captchaCode).ToCharArray();

            // Fuck the shit up fam
            DrawPhantomText(e.Graphics);
            switch (difficulty)
            {
                case Mode.Easy:
                    DrawBlobs(e.Graphics);
                    break;
                case Mode.Medium:
                    DrawBlobs(e.Graphics);
                    DrawNoise(e.Graphics);
                    break;
                case Mode.Hard:
                    DrawBlobs(e.Graphics);
                    DrawNoise(e.Graphics);
                    DrawLines(e.Graphics);
                    // DrawBoxes(e.Graphics);
                    break;
            }
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            // Draw characters
            drawRotatedText(e.Graphics, pts[0].X, pts[0].Y, sRandom.Next(-35, 35), code[0].ToString(), RandomFont(), new SolidBrush(RandomColor(ColorType.Regular)));
            drawRotatedText(e.Graphics, pts[1].X, pts[0].Y, sRandom.Next(-35, 35), code[1].ToString(), RandomFont(), new SolidBrush(RandomColor(ColorType.Regular)));
            drawRotatedText(e.Graphics, pts[2].X, pts[0].Y, sRandom.Next(-35, 35), code[2].ToString(), RandomFont(), new SolidBrush(RandomColor(ColorType.Regular)));
            drawRotatedText(e.Graphics, pts[3].X, pts[0].Y, sRandom.Next(-35, 35), code[3].ToString(), RandomFont(), new SolidBrush(RandomColor(ColorType.Regular)));
            drawRotatedText(e.Graphics, pts[4].X, pts[0].Y, sRandom.Next(-35, 35), code[4].ToString(), RandomFont(), new SolidBrush(RandomColor(ColorType.Regular)));
        }
        private static string SecureStringToString(SecureString str)
        {
            IntPtr intPtr = IntPtr.Zero;
            try
            {
                intPtr = Marshal.SecureStringToGlobalAllocUnicode(str);
                return Marshal.PtrToStringUni(intPtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
            }
        }

        #region Obscuring
        private void DrawPhantomText(Graphics e)
        {
            int count = sRandom.Next(5, 25);
            for (int i = 0; i < count; i++)
                drawRotatedText(e, sRandom.Next(Width), sRandom.Next(Height), sRandom.Next(-25, 25), SecureStringToString(sRandom.String(1)), new Font("Arial", 16), new SolidBrush(Color.FromArgb(175, Color.Black)));
        }
        private void DrawNoise(Graphics e)
        {
            int count = sRandom.Next(5, 30);
            for (int i = 0; i < count; i++)
            {
                Point one = new Point(sRandom.Next(Width), sRandom.Next(Height));
                Point two = new Point(sRandom.Next(Width), sRandom.Next(Height));
                e.DrawRectangle(new Pen(Color.Black), new Rectangle(one, new Size(2, 2)));
                e.FillRectangle(new SolidBrush(Color.Black), new Rectangle(one, new Size(2, 2)));
            }
        }
        private void DrawBoxes(Graphics e)
        {
            int count = sRandom.Next(4, 12);
            for (int i = 0; i < count; i++)
            {
                Point one = new Point(sRandom.Next(Width), sRandom.Next(Height));
                Point two = new Point(sRandom.Next(Width), sRandom.Next(Height));
                e.DrawRectangle(new Pen(Color.FromArgb(200, Color.Black)), new Rectangle(one, new Size(sRandom.Next(50), sRandom.Next(50))));
            }
        }
        private void DrawLines(Graphics e)
        {
            int count = sRandom.Next(5, 10);
            for(int i = 0; i < count; i++)
            {
                Point one = new Point(sRandom.Next(Width), sRandom.Next(Height));
                Point two = new Point(sRandom.Next(Width), sRandom.Next(Height));
                e.DrawLine(new Pen(Color.Black), one, two);
            }
        }
        private void DrawBlobs(Graphics e)
        {
            int count = sRandom.Next(0, 4);
            for (int i = 0; i < 2; i++)
            {
                int randX = sRandom.Next(Width / 20, Width / 2);
                int randY = sRandom.Next(Height / 20, Height / 2);

                int randH = sRandom.Next(30, 80);
                int randW = sRandom.Next(30, 80);
                e.FillEllipse(new SolidBrush(RandomColor(ColorType.Blob)), randX, randY, randW, randH);
            }
        }
        #endregion

        #region Randoms
        private Point[] RandomPoints(int count)
        {
            const int Y_MARGIN = 20;
            const int X_MARGIN = 20;
            List<Point> points = new List<Point>();
            Point control = new Point(sRandom.Next(20, X_MARGIN + 10), sRandom.Next(Y_MARGIN, Height - Y_MARGIN));
            points.Add(control);
            int lastXPos = control.X;
            for(int i = 0; i < count-1 ; i++)
            {
                Point temp = new Point(sRandom.Next(lastXPos + X_MARGIN, lastXPos + 24), sRandom.Next(Y_MARGIN, Height - Y_MARGIN));
           
                lastXPos = temp.X;
                points.Add(temp);
            }
            return points.ToArray();
        }
        private SecureString GenerateCaptcha()
        {
            SecureString code = sRandom.String(5);
            captchaCode = code;
            return code;
        }
        private Color RandomColor(ColorType cType)
        {
            Color[] ColorList = null;
            switch(cType)
            {
                case ColorType.Regular:
                    {
                       ColorList = new Color[]
                            {
                                Color.Black
                                //Color.Red,
                                //Color.Blue,
                                //Color.Green,
                                //Color.Black,
                                //Color.Orange,
                                //Color.Purple,
                            };
                        break;
                    }
                case ColorType.Blob:
                    {
                        ColorList = new Color[]
                            {
                                Color.DarkGray,
                                Color.DarkSlateGray,
                                SystemColors.Highlight,
                                
                               // Color.Yellow,
                              //  Color.Blue
                            };
                        break;
                    }
            }
            return ColorList[sRandom.Next(ColorList.Length)];
        }

        private float RandomSize()
        {
            const int MIN_SIZE = 18;
            const int MAX_SIZE = 22;
            return sRandom.Next(MIN_SIZE, MAX_SIZE);
        }
        private Font RandomFont()
        {
            string[] FontNames =
            {
                "Arial",
                "Verdana",
                "Segui UI",
                "Consolas",
                "Calibri"
            };
            return new Font(FontNames[sRandom.Next(FontNames.Length)], RandomSize(), FontStyle.Bold);
        }
        #endregion

    }
    sealed class SecureRandom : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator rng;

        public SecureRandom()
        {
            this.rng = new RNGCryptoServiceProvider();
        }
        public int Next()
        {
            var data = new byte[sizeof(int)];
            rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
        }
        public SecureString String(int len, string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
        {
            SecureString sb = new SecureString();
            string letter = "";
            while (sb.Length != len)
            {
                while (letter == "" || !charset.Contains(letter))
                {
                    if (sb.Length == len)
                    {
                        sb.MakeReadOnly();
                        return sb;
                    }
                    byte[] oneByte = new byte[1];
                    rng.GetBytes(oneByte);
                    char c = (char)oneByte[0];
                    if (char.IsDigit(c) || char.IsLetter(c))
                        letter = c.ToString();
                }
                sb.AppendChar(letter[0]);
                letter ="";
            }
            sb.MakeReadOnly();
            return sb;
        }
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (int)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
        }

        public double NextDouble()
        {
            var data = new byte[sizeof(uint)];
            rng.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint / (uint.MaxValue + 1.0);
        }
        public double NextDouble(double minimum, double maximum)
        {
            return NextDouble() * (maximum - minimum) + minimum;
        }
        public override void GetBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
        public override void GetNonZeroBytes(byte[] data)
        {
            rng.GetNonZeroBytes(data);
        }
    }
}
