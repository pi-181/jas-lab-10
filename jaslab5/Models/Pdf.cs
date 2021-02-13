using System.IO;

namespace jaslab5
{
    public class Pdf
    {
        public static byte[] PdfSharpConvert(string html)
        {
            byte[] res;
            using (var ms = new MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
    }
}