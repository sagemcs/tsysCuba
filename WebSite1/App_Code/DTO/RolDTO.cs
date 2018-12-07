﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proveedores_Model;

public class RolDTO
{
    public string ID { get; set; }
    public string Descripcion { get; set; }
    public string Fecha { get; set; }

    public RolDTO()
    { }
    public RolDTO(string ID, string Descripcion, string Fecha)
    {
        this.ID = ID;
        this.Descripcion = Descripcion;
        this.Fecha = Fecha;
    }
}

public class Roles_on_DTO
{

    public Roles_on_DTO()
    {
    }

    public static List<RolDTO> ObtenerRoles(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            //throw new MulticonsultingException("No está autenticado por ninguna empresa");
            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;
            //throw new MulticonsultingException("No está autenticado");

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;
            //throw new MulticonsultingException("Acceso Denegado");

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<Roles> list = db.Roles.ToList();
            List<RolDTO> roles = new List<RolDTO>();
            foreach (var rol in list)
                roles.Add(new RolDTO(rol.RoleID, rol.Description, rol.CreateDate != null ? rol.CreateDate.Value.ToShortDateString() : string.Empty));

            return roles;
        }
        catch(Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<RolDTO>();
        }
    }
    public static List<RolDTO> ObtenerRoles(string ID, string Description, bool directo_en_vista = false)
    {
        try
        {
            List<RolDTO> roles = ObtenerRoles();

            if (roles == null)
                return null;

            if (!string.IsNullOrWhiteSpace(ID) && ID != "null")
                roles = roles.Where(r => r.ID.ToUpper().Contains(ID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Description) && Description != "null")
                roles = roles.Where(r => r.Descripcion.ToUpper().Contains(Description.ToUpper())).ToList();

            return roles;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<RolDTO>();
        }
    }
}

