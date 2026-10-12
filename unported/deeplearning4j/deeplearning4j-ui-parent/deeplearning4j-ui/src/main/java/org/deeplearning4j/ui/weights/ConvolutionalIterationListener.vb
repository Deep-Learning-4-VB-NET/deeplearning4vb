Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ImageLoader = org.datavec.image.loader.ImageLoader
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports UiConnectionInfo = org.deeplearning4j.core.ui.UiConnectionInfo
Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports ConvolutionListenerPersistable = org.deeplearning4j.ui.model.weights.ConvolutionListenerPersistable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.ui.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ConvolutionalIterationListener extends org.deeplearning4j.optimize.api.BaseTrainingListener
	Public Class ConvolutionalIterationListener
		Inherits BaseTrainingListener

		Private Enum Orientation
			LANDSCAPE
			PORTRAIT
		End Enum

		Private freq As Integer = 10
		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ConvolutionalIterationListener))
		Private minibatchNum As Integer = 0
		Private openBrowser As Boolean = True
		Private path As String
		Private firstIteration As Boolean = True

		Private borderColor As New Color(140, 140, 140)
		Private bgColor As New Color(255, 255, 255)

		Private ReadOnly ssr As StatsStorageRouter
		Private ReadOnly sessionID As String
		Private ReadOnly workerID As String


		Public Sub New(ByVal connectionInfo As UiConnectionInfo, ByVal visualizationFrequency As Integer)
			Me.New(New MapDBStatsStorage(), visualizationFrequency, True)
		End Sub

		Public Sub New(ByVal visualizationFrequency As Integer)
			Me.New(visualizationFrequency, True)
		End Sub

		Public Sub New(ByVal iterations As Integer, ByVal openBrowser As Boolean)
			Me.New(New MapDBStatsStorage(), iterations, openBrowser)
		End Sub

		Public Sub New(ByVal ssr As StatsStorageRouter, ByVal iterations As Integer, ByVal openBrowser As Boolean)
			Me.New(ssr, iterations, openBrowser, Nothing, Nothing)
		End Sub

		Public Sub New(ByVal ssr As StatsStorageRouter, ByVal iterations As Integer, ByVal openBrowser As Boolean, ByVal sessionID As String, ByVal workerID As String)
			Me.ssr = ssr
			If sessionID Is Nothing Then
				'TODO handle syncing session IDs across different listeners in the same model...
				Me.sessionID = System.Guid.randomUUID().ToString()
			Else
				Me.sessionID = sessionID
			End If
			If workerID Is Nothing Then
				Me.workerID = UIDProvider.JVMUID & "_" & Thread.CurrentThread.getId()
			Else
				Me.workerID = workerID
			End If

			Dim subPath As String = "activations"

			Me.freq = iterations
			Me.openBrowser = openBrowser
			path = "http://localhost:" & UIServer.getInstance().getPort() & "/" & subPath

			If openBrowser AndAlso TypeOf ssr Is StatsStorage Then
				UIServer.getInstance().attach(DirectCast(ssr, StatsStorage))
			End If

			Console.WriteLine("ConvolutionTrainingListener path: " & path)
		End Sub

		''' <summary>
		''' Event listener for each iteration
		''' </summary>
		''' <param name="model">     the model iterating </param>
		''' <param name="iteration"> the iteration number </param>
		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)

		End Sub

		Public Overrides Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))

			Dim iteration As Integer = (If(TypeOf model Is MultiLayerNetwork, DirectCast(model, MultiLayerNetwork).IterationCount, DirectCast(model, ComputationGraph).IterationCount))
			If iteration Mod freq = 0 Then

				Dim tensors As IList(Of INDArray) = New List(Of INDArray)()
				Dim cnt As Integer = 0
				Dim rnd As New Random()
				Dim sourceImage As BufferedImage = Nothing
				Dim sampleIdx As Integer = -1 'output.shape()[0] == 1 ? 0 : rnd.nextInt((int) output.shape()[0] - 1) + 1;
				If TypeOf model Is ComputationGraph Then
					Dim l As ComputationGraph = DirectCast(model, ComputationGraph)
					Dim layers() As Layer = l.Layers
					If layers.Length <> activations.Count Then
						Throw New Exception("layers.length != activations.size(). Got layers.length=" & layers.Length & ", activations.size()=" & activations.Count)
					End If
					For i As Integer = 0 To layers.Length - 1
						If layers(i).type() = Layer.Type.CONVOLUTIONAL Then
							Dim layerName As String = layers(i).conf().getLayer().getLayerName()
							Dim output As INDArray = activations(layerName) 'Offset by 1 - activations list includes input

							If sampleIdx < 0 Then
								sampleIdx = If(output.shape()(0) = 1, 0, rnd.Next(CInt(output.shape()(0)) - 1) + 1)
							End If

							Dim tad As INDArray = output.tensorAlongDimension(sampleIdx, 3, 2, 1)
							tensors.Add(tad)
							cnt += 1
						End If
					Next i
				Else
					'MultiLayerNetwork: no op (other forward pass method should be called instead)
					Return
				End If

				'Try to work out source image:
				Dim cg As ComputationGraph = DirectCast(model, ComputationGraph)
				Dim arr() As INDArray = cg.Inputs
				If arr.Length > 1 Then
					Throw New System.InvalidOperationException("ConvolutionIterationListener does not support ComputationGraph models with more than 1 input; model has " & arr.Length & " inputs")
				End If

				If arr(0).rank() = 4 Then
					sourceImage = Nothing
					If cnt = 0 Then
						Try
							sourceImage = restoreRGBImage(arr(0).tensorAlongDimension(sampleIdx, 3, 2, 1))
						Catch e As Exception
							Throw New Exception(e)
						End Try
					End If
				End If

				Dim render As BufferedImage = rasterizeConvoLayers(tensors, sourceImage)
				Dim p As Persistable = New ConvolutionListenerPersistable(sessionID, workerID, DateTimeHelper.CurrentUnixTimeMillis(), render)
				ssr.putStaticInfo(p)

				minibatchNum += 1
			End If
		End Sub

		Public Overrides Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))
			Dim iteration As Integer = (If(TypeOf model Is MultiLayerNetwork, DirectCast(model, MultiLayerNetwork).IterationCount, DirectCast(model, ComputationGraph).IterationCount))
			If iteration Mod freq = 0 Then

				Dim tensors As IList(Of INDArray) = New List(Of INDArray)()
				Dim cnt As Integer = 0
				Dim rnd As New Random()
				Dim sourceImage As BufferedImage = Nothing
				If TypeOf model Is MultiLayerNetwork Then
					Dim l As MultiLayerNetwork = DirectCast(model, MultiLayerNetwork)
					Dim layers() As Layer = l.Layers
					If layers.Length <> activations.Count Then
						Throw New Exception()
					End If
					For i As Integer = 0 To layers.Length - 1
						If layers(i).type() = Layer.Type.CONVOLUTIONAL Then
							Dim output As INDArray = activations(i+1) 'Offset by 1 - activations list includes input

							If output.shape()(0) - 1 > Integer.MaxValue Then
								Throw New ND4JArraySizeException()
							End If
							Dim sampleDim As Integer = If(output.shape()(0) = 1, 0, rnd.Next(CInt(output.shape()(0)) - 1) + 1)
							If cnt = 0 Then
								Dim inputs As INDArray = layers(i).input()

								Try
									sourceImage = restoreRGBImage(inputs.tensorAlongDimension(sampleDim, New Integer() {3, 2, 1}))
								Catch e As Exception
									Throw New Exception(e)
								End Try
							End If

							Dim tad As INDArray = output.tensorAlongDimension(sampleDim, 3, 2, 1)

							tensors.Add(tad)

							cnt += 1
						End If
					Next i
				Else
					'Compgraph: no op (other forward pass method should be called instead)
					Return
				End If
				Dim render As BufferedImage = rasterizeConvoLayers(tensors, sourceImage)
				Dim p As Persistable = New ConvolutionListenerPersistable(sessionID, workerID, DateTimeHelper.CurrentUnixTimeMillis(), render)
				ssr.putStaticInfo(p)

				minibatchNum += 1
			End If
		End Sub

		''' <summary>
		''' We visualize set of tensors as vertically aligned set of patches
		''' </summary>
		''' <param name="tensors3D"> list of tensors retrieved from convolution </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private java.awt.image.BufferedImage rasterizeConvoLayers(@NonNull List<org.nd4j.linalg.api.ndarray.INDArray> tensors3D, java.awt.image.BufferedImage sourceImage)
		Private Function rasterizeConvoLayers(ByVal tensors3D As IList(Of INDArray), ByVal sourceImage As BufferedImage) As BufferedImage
			Dim width As Long = 0
			Dim height As Long = 0

			Dim border As Integer = 1
			Dim padding_row As Integer = 2
			Dim padding_col As Integer = 80

	'        
	'            We determine height of joint output image. We assume that first position holds maximum dimensionality
	'         
			Dim shape As val = tensors3D(0).shape()
			Dim numImages As val = shape(0)
			height = (shape(2))
			width = (shape(1))
			'        log.info("Output image dimensions: {height: " + height + ", width: " + width + "}");
			Dim maxHeight As Integer = 0 '(height + (border * 2 ) + padding_row) * numImages;
			Dim totalWidth As Integer = 0
			Dim iOffset As Integer = 1

			Dim orientation As Orientation = Orientation.LANDSCAPE
	'        
	'            for debug purposes we'll use portait only now
	'         
			If tensors3D.Count > 3 Then
				orientation = Orientation.PORTRAIT
			End If



			Dim images As IList(Of BufferedImage) = New List(Of BufferedImage)()
			For layer As Integer = 0 To tensors3D.Count - 1
				Dim tad As INDArray = tensors3D(layer)
				Dim zoomed As Integer = 0

				Dim image As BufferedImage = Nothing
				If orientation = Orientation.LANDSCAPE Then
					maxHeight = CInt(Math.Truncate((height + (border * 2) + padding_row) * numImages))
					image = renderMultipleImagesLandscape(tad, maxHeight, CInt(width), CInt(height))
					totalWidth += image.getWidth() + padding_col
				ElseIf orientation = Orientation.PORTRAIT Then
					totalWidth = CInt(Math.Truncate((width + (border * 2) + padding_row) * numImages))
					image = renderMultipleImagesPortrait(tad, totalWidth, CInt(width), CInt(height))
					maxHeight += image.getHeight() + padding_col
				End If

				images.Add(image)
			Next layer

			If orientation = Orientation.LANDSCAPE Then
				' append some space for arrows
				totalWidth += padding_col * 2
			ElseIf orientation = Orientation.PORTRAIT Then
				maxHeight += padding_col * 2
				maxHeight += sourceImage.getHeight() + (padding_col * 2)
			End If

			Dim output As New BufferedImage(totalWidth, maxHeight, BufferedImage.TYPE_INT_RGB)
			Dim graphics2D As Graphics2D = output.createGraphics()

			graphics2D.setPaint(bgColor)
			graphics2D.fillRect(0, 0, output.getWidth(), output.getHeight())

			Dim singleArrow As BufferedImage = Nothing
			Dim multipleArrows As BufferedImage = Nothing

	'        
	'            We try to add nice flow arrow here
	'         
			Try

				If orientation = Orientation.LANDSCAPE Then
					Try
						Dim resource As New ClassPathResource("arrow_sing.PNG")
						Dim resource2 As New ClassPathResource("arrow_mul.PNG")

						singleArrow = ImageIO.read(resource.InputStream)
						multipleArrows = ImageIO.read(resource2.InputStream)
					Catch e As Exception
					End Try

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					graphics2D.drawImage(sourceImage, (padding_col \ 2) - (sourceImage.getWidth() / 2), (maxHeight \ 2) - (sourceImage.getHeight() / 2), Nothing)

					graphics2D.setPaint(borderColor)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					graphics2D.drawRect((padding_col \ 2) - (sourceImage.getWidth() / 2), (maxHeight \ 2) - (sourceImage.getHeight() / 2), sourceImage.getWidth(), sourceImage.getHeight())

					iOffset += sourceImage.getWidth()

					If singleArrow IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						graphics2D.drawImage(singleArrow, iOffset + (padding_col \ 2) - (singleArrow.getWidth() / 2), (maxHeight \ 2) - (singleArrow.getHeight() / 2), Nothing)
					End If
				Else
					Try
						Dim resource As New ClassPathResource("arrow_singi.PNG")
						Dim resource2 As New ClassPathResource("arrow_muli.PNG")

						singleArrow = ImageIO.read(resource.InputStream)
						multipleArrows = ImageIO.read(resource2.InputStream)
					Catch e As Exception
					End Try

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					graphics2D.drawImage(sourceImage, (totalWidth \ 2) - (sourceImage.getWidth() / 2), (padding_col \ 2) - (sourceImage.getHeight() / 2), Nothing)

					graphics2D.setPaint(borderColor)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					graphics2D.drawRect((totalWidth \ 2) - (sourceImage.getWidth() / 2), (padding_col \ 2) - (sourceImage.getHeight() / 2), sourceImage.getWidth(), sourceImage.getHeight())

					iOffset += sourceImage.getHeight()
					If singleArrow IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						graphics2D.drawImage(singleArrow, (totalWidth \ 2) - (singleArrow.getWidth() / 2), iOffset + (padding_col \ 2) - (singleArrow.getHeight() / 2), Nothing)
					End If

				End If
				iOffset += padding_col
			Catch e As Exception
				' if we can't load images - ignore them
			End Try



	'        
	'            now we merge all images into one big image with some offset
	'        


			For i As Integer = 0 To images.Count - 1
				Dim curImage As BufferedImage = images(i)
				If orientation = Orientation.LANDSCAPE Then
					' image grows from left to right
					graphics2D.drawImage(curImage, iOffset, 1, Nothing)
					iOffset += curImage.getWidth() + padding_col

					If singleArrow IsNot Nothing AndAlso multipleArrows IsNot Nothing Then
						If i < images.Count - 1 Then
							' draw multiple arrows here
							If multipleArrows IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
								graphics2D.drawImage(multipleArrows, iOffset - (padding_col \ 2) - (multipleArrows.getWidth() / 2), (maxHeight \ 2) - (multipleArrows.getHeight() / 2), Nothing)
							End If
						Else
							' draw single arrow
							'    graphics2D.drawImage(singleArrow, iOffset - (padding_col / 2) - (singleArrow.getWidth() / 2), (maxHeight / 2) - (singleArrow.getHeight() / 2), null);
						End If
					End If
				ElseIf orientation = Orientation.PORTRAIT Then
					' image grows from top to bottom
					graphics2D.drawImage(curImage, 1, iOffset, Nothing)
					iOffset += curImage.getHeight() + padding_col

					If singleArrow IsNot Nothing AndAlso multipleArrows IsNot Nothing Then
						If i < images.Count - 1 Then
							' draw multiple arrows here
							If multipleArrows IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
								graphics2D.drawImage(multipleArrows, (totalWidth \ 2) - (multipleArrows.getWidth() / 2), iOffset - (padding_col \ 2) - (multipleArrows.getHeight() / 2), Nothing)
							End If
						Else
							' draw single arrow
							'   graphics2D.drawImage(singleArrow, (totalWidth / 2) - (singleArrow.getWidth() / 2),  iOffset - (padding_col / 2) - (singleArrow.getHeight() / 2) , null);
						End If
					End If
				End If
			Next i

			Return output
		End Function


		Private Function renderMultipleImagesPortrait(ByVal tensor3D As INDArray, ByVal maxWidth As Integer, ByVal zoomWidth As Integer, ByVal zoomHeight As Integer) As BufferedImage
			Dim border As Integer = 1
			Dim padding_row As Integer = 2
			Dim padding_col As Integer = 2
			Dim zoomPadding As Integer = 20

			Dim tShape As val = tensor3D.shape()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim numRows As val = tShape(0) / tShape(2)

			Dim height As val = (numRows * (tShape(1) + border + padding_col)) + padding_col + zoomPadding + zoomWidth

			If height > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim outputImage As New BufferedImage(maxWidth, CInt(height), BufferedImage.TYPE_BYTE_GRAY)
			Dim graphics2D As Graphics2D = outputImage.createGraphics()

			graphics2D.setPaint(bgColor)
			graphics2D.fillRect(0, 0, outputImage.getWidth(), outputImage.getHeight())

			Dim columnOffset As Integer = 0
			Dim rowOffset As Integer = 0
			Dim numZoomed As Integer = 0
			Dim limZoomed As Integer = 5
			Dim zoomSpan As Integer = maxWidth \ limZoomed

			Dim z As Integer = 0
			Do While z < tensor3D.shape()(0)

				Dim tad2D As INDArray = tensor3D.tensorAlongDimension(z, 2, 1)

				Dim rWidth As val = tad2D.shape()(0)
				Dim rHeight As val = tad2D.shape()(1)

				Dim loc_height As val = (rHeight) + (border * 2) + padding_row
				Dim loc_width As val = (rWidth) + (border * 2) + padding_col



				Dim currentImage As BufferedImage = renderImageGrayscale(tad2D)

	'            
	'                if resulting image doesn't fit into image, we should step to next columns
	'             
				If columnOffset + loc_width > maxWidth Then
					rowOffset += loc_height
					columnOffset = 0
				End If

	'            
	'                now we should place this image into output image
	'            

				graphics2D.drawImage(currentImage, columnOffset + 1, rowOffset + 1, Nothing)


	'            
	'                draw borders around each image
	'            

				graphics2D.setPaint(borderColor)
				graphics2D.drawRect(columnOffset, rowOffset, CInt(tad2D.shape()(0)), CInt(tad2D.shape()(1)))



	'            
	'                draw one of 3 zoomed images if we're not on first level
	'            

				If z Mod 7 = 0 AndAlso z <> 0 AndAlso numZoomed < limZoomed AndAlso (rHeight <> zoomHeight AndAlso rWidth <> zoomWidth) Then

					Dim cY As Integer = (zoomSpan * numZoomed) + (zoomHeight)
					Dim cX As Integer = (zoomSpan * numZoomed) + (zoomWidth)

					graphics2D.drawImage(currentImage, cX - 1, CInt(height) - zoomWidth - 1, zoomWidth, zoomHeight, Nothing)
					graphics2D.drawRect(cX - 2, CInt(height) - zoomWidth - 2, zoomWidth, zoomHeight)

					' draw line to connect this zoomed pic with its original
					graphics2D.drawLine(columnOffset + CInt(rWidth), rowOffset + CInt(rHeight), cX - 2, CInt(height) - zoomWidth - 2)
					numZoomed += 1

				End If

				columnOffset += loc_width
				z += 1
			Loop

			Return outputImage
		End Function

		''' <summary>
		''' This method renders 1 convolution layer as set of patches + multiple zoomed images </summary>
		''' <param name="tensor3D">
		''' @return </param>
		Private Function renderMultipleImagesLandscape(ByVal tensor3D As INDArray, ByVal maxHeight As Integer, ByVal zoomWidth As Integer, ByVal zoomHeight As Integer) As BufferedImage
	'        
	'            first we need to determine, weight of output image.
	'         
			Dim border As Integer = 1
			Dim padding_row As Integer = 2
			Dim padding_col As Integer = 2
			Dim zoomPadding As Integer = 20

			Dim tShape As val = tensor3D.shape()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim numColumns As val = tShape(0) / tShape(1)

			Dim width As val = (numColumns * (tShape(1) + border + padding_col)) + padding_col + zoomPadding + zoomWidth

			Dim outputImage As New BufferedImage(CInt(width), maxHeight, BufferedImage.TYPE_BYTE_GRAY)
			Dim graphics2D As Graphics2D = outputImage.createGraphics()

			graphics2D.setPaint(bgColor)
			graphics2D.fillRect(0, 0, outputImage.getWidth(), outputImage.getHeight())

			Dim columnOffset As Integer = 0
			Dim rowOffset As Integer = 0
			Dim numZoomed As Integer = 0
			Dim limZoomed As Integer = 5
			Dim zoomSpan As Integer = maxHeight \ limZoomed
			Dim z As Integer = 0
			Do While z < tensor3D.shape()(0)

				Dim tad2D As INDArray = tensor3D.tensorAlongDimension(z, 2, 1)

				Dim rWidth As val = tad2D.shape()(0)
				Dim rHeight As val = tad2D.shape()(1)

				Dim loc_height As val = (rHeight) + (border * 2) + padding_row
				Dim loc_width As val = (rWidth) + (border * 2) + padding_col



				Dim currentImage As BufferedImage = renderImageGrayscale(tad2D)

	'            
	'                if resulting image doesn't fit into image, we should step to next columns
	'             
				If rowOffset + loc_height > maxHeight Then
					columnOffset += loc_width
					rowOffset = 0
				End If

	'            
	'                now we should place this image into output image
	'            

				graphics2D.drawImage(currentImage, columnOffset + 1, rowOffset + 1, Nothing)


	'            
	'                draw borders around each image
	'            

				graphics2D.setPaint(borderColor)
				If tad2D.shape()(0) > Integer.MaxValue OrElse tad2D.shape()(1) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				graphics2D.drawRect(columnOffset, rowOffset, CInt(tad2D.shape()(0)), CInt(tad2D.shape()(1)))



	'            
	'                draw one of 3 zoomed images if we're not on first level
	'            

				If z Mod 5 = 0 AndAlso z <> 0 AndAlso numZoomed < limZoomed AndAlso (rHeight <> zoomHeight AndAlso rWidth <> zoomWidth) Then

					Dim cY As Integer = (zoomSpan * numZoomed) + (zoomHeight)

					graphics2D.drawImage(currentImage, CInt(width) - zoomWidth - 1, cY - 1, zoomWidth, zoomHeight, Nothing)
					graphics2D.drawRect(CInt(width) - zoomWidth - 2, cY - 2, zoomWidth, zoomHeight)

					' draw line to connect this zoomed pic with its original
					graphics2D.drawLine(columnOffset + CInt(rWidth), rowOffset + CInt(rHeight), CInt(width) - zoomWidth - 2, cY - 2 + zoomHeight)
					numZoomed += 1
				End If

				rowOffset += loc_height
				z += 1
			Loop
			Return outputImage
		End Function

		''' <summary>
		''' Returns RGB image out of 3D tensor
		''' </summary>
		''' <param name="tensor3D">
		''' @return </param>
		Private Function restoreRGBImage(ByVal tensor3D As INDArray) As BufferedImage
			Dim arrayR As INDArray = Nothing
			Dim arrayG As INDArray = Nothing
			Dim arrayB As INDArray = Nothing

			' entry for 3D input vis
			If tensor3D.shape()(0) = 3 Then
				arrayR = tensor3D.tensorAlongDimension(2, 2, 1)
				arrayG = tensor3D.tensorAlongDimension(1, 2, 1)
				arrayB = tensor3D.tensorAlongDimension(0, 2, 1)
			Else
				' for all other cases input is just black & white, so we just assign the same channel data to RGB, and represent everything as RGB
				arrayB = tensor3D.tensorAlongDimension(0, 2, 1)
				arrayG = arrayB
				arrayR = arrayB
			End If

			Dim imageToRender As New BufferedImage(arrayR.columns(), arrayR.rows(), BufferedImage.TYPE_INT_RGB)
			Dim x As Integer = 0
			Do While x < arrayR.columns()
				Dim y As Integer = 0
				Do While y < arrayR.rows()
					Dim pix As New Color(CInt(Math.Truncate(255 * arrayR.getRow(y).getDouble(x))), CInt(Math.Truncate(255 * arrayG.getRow(y).getDouble(x))), CInt(Math.Truncate(255 * arrayB.getRow(y).getDouble(x))))
					Dim rgb As Integer = pix.getRGB()
					imageToRender.setRGB(x, y, rgb)
					y += 1
				Loop
				x += 1
			Loop
			Return imageToRender
		End Function

		''' <summary>
		''' Renders 2D INDArray into BufferedImage
		''' </summary>
		''' <param name="array"> </param>
		Private Function renderImageGrayscale(ByVal array As INDArray) As BufferedImage
			Dim imageToRender As New BufferedImage(array.columns(), array.rows(), BufferedImage.TYPE_BYTE_GRAY)
			Dim x As Integer = 0
			Do While x < array.columns()
				Dim y As Integer = 0
				Do While y < array.rows()
					imageToRender.getRaster().setSample(x, y, 0, CInt(Math.Truncate(255 * array.getRow(y).getDouble(x))))
					y += 1
				Loop
				x += 1
			Loop

			Return imageToRender
		End Function

		Private Sub writeImageGrayscale(ByVal array As INDArray, ByVal file As File)
			Try
				ImageIO.write(renderImageGrayscale(array), "png", file)
			Catch e As IOException
				log.error("",e)
			End Try
		End Sub

		Private Sub writeImage(ByVal array As INDArray, ByVal file As File)
			Dim image As BufferedImage = ImageLoader.toImage(array)
			Try
				ImageIO.write(image, "png", file)
			Catch e As IOException
				log.error("",e)
			End Try

		End Sub

		Private Sub writeRows(ByVal array As INDArray, ByVal file As File)
			Try
				Dim writer As New PrintWriter(file)
				Dim x As Integer = 0
				Do While x < array.rows()
					writer.println("Row [" & x & "]: " & array.getRow(x))
					x += 1
				Loop
				writer.flush()
				writer.close()
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub
	End Class

End Namespace