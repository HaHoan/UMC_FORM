using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using System;
using System.IO;

[assembly: OwinStartup(typeof(UMC_FORM.Startup))]

namespace UMC_FORM
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            changeNameFileVersion();
            GlobalConfiguration.Configuration
          .UseSqlServerStorage(
              "DataConnection",
              new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) });


            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }

        private Tuple<string, string> getOldVersion(string file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var start = fileName.IndexOf('[');
            var end = fileName.IndexOf(']');
            string versionOld = "0";
            if (start > 0 && end > 0)
            {
                versionOld = fileName.Substring(start + 1, end - start);
                fileName = fileName.Substring(0, start);
            }
            return Tuple.Create(versionOld, fileName);
        }
        private void changeNameFileVersion()
        {
            try
            {
                var path = Bet.Util.Config.GetValue("folder") + @"Content\css\";
                string[] files = Directory.GetFiles(path, "*.css",SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var version = Bet.Util.Config.GetValue("version");
                    var getVer = getOldVersion(file);
                    var versionOld = getVer.Item1;

                    if (version != versionOld)
                    {
                        if (version == "0")
                        {
                            File.Move(file, path + getVer.Item2 + ".css");
                        }
                        else
                        {
                            File.Move(file, path + getVer.Item2 + "[" + version + "].css");
                        }
                    }

                }
                var pathJS = Bet.Util.Config.GetValue("folder") + @"Content\js\";
                string[] filesJs = Directory.GetFiles(pathJS, "*.js",SearchOption.AllDirectories);
                foreach (var file in filesJs)
                {
                    var version = Bet.Util.Config.GetValue("version");
                    var getVer = getOldVersion(file);
                    var versionOld = getVer.Item1;

                    if (version != versionOld)
                    {
                        if (version == "0")
                        {
                            File.Move(file, pathJS + getVer.Item2 + ".js");
                        }
                        else
                        {
                            File.Move(file, pathJS + getVer.Item2 + "[" + version + "].js");
                        }
                    }

                }
            }
            catch (Exception e)
            {

                Console.Write(e.Message.ToString());
            }

        }
    }
}
