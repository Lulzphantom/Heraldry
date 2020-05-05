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
        // Config file name
        static string Cfg = "Cfg.dat";

        // User preference
        public static double Opacity { get; set; } = 1;
        public static string LifRoute { get; set; } = "";

        // Position config
        public static double Top { get; set; } = 0;
        public static double Left { get; set; } = 0;

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
                }
                if (data["Configuration"]["left"] != null)
                {
                    Left = Convert.ToDouble(data["Configuration"]["left"]);
                }
                if (data["Configuration"]["top"] != null)
                {
                    Top = Convert.ToDouble(data["Configuration"]["top"]);
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
                data["Configuration"]["left"] = Left.ToString();
                data["Configuration"]["top"] = Top.ToString();
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
                data["Configuration"].AddKey("left", Left.ToString());
                data["Configuration"].AddKey("top", Top.ToString());
                parser.WriteFile(AppDomain.CurrentDomain.BaseDirectory + @"\" + Cfg, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }   
}
