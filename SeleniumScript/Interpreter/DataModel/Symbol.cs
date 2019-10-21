namespace SeleniumScript.Implementation.DataModel
{
  using global::SeleniumScript.Implementation.Enums;
  using global::SeleniumScript.Interpreter.Enums;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public class Symbol
  {
    private object _value = null;

    public object Value
    {
      get
      {
        if (_value is Symbol)
        {
          return ((Symbol)_value).Value;
        }

        return _value;
      }

      set { _value = value; }
    }

    public string Name { get; set; }
    public ReturnType ReturnType { get; set; }
    public SymbolType SymbolType { get; protected set; }

    public FunctionBodyContext AsFunction => (FunctionBodyContext)Value;
    public int AsInt => Value is int ? (int)Value : int.Parse((string)Value);
    public string AsString => Value.ToString();
    public bool AsBool => (bool)Value;
    public string[] AsArray => (string[])Value;

    public Symbol(string name, ReturnType returnType, object value = null)
    {
      this.Name = name;
      this.ReturnType = returnType;
      this.Value = value; 
    }


  }
}
