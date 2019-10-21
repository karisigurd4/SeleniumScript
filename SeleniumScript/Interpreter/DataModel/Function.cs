namespace SeleniumScript.Implementation.DataModel
{
  using System.Collections.Generic;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;
  using Implementation.Enums;
  using global::SeleniumScript.Interpreter.Enums;

  public class Function : Symbol
  {
    public Dictionary<string, ReturnType> Parameters { get; set; }
    public FunctionBodyContext Body { get { return (FunctionBodyContext)Value; } set { Value = (FunctionBodyContext)value; } }

    public Function(string name, ReturnType? returnType) : base(name, returnType ?? ReturnType.Unspecified)
    {
      this.SymbolType = SymbolType.Function;
    }
  }
}
