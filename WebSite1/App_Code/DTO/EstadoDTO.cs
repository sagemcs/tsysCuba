using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EstadoDTO
/// </summary>
public abstract class EstadoDTO
{
    public string Id { get; set; }
    public string Descripcion { get; set; }
    public EstadoDTO()
    {
    }

    public EstadoDTO(string Id, string Descripcion)
    {
        this.Id = Id;
        this.Descripcion = Descripcion;
    }

    public EstadoDTO(int Id, string Descripcion)
    {
        this.Id = Id.ToString();
        this.Descripcion = Descripcion;
    }
}