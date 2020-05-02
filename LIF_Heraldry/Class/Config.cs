using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LIF_Heraldry
{
    public static class Config
    {
        static string Cfg = "Cfg.dat";

        public static double Opacity { get; set; } = 1;
        public static string LifRoute { get; set; } = "";

        public static void CheckConfig()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg);
                LifRoute = data["Configuration"]["game_route"];
                if (data["Configuration"]["opacity"] != null)
                {
                    Opacity = Convert.ToDouble(data["Configuration"]["opacity"]);
                    Console.WriteLine(Opacity);
                }                
            }
            else
            {
                CreateConfig();
            }
        }
        public static void SaveConfig()
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = new IniData();
                data["Configuration"]["game_route"] = LifRoute;
                data["Configuration"]["opacity"] = Opacity.ToString();
                parser.WriteFile(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        static void CreateConfig()
        {
            try
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg).Dispose();
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg);
                data.Sections.AddSection("Configuration");
                data["Configuration"].AddKey("game_route", LifRoute);
                data["Configuration"].AddKey("opacity", Opacity.ToString());
                parser.WriteFile(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }   
}
