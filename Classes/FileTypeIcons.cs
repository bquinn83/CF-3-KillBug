using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.SqlServer;

namespace KillBug.Classes
{
    public class FileTypeIcons
    {
        public static string IconPathV2(string filePath)
        {
            string blankPath = "/Content/Images/FileTypeIcons/blank.png";
            string iconPath = "/Content/Images/FileTypeIcons/";

            string type = $"{ Path.GetExtension(filePath).Substring(1) }.png";
            iconPath += type;
            return File.Exists(iconPath) ? iconPath : blankPath; ;
        }

        public static string AttachmentIconPath(string filePath)
        {
            string iconPath;
            switch (Path.GetExtension(filePath))
            {
                case ".aac":
                    iconPath = "/Content/Images/FileTypeIcons/aac.png";
                    break;
                case ".ai":
                    iconPath = "/Content/Images/FileTypeIcons/ai.png";
                    break;
                case ".aiff":
                    iconPath = "/Content/Images/FileTypeIcons/aiff.png";
                    break;
                case ".asp":
                    iconPath = "/Content/Images/FileTypeIcons/asp.png";
                    break;
                case ".avi":
                    iconPath = "/Content/Images/FileTypeIcons/avi.png";
                    break;
                case ".blank":
                    iconPath = "/Content/Images/FileTypeIcons/blank.png";
                    break;
                case ".bmp":
                    iconPath = "/Content/Images/FileTypeIcons/bmp.png";
                    break;
                case ".c":
                    iconPath = "/Content/Images/FileTypeIcons/c.png";
                    break;
                case ".cpp":
                    iconPath = "/Content/Images/FileTypeIcons/cpp.png";
                    break;
                case ".css":
                    iconPath = "/Content/Images/FileTypeIcons/css.png";
                    break;
                case ".dat":
                    iconPath = "/Content/Images/FileTypeIcons/dat.png";
                    break;
                case ".dmg":
                    iconPath = "/Content/Images/FileTypeIcons/dmg.png";
                    break;
                case ".doc":
                    iconPath = "/Content/Images/FileTypeIcons/doc.png";
                    break;
                case ".docx":
                    iconPath = "/Content/Images/FileTypeIcons/docx.png";
                    break;
                case ".dot":
                    iconPath = "/Content/Images/FileTypeIcons/dot.png";
                    break;
                case ".dotx":
                    iconPath = "/Content/Images/FileTypeIcons/dotx.png";
                    break;
                case ".dwg":
                    iconPath = "/Content/Images/FileTypeIcons/dwg.png";
                    break;
                case ".dxf":
                    iconPath = "/Content/Images/FileTypeIcons/dxf.png";
                    break;
                case ".eps":
                    iconPath = "/Content/Images/FileTypeIcons/eps.png";
                    break;
                case ".exe":
                    iconPath = "/Content/Images/FileTypeIcons/exe.png";
                    break;
                case ".flv":
                    iconPath = "/Content/Images/FileTypeIcons/flv.png";
                    break;
                case ".gif":
                    iconPath = "/Content/Images/FileTypeIcons/gif.png";
                    break;
                case ".h":
                    iconPath = "/Content/Images/FileTypeIcons/h.png";
                    break;
                case ".html":
                    iconPath = "/Content/Images/FileTypeIcons/html.png";
                    break;
                case ".ics":
                    iconPath = "/Content/Images/FileTypeIcons/ics.png";
                    break;
                case ".iso":
                    iconPath = "/Content/Images/FileTypeIcons/iso.png";
                    break;
                case ".java":
                    iconPath = "/Content/Images/FileTypeIcons/java.png";
                    break;
                case ".jpg":
                    iconPath = "/Content/Images/FileTypeIcons/jpg.png";
                    break;
                case ".key":
                    iconPath = "/Content/Images/FileTypeIcons/key.png";
                    break;
                case ".m4v":
                    iconPath = "/Content/Images/FileTypeIcons/m4v.png";
                    break;
                case ".mid":
                    iconPath = "/Content/Images/FileTypeIcons/mid.png";
                    break;
                case ".mov":
                    iconPath = "/Content/Images/FileTypeIcons/mov.png";
                    break;
                case ".mp3":
                    iconPath = "/Content/Images/FileTypeIcons/mp3.png";
                    break;
                case ".mp4":
                    iconPath = "/Content/Images/FileTypeIcons/mp4.png";
                    break;
                case ".mpg":
                    iconPath = "/Content/Images/FileTypeIcons/mpg.png";
                    break;
                case ".odp":
                    iconPath = "/Content/Images/FileTypeIcons/odp.png";
                    break;
                case ".ods":
                    iconPath = "/Content/Images/FileTypeIcons/ods.png";
                    break;
                case ".odt":
                    iconPath = "/Content/Images/FileTypeIcons/odt.png";
                    break;
                case ".otp":
                    iconPath = "/Content/Images/FileTypeIcons/otp.png";
                    break;
                case ".ots":
                    iconPath = "/Content/Images/FileTypeIcons/ots.png";
                    break;
                case ".ott":
                    iconPath = "/Content/Images/FileTypeIcons/ott.png";
                    break;
                case ".pdf":
                    iconPath = "/Content/Images/FileTypeIcons/pdf.png";
                    break;
                case ".php":
                    iconPath = "/Content/Images/FileTypeIcons/php.png";
                    break;
                case ".png":
                    iconPath = "/Content/Images/FileTypeIcons/png.png";
                    break;
                case ".pps":
                    iconPath = "/Content/Images/FileTypeIcons/pps.png";
                    break;
                case ".ppt":
                    iconPath = "/Content/Images/FileTypeIcons/ppt.png";
                    break;
                case ".psd":
                    iconPath = "/Content/Images/FileTypeIcons/psd.png";
                    break;
                case ".py":
                    iconPath = "/Content/Images/FileTypeIcons/py.png";
                    break;
                case ".qt":
                    iconPath = "/Content/Images/FileTypeIcons/qt.png";
                    break;
                case ".rar":
                    iconPath = "/Content/Images/FileTypeIcons/rar.png";
                    break;
                case ".rb":
                    iconPath = "/Content/Images/FileTypeIcons/rb.png";
                    break;
                case ".rtf":
                    iconPath = "/Content/Images/FileTypeIcons/rtf.png";
                    break;
                case ".sql":
                    iconPath = "/Content/Images/FileTypeIcons/sql.png";
                    break;
                case ".tga":
                    iconPath = "/Content/Images/FileTypeIcons/tga.png";
                    break;
                case ".tgz":
                    iconPath = "/Content/Images/FileTypeIcons/tgz.png";
                    break;
                case ".tiff":
                    iconPath = "/Content/Images/FileTypeIcons/tiff.png";
                    break;
                case ".txt":
                    iconPath = "/Content/Images/FileTypeIcons/txt.png";
                    break;
                case ".wav":
                    iconPath = "/Content/Images/FileTypeIcons/wav.png";
                    break;
                case ".xls":
                    iconPath = "/Content/Images/FileTypeIcons/xls.png";
                    break;
                case ".xlsx":
                    iconPath = "/Content/Images/FileTypeIcons/xlsx.png";
                    break;
                case ".xml":
                    iconPath = "/Content/Images/FileTypeIcons/xml.png";
                    break;
                case ".yml":
                    iconPath = "/Content/Images/FileTypeIcons/yml.png";
                    break;
                case ".zip":
                    iconPath = "/Content/Images/FileTypeIcons/zip.png";
                    break;
                default:
                    iconPath = "/Content/Images/FileTypeIcons//blank.png";
                    break;
            }
            return iconPath;
        }
    }
}