namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Implementation.DataModel;

  public interface ISymbolScope 
  {
    void Define(Symbol symbol);
    Symbol Lookup(string name);
  }
}
