using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace Elgin.Portal.Services.Model
{
    public class Video
    {
        public Guid Id { get; set; }
        public string LinkVideo { get; set; }
        public string TituloVideo { get; set; }
        public string DescricaoVideo { get; set; }
        public string Modulo { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public string CodigoVideo
        {
            get
            {
                try
                {
                    if (LinkVideo.Contains("youtu.be"))
                    {
                        var url = LinkVideo.Split('/');
                        var finalUrl = url.Length > 0 ? url[url.Length - 1] : string.Empty;
                        if (!string.IsNullOrEmpty(finalUrl))
                        {
                            var param = finalUrl.Split('?');
                            return param.Length > 0 ? param[0] : string.Empty;
                        }
                    }
                    else
                    {
                        Uri myUri = new Uri(LinkVideo);
                        return HttpUtility.ParseQueryString(myUri.Query).Get("v");
                    }
                }
                catch (Exception) { }

                return string.Empty;
            }
        }
        public string Thumbnail
        {
            get
            {
                return !string.IsNullOrEmpty(CodigoVideo) ? "https://img.youtube.com/vi/" + CodigoVideo + "/mqdefault.jpg" : string.Empty;
            }
        }
    }


}
