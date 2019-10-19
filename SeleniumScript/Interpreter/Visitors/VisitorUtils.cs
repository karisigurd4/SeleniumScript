namespace SeleniumScript.Implementation
{
  using Grammar;
  using Interfaces;
  using static Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>, ISeleniumScriptInterpreter
  {
    private object ResolveoptionalParameter(DataContext[] context, int parameterIndex)
    {
      if (context == null || parameterIndex >= context.Length)
      {
        return string.Empty;
      }
      var data = Visit(context[parameterIndex]);
      if (data == null)
      {
        return string.Empty;
      }

      return data;
    }

  }
}
