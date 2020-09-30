using System;
using System.IO;
using System.Web;

namespace RentACarMVC.Classes
{
    public class Helper
    {
        public static bool UploadPhoto(HttpPostedFileBase file, string folder, string name)
        {
            if (file == null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name))
            {
                return false;
            }
            try
            {
                var path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                file.SaveAs(path);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public static bool DeletePhoto(string name)
        {
            try
            {
                var archiv = new FileInfo(HttpContext.Current.Server.MapPath(name));
                if (!archiv.Exists)
                {
                    return false;
                }
                archiv.Delete();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}