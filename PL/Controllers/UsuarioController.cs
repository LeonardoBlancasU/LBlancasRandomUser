using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PL.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        [HttpGet]
        public ActionResult GetAll()
        {
            ML.Result result = GetAllRest();
            if (!result.Correct)
            {
                TempData["Error"] = result.ErrorMessage;
            }
            return View();
        }
        [NonAction]
        public ML.Result GetAllRest()
        {
            ML.Result result = new ML.Result();
            
            result.Objects = new List<object>();

            try
            {
                using (var cliente = new HttpClient())
                {
                    string url = ConfigurationManager.AppSettings["UrlAPI"].ToString();
                    cliente.BaseAddress = new Uri(url);

                    var responseTask = cliente.GetAsync("");
                    responseTask.Wait();

                    var resultServicio = responseTask.Result;

                    if(resultServicio.IsSuccessStatusCode)
                    {
                        var readTask = resultServicio.Content.ReadAsAsync<ML.ResultAPI>(); //deserializando Json Result 
                        readTask.Wait();

                        result.Object = readTask.Result.results[0];
                        if (Session["Usuarios"] == null)
                        {
                            result.Objects.Add(result.Object);
                            Session["Usuarios"] = result.Objects;
                        }
                        else
                        {
                            result.Objects = (List<object>)Session["Usuarios"];
                            result.Objects.Add(result.Object);
                            Session["Usuarios"] = result.Objects;
                        }
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se pudo generar al Usuario";
                    }
                }
            }
            catch(Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }
        [HttpPost]
        public JsonResult LimpiarSession()
        {
            ML.Result result = new ML.Result();
            try
            {
                Session["Usuarios"] = null;
                result.Correct= true;
            }
            catch (Exception ex)
            { 
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}