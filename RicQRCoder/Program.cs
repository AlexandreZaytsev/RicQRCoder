using CommandLine;
using CommandLine.Text;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Collections.Generic;

namespace RicQRCoder
{
    
    class MainClass
    {
        public static void Main(string[] args)
        {
            var parser = new CommandLine.Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            parserResult
                .WithParsed<Options>(options => RunOptionsAndReturnExitCode(options))
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        //in case of errors or --help or --version
        static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false; //remove the extra newline between options
                h.AddEnumValuesToHelpText = true;
                h.MaximumDisplayWidth = 200;
                h.Heading = "RicQRCoder 1.0.0 (uses two libraries (in the program directory): QRCoder.dll, CommandLine.dll)"; //change header
                h.Copyright = "Copyright (c) 2021 cad.ru"; //change copyrigt text
                h.AddPreOptionsLine("");// ("<<license as is>>");
                h.AddPostOptionsText("Good luck...");
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText); 
        }
              

        //In sucess: the main logic to handle the options
        static int RunOptionsAndReturnExitCode(Options opts)
        {
            Bitmap ImgBitmap = null;
            var exitCode = 0;
            string appPath = AppDomain.CurrentDomain.BaseDirectory; //string yourpath = Environment.CurrentDirectory (нет закрывающего слеша)
//            Console.WriteLine("props= {0}", string.Join(",", props));

            //загрузка контента
            if (opts.Content != null)
            {
                if (File.Exists(opts.Content))                                          //если файл существует
                {
                    opts.Content = GetTextFromFile(new FileInfo(opts.Content));         //читаем из файла
                }
                else
                {
                    opts.Content = opts.Content;                                        //иначе читаем из строкового параметра                                         
                }
            }
            else
            {
                opts.Content = "Content not defined";
            }

            //файл QR
            //если имя не пустое и каталог и имя не содержат недопустимыз символов)
            if (opts.OutputFileName != null)// && ((opts.OutputFileName.IndexOfAny(Path.GetInvalidPathChars()) == -1) && (opts.OutputFileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)))
            {
                //удалить расширение если есть и добавить из параметров
                if(Path.HasExtension(opts.OutputFileName))                          //если расширения есть - удалим его
                {
                    opts.OutputFileName = Path.GetDirectoryName(opts.OutputFileName)+ Path.GetFileNameWithoutExtension(opts.OutputFileName);
                }
                opts.OutputFileName += "."+opts.ImageFormat.ToString().ToLower();

                //если каталог не существует
                if ((!Directory.Exists(Path.GetDirectoryName(opts.OutputFileName))))
                {
                    opts.OutputFileName = appPath + Path.GetFileName(opts.OutputFileName);
                }
            }
            else
            {
                opts.OutputFileName = appPath + "QRImageFile." + opts.ImageFormat.ToString().ToLower();
            }

            //файл иконки
            if (opts.LogoFileName != null)
            {
                if (File.Exists(opts.LogoFileName))                                      //если файл существует
                { 
                    ImgBitmap = (Bitmap)Bitmap.FromFile(opts.LogoFileName);
                }
                else 
                {
                    Console.WriteLine($"{appPath}: {opts.LogoFileName}: No such image logo file or directory");
                }
            }

            //проверка количества пикселей
            if ((opts.PixelsPerModule < 1) || (opts.PixelsPerModule >100)) { opts.PixelsPerModule = 20; }

            GenerateQRCode(opts.Content, opts.EccLevel, opts.OutputFileName, opts.ImageFormat, opts.PixelsPerModule, opts.ForegroundColor, opts.BackgroundColor, ImgBitmap, opts.LogoSize);
//            Console.WriteLine($"{Path.GetFullPath(opts.OutputFileName)}: QR file created");
            return exitCode;
        }

        private static void GenerateQRCode(string payloadString, QRCodeGenerator.ECCLevel eccLevel, string outputFileName, SupportedImageFormat imgFormat, int pixelsPerModule, string foreground, string background, Bitmap imgObj, int imgSize)
        {
            //            using (var generator = new QRCodeGenerator())
            using (QRCoder.QRCodeGenerator generator = new QRCodeGenerator())
            {
//                using (var data = generator.CreateQrCode(payloadString, eccLevel))
                using (QRCoder.QRCodeData data = generator.CreateQrCode(payloadString, eccLevel))
                {
                    switch (imgFormat)
                    {
                        case SupportedImageFormat.Png:
                        case SupportedImageFormat.Jpg:
                        case SupportedImageFormat.Gif:
                        case SupportedImageFormat.Bmp:
                        case SupportedImageFormat.Tiff:
                         //   using (var code = new QRCode(data))
                            using (QRCoder.QRCode code = new QRCode(data))
                            {
                                using (var bitmap = code.GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(foreground), ColorTranslator.FromHtml(background), imgObj, imgSize))
                                {
                                    var actualFormat = new OptionSetter().GetImageFormat(imgFormat.ToString());
                                    bitmap.Save(outputFileName, actualFormat);
                                }
                            }
                            break;
                        case SupportedImageFormat.Svg:
                            using (var code = new SvgQRCode(data))
                            {
                                var test = code.GetGraphic(pixelsPerModule, foreground, background, true);
                                using (var f = File.CreateText(outputFileName))
                                {
                                    f.Write(test);
                                    f.Flush();
                                }
                            }
                            break;
#if !NET5_0 && !NET5_0_WINDOWS
                        case SupportedImageFormat.Xaml:
                            using (var code = new XamlQRCode(data))
                            {
                                var test = XamlWriter.Save(code.GetGraphic(pixelsPerModule, foreground, background, true));
                                using (var f = File.CreateText(outputFileName))
                                {
                                    f.Write(test);
                                    f.Flush();
                                }
                            }
                            break;
#endif
                        case SupportedImageFormat.Ps:
                        case SupportedImageFormat.Eps:
                            using (var code = new PostscriptQRCode(data))
                            {
                                var test = code.GetGraphic(pixelsPerModule, foreground, background, true,
                                    imgFormat == SupportedImageFormat.Eps);
                                using (var f = File.CreateText(outputFileName))
                                {
                                    f.Write(test);
                                    f.Flush();
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(imgFormat), imgFormat, null);
                    }

                }
            }
        }

        //прочитать контент из файла
        private static string GetTextFromFile(FileInfo fileInfo)
        {
            var buffer = new byte[fileInfo.Length];

            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                fileStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }

    }

    public class OptionSetter
    {
        public QRCodeGenerator.ECCLevel GetECCLevel(string value)
        {
            Enum.TryParse(value, out QRCodeGenerator.ECCLevel level);
            return level;
        }

        public ImageFormat GetImageFormat(string value)
        {
            switch (value.ToLower())
            {
                case "jpg":
                    return ImageFormat.Jpeg;
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                case "tiff":
                    return ImageFormat.Tiff;
                case "png":
                default:
                    return ImageFormat.Png;
            }
        }
    }
}

