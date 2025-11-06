using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MSIS_HMS.Helpers
{
    public static class FtpHelper
    {
        public static string ftpurl = @"ftp://13.250.191.23:21";
        //public static string ftpurl = @"ftp://127.0.0.1:21";
        //public static string ftpurl = @"ftp://192.168.57.17:89";
        public static string ftpusername = "FTP_USER";
        public static string ftppassword = "P@ssw0rd";
        public static string ftpMenuIconFolderPath = "/icons/menus/";
        public static string ftpItemImageFolderPath = "/images/items/";
        public static string ftpDoctorNotesImageFolderPath = "/images/medicalrecords/";
        public static string ftpPatientImageFolderPath = "/images/patients/";
        public static string ftpPatientResultImageFolderPath = "/images/resultImage/";
        public static string ftpDoctorImageFolderPath = "/images/doctors/";
        public static string ftplabImageFolderPath = "/images/labs/";

        public static FtpWebResponse UploadFileToServer(IFormFile file, string filepath)
        {
            try
            {
                if (file != null)
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpurl + filepath);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(ftpusername, ftppassword);
                    request.EnableSsl = false;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        file.CopyTo(requestStream);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        return response;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
            return null;
        }

        public static FtpWebResponse UploadFileToServer(string file, string filepath)
        {
            if (!string.IsNullOrEmpty(file))
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpurl + filepath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpusername, ftppassword);
                request.EnableSsl = false;
                request.UseBinary = true;

                byte[] b = Convert.FromBase64String(file.Substring(file.LastIndexOf(',') + 1));

                request.ContentLength = b.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(b, 0, b.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response;
                }
            }
            return null;
        }

        public static byte[] DownloadFileFromServer(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return null;
            }
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpurl + filepath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ftpusername, ftppassword);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            reader.BaseStream.CopyTo(memstream);
                            return memstream.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string GetBase64(this byte[] filecontent)
        {
            return filecontent != null ? "data:image;base64," + Convert.ToBase64String(filecontent) : "";
        }

        public static string GetBase64(this string filepath)
        {
            return DownloadFileFromServer(filepath).GetBase64();
        }

        public static FtpWebResponse DeleteFileOnServer(string filepath)
        {
            // The serverUri parameter should use the ftp:// scheme.
            // It contains the name of the server file that is to be deleted.
            // Example: ftp://contoso.com/someFile.txt.
            //
            if (string.IsNullOrEmpty(filepath))
            {
                return null;
            }
            var path = filepath.GetFullPath();
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(ftpusername, ftppassword);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response;
            }
        }

        public static string GetExtension(this string filename)
        {
            var _arr = filename.Split('.');
            return _arr.Length > 0 ? "." + _arr[_arr.Length - 1] : "";
        }

        public static string GetUniqueName(this IFormFile file)
        {
            return Guid.NewGuid().ToString() + file.FileName.GetExtension();
        }

        public static string GetUniqueName(string extension = null)
        {
            return Guid.NewGuid().ToString() + "." + extension ?? "png";
        }

        public static bool IsSucceed(this FtpWebResponse response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
        }

        public static bool CheckIfFileExistsOnServer(string filename)
        {
            var path = filename.GetFullPath();
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(ftpusername, ftppassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }
        public static string GetFullPath(this string filepath)
        {
            return ftpurl + filepath;
        }
    }
}
