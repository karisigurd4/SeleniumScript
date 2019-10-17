namespace SeleniumScript.Interfaces
{
  using static SeleniumScript.Grammar.SeleniumScriptParser;

  public interface ISeleniumScriptVisitor
  {
    void Visit(ExecutionUnitContext context);
  }
}
