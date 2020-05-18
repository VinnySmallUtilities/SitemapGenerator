using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace vinnysitemapgenerator
{
    class Program
    {
        public static readonly int    Version     = 2020_05_18;
        public static readonly string cfgFileName = "vsm.cfg";
        public static readonly string cfgSite     = "site.cfg";

        public static          string CurDir  = "";
        public static          string SiteUrl = "";
        static void Main(string[] args)
        {
            CurDir     = Directory.GetCurrentDirectory();
            bool error = false;
            try
            {
                var siteConfig = File.ReadAllLines(cfgSite);

                SiteUrl = siteConfig[0].Trim();
            }
            catch
            {
                error = true;
            }

            Console.WriteLine("Vinny sitemap generator: " + Version);
            Console.WriteLine("Current directory: \r\n" + CurDir);

            if (error || args.Length >= 1)
            {
                GrantHelp();
                return;
            }

            var collection = GetFilesFromConfig();
            AddNewFiles(collection);

            GenerateCfg(collection);
            GenerateXml(collection);
        }

        public static void GenerateXml(SortedList<string, SiteFile> collection)
        {
            List<String> xml = new List<string>(collection.Count);
            xml.Add
            (
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
                "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">"
            );

            foreach (var sf in collection)
            {
                var FileName = Path.GetRelativePath(CurDir, sf.Value.fi.FullName);

                var dt = sf.Value.fi.LastWriteTime;
                var date = dt.Year.ToString("D4") + "-" + dt.Month.ToString("D2") + "-" + dt.Day.ToString("D2");

                xml.Add("<url>");
                xml.Add("<loc>" + sf.Value.getLocation(SiteUrl) + "</loc>");
                xml.Add("<lastmod>" + date + "</lastmod>");
                xml.Add("<priority>" + sf.Value.priority.ToString("F2") + "</priority>");
                xml.Add("</url>");
                xml.Add("");
            }

            xml.Add("</url></urlset>");

            File.WriteAllLines("sitemap.xml", xml, Encoding.UTF8);
        }

        public static void GenerateCfg(SortedList<string, SiteFile> collection)
        {
            List<String> cfg = new List<string>(collection.Count);
            foreach (var sf in collection)
            {
                var FileName = Path.GetRelativePath(CurDir, sf.Value.fi.FullName);
                cfg.Add(sf.Value.getPriorityString() + ": " + FileName);
            }

            File.WriteAllLines(cfgFileName, cfg, Encoding.UTF8);
        }

        public static void AddNewFiles(SortedList<string, SiteFile> collection)
        {
            var e = Directory.EnumerateFiles(CurDir, "*", SearchOption.AllDirectories);

            foreach (var FileName in e)
            {
                if (collection.ContainsKey(FileName))
                    continue;

                var sf = new SiteFile("0.5: " + FileName);

                if (sf.fi.Name == "vsm.exe" || sf.fi.Name == cfgFileName || sf.fi.Name == cfgSite || sf.fi.Name == "robots.txt")
                    continue;

                sf.priority = sf.calculatePriority();
                collection.Add(sf.fi.FullName, sf);
            }
        }

        public static SortedList<string, SiteFile> GetFilesFromConfig()
        {
            var collection = new SortedList<string, SiteFile>(1024);

            if (!File.Exists(cfgFileName))
                return collection;

            var lines = File.ReadAllLines(cfgFileName);

            foreach (var line in lines)
            {
                if (line.Trim().Length <= 0)
                    continue;

                var sf = new SiteFile(line);
                if (!sf.fi.Exists)
                    continue;

                if (!collection.ContainsKey(sf.FileName))
                {
                    collection.Add(sf.fi.FullName, sf);
                }
                else
                {
                    Console.WriteLine("Error in file: duplicate entries for file " + sf.FileName);
                    Console.WriteLine("Duplicated entries will be deleted");
                }
            }

            return collection;
        }

        public class SiteFile
        {
            public readonly string   FileName;
            public readonly string   relativeUrl;
            public          float    priority;
            public readonly FileInfo fi;
            public readonly int      forbidden;

            public SiteFile(string DescriptionLine)
            {
                var splitIndex = DescriptionLine.IndexOf(": ");
                if (splitIndex < 0)
                {
                    throw new ArgumentException("Description line mustbe format '0.88: C:\\Dir\\File.html'", "DesctiptionLine");
                }

                this.FileName = DescriptionLine.Substring(splitIndex + 2).Trim();

                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                var priority  = DescriptionLine.Substring(0, splitIndex).Trim().Replace(",", ".");

                if (priority.Contains("X"))
                {
                    forbidden = 1;
                }
                else
                {
                    this.priority = float.Parse(priority, System.Globalization.NumberStyles.Float);
                }

                fi = new FileInfo(Path.Combine(CurDir, FileName));
                relativeUrl = Path.GetRelativePath(CurDir, fi.FullName);
                if (relativeUrl.StartsWith(@"\") || relativeUrl.StartsWith("/"))
                    relativeUrl = relativeUrl.Substring(1);

                relativeUrl = relativeUrl.Replace(Path.DirectorySeparatorChar, '/');
            }

            public string getPriorityString()
            {
                if (forbidden > 0)
                    return "X";
                else
                    return this.priority.ToString("F4");
            }

            public string getLocation(string SiteUrl)
            {
                if (!SiteUrl.EndsWith("/"))
                    SiteUrl += "/";

                return SiteUrl + relativeUrl;
            }

            public float calculatePriority()
            {
                var cnt = 1;
                var str = relativeUrl;

                do
                {
                    var i = str.IndexOf("/");
                    if (i <= 0)
                        break;

                    str = str.Substring(i);
                    cnt++;
                }
                while (true);

                if (relativeUrl.Contains("index."))
                    cnt--;

                return (float) Math.Pow(0.875, cnt);
            }
        }

        private static void GrantHelp()
        {
            Console.WriteLine();
            Console.WriteLine("usage\t\tИспользование");
            Console.WriteLine("Set in file " + cfgSite + " in first line url of site (in root site directory)");
            Console.WriteLine("Укажите в первой строке файла " + cfgSite + " url сайта (в корневой директории сайта)");
            Console.WriteLine();
            Console.WriteLine("Execute vsm.exe in the root site directory");
            Console.WriteLine("Запустите vsm.exe в корневой директории сайта");
            Console.WriteLine();
            Console.WriteLine("Will generated " + cfgFileName + " and sitemap.xml");
            Console.WriteLine("Сгенерируются " + cfgFileName + " и sitemap.xml");
            Console.WriteLine();
            Console.WriteLine("Change priorities in " + cfgFileName + " if need. String 'X: FileName' deleted file from sitemap.xml");
            Console.WriteLine("Измените, по необходимости, приоритеты в " + cfgFileName + ". Строка 'X: ИмяФайла' удалить файл из sitemap.xml");
            Console.WriteLine();
            Console.WriteLine("Execute vsm.exe again if need");
            Console.WriteLine("Запустите ещё раз vsm.exe , если это необходимо");
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
