namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interfaces;
  using System.Collections.Generic;

  public class SymbolScope : ISymbolScope
  {
    private readonly SymbolScope enclosingScope;
    private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

    public SymbolScope(SymbolScope enclosingScope)
    {
      this.enclosingScope = enclosingScope;
    }

    public void Define(Symbol symbol)
    {
      symbols.Add(symbol.Name, symbol);
    }

    public Symbol Lookup(string name)
    {
      if(symbols.ContainsKey(name))
      {
        return symbols[name];
      }

      if(enclosingScope != null)
      {
        return enclosingScope.Lookup(name);
      }

      return null;
    }
  }
}
