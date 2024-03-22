﻿#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion
using JpegXmpWritePluginMDE.MetadataExtractor;
using XmpCore;
using Xunit;

namespace JpegXmpWritePluginMDE.Tests
{
	/// <summary>Unit tests for <see cref="ImageMetadataWriter"/>.</summary>
	/// <author>Michael Osthege</author>
	public sealed class ImageMetadataWriterTest
	{
		[Fact]
		public void TestWriteImageMetadata()
		{
			var originalStream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithMicrosoftXmp.jpg");
			IXmpMeta xmp = XmpMetaFactory.ParseFromString(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
			byte[] expectedResult = TestDataUtil.GetBytes("Data/xmpWriting_PictureWithMicrosoftXmpReencoded.jpg");
			
			var metadata_objects = new object[] { xmp };
			var updatedStream = ImageMetadataWriter.WriteMetadata(originalStream, metadata_objects);

			var actualResult = updatedStream.ToArray();

			Assert.True(actualResult.SequenceEqual(expectedResult));
		}

		[Fact]
		public void TestWriteImageMetadataToFile()
		{
			// Set up a temporary file to write to 
			string sourceFile = Path.Combine("Data", "xmpWriting_PictureWithMicrosoftXmp.jpg");
			string expectedJpegFile = Path.Combine("Data", "xmpWriting_PictureWithMicrosoftXmpReencoded.jpg");
			string expectedXmpFile = Path.Combine("Data", "xmpWriting_XmpContent.xmp");
			string tempFile = Path.Combine("Data", $"{Guid.NewGuid()}");
			File.Copy(sourceFile, tempFile);
            
			// Test the write with known data
			IXmpMeta xmp = XmpMetaFactory.ParseFromString(File.ReadAllText(expectedXmpFile));
			var metadata_objects = new object[] { xmp };
			ImageMetadataWriter.WriteMetadata(tempFile, metadata_objects);

			byte[] expectedResult = TestDataUtil.GetBytes(expectedJpegFile);
			byte[] actualResult = TestDataUtil.GetBytes(tempFile);
			File.Delete(tempFile);

			// Check results are as expected
			Assert.Equal(expectedResult, actualResult);
		}
    }
}
