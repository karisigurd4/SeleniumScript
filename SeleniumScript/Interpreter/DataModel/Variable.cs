namespace SeleniumScript.Implementation.DataModel
{
  using global::SeleniumScript.Interpreter.Enums;
  using Implementation.Enums;

  public class Variable : Symbol
  {
    public Variable(string name, ReturnType returnType, string value = null) : base(name, returnType)
    {
      this.Value = value;
      this.SymbolType = SymbolType.Variable;
    }
  }
}
