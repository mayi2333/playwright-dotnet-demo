using Microsoft.Playwright;
using System.Text.RegularExpressions;
Console.ReadLine();
using var playwright = await Playwright.CreateAsync();
//await using var browser = await playwright.Chromium.ConnectAsync()
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions() { Headless=false, ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe" });
var page = await browser.NewPageAsync();
//await page.GotoAsync("chrome://dino/");
//page.Response += (sender, e) =>
//{
//    Console.WriteLine($"status: {e.Status} url: {e.Url}");
//};
await page.GotoAsync("https://dazi.kukuw.com/");
await page.Locator("[id='time']").FillAsync("2");
await page.ClickAsync("[value='开始测试']");
var contentLocator = page.Locator("//*[@id=\"content\"]");
await contentLocator.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
var locators = await contentLocator.Locator("//div[@class=\"text\"]").AllAsync();
Console.WriteLine($"elements count: {locators.Count}");
string speedRegex = @"([1-9]\d*\.?\d*)|(0\.\d*[1-9])";
int interval = 250;
try
{

    foreach (var locator in locators)
    {
        var text = await locator.Locator("//span").InnerTextAsync();
        Console.WriteLine(text);
        foreach (var input in text)
        {
            Thread.Sleep(interval);
            var speedText = page.Locator("//*[@id=\"typing_info_li\"]/ul/li[4]").InnerTextAsync().Result;
            Console.WriteLine(speedText);
            int speed = 0;
            int.TryParse(Regex.Match(speedText, speedRegex)?.Value, out speed);
            if (speed < 515 && interval > 0)
            {
                interval -= 5;
            }
            else
            {
                interval += 10;
            }
            //SpinWait.SpinUntil(() =>
            //{
            //    var speedText = page.Locator("//*[@id=\"typing_info_li\"]/ul/li[4]").InnerTextAsync().Result;
            //    Console.WriteLine(speedText);
            //    int speed = 0;
            //    int.TryParse(Regex.Match(speedText, speedRegex)?.Value, out speed);
            //    return speed < 280;
            //});
            await page.PressAsync("body", input.ToString());
        }
        await page.PressAsync("body", " ");
        Thread.Sleep(300);
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
SpinWait.SpinUntil(() => page.IsClosed);
Console.WriteLine("页面已关闭");