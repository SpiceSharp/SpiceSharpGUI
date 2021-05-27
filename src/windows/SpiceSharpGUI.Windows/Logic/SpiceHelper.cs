using System;
using System.Text;
using SpiceSharpParser;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Plots;

namespace SpiceSharpGUI.Windows.Logic
{
    public class SpiceHelper
    {
        public static SpiceNetlistParseResult GetSpiceSharpNetlist(string netlist, int? randomSeed, bool hasTitle, Encoding encoding)
        {
            var parser = new SpiceNetlistParser();
            parser.Settings.Lexing.HasTitle = hasTitle;
            parser.Settings.Parsing.IsNewlineRequired = false;
            parser.Settings.Parsing.IsEndRequired = false;
            parser.Settings.WorkingDirectory = Environment.CurrentDirectory;
            
            if (encoding != null)
            {
                parser.Settings.ExternalFilesEncoding = encoding;
            }

            var parserResult = parser.ParseNetlist(netlist);
            return parserResult;
        }

        public static bool IsPlotPositive(XyPlot plot)
        {
            for (var i = 0; i < plot.Series.Count; i++)
            {
                for (var j = 0; j < plot.Series[i].Points.Count; j++)
                {
                    var y = plot.Series[i].Points[j].Y;
                    if (y <= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
 