using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrnaEletronica.Models;
using static UrnaEletronica.Models.ApiModels;
using static UrnaEletronica.Models.CandidatoViewModels;
using static UrnaEletronica.Models.VotoViewModels;

namespace UrnaEletronica.Controllers
{
    public class VotoController : Controller
    {
        public static HttpClient client = new HttpClient();
        public static int etapa = 0;
        public static int legenda = 0;
        //1 = escolhe o candidato / 2 =  confirma a escolha / 3 = finaliza

        public void Reiniciar()
        {
            //reinicia para votar novamente
            legenda = 0; etapa = 0;
        }
        public IActionResult Index()
        {
            Reiniciar();
            return PartialView("_TelaInicial");
        }
        public IActionResult BotaoBranco()
        {
            // anula o voto para o branco

            etapa = 1; 
            legenda = 000; // identifica o voto em branco

            ViewBag.Legenda = legenda;
            return PartialView("_TelaConfirma");
        }
        public IActionResult BotaoCorrige()
        {
            Reiniciar();
            return RedirectToAction("Index", "Voto");
        }
        public async Task<IActionResult> BotaoConfirma(int data)
        {
            //recebe o front 

            if (etapa == 0) 
            {
                
                HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/getcand/" + data);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Candidato>(result);
                    etapa = 1; 
                    legenda = obj.Legenda;

                    ViewBag.Legenda = obj.Legenda;
                    ViewBag.Nome = obj.NomeCompleto;
                    ViewBag.Vice = obj.Vice;

                    return PartialView("_TelaConfirma");
                }
                else
                {
                    return PartialView("_Info", "Houve algum erro, tente novamente!");
                }
            }
            else if (etapa == 1) 
            {
                //Confirma voto e vai para a fase final
                var nVoto = new VotoViewModel
                {
                    Candidato = legenda
                };

                
                HttpRequestMessage request = new HttpRequestMessage
                {
                    
                    Content = new StringContent(JsonConvert.SerializeObject(nVoto), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://" + this.Request.Host + "/api/vote")

                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    etapa = 2; 

                    ViewBag.Legenda = nVoto.Candidato;

                    var obj = JsonConvert.DeserializeObject<CandidatoViewModel>(result);
                    return PartialView("_TelaFinal", obj);
                }
                else
                {
                    return PartialView("_Info", "Houve algum erro, tente novamente!");
                }

            }
            else
            {
                Reiniciar();
                return PartialView("_TelaInicial");
            }
        }
        public async Task<IActionResult> Resultados()
        {
            HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/votes");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<ResultadosViewModel>>(result);
                return PartialView("_Resultados", obj);
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public async Task<IActionResult> ResetarBrancos()
        {
            
            HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/resetwhite/");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Resultados", "Voto");
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }

        }
    }
}
