Imports System
Imports System.Collections.Generic
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports FileFromPathIterator = org.datavec.api.util.files.FileFromPathIterator
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NDArrayRecordBatch = org.datavec.api.writable.batch.NDArrayRecordBatch
Imports Image = org.datavec.image.data.Image
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports BaseImageRecordReader = org.datavec.image.recordreader.BaseImageRecordReader
Imports ImageUtils = org.datavec.image.util.ImageUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports Record = org.datavec.api.records.Record
Imports RecordMetaDataImageURI = org.datavec.api.records.metadata.RecordMetaDataImageURI
Imports URIUtil = org.datavec.api.util.files.URIUtil
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports ImageTransform = org.datavec.image.transform.ImageTransform
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.image.recordreader.objdetect


	<Serializable>
	Public Class ObjectDetectionRecordReader
		Inherits BaseImageRecordReader

		Private ReadOnly gridW As Integer
		Private ReadOnly gridH As Integer
		Private ReadOnly labelProvider As ImageObjectLabelProvider
		Private ReadOnly nchw As Boolean

		Protected Friend currentImage As Image

		''' <summary>
		''' As per <seealso cref="ObjectDetectionRecordReader(Integer, Integer, Integer, Integer, Integer, Boolean, ImageObjectLabelProvider)"/> but hardcoded
		''' to NCHW format
		''' </summary>
		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal gridH As Integer, ByVal gridW As Integer, ByVal labelProvider As ImageObjectLabelProvider)
			Me.New(height, width, channels, gridH, gridW, True, labelProvider)
		End Sub

		''' <summary>
		''' Create ObjectDetectionRecordReader with
		''' </summary>
		''' <param name="height">        Height of the output images </param>
		''' <param name="width">         Width of the output images </param>
		''' <param name="channels">      Number of channels for the output images </param>
		''' <param name="gridH">         Grid/quantization size (along  height dimension) - Y axis </param>
		''' <param name="gridW">         Grid/quantization size (along  height dimension) - X axis </param>
		''' <param name="nchw">          If true: return NCHW format labels with array shape [minibatch, 4+C, h, w]; if false, return
		'''                      NHWC format labels with array shape [minibatch, h, w, 4+C] </param>
		''' <param name="labelProvider"> ImageObjectLabelProvider - used to look up which objects are in each image </param>
		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal gridH As Integer, ByVal gridW As Integer, ByVal nchw As Boolean, ByVal labelProvider As ImageObjectLabelProvider)
			MyBase.New(height, width, channels, Nothing, Nothing)
			Me.gridW = gridW
			Me.gridH = gridH
			Me.nchw = nchw
			Me.labelProvider = labelProvider
			Me.appendLabel = labelProvider IsNot Nothing
		End Sub

		''' <summary>
		''' As per <seealso cref="ObjectDetectionRecordReader(Integer, Integer, Integer, Integer, Integer, Boolean, ImageObjectLabelProvider, ImageTransform)"/>
		''' but hardcoded to NCHW format
		''' </summary>
		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal gridH As Integer, ByVal gridW As Integer, ByVal labelProvider As ImageObjectLabelProvider, ByVal imageTransform As ImageTransform)
			Me.New(height, width, channels, gridH, gridW, True, labelProvider, imageTransform)
		End Sub

		''' <summary>
		''' When imageTransform != null, object is removed if new center is outside of transformed image bounds.
		''' </summary>
		''' <param name="height">         Height of the output images </param>
		''' <param name="width">          Width of the output images </param>
		''' <param name="channels">       Number of channels for the output images </param>
		''' <param name="gridH">          Grid/quantization size (along  height dimension) - Y axis </param>
		''' <param name="gridW">          Grid/quantization size (along  height dimension) - X axis </param>
		''' <param name="labelProvider">  ImageObjectLabelProvider - used to look up which objects are in each image </param>
		''' <param name="nchw">           If true: return NCHW format labels with array shape [minibatch, 4+C, h, w]; if false, return
		'''                       NHWC format labels with array shape [minibatch, h, w, 4+C] </param>
		''' <param name="imageTransform"> ImageTransform - used to transform image and coordinates </param>
		Public Sub New(ByVal height As Integer, ByVal width As Integer, ByVal channels As Integer, ByVal gridH As Integer, ByVal gridW As Integer, ByVal nchw As Boolean, ByVal labelProvider As ImageObjectLabelProvider, ByVal imageTransform As ImageTransform)
			MyBase.New(height, width, channels, Nothing, Nothing)
			Me.gridW = gridW
			Me.gridH = gridH
			Me.nchw = nchw
			Me.labelProvider = labelProvider
			Me.appendLabel = labelProvider IsNot Nothing
			Me.imageTransform = imageTransform
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			Return [next](1)(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws java.io.IOException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			If imageLoader Is Nothing Then
				imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If
			inputSplit = split
			Dim locations() As URI = split.locations()
			Dim labelSet As ISet(Of String) = New HashSet(Of String)()
			If locations IsNot Nothing AndAlso locations.Length >= 1 Then
				For Each location As URI In locations
					Dim imageObjects As IList(Of ImageObject) = labelProvider.getImageObjectsForPath(location)
					For Each io As ImageObject In imageObjects
						Dim name As String = io.getLabel()
						If Not labelSet.Contains(name) Then
							labelSet.Add(name)
						End If
					Next io
				Next location
				iter = New FileFromPathIterator(inputSplit.locationsPathIterator()) 'This handles randomization internally if necessary
			Else
				Throw New System.ArgumentException("No path locations found in the split.")
			End If

			If TypeOf split Is org.datavec.api.Split.FileSplit Then
				'remove the root directory
				Dim split1 As org.datavec.api.Split.FileSplit = DirectCast(split, org.datavec.api.Split.FileSplit)
				labels_Conflict.Remove(split1.RootDir)
			End If

			'To ensure consistent order for label assignment (irrespective of file iteration order), we want to sort the list of labels
			labels_Conflict = New List(Of String)(labelSet)
			labels_Conflict.Sort()
		End Sub

		Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Dim files As IList(Of File) = New List(Of File)(num)
			Dim objects As IList(Of IList(Of ImageObject)) = New List(Of IList(Of ImageObject))(num)

			Dim i As Integer = 0
			Do While i < num AndAlso hasNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim f As File = iter.next()
				Me.currentFile_Conflict = f
				If Not f.isDirectory() Then
					files.Add(f)
					objects.Add(labelProvider.getImageObjectsForPath(f.getPath()))
				End If
				i += 1
			Loop

			Dim nClasses As Integer = labels_Conflict.Count

			Dim outImg As INDArray = Nd4j.create(files.Count, channels, height, width)
			Dim outLabel As INDArray = Nd4j.create(files.Count, 4 + nClasses, gridH, gridW)

			Dim exampleNum As Integer = 0
			For i As Integer = 0 To files.Count - 1
				Dim imageFile As File = files(i)
				Me.currentFile_Conflict = imageFile
				Try
					Me.invokeListeners(imageFile)
					Dim image As Image = Me.imageLoader.asImageMatrix(imageFile)
					Me.currentImage = image
					Nd4j.AffinityManager.ensureLocation(image.getImage(), AffinityManager.Location.DEVICE)

					outImg.put(New INDArrayIndex(){point(exampleNum), all(), all(), all()}, image.getImage())

					Dim objectsThisImg As IList(Of ImageObject) = objects(exampleNum)

					label(image, objectsThisImg, outLabel, exampleNum)
				Catch e As IOException
					Throw New Exception(e)
				End Try

				exampleNum += 1
			Next i

			If Not nchw Then
				outImg = outImg.permute(0, 2, 3, 1) 'NCHW to NHWC
				outLabel = outLabel.permute(0, 2, 3, 1)
			End If
			Return New List(Of IList(Of Writable)) From {outImg, outLabel}
		End Function

		Private Sub label(ByVal image As Image, ByVal objectsThisImg As IList(Of ImageObject), ByVal outLabel As INDArray, ByVal exampleNum As Integer)
			Dim oW As Integer = image.getOrigW()
			Dim oH As Integer = image.getOrigH()

			Dim W As Integer = oW
			Dim H As Integer = oH

			'put the label data into the output label array
			For Each io As ImageObject In objectsThisImg
				Dim cx As Double = io.XCenterPixels
				Dim cy As Double = io.YCenterPixels
				If imageTransform IsNot Nothing Then
					W = imageTransform.CurrentImage.Width
					H = imageTransform.CurrentImage.Height

					Dim pts() As Single = imageTransform.query(io.getX1(), io.getY1(), io.getX2(), io.getY2())

					Dim minX As Integer = CInt(Math.Round(Math.Min(pts(0), pts(2)), MidpointRounding.AwayFromZero))
					Dim maxX As Integer = CInt(Math.Round(Math.Max(pts(0), pts(2)), MidpointRounding.AwayFromZero))
					Dim minY As Integer = CInt(Math.Round(Math.Min(pts(1), pts(3)), MidpointRounding.AwayFromZero))
					Dim maxY As Integer = CInt(Math.Round(Math.Max(pts(1), pts(3)), MidpointRounding.AwayFromZero))

					io = New ImageObject(minX, minY, maxX, maxY, io.getLabel())
					cx = io.XCenterPixels
					cy = io.YCenterPixels

					If cx < 0 OrElse cx >= W OrElse cy < 0 OrElse cy >= H Then
						Continue For
					End If
				End If

				Dim cxyPostScaling() As Double = ImageUtils.translateCoordsScaleImage(cx, cy, W, H, width, height)
				Dim tlPost() As Double = ImageUtils.translateCoordsScaleImage(io.getX1(), io.getY1(), W, H, width, height)
				Dim brPost() As Double = ImageUtils.translateCoordsScaleImage(io.getX2(), io.getY2(), W, H, width, height)

				'Get grid position for image
				Dim imgGridX As Integer = CInt(Math.Truncate(cxyPostScaling(0) / width * gridW))
				Dim imgGridY As Integer = CInt(Math.Truncate(cxyPostScaling(1) / height * gridH))

				'Convert pixels to grid position, for TL and BR X/Y
				tlPost(0) = tlPost(0) / width * gridW
				tlPost(1) = tlPost(1) / height * gridH
				brPost(0) = brPost(0) / width * gridW
				brPost(1) = brPost(1) / height * gridH

				'Put TL, BR into label array:
				Preconditions.checkState(imgGridY >= 0 AndAlso imgGridY < outLabel.size(2), "Invalid image center in Y axis: " & "calculated grid location of %s, must be between 0 (inclusive) and %s (exclusive). Object label center is outside " & "of image bounds. Image object: %s", imgGridY, outLabel.size(2), io)
				Preconditions.checkState(imgGridX >= 0 AndAlso imgGridX < outLabel.size(3), "Invalid image center in X axis: " & "calculated grid location of %s, must be between 0 (inclusive) and %s (exclusive). Object label center is outside " & "of image bounds. Image object: %s", imgGridY, outLabel.size(2), io)

				outLabel.putScalar(exampleNum, 0, imgGridY, imgGridX, tlPost(0))
				outLabel.putScalar(exampleNum, 1, imgGridY, imgGridX, tlPost(1))
				outLabel.putScalar(exampleNum, 2, imgGridY, imgGridX, brPost(0))
				outLabel.putScalar(exampleNum, 3, imgGridY, imgGridX, brPost(1))

				'Put label class into label array: (one-hot representation)
				Dim labelIdx As Integer = labels_Conflict.IndexOf(io.getLabel())
				outLabel.putScalar(exampleNum, 4 + labelIdx, imgGridY, imgGridX, 1.0)
			Next io
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			invokeListeners(uri)
			If imageLoader Is Nothing Then
				imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If
			Dim image As Image = Me.imageLoader.asImageMatrix(dataInputStream)
			If Not nchw Then
				image.setImage(image.getImage().permute(0,2,3,1))
			End If
			Nd4j.AffinityManager.ensureLocation(image.getImage(), AffinityManager.Location.DEVICE)

			Dim ret As IList(Of Writable) = RecordConverter.toRecord(image.getImage())
			If appendLabel Then
				Dim imageObjectsForPath As IList(Of ImageObject) = labelProvider.getImageObjectsForPath(uri.getPath())
				Dim nClasses As Integer = labels_Conflict.Count
				Dim outLabel As INDArray = Nd4j.create(1, 4 + nClasses, gridH, gridW)
				label(image, imageObjectsForPath, outLabel, 0)
				If Not nchw Then
					outLabel = outLabel.permute(0,2,3,1) 'NCHW to NHWC
				End If
				ret.Add(New NDArrayWritable(outLabel))
			End If
			Return ret
		End Function

		Public Overrides Function nextRecord() As Record
			Dim list As IList(Of Writable) = [next]()
			Dim uri As URI = URIUtil.fileToURI(currentFile_Conflict)
			Return New org.datavec.api.records.impl.Record(list, New RecordMetaDataImageURI(uri, GetType(BaseImageRecordReader), currentImage.getOrigC(), currentImage.getOrigH(), currentImage.getOrigW()))
		End Function
	End Class

End Namespace