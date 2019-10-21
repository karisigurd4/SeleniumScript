namespace SeleniumScript.Implementation.DataModel
{
  using Enums;
  using global::SeleniumScript.Interpreter.Enums;

  public class Literal : Symbol
  {
    public Literal(ReturnType returnType, string value) : base ("constant", returnType)
    {
      this.Value = value;
      this.SymbolType = SymbolType.Literal;
    }
  }
}
