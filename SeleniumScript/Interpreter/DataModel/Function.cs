namespace SeleniumScript.Implementation.DataModel
{
  using System.Collections.Generic;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;
  using Implementation.Enums;

  public class Function : Symbol
  {
    public Dictionary<string, string> Parameters { get; set; }
    public FunctionBodyContext Body { get; set; }

    public Function(string name, string returnType) : base(name, returnType)
    {
      this.SymbolType = SymbolType.Function;
    }
  }
}
