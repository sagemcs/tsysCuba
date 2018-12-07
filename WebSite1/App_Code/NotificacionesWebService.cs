using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using Proveedores_Model;

/// <summary>
/// Summary description for NotificacionesWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class NotificacionesWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public NotificacionesWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar()
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
            }

            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<String> menssagesFacturas = Tools.Mensaje_Estado_de_Facturas();

                List<String> menssagesPagos = Tools.Mensaje_Estado_de_Pagos();

                List<String> menssagesDocumentos = Tools.Mensaje_Estado_de_Documentos_de_Proveedor();

                var result = new
                {
                    facturas = menssagesFacturas,
                    pagos = menssagesPagos,
                    documentos = menssagesDocumentos
                };
                var js = new JavaScriptSerializer();
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
    
}
