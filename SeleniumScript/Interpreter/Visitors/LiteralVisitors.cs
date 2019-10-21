namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interpreter.Enums;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitResolveReference([NotNull] ResolveReferenceContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      seleniumLogger.Log($"Resolving reference {identifier}", SeleniumScriptLogLevel.InterpreterDetails);
      return callStack.Current.ResolveVariable(identifier);
    }

    public override Symbol VisitResolveIntLiteral([NotNull] ResolveIntLiteralContext context)
    {
      seleniumLogger.Log($"Resolving int literal", SeleniumScriptLogLevel.InterpreterDetails);
      var data = context.INTLITERAL().GetText();
      return new Symbol(string.Empty, ReturnType.Int, data);
    }

    public override Symbol VisitResolveStringLiteral([NotNull] ResolveStringLiteralContext context)
    {
      seleniumLogger.Log($"Resolving string literal", SeleniumScriptLogLevel.InterpreterDetails);
      var data = context.STRINGLITERAL().GetText();
      return new Symbol(string.Empty, ReturnType.String, data.Substring(1, data.Length - 2));
    }
  }
}
