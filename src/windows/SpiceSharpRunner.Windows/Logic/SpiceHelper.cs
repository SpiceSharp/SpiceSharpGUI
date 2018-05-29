using SpiceSharpParser.ModelReader.Netlist.Spice;
using SpiceSharpParser.ModelReader.Netlist.Spice.Processors.Controls.Plots;
using System;

namespace SpiceSharpRunner.Windows.Logic
{
    public class SpiceHelper
    {
        public static SpiceModelReaderResult GetSpiceSharpNetlist(string netlist)
        {
            SpiceSharpParser.ParserFacade facade = new SpiceSharpParser.ParserFacade();

            var settings = new SpiceSharpParser.ParserSettings();
            settings.SpiceNetlistParserSettings.HasTitle = true;
            settings.SpiceNetlistParserSettings.IsNewlineRequired = true;
            settings.SpiceNetlistParserSettings.IsEndRequired = false;

            settings.SpiceModelReaderSettings.EvaluatorMode = SpiceSharpParser.ModelReader.Netlist.Spice.Evaluation.CustomFunctions.SpiceEvaluatorMode.Spice3f5;
            var parserResult = facade.ParseNetlist(netlist, settings, Environment.CurrentDirectory);

            return parserResult.ReaderResult;
        }

        public static bool IsPlotPositive(Plot plot)
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
 