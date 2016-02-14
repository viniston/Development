using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.Http.Headers;


namespace Development.Web
{
    public partial class GenerateCaptcha : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString.Count > 0)
            {
                string qryPurpose = Request.QueryString["purpose"];  // this should match exactly with the api/Authorization/GeneratingCaptchaCookie captcha location
                string captchaValue = "";
                if (Request.Cookies[qryPurpose] != null) captchaValue = Request.Cookies[qryPurpose].Value.ToString();
                Response.Clear();
                int height = 30;
                int width = 100;
                Bitmap bmp = new Bitmap(width, height);
                RectangleF rectf = new RectangleF(10, 5, 0, 0);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(captchaValue, new Font("Thaoma", 12, FontStyle.Italic), Brushes.Gray, rectf);
                g.DrawRectangle(new Pen(Color.Gray), 1, 1, width - 2, height - 2);
                g.Flush();
                Response.ContentType = "image/jpeg";
                bmp.Save(Response.OutputStream, ImageFormat.Jpeg);
                g.Dispose();
                bmp.Dispose();
            }

        }
    }
}
