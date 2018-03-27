using SpiceSharpParser.Connector;
using SpiceSharpParser.Connector.Processors.Controls.Plots;
using SpiceSharpParser.Parser.Parsing;
using SpiceSharpParser.Parser.Translation;
using SpiceSharpParser.SpiceLexer;
using System.Linq;

namespace SpiceSharpRunner.Windows.Logic
{
    public class SpiceHelper
    {
        public static SpiceToken[] GetTokens(string text)
        {
            var lexer = new SpiceLexer(new SpiceLexerOptions { HasTitle = true });
            var tokensEnumerable = lexer.GetTokens(text);
            return tokensEnumerable.ToArray();
        }

        public static ParseTreeNonTerminalNode GetParseTree(SpiceToken[] tokens)
        {
            return new SpiceSharpParser.Parser.Parsing.Parser().GetParseTree(tokens); 
        }

        public static SpiceSharpParser.Model.Netlist GetNetlist(ParseTreeNonTerminalNode root)
        {
            var translator = new ParseTreeTranslator();
            return translator.Evaluate(root) as SpiceSharpParser.Model.Netlist;
        }

        public static Netlist GetSpiceSharpNetlist(SpiceSharpParser.Model.Netlist netlist)
        {
            var connector = new Connector();
            return connector.Translate(netlist);
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
 