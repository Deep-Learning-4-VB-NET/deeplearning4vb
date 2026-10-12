Imports System
Imports System.Collections.Generic
Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports ObjectDetectionRecordReader = org.datavec.image.recordreader.objdetect.ObjectDetectionRecordReader
Imports VocLabelProvider = org.datavec.image.recordreader.objdetect.impl.VocLabelProvider
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports org.deeplearning4j.nn.conf
Imports GaussianDistribution = org.deeplearning4j.nn.conf.distribution.GaussianDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports Arguments = org.junit.jupiter.params.provider.Arguments
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.FILE_IO) @NativeTag public class YoloGradientCheckTests extends org.deeplearning4j.BaseDL4JTest
	Public Class YoloGradientCheckTests
		Inherits BaseDL4JTest

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

		Public Shared Function params() As Stream(Of Arguments)
			Dim args As IList(Of Arguments) = New List(Of Arguments)()
			For Each nd4jBackend As Nd4jBackend In BaseNd4jTestWithBackends.BACKENDS
				For Each format As CNN2DFormat In CNN2DFormat.values()
					args.Add(Arguments.of(format,nd4jBackend))
				Next format
			Next nd4jBackend
			Return args.stream()
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.YoloGradientCheckTests#params") public void testYoloOutputLayer(CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testYoloOutputLayer(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Dim depthIn As Integer = 2
			Dim c As Integer = 3
			Dim b As Integer = 3

			Dim yoloDepth As Integer = b * (5 + c)
			Dim a As Activation = Activation.TANH

			Nd4j.Random.setSeed(1234567)

			Dim minibatchSizes() As Integer = {1, 3}
			Dim widths() As Integer = {4, 7}
			Dim heights() As Integer = {4, 5}
			Dim l1() As Double = {0.0, 0.3}
			Dim l2() As Double = {0.0, 0.4}

			For i As Integer = 0 To widths.Length - 1

				Dim w As Integer = widths(i)
				Dim h As Integer = heights(i)
				Dim mb As Integer = minibatchSizes(i)

				Nd4j.Random.setSeed(12345)
				Dim bbPrior As INDArray = Nd4j.rand(b, 2).muliRowVector(Nd4j.create(New Double(){w, h})).addi(0.1)

				Nd4j.Random.setSeed(12345)

				Dim input, labels As INDArray
				If format = CNN2DFormat.NCHW Then
					input = Nd4j.rand(DataType.DOUBLE, mb, depthIn, h, w)
					labels = yoloLabels(mb, c, h, w)
				Else
					input = Nd4j.rand(DataType.DOUBLE, mb, h, w, depthIn)
					labels = yoloLabels(mb, c, h, w).permute(0,2,3,1)
				End If

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).dataType(DataType.DOUBLE).updater(New NoOp()).activation(a).l1(l1(i)).l2(l2(i)).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).kernelSize(2, 2).stride(1, 1).dataFormat(format).nIn(depthIn).nOut(yoloDepth).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPrior).build()).setInputType(InputType.convolutional(h, w, depthIn, format)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim msg As String = "testYoloOutputLayer() - minibatch = " & mb & ", w=" & w & ", h=" & h & ", l1=" & l1(i) & ", l2=" & l2(i)
				Console.WriteLine(msg)

				Dim [out] As INDArray = net.output(input)
				If format = CNN2DFormat.NCHW Then
					assertArrayEquals(New Long(){mb, yoloDepth, h, w}, [out].shape())
				Else
					assertArrayEquals(New Long(){mb, h, w, yoloDepth}, [out].shape())
				End If

				net.fit(input, labels)


				Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(input).minAbsoluteError(1e-6).labels(labels).subset(True).maxPerParam(100))

				assertTrue(gradOK,msg)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

		Private Shared Function yoloLabels(ByVal mb As Integer, ByVal c As Integer, ByVal h As Integer, ByVal w As Integer) As INDArray
			Dim labelDepth As Integer = 4 + c
			Dim labels As INDArray = Nd4j.zeros(mb, labelDepth, h, w)
			'put 1 object per minibatch, at positions (0,0), (1,1) etc.
			'Positions for label boxes: (1,1) to (2,2), (2,2) to (4,4) etc

			For i As Integer = 0 To mb - 1
				'Class labels
				labels.putScalar(i, 4 + i Mod c, i Mod h, i Mod w, 1)

				'BB coordinates (top left, bottom right)
				labels.putScalar(i, 0, 0, 0, i Mod w)
				labels.putScalar(i, 1, 0, 0, i Mod h)
				labels.putScalar(i, 2, 0, 0, (i Mod w)+1)
				labels.putScalar(i, 3, 0, 0, (i Mod h)+1)
			Next i

			Return labels
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.deeplearning4j.gradientcheck.YoloGradientCheckTests#params") public void yoloGradientCheckRealData(CNN2DFormat format,org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub yoloGradientCheckRealData(ByVal format As CNN2DFormat, ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim is1 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/JPEGImages/2007_009346.jpg")).InputStream
			Dim is2 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/Annotations/2007_009346.xml")).InputStream
			Dim is3 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/JPEGImages/2008_003344.jpg")).InputStream
			Dim is4 As Stream = (New ClassPathResource("yolo/VOC_TwoImage/Annotations/2008_003344.xml")).InputStream

			Dim dir As New File(testDir.toFile(),"testYoloOverfitting")
			dir.mkdirs()
			Dim jpg As New File(dir, "JPEGImages")
			Dim annot As New File(dir, "Annotations")
			jpg.mkdirs()
			annot.mkdirs()

			Dim imgOut As New File(jpg, "2007_009346.jpg")
			Dim annotationOut As New File(annot, "2007_009346.xml")

			Try
					Using fos As New FileStream(imgOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is1, fos)
					End Using
			Finally
				is1.Close()
			End Try

			Try
					Using fos As New FileStream(annotationOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is2, fos)
					End Using
			Finally
				is2.Close()
			End Try

			imgOut = New File(jpg, "2008_003344.jpg")
			annotationOut = New File(annot, "2008_003344.xml")
			Try
					Using fos As New FileStream(imgOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is3, fos)
					End Using
			Finally
				is3.Close()
			End Try

			Try
					Using fos As New FileStream(annotationOut, FileMode.Create, FileAccess.Write)
					IOUtils.copy(is4, fos)
					End Using
			Finally
				is4.Close()
			End Try

			Dim bbPriors As INDArray = Nd4j.create(New Double()(){
				New Double() {3, 3},
				New Double() {3, 5},
				New Double() {5, 7}
			})

			'2x downsampling to 13x13 = 26x26 input images
			'Required depth at output layer: 5B+C, with B=3, C=20 object classes, for VOC
			Dim lp As New VocLabelProvider(dir.getPath())
			Dim h As Integer = 26
			Dim w As Integer = 26
			Dim c As Integer = 3
			Dim rr As RecordReader = New ObjectDetectionRecordReader(h, w, c, 13, 13, lp)
			rr.initialize(New org.datavec.api.Split.FileSplit(jpg))

			Dim nClasses As Integer = rr.getLabels().Count
			Dim depthOut As Long = bbPriors.size(0) * (5 + nClasses)


			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr,2,1,1,True)
			iter.PreProcessor = New ImagePreProcessingScaler()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).convolutionMode(ConvolutionMode.Same).updater(New NoOp()).dist(New GaussianDistribution(0,0.1)).seed(12345).list().layer((New ConvolutionLayer.Builder()).kernelSize(3,3).stride(1,1).nOut(4).build()).layer((New SubsamplingLayer.Builder()).kernelSize(2,2).stride(2,2).build()).layer((New ConvolutionLayer.Builder()).activation(Activation.IDENTITY).kernelSize(3,3).stride(1,1).nOut(depthOut).build()).layer((New Yolo2OutputLayer.Builder()).boundingBoxPriors(bbPriors).build()).setInputType(InputType.convolutional(h,w,c)).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			Dim f As INDArray = ds.Features
			Dim l As INDArray = ds.Labels

			Dim ok As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.MLNConfig()).net(net).input(f).labels(l).inputMask(Nothing).subset(True).maxPerParam(64))

			assertTrue(ok)
			TestUtils.testModelSerialization(net)
		End Sub
	End Class

End Namespace