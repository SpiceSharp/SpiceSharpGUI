using SpiceSharpParser.ModelsReaders.Netlist.Spice;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation.CustomFunctions;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using System;

namespace SpiceSharpRunner.Windows.Logic
{
    public class SpiceHelper
    {
        public static SpiceNetlistReaderResult GetSpiceSharpNetlist(string netlist, SpiceEvaluatorMode evaluatorMode, int? randomSeed)
        {
            SpiceSharpParser.ParserFacade facade = new SpiceSharpParser.ParserFacade();

            var settings = new SpiceSharpParser.ParserSettings();
            settings.SpiceNetlistParserSettings.HasTitle = true;
            settings.SpiceNetlistParserSettings.IsNewlineRequired = true;
            settings.SpiceNetlistParserSettings.IsEndRequired = false;
            settings.SpiceNetlistModelReaderSettings.EvaluatorRandomSeed = randomSeed;
            settings.SpiceNetlistModelReaderSettings.EvaluatorMode = evaluatorMode;

            settings.SpiceNetlistModelReaderSettings.EvaluatorMode = SpiceEvaluatorMode.Spice3f5;
            settings.WorkingDirectoryPath = Environment.CurrentDirectory;
            var parserResult = facade.ParseNetlist(netlist, settings);

            return parserResult.ReaderResult;
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
 