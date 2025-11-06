<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context)
    {        
        context.Response.ContentType = "image/png";
        string Path = context.Server.MapPath("~/images/add-image.png");
        byte[] file = System.IO.File.ReadAllBytes(Path);

        context.Response.BinaryWrite(file);
        context.Response.End();       

    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}