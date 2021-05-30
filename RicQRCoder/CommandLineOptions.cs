using CommandLine;
using CommandLine.Text;
using QRCoder;
using System.Collections.Generic;

namespace RicQRCoder
{
     class Options
    {
        /*
        [Value(index: 0, Required = true, HelpText = "Image file Path to analyze.")]
        public string Path { get; set; }
*/
        [Option(shortName: 'i', longName: "content", Required = true, HelpText = "String or full File name with your content.", Default = null)]
        public string Content { get; set; }

        [Option(shortName: 'o', longName: "outFile", Required = true, HelpText = "Output file. Full file name without extension (extension from outFormat parameter).", Default = null)]
        public string OutputFileName { get; set; }

        [Option(longName: "outFormat", Required = false, HelpText = "Image format for outputfile.", Default = SupportedImageFormat.Png)]
        public SupportedImageFormat ImageFormat { get; set; }

        [Option(longName: "eccLevel", Required = false, HelpText = "Error correction level: L-7%, M-15%, Q-25%, H-30%.", Default = QRCodeGenerator.ECCLevel.M)]
        public QRCodeGenerator.ECCLevel EccLevel { get; set; }

        [Option(longName: "pixelSize", Required = false, HelpText = "The pixel size each b/w module is drawn (from 1 and more).", Default = 20)]
        public int PixelsPerModule { get; set; }

        [Option(longName: "background", Required = false, HelpText = "Background color.", Default = "#000000")]
        public string ForegroundColor { get; set; }

        [Option(longName: "foreground", Required = false, HelpText = "Foreground color.", Default = "#FFFFFF")]
        public string BackgroundColor { get; set; }

        [Option(shortName: 'l', longName: "labelPath", Required = false, HelpText = "Image from file (full file name with extension).", Default = null)]
        public string ImgFileName { get; set; }

        [Option(longName: "labelSize", Required = false, HelpText = "Sets how much 1-99% of the QR Code will be covered by the icon.", Default = 15)]
        public int ImgSize { get; set; } 

        [Usage(ApplicationAlias = "RicQRCoder")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Creates a QR image file from your content (string or file)", new Options { OutputFileName = "QRImageFile.png" })

//                yield return new Example("Normal scenario", new Options { InputFile = "file.bin", OutputFile = "out.bin" });
//                yield return new Example("Logging warnings", UnParserSettings.WithGroupSwitchesOnly(), new Options { InputFile = "file.bin", LogWarning = true });
//                yield return new Example("Logging errors", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new Options { InputFile = "file.bin", LogError = true }
                };
            }
        }    
            /*
                    [HelpOption]
                    public string GetUsage()
                    {
                        return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
                    }

             [HelpOption]
                        public string GetUsage()
                        {
                            var help = new HelpText {
                                Heading = new HeadingInfo(ThisAssembly.Title, ThisAssembly.InformationalVersion),
                                Copyright = new CopyrightInfo(ThisAssembly.Author, 2012),
                                AdditionalNewLineAfterOption = true,
                                AddDashesToOption = true
                            };
                            this.HandleParsingErrorsInHelp(help);
                            help.AddPreOptionsLine("<<license details here.>>");
                            help.AddPreOptionsLine("Usage: CSharpTemplate -tSomeText --numeric 2012 -b");
                            help.AddOptions(this);

                            return help;
                        }

             */
        }
}

/*
                 Console.WriteLine("Encodes barcode images using the library https://github.com/codebude/QRCoder\n\n setting:");
                Console.WriteLine(" content from (or):");
                Console.WriteLine("  \"\"              noname String (\"your content\")");
                Console.WriteLine("  --inStr=        String (\"your content\")");
                Console.WriteLine("  --inFile=\"\"     File with your content (full file name with extension)\n");
                Console.WriteLine(" coder setting:");
                Console.WriteLine("  --outFormat=    Image format for outputfile: png, jpg, gif, bmp, tiff, svg, xaml, ps, eps (def: png)");
                Console.WriteLine("  --outFile=\"\"    Output file. Full file name without extension (extension from outFormat parameter)");
                Console.WriteLine("  --eccLevel=     Error correction level: L-7%, M-15%, Q-25%, H-30% (def: M)");
                Console.WriteLine("  --pixelSize=    Set pixel size each b/w module is drawn (from 1px and above)(def: 20)");
                Console.WriteLine("  --background=   Background color (def: #000000)");
                Console.WriteLine("  --foreground=   Foreground color (def: #FFFFFF)");
                Console.WriteLine(" extension (the central picture inside):");
                Console.WriteLine("  --iconPath=\"\"   Image from file (full file name with extension)");
                Console.WriteLine("  --iconSize=     Sets how much 1-99% of the QR Code will be covered by the icon (def:15)");
                //                Console.WriteLine("  --iconBorder= Width of the border which is drawn around the icon. Minimum: 1 (def:6)");
                Console.WriteLine("\ntest string\nRicQRCoder.exe --inStr=\"Hello QR\u0022--outFile=\u0022C:\\test_QR\u0022--outFormat=gif --eccLevel=H --pixelSize=10 --foreground=#1E9AFE");
 
 */