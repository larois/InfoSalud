using Facebook.Client;
using System;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
public class BioequivalenteClass
{
    public string Composicion { get; set; }
    public string Laboratorio { get; set; }
}

public class NOSE
{
    public string variable { get; set; }
}

public class Farmacias
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Ciudad { get; set; }
    public string Sector { get; set; }
    public string Apertura { get; set; }
    public string Cierre { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Telefono { get; set; }
}

public class Hospinica
{
    public string NumRegion { get; set; }
    public string Region { get; set; }
    public string Comuna { get; set; }
    public string Departamento { get; set; }
    public string Tipo { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Coordenada { get; set; }
}



