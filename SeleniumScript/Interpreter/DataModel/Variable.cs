namespace SeleniumScript.Implementation.DataModel
{
  using Implementation.Enums;

  public class Variable : Symbol
  {
    public Variable(string name, string type) : base(name, type)
    {
      this.SymbolType = SymbolType.Variable;
    }
  }
}
