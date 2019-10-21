namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interpreter.Enums;
  using Grammar;
  using static Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    private Symbol ResolveoptionalParameter(DataContext[] context, int parameterIndex)
    {
      if (context == null || parameterIndex >= context.Length)
      {
        return new Symbol(string.Empty, ReturnType.String, string.Empty);
      }

      var data = Visit(context[parameterIndex]);

      if (data == null)
      {
        return new Symbol(string.Empty, ReturnType.String, string.Empty);
      }

      return data;
    }

  }
}
