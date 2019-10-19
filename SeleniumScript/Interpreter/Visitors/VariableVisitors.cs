namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Grammar;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitVariableDeclaration([NotNull] VariableDeclarationContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      callStack.Current.AddVariable(identifier);
      if (context.ASSIGNMENT() != null && context.data() != null)
      {
        callStack.Current.SetVariable(identifier, (string)context.data().Accept(this));
      }
      return null;
    }

    public override object VisitVariableAssignment([NotNull] VariableAssignmentContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var data = (string)context.data().Accept(this);
      callStack.Current.SetVariable(identifier, data);
      return null;
    }
  }
}
