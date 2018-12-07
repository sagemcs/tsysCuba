using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using Proveedores_Model;

/// <summary>
/// Summary description for FacturasWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class FacturasWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public FacturasWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar_estado_factura(string VendorId, string Folio, string Fecha, string contrarecibo, string solicitud, string Estado, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();

                if (!string.IsNullOrWhiteSpace(Fecha))
                {
                    string[] camps = Fecha.Split('/');
                    DateTime date = new DateTime(Convert.ToInt32(camps[2]), Convert.ToInt32(camps[1]), Convert.ToInt32(camps[0]));
                    Fecha = date.ToShortDateString();
                }
                
                list_dto = Facturas.ObtenerFacturas(VendorId, Folio, Fecha, contrarecibo, solicitud, Estado);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    foreach (FacturaDTO factura in list_dto)
                    {
                        string path = "..\\..\\Img\\estados\\" + (string.IsNullOrWhiteSpace(factura.Estado_Id) ? "0.png" : factura.Estado_Id + ".png");
                        factura.Estado_Img = "<img src=\"" + path + "\"style=\"width: 50px; height 50px \">";
                    }

                    int total = list_dto.Count();
                    list_dto = list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }

    }
    

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string Folio, string Serie, string Fecha, string VendID, string Total, string UUID, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();

                if (!string.IsNullOrWhiteSpace(Fecha))
                {
                    string[] camps = Fecha.Split('/');
                    DateTime date = new DateTime(Convert.ToInt32(camps[2]), Convert.ToInt32(camps[1]), Convert.ToInt32(camps[0]));
                    Fecha = date.ToShortDateString();
                }

                list_dto = Facturas.ObtenerFacturas(VendID, Folio, Serie, Fecha, Total, UUID, Status);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    int total = list_dto.Count();
                    list_dto = list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar_sin_contrarecibo(string Folio, string Serie, string Fecha, string VendID, string Total, string UUID, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();

                if (!string.IsNullOrWhiteSpace(Fecha))
                {
                    string[] camps = Fecha.Split('/');
                    DateTime date = new DateTime(Convert.ToInt32(camps[2]), Convert.ToInt32(camps[1]), Convert.ToInt32(camps[0]));
                    Fecha = date.ToShortDateString();
                }

                list_dto = Facturas.ObtenerFacturasSinContrarecibo(VendID, Folio, Serie, Fecha, Total, UUID, Status);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    int total = list_dto.Count();
                    list_dto = list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public bool es_proveedor()
    {
        return Tools.EsProveedor();
    }

}
