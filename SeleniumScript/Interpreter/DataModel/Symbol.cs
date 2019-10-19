namespace SeleniumScript.Implementation.DataModel
{
  using global::SeleniumScript.Implementation.Enums;
 
  public abstract class Symbol
  {
    public string Name { get; set; }
    public SymbolType SymbolType { get; protected set; }
    public string ReturnType { get; set; }

    public Symbol(string name, string type)
    {
      this.Name = name;
      this.ReturnType = type;
    }
  }
}
