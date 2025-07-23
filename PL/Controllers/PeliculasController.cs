using Antlr.Runtime;
using ML;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PL.Controllers
{
    public class PeliculasController : Controller
    {
        // GET: Peliculas
        public ActionResult GetAll(string accion)
        {
            Session["Accion"] = null;
            ML.Pelicula pelicula = new ML.Pelicula();
            if (accion == "Favoritas")
            {
                Session["Accion"] = accion;
                ML.Result resultFavoritas = GetAllRest(accion);
                if (resultFavoritas.Correct)
                {
                    ML.Result result = new Result();
                    result.Objects = new List<object>();
                    foreach (ML.Pelicula itemFavoritas in resultFavoritas.Objects)
                    {
                        itemFavoritas.favorito = true;
                        result.Objects.Add(itemFavoritas);
                    }
                    pelicula.Peliculas = result.Objects;
                }
            }
            else
            {
                Session["Accion"] = accion;
                ML.Result result = new ML.Result();
                ML.Result resultPopulares = GetAllRest(accion);
                result.Objects = new List<object>();
                if (resultPopulares.Correct)
                {
                    ML.Result resultFavoritas = GetAllRest("Favoritas");
                    foreach (ML.Pelicula itemPopulares in resultPopulares.Objects)
                    {
                        foreach(ML.Pelicula itemFavoritas in resultFavoritas.Objects)
                        {
                            if(itemFavoritas.id == itemPopulares.id)
                            {
                                itemPopulares.favorito = true;
                                break;
                            }
                        }
                        result.Objects.Add(itemPopulares);
                    }
                    pelicula.Peliculas = result.Objects;
                }
                else
                {
                    pelicula.Peliculas = new List<object>();
                }
            }
            return View(pelicula);
        }
        [NonAction]
        public ML.Result GetAllRest(string accion)
        {
            ML.Result result = new ML.Result();
            result.Objects = new List<object>();
            string url;
            try
            {
                using(var cliente = new HttpClient())
                {
                    if(accion == "Populares") 
                    {
                        url = ConfigurationManager.AppSettings["UrlPeliculas"].ToString();
                    }
                    else 
                    {
                        url = ConfigurationManager.AppSettings["UrlFavorite"].ToString(); 
                    }
                    
                    string token = ConfigurationManager.AppSettings["Token"].ToString();

                    cliente.BaseAddress = new Uri(url);
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var responseTask = cliente.GetAsync("");
                    responseTask.Wait();
                    var resultServicio = responseTask.Result;
                    if (resultServicio.IsSuccessStatusCode)
                    {
                        var readTask = resultServicio.Content.ReadAsAsync<ML.ResultPeliculas>(); //deserializando Json Result 
                        readTask.Wait();

                        foreach(var item in readTask.Result.results) 
                        {
                            //ML.Pelicula pelicula = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Pelicula>(item.ToString());
                            result.Objects.Add(item);
                        }
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontraron Peliculas";
                    }
                }
            } catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }
        
        [NonAction]
        public ML.Result AddAndDeleteFavoritas(ML.Favorito favorito)
        {
            ML.Result result = new ML.Result();
            string url = ConfigurationManager.AppSettings["UrlAddFavorite"].ToString();
            string token = ConfigurationManager.AppSettings["Token"].ToString();
            try
            {
                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri(url);
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    //HTTP POST 
                    var postTask = cliente.PostAsJsonAsync<ML.Favorito>("", favorito); //Serializar
                    postTask.Wait();

                    var request = postTask.Result;
                    if (request.IsSuccessStatusCode)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        [HttpGet]
        public JsonResult DeleteFavoritos(int IdPelicula)
        {
            ML.Favorito favorito = new ML.Favorito();
            favorito.media_id = IdPelicula;
            favorito.media_type = "movie";
            favorito.favorite = false;
            ML.Result result = AddAndDeleteFavoritas(favorito);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddFavoritos(int IdPelicula)
        {
            ML.Favorito favorito = new ML.Favorito();
            favorito.media_id = IdPelicula;
            favorito.media_type = "movie";
            favorito.favorite = true;
            ML.Result result = AddAndDeleteFavoritas(favorito);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}