using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMP_to_CSV
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("BMPToCSV");
            Console.WriteLine("Nov 1 2021");
            Console.WriteLine("Mark Colling\n");
            Console.WriteLine("Converts bmp files into a csv for each pixal. It uses a legend file to convert the hex value to a custom value.");

            // Get Legend File
            #region get legend
            string legendPath = "legend.txt";
            Dictionary<string, string> legend = new Dictionary<string, string>();
            if (File.Exists(legendPath))
            {
                var reader = new StreamReader(legendPath);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var lineArray = line.Split(',');
                    legend.Add(lineArray[0].Trim().ToLower(), lineArray[1].Trim());
                }
            }
            else
            {
                legend = null;
                Console.WriteLine("Legend file not found, will instead us HEX vaules");
            }
            #endregion

            // Get input from user on which file to convert
            #region User Prompt
            var localFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bmp");

            if (localFiles.Length == 0)
            {
                Console.WriteLine("No bmp files found in this directory.");
                return;
            }

            Console.WriteLine("Please select which file you would like to convert:");
            int count = 0;
            foreach (var lf in localFiles)
            {
                Console.WriteLine("  " + count + " - " + Path.GetFileName(lf));
                count++;
            }
            Console.Write("> ");
            string pathNum = Console.ReadLine();

            // Is input valid?
            if (int.Parse(pathNum) < 0 || int.Parse(pathNum) > count + 1)
            {
                Console.WriteLine("Sorry, your selection was invalid.");
                return;
            }

            string path = localFiles[int.Parse(pathNum)];                   // The file path
            string originalName = Path.GetFileNameWithoutExtension(path);   // This is used for the output file name
            Bitmap input = new Bitmap(path);
            #endregion

            // Convert pixals to proper values
            #region Convert Pixals to Values
            string finalString = "";

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Color pixel = input.GetPixel(x, y);
                    finalString += x + ",0," + y + "," + GetLegendName(legend, HexConverter(pixel)) + "\n";
                }
            }
            #endregion

            // Save to CSV
            File.WriteAllText(originalName + "_converted.csv", finalString);
            Console.WriteLine("\nBmp has been converted!");
            Console.WriteLine("It can be found in the same location as this exe under the name \"" + originalName + "_converted.csv" + "\"");
        }

        private static string GetLegendName(Dictionary<string, string> legend, string v)
        {
            if (legend.ContainsKey(v.ToLower()))
            {
                return legend[v.ToLower()];
            }
            else
            {
                return v;
            }

        }

        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
