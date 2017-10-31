﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;
using GuildId = System.UInt32;

namespace TCC.Parsing.Messages
{
    public class S_IMAGE_DATA : ParsedMessage
    {
        private static Dictionary<GuildId, Bitmap> _database;
        public static Dictionary<GuildId, Bitmap> Database => _database ?? (_database = new Dictionary<GuildId, Bitmap>());


        public S_IMAGE_DATA(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            var imageOffset = reader.ReadUInt16();
            var imageLength = reader.ReadUInt16();

            reader.BaseStream.Position = nameOffset - 4;
            var imageName = reader.ReadTeraString();
            if(!imageName.StartsWith("guildlogo")) return;
            reader.BaseStream.Position = imageOffset - 4;
            var imageBytes = reader.ReadBytes(imageLength);
            var width = BitConverter.ToInt32(imageBytes, 8);
            var type = BitConverter.ToInt32(imageBytes, 12);

            var image = new Bitmap(width, width, PixelFormat.Format8bppIndexed);
            var palette = image.Palette;
            int length;
            switch (type)
            {
                case 0:
                case 1:
                    var paletteSize = BitConverter.ToInt32(imageBytes, 16);
                    if (paletteSize >= imageLength - 24) { Debug.WriteLine("palette size too big"); return; }
                    for (var i = 0; i < paletteSize; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(imageBytes[0x14 + i * 3], imageBytes[0x15 + i * 3], imageBytes[0x16 + i * 3]);
                    }
                    length = BitConverter.ToInt32(imageBytes, paletteSize * 3 + 20);
                    break;
                case 2:
                case 3:
                    for (var i = 0; i < 255; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    length = BitConverter.ToInt32(imageBytes, 16);
                    break;
                default:
                    Debug.WriteLine("Unknown guild logo format");
                    return;
            }
            var pixels = image.LockBits(new Rectangle(0, 0, width, width), ImageLockMode.WriteOnly, image.PixelFormat);
            Marshal.Copy(imageBytes, imageLength - length, pixels.Scan0, length);
            image.UnlockBits(pixels);
            image.Palette = palette;
            var id = Convert.ToUInt32(imageName.Split('_')[2]);
            if (Database.ContainsKey(id)) return;
            Database.Add(id, image);
        }
    }
}