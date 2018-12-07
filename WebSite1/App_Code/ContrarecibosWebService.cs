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
/// Summary description for ContrarecibosWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class ContrarecibosWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public ContrarecibosWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string Folio, string VendID, string AliasDBA, string Total, string RcptDate, int start, int length, bool sin_solicitud = false)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<ContrareciboDTO> list_dto = new List<ContrareciboDTO>();
                RcptDate = Tools.ObtenerFechaEnFormato(RcptDate);
                list_dto = Contrarecibos.ObtenerContrarecibos(Folio, VendID, AliasDBA, Total, RcptDate, sin_solicitud);

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
    public void generar(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(ids);
                List<string> UUIDs = new List<string>();
                foreach (var item in UUIDs_obj)
                {
                    UUIDs.Add(item.ToString());
                }


                bool success = true;
                int id = -1;
                string msg = "";
                try
                {
                    string rpt_path = Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport.rpt");
                    id = Contrarecibos.GenerarContrarecibo(UUIDs, rpt_path);
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    success = false;
                }
                if (id == -1)
                    success = false;
                var result = new
                {
                    success = success,
                    id = id,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
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
