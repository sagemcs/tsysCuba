﻿using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EstadoDocumentoDTO
/// </summary>
public class EstadoDocumentoDTO : EstadoDTO
{
    public EstadoDocumentoDTO()
    {
    }

    public EstadoDocumentoDTO(string Id, string Descripcion): base(Id, Descripcion)
    {
    }

    public EstadoDocumentoDTO(int Id, string Descripcion): base(Id, Descripcion)
    {
    }
}

public class EstadosDocumento
{
    public EstadosDocumento()
    {
    }

    public static List<EstadoDocumentoDTO> Obtener()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<StatusDocs> list = db.StatusDocs.ToList();
            List<EstadoDocumentoDTO> list_dto = new List<EstadoDocumentoDTO>();

            foreach (var item in list)
                list_dto.Add(new EstadoDocumentoDTO(item.Status, item.Descripcion));

            return list_dto;
        }
        catch(Exception e)
        {
            throw new MulticonsultingException(e.ToString());
        }
    }
}