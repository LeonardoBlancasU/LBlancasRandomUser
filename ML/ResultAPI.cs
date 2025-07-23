using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class ResultAPI
    {
        public List<Usuario> results {  get; set; }
    }
    public class ResultPeliculas
    {
        public List<Pelicula> results { get; set; }
    }
    public class Usuario
    {
        public string gender { get; set; }
        public Nombre name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public Direccion location { get; set; }
        public Imagen picture { get; set; }
    }
    public class Nombre
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }
    public class Imagen
    {
        public string large { get; set; }
    }
    public class Direccion
    {
        public string city { get; set; }
        public int postcode { get; set; }
        public string state { get; set; }
        public string country { get; set; }

        public Calle street { get; set; }
    }
    public class Calle
    {
        public int number { get; set; }
        public string name { get; set; }
    }

    public class Pelicula
    {
        public string title { get; set; }
        public string poster_path { get; set; }
        public string backdrop_path { get; set; }
        public int id { get; set; }
        public string overview { get; set; }
        public List<object> Peliculas { get; set; }
        public List<object> Favoritas { get; set; }
        public bool favorito { get; set; }
    }
    public class Favorito
    {
        public string  media_type { get; set; }
        public int media_id { get; set; }
        public bool favorite { get; set; }
    }
}
