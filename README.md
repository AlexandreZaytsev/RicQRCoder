# RicQRCoder
Консольное приложение - создание файла с картинкой QR кода использует библиотеки:
 - генератор картинки QR кода QRCoder 1.4.1 (https://github.com/codebude/QRCoder)
 - парсер командной строки CommandLineParser 2.8.0 (https://github.com/commandlineparser/commandline)

_Windows/собирал под  .NET Framework 4.0_  
_Использование и параметры (RicQRCoder --help)_
***
RicQRCoder 1.0.0  
Copyright (c) 2021 cad.ru  
USAGE:  
Creates a QR image file from your content (string or file):  
  RicQRCoder.exe --content "your content" --outFile "your FileName QRImageFile"

-  -i, --content     Required. String or full File name with your content.
-  -o, --outFile     Required. Output file. Full file name without extension (extension from outFormat parameter).
-  --outFormat       (Default: Png) Image format for outputfile. Valid values: Png, Jpg, Gif, Bmp, Tiff, Svg, Xaml, Ps, Eps
-  --eccLevel        (Default: M) Error correction level: L-7%, M-15%, Q-25%, H-30%. Valid values: L, M, Q, H
-  --pixelSize       (Default: 20) The pixel size each b/w module is drawn (from 1 and more).
-  --background      (Default: #000000) Background color.
-  --foreground      (Default: #FFFFFF) Foreground color.
-  -l, --iconPath    Bitmap image from file (full file name with extension).
-  --iconSize        (Default: 15) Sets how much 1-99% of the QR Code will be covered by the icon.
-  --help            Display this help screen.
-  --version         Display version information.

Good luck...
***
использую для документов MSOffice выпускаемых из CRM - создать файл на диске - вставить в документ в рамку Shape найденную по имени  
например:  
```
    For each Shape in docWord.Shapes ' цикл по всем Shapes документа   
      If ... Then    
        tmp = FSO.FindFile(vPathTemp & "\", Shape.Title, "png") 'попробовать найти файйл QR на диске
        if tmp<>"" then
          Shape.Fill.UserPicture vPathTemp & "\" & tmp  ' если есть вставить ссылку в Shape
       Else
          Shape.Fill.Visible = False    'если нет - загасить Shape 
       End if
     End if
     Shape.Line.Visible = False ' удалить рамку Shape в принципе (типа был обработан)
     Next
```
