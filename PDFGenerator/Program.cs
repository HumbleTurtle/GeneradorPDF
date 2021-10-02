using PuppeteerSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDFGenerator
{
    class Program
    {
        private Browser browser;

        public async Task Inicializar()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            string[] launchArgs = {   "--autoplay-policy=user-gesture-required",
                                      "--disable-background-networking",
                                      "--disable-background-timer-throttling",
                                      "--disable-backgrounding-occluded-windows",
                                      "--disable-breakpad",
                                      "--disable-client-side-phishing-detection",
                                      "--disable-component-update",
                                      "--disable-default-apps",
                                      "--disable-dev-shm-usage",
                                      "--disable-domain-reliability",
                                      "--disable-extensions",
                                      "--disable-features=AudioServiceOutOfProcess",
                                      "--disable-hang-monitor",
                                      "--disable-ipc-flooding-protection",
                                      "--disable-notifications",
                                      "--disable-offer-store-unmasked-wallet-cards",
                                      "--disable-popup-blocking",
                                      "--disable-print-preview",
                                      "--disable-prompt-on-repost",
                                      "--disable-renderer-backgrounding",
                                      "--disable-setuid-sandbox",
                                      "--disable-speech-api",
                                      "--disable-sync",
                                      "--hide-scrollbars",
                                      "--ignore-gpu-blacklist",
                                      "--metrics-recording-only",
                                      "--mute-audio",
                                      "--no-default-browser-check",
                                      "--no-first-run",
                                      "--no-pings",
                                      "--no-sandbox",
                                      "--no-zygote",
                                      "--password-store=basic",
                                      "--use-gl=swiftshader",
                                      "--use-mock-keychain"
            };

            this.browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = launchArgs
            });
        }

        public async void GenerarPDF(string codigo, string url)
        {
            try
            {
                var page = await this.browser.NewPageAsync();

                await page.SetViewportAsync(
                    new ViewPortOptions { Width = 1280, Height = 720 }
                );

                await page.GoToAsync(url);
                await page.PdfAsync($"{DateTime.Today.ToShortDateString().Replace("/", "-")}.pdf");
                await page.CloseAsync();

                Console.WriteLine(codigo + " OK", Console.Out);
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    w.WriteLine(codigo + " OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(codigo + " ERROR", Console.Out);
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    w.WriteLine(codigo + " ERROR");
                }
            }
        }

        static async Task Main(string[] args)
        {
            Program program = new Program();

            await program.Inicializar();

            Console.WriteLine("INICIADO");

            var line = Console.ReadLine();
            while (line != "-1")
            {
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    w.WriteLine(line);
                }
                    Regex patron = new Regex(@"([^\ ]+) ([^\ ]+)", RegexOptions.IgnoreCase);

                var matches = patron.Match(line);
                if (matches.Groups.Count >= 3)
                {
                    program.GenerarPDF(matches.Groups[1].Value, matches.Groups[2].Value);
                }

                line = Console.ReadLine();
            }

        }
    }
}
