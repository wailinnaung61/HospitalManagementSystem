using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MSIS_HMS.Controllers
{
    public class ImagesController : Controller
    {
        // GET: /<controller>/
        public ActionResult Index(string path)
        {
            try
            {
                return File(FtpHelper.DownloadFileFromServer(path), "image/png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        [HttpPost]
        public IActionResult UploadFilesAjax([FromForm]IFormFile file)/*List<IFormFile> files*/
        {

            string base64String = string.Empty;
            var filePath = FtpHelper.ftpPatientResultImageFolderPath + file.GetUniqueName();
            try
            {              
                    if (file != null)
                    {
                        
                        FtpHelper.UploadFileToServer(file, filePath);

                    }
                return Json(filePath);
            }
            catch (Exception ex)
            {
                return BadRequest(StatusCodes.Status400BadRequest);
            }
            


        }
        [HttpPost]
        public IActionResult UploadFilesAjaxForLab([FromForm] IFormFile file)/*List<IFormFile> files*/
        {

            string base64String = string.Empty;
            var filePath = FtpHelper.ftplabImageFolderPath + file.GetUniqueName();
            try
            {
                if (file != null)
                {

                    FtpHelper.UploadFileToServer(file, filePath);

                }
                return Json(filePath);
            }
            catch (Exception ex)
            {
                return BadRequest(StatusCodes.Status400BadRequest);
            }



        }
    }
}
