using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static UrnaEletronica.Models.ApiModels;
using static UrnaEletronica.Models.CandidatoViewModels;

namespace UrnaEletronica.Controllers
{
    public class CandidatoController : Controller
    {
        public static HttpClient client = new HttpClient();

        [HttpPost]
        public async Task<IActionResult> AdicionarCandidato(CandidatoViewModel data)
        {
            var nCand = new CandidatoViewModel
            {
                Inscricao = DateTime.Now,
                Legenda = data.Legenda,
                NomeCompleto = data.NomeCompleto,
                Vice = data.Vice
            };

            
            HttpRequestMessage request = new HttpRequestMessage
            {
                //converte objeto json para ser enviado
                Content = new StringContent(JsonConvert.SerializeObject(nCand), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://" + this.Request.Host + "/api/candidate")
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                if (result.Contains("Erro:"))
                {
                    return PartialView("_Info", JsonConvert.DeserializeObject<string>(result));
                }
                else
                {
                    return RedirectToAction("AdministrarCandidatos", "Candidato");
                }
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public IActionResult AdicionarCandidato()
        {
            return PartialView("_NovoCandidato");
        }
        public async Task<IActionResult> RemoverCandidato(int data)
        {
            var nCand = new CandidatoViewModel
            {
                Legenda = data,
            };

            var objString = JsonConvert.SerializeObject(nCand);


            //Cria a request
            HttpRequestMessage request = new HttpRequestMessage
            {
                
                Content = new StringContent(objString, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://" + this.Request.Host + "/api/candidate")
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return RedirectToAction("AdministrarCandidatos", "Candidato");
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public async Task<IActionResult> AdministrarCandidatos()
        {
            HttpResponseMessage response = await client.GetAsync("https://" + this.Request.Host + "/api/getcands/");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<Candidato>>(result);

                return PartialView("_AdministrarCandidatos", obj);
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public async Task<IActionResult> Candidatos()
        {
            HttpResponseMessage response = await client.GetAsync("api/getcands");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<Candidato>>(result);
                return PartialView("_Candidatos", obj);
            }
            else
            {
                return PartialView("_Info", "Houve algum erro, tente novamente!");
            }
        }
        public IActionResult Index()
        {
            return RedirectToAction("Candidatos", "Candidato");
        }
    }
}

