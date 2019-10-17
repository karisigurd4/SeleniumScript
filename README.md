# SeleniumScript
A scripting language abstraction built on top of Selenium WebDriver. Intended to make the process of writing frontend tests quicker and easier to maintain. Any flavour of IWebDriver can be plugged into the component. 

## Example script
    NavigateTo("https://www.microsoft.com/sv-se/");
    Click("//button[@id = 'search']");
    SendKeys("//input[@id = 'cli_shellHeaderSearchInput']", "Windows 10");
    Click("//input[@id = 'cli_shellHeaderSearchInput']//following::a", "First search result");
    Click("//div[@class = 'sfw-dialog']//following::div[@class = 'c-glyph glyph-cancel']", "Close dialog button");
    string productTitle = GetElementText("//h1[@id = 'DynamicHeading_productTitle']", "Product title");

    NavigateTo("https://www.google.com/");
    SendKeys("//input[@name = 'q']", productTitle, "Search bar");
    Click("//input[@name = 'q']"//following::a, "First search result");

## Usage
    var script = "NavigateTo("https://www.microsoft.com/sv-se/");  Click("//button[@id = 'search']"); ...";

    var driver = new ChromeDriver(new ChromeOptions() 
    {
      LeaveBrowserRunning = false
    });

    using (var seleniumScript = new SeleniumScript())
    {
      seleniumScript.Run(script);
    }
