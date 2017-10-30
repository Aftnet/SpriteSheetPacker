#region MIT License

/*
 * Copyright (c) 2009-2010 Nick Gravelyn (nick@gravelyn.com), Markus Ewald (cygon@nuclex.org)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 * 
 */

#endregion

using Microsoft.Extensions.CommandLineUtils;
using SpriteSheetPacker.Core;
using System;
using System.IO;
using System.Linq;

namespace SpriteSheetPacker.Cmdline
{
    public class Program
    {
        private const int DefaultSize = 4096;

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            var folderOption = app.Option("-f | --folder", "Specifies a folder to look for images to pack in", CommandOptionType.SingleValue);
            var outputOption = app.Option("-o | --output", "Specifies the output image's file name", CommandOptionType.SingleValue);
            var mapOption = app.Option("-m | --map", "Specifies the map's file name", CommandOptionType.SingleValue);
            var powTwoOption = app.Option("-p | --pow2", "Forces that the output to have power of two dimensions", CommandOptionType.NoValue);
            var squareOption = app.Option("-s | --square", "Forces that the output to be have equal width and length", CommandOptionType.NoValue);
            var maxWidthOption = app.Option("-w | --maxwidth", "Specifies the maximum allowed output width", CommandOptionType.SingleValue);
            var maxHeightOption = app.Option("-h | --maxwidth", "Specifies the maximum allowed output height", CommandOptionType.SingleValue);

            app.HelpOption("-? | -h | --help");
            app.OnExecute(() =>
            {
                if (!folderOption.HasValue() || outputOption.HasValue())
                {
                    Console.WriteLine("An input folder and an output filename are required");
                    return 1;
                }

                var inputDir = new DirectoryInfo(folderOption.Value());
                var inputFiles = inputDir.GetFiles().Where(d => ImagePacker.SupportedImageExtensions.Contains(d.Extension)).ToArray();
                if (!inputFiles.Any())
                {
                    Console.WriteLine("No supported files found");
                    return 1;
                }

                var outFile = new FileInfo(outputOption.Value());
                var outEncoder = ImagePacker.GetEncoderFromExtension(outFile.Extension);
                if (outEncoder == null)
                {
                    Console.WriteLine("Unsupported output file format");
                    return 1;
                }

                var outMap = mapOption.HasValue() ? new FileInfo(mapOption.Value()) : null;

                var valueParsed = int.TryParse(maxWidthOption.Value(), out var maxWidth);
                if (!valueParsed) maxWidth = DefaultSize;
                valueParsed = int.TryParse(maxHeightOption.Value(), out var maxHeight);
                if (!valueParsed) maxHeight = DefaultSize;

                var packer = new ImagePacker();
                packer.PackImage(inputFiles, powTwoOption.HasValue(), squareOption.HasValue(), maxWidth, maxHeight, 0, out var packedImage, out var packedMap);

                using (var outStream = outFile.Open(FileMode.Create))
                {
                    packedImage.Save(outStream, outEncoder);
                }

                return 0;
            });

            return app.Execute(args);
        }
    }
}
