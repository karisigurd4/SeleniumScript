namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Grammar;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitResolveReference([NotNull] ResolveReferenceContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      seleniumLogger.Log($"Resolving reference {identifier}", SeleniumScriptLogLevel.InterpreterDetails);
      return callStack.Current.ResolveVariable(identifier);
    }

    public override object VisitResolveIntLiteral([NotNull] ResolveIntLiteralContext context)
    {
      seleniumLogger.Log($"Resolving int literal", SeleniumScriptLogLevel.InterpreterDetails);
      return context.INTLITERAL().GetText();
    }

    public override object VisitResolveStringLiteral([NotNull] ResolveStringLiteralContext context)
    {
      seleniumLogger.Log($"Resolving string literal", SeleniumScriptLogLevel.InterpreterDetails);
      var data = context.STRINGLITERAL().GetText();
      return data.Substring(1, data.Length - 2);
    }
  }
}
