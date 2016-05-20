using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captcha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            label1.Text = "Code: " + SecureStringToString(dCaptcha1.CaptchaCode);

        }
        // Dont ever convert to string besides for testing.
        public static String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            dCaptcha1.GenerateNew();
            label1.Text = "Code: " + SecureStringToString(dCaptcha1.CaptchaCode);
        }

        private void dCaptcha1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    dCaptcha1.Difficulty = dCaptcha.Mode.Easy;
                    break;
                case 1:
                    dCaptcha1.Difficulty = dCaptcha.Mode.Medium;
                    break;
                case 2:
                    dCaptcha1.Difficulty = dCaptcha.Mode.Hard;
                    break;
            }
        }
    }
}
