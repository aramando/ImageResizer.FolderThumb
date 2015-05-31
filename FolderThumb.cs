using System;
using System.Collections.Generic;
//using System.Text;
//using ImageResizer.Resizing;
//using System.Web.Hosting;
using ImageResizer.Configuration;
//using System.Text.RegularExpressions;
using System.Collections.Specialized;
//using ImageResizer.ExtensionMethods;
using ImageResizer.Util;
using System.IO;

namespace ImageResizer.Plugins {

    /// <summary>
    /// Plugin to select a random image from a folder to use as a thumbnail if the requested file is not available.
    /// Add &lt;folderThumb activateOnFilename="folder.jpg" /&gt; to &lt;resizer&gt; section in web.config with list of filenames to enable replacement on
    /// </summary>
    public class FolderThumb : IQuerystringPlugin, IPlugin {        

        Config c;

        List<string> activeFilenames;

        void Pipeline_Rewrite(System.Web.IHttpModule sender, System.Web.HttpContext context, Configuration.IUrlEventArgs e)
        {
            string file = Path.GetFileName(e.VirtualPath);

            if (activeFilenames.Contains(file))
            {
                string filePhysicalPath = PathUtils.MapPathIfAppRelative("~" + e.VirtualPath);

                if (!File.Exists(filePhysicalPath))
                {
                    string physicalDirPath = Path.GetDirectoryName(filePhysicalPath);
                    string[] files = Directory.GetFiles(physicalDirPath, "*.jpg", SearchOption.TopDirectoryOnly);

                    if (files.Length > 0)
                    {
                        Random r = new Random();
                        int index = r.Next(0, files.Length);
                        file = files[index];
                        string newVirtualPath = Path.GetDirectoryName(e.VirtualPath).Replace("\\", "/") + "/" + Path.GetFileName(file);
                        e.VirtualPath = newVirtualPath;
                    }
                    else
                    {
                        string fallback = c.get("folderThumb.default", "");
                        e.VirtualPath = "/images/" + fallback;
                    }

                    if (e.QueryString.Count == 0)
                    {
                        var col = new NameValueCollection();
                        col.Add("fthumb", "true");
                        e.QueryString.Add(col);
                    }  
                } 

            }

        }


        public IPlugin Install(Configuration.Config c)
        {
            this.c = c;
            this.activeFilenames = new List<String>(c.get("folderThumb.activateOnFilename", "").Split(new char[] { ',', ' ', ';' }));

            if (c.Plugins.Has<FolderThumb>())
                throw new InvalidOperationException();

            c.Pipeline.Rewrite += new UrlRewritingEventHandler(Pipeline_Rewrite);
            c.Plugins.add_plugin(this);
            return this;
        }


        public bool Uninstall(Configuration.Config c)
        {
            c.Pipeline.ImageMissing -= Pipeline_Rewrite;
            c.Plugins.remove_plugin(this);
            return true;
        }

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return new string[] { "fthumb" };
        }

    }
}
