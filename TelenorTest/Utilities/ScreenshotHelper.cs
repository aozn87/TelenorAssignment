using OpenQA.Selenium;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using static System.Net.Mime.MediaTypeNames;

namespace TelenorTest.Utilities
{
    public static class ScreenshotHelper
    {
        public static void TakeScreenshot(IWebDriver driver, string testName)
        {
            try
            {
                ITakesScreenshot screenshotDriver = (ITakesScreenshot)driver;
                Screenshot screenshot = screenshotDriver.GetScreenshot();

                string screenshotsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                if (!Directory.Exists(screenshotsDirectory))
                    Directory.CreateDirectory(screenshotsDirectory);

                string filePath = Path.Combine(screenshotsDirectory, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                using (var ms = new MemoryStream(screenshot.AsByteArray))
                using (var image = SixLabors.ImageSharp.Image.Load(ms))
                {
                    image.Save(filePath, new PngEncoder());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot failed: {ex.Message}");
            }
        }

    }
}
