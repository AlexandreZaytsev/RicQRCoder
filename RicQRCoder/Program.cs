﻿using CommandLine;
using CommandLine.Text;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Linq;

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
                h.Heading = "RicQRCoder 1.0.0"; //change header
                h.Copyright = "Copyright (c) 2021 cad.ru"; //change copyrigt text
                h.AddPreOptionsLine("");
                h.AddPostOptionsText("Good luck...");
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }
              

        //3)	//In sucess: the main logic to handle the options
        static int RunOptionsAndReturnExitCode(Options opts)
        {
            Bitmap ImgBitmap = null;
            var exitCode = 0;
            string appPath = AppDomain.CurrentDomain.BaseDirectory; //string yourpath = Environment.CurrentDirectory (нет закрывающего слеша)
//            Console.WriteLine("props= {0}", string.Join(",", props));

            //загрузка контента
            if (opts.Content != null)
            {
                if (File.Exists(GetResorePath(opts.Content,"")))
                {
                    opts.Content = GetTextFromFile(new FileInfo(GetResorePath(opts.Content, "")));         //сначала из файла
                }
                else
                {
                    opts.Content = opts.Content;                                        //потом из строкового параметра                                         
                }
            }
            else
            {
                opts.Content = "Content not defined";
            }

            //файл QR
            if (opts.OutputFileName != null)
            {
                opts.OutputFileName = GetResorePath(opts.OutputFileName, opts.ImageFormat.ToString().ToLower());
            }
            else
            {
                opts.OutputFileName = appPath + "QRImageFile." + opts.ImageFormat.ToString().ToLower();
            }

            //файл иконки
            if (opts.ImgFileName != null)
            {
                opts.ImgFileName = GetResorePath(opts.ImgFileName, ""); 
                if (File.Exists(opts.ImgFileName))
                {
                    ImgBitmap = (Bitmap)Bitmap.FromFile(opts.ImgFileName);
                }
                else 
                {
                    Console.WriteLine($"{appPath}: {opts.ImgFileName}: No such file or directory");
                }
            }

            //проверка количества пикселей
            if ((opts.PixelsPerModule < 1) || (opts.PixelsPerModule >100)) { opts.PixelsPerModule = 20; }

            GenerateQRCode(opts.Content, opts.EccLevel, opts.OutputFileName, opts.ImageFormat, opts.PixelsPerModule, opts.ForegroundColor, opts.BackgroundColor, ImgBitmap, opts.ImgSize);

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

        private static string GetTextFromFile(FileInfo fileInfo)
        {
            var buffer = new byte[fileInfo.Length];

            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                fileStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }

        //восстановить имя файла из параметров
        private static string GetResorePath(string filePath, string fileExt)
        {
            //добавим каталог приложения
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + filePath;
            }
            //добавим расширение если нужно
            if ((Path.GetExtension(filePath) == "") && (fileExt !=""))
            {
                filePath = filePath + "." + fileExt;
            }
            return filePath;
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
