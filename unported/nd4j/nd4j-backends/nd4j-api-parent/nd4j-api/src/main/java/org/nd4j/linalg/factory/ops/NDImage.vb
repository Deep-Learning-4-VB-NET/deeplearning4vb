import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ImageResizeMethod = org.nd4j.enums.ImageResizeMethod
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.linalg.factory.ops

	Public Class NDImage
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Given an input image and some crop boxes, extract out the image subsets and resize them to the specified size.<br>
	  ''' </summary>
	  ''' <param name="image"> Input image, with shape [batch, height, width, channels] (NUMERIC type) </param>
	  ''' <param name="cropBoxes"> Float32 crop, shape [numBoxes, 4] with values in range 0 to 1 (NUMERIC type) </param>
	  ''' <param name="boxIndices"> Indices: which image (index to dimension 0) the cropBoxes belong to. Rank 1, shape [numBoxes] (NUMERIC type) </param>
	  ''' <param name="cropOutSize"> Output size for the images - int32, rank 1 with values [outHeight, outWidth] (INT type) </param>
	  ''' <param name="extrapolationValue"> Used for extrapolation, when applicable. 0.0 should be used for the default </param>
	  ''' <returns> output Cropped and resized images (NUMERIC type) </returns>
	  Public Overridable Function cropAndResize(ByVal image As INDArray, ByVal cropBoxes As INDArray, ByVal boxIndices As INDArray, ByVal cropOutSize As INDArray, ByVal extrapolationValue As Double) As INDArray
		NDValidation.validateNumerical("CropAndResize", "image", image)
		NDValidation.validateNumerical("CropAndResize", "cropBoxes", cropBoxes)
		NDValidation.validateNumerical("CropAndResize", "boxIndices", boxIndices)
		NDValidation.validateInteger("CropAndResize", "cropOutSize", cropOutSize)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.CropAndResize(image, cropBoxes, boxIndices, cropOutSize, extrapolationValue))(0)
	  End Function

	  ''' <summary>
	  ''' Given an input image and some crop boxes, extract out the image subsets and resize them to the specified size.<br>
	  ''' </summary>
	  ''' <param name="image"> Input image, with shape [batch, height, width, channels] (NUMERIC type) </param>
	  ''' <param name="cropBoxes"> Float32 crop, shape [numBoxes, 4] with values in range 0 to 1 (NUMERIC type) </param>
	  ''' <param name="boxIndices"> Indices: which image (index to dimension 0) the cropBoxes belong to. Rank 1, shape [numBoxes] (NUMERIC type) </param>
	  ''' <param name="cropOutSize"> Output size for the images - int32, rank 1 with values [outHeight, outWidth] (INT type) </param>
	  ''' <returns> output Cropped and resized images (NUMERIC type) </returns>
	  Public Overridable Function cropAndResize(ByVal image As INDArray, ByVal cropBoxes As INDArray, ByVal boxIndices As INDArray, ByVal cropOutSize As INDArray) As INDArray
		NDValidation.validateNumerical("CropAndResize", "image", image)
		NDValidation.validateNumerical("CropAndResize", "cropBoxes", cropBoxes)
		NDValidation.validateNumerical("CropAndResize", "boxIndices", boxIndices)
		NDValidation.validateInteger("CropAndResize", "cropOutSize", cropOutSize)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.CropAndResize(image, cropBoxes, boxIndices, cropOutSize, 0.0))(0)
	  End Function

	  ''' <summary>
	  ''' Adjusts contrast of RGB or grayscale images.<br>
	  ''' </summary>
	  ''' <param name="in"> images to adjust. 3D shape or higher (NUMERIC type) </param>
	  ''' <param name="factor"> multiplier for adjusting contrast </param>
	  ''' <returns> output Contrast-adjusted image (NUMERIC type) </returns>
	  Public Overridable Function adjustContrast(ByVal [in] As INDArray, ByVal factor As Double) As INDArray
		NDValidation.validateNumerical("adjustContrast", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.AdjustContrast([in], factor))(0)
	  End Function

	  ''' <summary>
	  ''' Adjust hue of RGB image <br>
	  ''' </summary>
	  ''' <param name="in"> image as 3D array (NUMERIC type) </param>
	  ''' <param name="delta"> value to add to hue channel </param>
	  ''' <returns> output adjusted image (NUMERIC type) </returns>
	  Public Overridable Function adjustHue(ByVal [in] As INDArray, ByVal delta As Double) As INDArray
		NDValidation.validateNumerical("adjustHue", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.AdjustHue([in], delta))(0)
	  End Function

	  ''' <summary>
	  ''' Adjust saturation of RGB images<br>
	  ''' </summary>
	  ''' <param name="in"> RGB image as 3D array (NUMERIC type) </param>
	  ''' <param name="factor"> factor for saturation </param>
	  ''' <returns> output adjusted image (NUMERIC type) </returns>
	  Public Overridable Function adjustSaturation(ByVal [in] As INDArray, ByVal factor As Double) As INDArray
		NDValidation.validateNumerical("adjustSaturation", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.AdjustSaturation([in], factor))(0)
	  End Function

	  ''' <summary>
	  ''' Given an input image, extract out image patches (of size kSizes - h x w) and place them in the depth dimension. <br>
	  ''' </summary>
	  ''' <param name="image"> Input image to extract image patches from - shape [batch, height, width, channels] (NUMERIC type) </param>
	  ''' <param name="kSizes"> Kernel size - size of the image patches, [height, width] (Size: Exactly(count=2)) </param>
	  ''' <param name="strides"> Stride in the input dimension for extracting image patches, [stride_height, stride_width] (Size: Exactly(count=2)) </param>
	  ''' <param name="rates"> Usually [1,1]. Equivalent to dilation rate in dilated convolutions - how far apart the output pixels
	  '''                  in the patches should be, in the input. A dilation of [a,b] means every {@code a}th pixel is taken
	  '''                  along the height/rows dimension, and every {@code b}th pixel is take along the width/columns dimension (Size: AtLeast(min=0)) </param>
	  ''' <param name="sameMode"> Padding algorithm. If true: use Same padding </param>
	  ''' <returns> output The extracted image patches (NUMERIC type) </returns>
	  Public Overridable Function extractImagePatches(ByVal image As INDArray, ByVal kSizes() As Integer, ByVal strides() As Integer, ByVal rates() As Integer, ByVal sameMode As Boolean) As INDArray
		NDValidation.validateNumerical("extractImagePatches", "image", image)
		Preconditions.checkArgument(kSizes.Length = 2, "kSizes has incorrect size/length. Expected: kSizes.length == 2, got %s", kSizes.Length)
		Preconditions.checkArgument(strides.Length = 2, "strides has incorrect size/length. Expected: strides.length == 2, got %s", strides.Length)
		Preconditions.checkArgument(rates.Length >= 0, "rates has incorrect size/length. Expected: rates.length >= 0, got %s", rates.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.ExtractImagePatches(image, kSizes, strides, rates, sameMode))(0)
	  End Function

	  ''' <summary>
	  ''' Converting image from HSV to RGB format <br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function hsvToRgb(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("hsvToRgb", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.HsvToRgb(input))(0)
	  End Function

	  ''' <summary>
	  ''' Resize images to size using the specified method.<br>
	  ''' </summary>
	  ''' <param name="input"> 4D image [NHWC] (NUMERIC type) </param>
	  ''' <param name="size"> new height and width (INT type) </param>
	  ''' <param name="preserveAspectRatio"> Whether to preserve the aspect ratio. If this is set, then images will be resized to a size that fits in size while preserving the aspect ratio of the original image. Scales up the image if size is bigger than the current size of the image. Defaults to False. </param>
	  ''' <param name="antialis"> Whether to use an anti-aliasing filter when downsampling an image </param>
	  ''' <param name="ImageResizeMethod"> ResizeBilinear: Bilinear interpolation. If 'antialias' is true, becomes a hat/tent filter function with radius 1 when downsampling.
	  ''' ResizeLanczos5: Lanczos kernel with radius 5. Very-high-quality filter but may have stronger ringing.
	  ''' ResizeBicubic: Cubic interpolant of Keys. Equivalent to Catmull-Rom kernel. Reasonably good quality and faster than Lanczos3Kernel, particularly when upsampling.
	  ''' ResizeGaussian: Gaussian kernel with radius 3, sigma = 1.5 / 3.0.
	  ''' ResizeNearest: Nearest neighbor interpolation. 'antialias' has no effect when used with nearest neighbor interpolation.
	  ''' ResizeArea: Anti-aliased resampling with area interpolation. 'antialias' has no effect when used with area interpolation; it always anti-aliases.
	  ''' ResizeMitchelcubic: Mitchell-Netravali Cubic non-interpolating filter. For synthetic images (especially those lacking proper prefiltering), less ringing than Keys cubic kernel but less sharp. </param>
	  ''' <returns> output Output image (NUMERIC type) </returns>
	  Public Overridable Function imageResize(ByVal input As INDArray, ByVal size As INDArray, ByVal preserveAspectRatio As Boolean, ByVal antialis As Boolean, ByVal ImageResizeMethod As ImageResizeMethod) As INDArray
		NDValidation.validateNumerical("imageResize", "input", input)
		NDValidation.validateInteger("imageResize", "size", size)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.ImageResize(input, size, preserveAspectRatio, antialis, ImageResizeMethod))(0)
	  End Function

	  ''' <summary>
	  ''' Resize images to size using the specified method.<br>
	  ''' </summary>
	  ''' <param name="input"> 4D image [NHWC] (NUMERIC type) </param>
	  ''' <param name="size"> new height and width (INT type) </param>
	  ''' <param name="ImageResizeMethod"> ResizeBilinear: Bilinear interpolation. If 'antialias' is true, becomes a hat/tent filter function with radius 1 when downsampling.
	  ''' ResizeLanczos5: Lanczos kernel with radius 5. Very-high-quality filter but may have stronger ringing.
	  ''' ResizeBicubic: Cubic interpolant of Keys. Equivalent to Catmull-Rom kernel. Reasonably good quality and faster than Lanczos3Kernel, particularly when upsampling.
	  ''' ResizeGaussian: Gaussian kernel with radius 3, sigma = 1.5 / 3.0.
	  ''' ResizeNearest: Nearest neighbor interpolation. 'antialias' has no effect when used with nearest neighbor interpolation.
	  ''' ResizeArea: Anti-aliased resampling with area interpolation. 'antialias' has no effect when used with area interpolation; it always anti-aliases.
	  ''' ResizeMitchelcubic: Mitchell-Netravali Cubic non-interpolating filter. For synthetic images (especially those lacking proper prefiltering), less ringing than Keys cubic kernel but less sharp. </param>
	  ''' <returns> output Output image (NUMERIC type) </returns>
	  Public Overridable Function imageResize(ByVal input As INDArray, ByVal size As INDArray, ByVal ImageResizeMethod As ImageResizeMethod) As INDArray
		NDValidation.validateNumerical("imageResize", "input", input)
		NDValidation.validateInteger("imageResize", "size", size)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.ImageResize(input, size, False, False, ImageResizeMethod))(0)
	  End Function

	  ''' <summary>
	  ''' Greedily selects a subset of bounding boxes in descending order of score<br>
	  ''' </summary>
	  ''' <param name="boxes"> Might be null. Name for the output variable (NUMERIC type) </param>
	  ''' <param name="scores"> vector of shape [num_boxes] (NUMERIC type) </param>
	  ''' <param name="maxOutSize"> scalar representing the maximum number of boxes to be selected </param>
	  ''' <param name="iouThreshold"> threshold for deciding whether boxes overlap too much with respect to IOU </param>
	  ''' <param name="scoreThreshold"> threshold for deciding when to remove boxes based on score </param>
	  ''' <returns> output vectort of shape [M] representing the selected indices from the boxes tensor, where M <= max_output_size (NUMERIC type) </returns>
	  Public Overridable Function nonMaxSuppression(ByVal boxes As INDArray, ByVal scores As INDArray, ByVal maxOutSize As Integer, ByVal iouThreshold As Double, ByVal scoreThreshold As Double) As INDArray
		NDValidation.validateNumerical("nonMaxSuppression", "boxes", boxes)
		NDValidation.validateNumerical("nonMaxSuppression", "scores", scores)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.image.NonMaxSuppression(boxes, scores, maxOutSize, iouThreshold, scoreThreshold))(0)
	  End Function

	  ''' <summary>
	  ''' Randomly crops image<br>
	  ''' </summary>
	  ''' <param name="input"> input array (NUMERIC type) </param>
	  ''' <param name="shape"> shape for crop (INT type) </param>
	  ''' <returns> output cropped array (NUMERIC type) </returns>
	  Public Overridable Function randomCrop(ByVal input As INDArray, ByVal shape As INDArray) As INDArray
		NDValidation.validateNumerical("randomCrop", "input", input)
		NDValidation.validateInteger("randomCrop", "shape", shape)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.RandomCrop(input, shape))(0)
	  End Function

	  ''' <summary>
	  ''' Converting array from HSV to RGB format<br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function rgbToHsv(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("rgbToHsv", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.RgbToHsv(input))(0)
	  End Function

	  ''' <summary>
	  ''' Converting array from RGB to YIQ format <br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function rgbToYiq(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("rgbToYiq", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.RgbToYiq(input))(0)
	  End Function

	  ''' <summary>
	  ''' Converting array from RGB to YUV format <br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function rgbToYuv(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("rgbToYuv", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.RgbToYuv(input))(0)
	  End Function

	  ''' <summary>
	  ''' Converting image from YIQ to RGB format <br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function yiqToRgb(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("yiqToRgb", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.YiqToRgb(input))(0)
	  End Function

	  ''' <summary>
	  ''' Converting image from YUV to RGB format <br>
	  ''' </summary>
	  ''' <param name="input"> 3D image (NUMERIC type) </param>
	  ''' <returns> output 3D image (NUMERIC type) </returns>
	  Public Overridable Function yuvToRgb(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("yuvToRgb", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.YuvToRgb(input))(0)
	  End Function
	End Class

End Namespace