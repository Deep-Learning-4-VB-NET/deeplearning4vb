Imports System.Collections.Generic
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports ColorConversionTransform = org.datavec.image.transform.ColorConversionTransform
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports DetectedObject = org.deeplearning4j.nn.layers.objdetect.DetectedObject
Imports YoloUtils = org.deeplearning4j.nn.layers.objdetect.YoloUtils
Imports Darknet19 = org.deeplearning4j.zoo.model.Darknet19
Imports TinyYOLO = org.deeplearning4j.zoo.model.TinyYOLO
Imports VGG19 = org.deeplearning4j.zoo.model.VGG19
Imports YOLO2 = org.deeplearning4j.zoo.model.YOLO2
Imports ClassPrediction = org.deeplearning4j.zoo.util.ClassPrediction
Imports Labels = org.deeplearning4j.zoo.util.Labels
Imports COCOLabels = org.deeplearning4j.zoo.util.darknet.COCOLabels
Imports DarknetLabels = org.deeplearning4j.zoo.util.darknet.DarknetLabels
Imports VOCLabels = org.deeplearning4j.zoo.util.darknet.VOCLabels
Imports ImageNetLabels = org.deeplearning4j.zoo.util.imagenet.ImageNetLabels
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports VGG16ImagePreProcessor = org.nd4j.linalg.dataset.api.preprocessor.VGG16ImagePreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.bytedeco.opencv.global.opencv_imgproc.COLOR_BGR2RGB
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.deeplearning4j.zoo


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestImageNet extends org.deeplearning4j.BaseDL4JTest
	Public Class TestImageNet
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testImageNetLabels() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImageNetLabels()
			' set up model
			Dim model As ZooModel = VGG19.builder().numClasses(0).build() 'num labels doesn't matter since we're getting pretrained imagenet
			Dim initializedModel As ComputationGraph = DirectCast(model.initPretrained(), ComputationGraph)

			' set up input and feedforward
			Dim loader As New NativeImageLoader(224, 224, 3)
			Dim classloader As ClassLoader = Thread.CurrentThread.getContextClassLoader()
			Dim image As INDArray = loader.asMatrix(classloader.getResourceAsStream("deeplearning4j-zoo/goldenretriever.jpg"))
			Dim scaler As DataNormalization = New VGG16ImagePreProcessor()
			scaler.transform(image)
			Dim output() As INDArray = initializedModel.output(False, image)

			' check output labels of result
			Dim decodedLabels As String = (New ImageNetLabels()).decodePredictions(output(0))
			log.info(decodedLabels)
			assertTrue(decodedLabels.Contains("golden_retriever"))

			' clean up for current model
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/30 - Failing (intermittently?) on CI linux - see issue 7657") public void testDarknetLabels() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDarknetLabels()
			' set up model
			Dim model As ZooModel = Darknet19.builder().numClasses(0).build() 'num labels doesn't matter since we're getting pretrained imagenet
			Dim initializedModel As ComputationGraph = DirectCast(model.initPretrained(), ComputationGraph)

			' set up input and feedforward
			Dim loader As New NativeImageLoader(224, 224, 3, New ColorConversionTransform(COLOR_BGR2RGB))
			Dim classloader As ClassLoader = Thread.CurrentThread.getContextClassLoader()
			Dim image As INDArray = loader.asMatrix(classloader.getResourceAsStream("deeplearning4j-zoo/goldenretriever.jpg"))
			Dim scaler As DataNormalization = New ImagePreProcessingScaler(0, 1)
			scaler.transform(image)
			Dim result As INDArray = initializedModel.outputSingle(image)
			Dim labels As Labels = New DarknetLabels()
			Dim predictions As IList(Of IList(Of ClassPrediction)) = labels.decodePredictions(result, 10)

			' check output labels of result
			log.info(predictions.ToString())
			assertEquals("golden retriever", predictions(0)(0).getLabel())

			' clean up for current model
			initializedModel.params().close()
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()

			' set up model
			model = TinyYOLO.builder().numClasses(0).build() 'num labels doesn't matter since we're getting pretrained imagenet
			initializedModel = DirectCast(model.initPretrained(), ComputationGraph)

			' set up input and feedforward
			loader = New NativeImageLoader(416, 416, 3, New ColorConversionTransform(COLOR_BGR2RGB))
			image = loader.asMatrix(classloader.getResourceAsStream("deeplearning4j-zoo/goldenretriever.jpg"))
			scaler = New ImagePreProcessingScaler(0, 1)
			scaler.transform(image)
			Dim outputs As INDArray = initializedModel.outputSingle(image)
			Dim objs As IList(Of DetectedObject) = YoloUtils.getPredictedObjects(Nd4j.create(DirectCast(model, TinyYOLO).getPriorBoxes()), outputs, 0.6, 0.4)
			assertEquals(1, objs.Count)

			' check output labels of result
			labels = New VOCLabels()
			For Each obj As DetectedObject In objs
				Dim classPrediction As ClassPrediction = labels.decodePredictions(obj.getClassPredictions(), 1)(0)(0)
				log.info(obj.ToString() & " " & classPrediction)
				assertEquals("dog", classPrediction.getLabel())
			Next obj

			' clean up for current model
			initializedModel.params().close()
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()

			' set up model
			model = YOLO2.builder().numClasses(1000).build() 'num labels doesn't matter since we're getting pretrained imagenet
			initializedModel = DirectCast(model.initPretrained(), ComputationGraph)

			' set up input and feedforward
			loader = New NativeImageLoader(608, 608, 3, New ColorConversionTransform(COLOR_BGR2RGB))
			image = loader.asMatrix(classloader.getResourceAsStream("deeplearning4j-zoo/goldenretriever.jpg"))
			scaler = New ImagePreProcessingScaler(0, 1)
			scaler.transform(image)
			outputs = initializedModel.outputSingle(image)
			objs = YoloUtils.getPredictedObjects(Nd4j.create(DirectCast(model, YOLO2).getPriorBoxes()), outputs, 0.6, 0.4)
			assertEquals(1, objs.Count)

			' check output labels of result
			labels = New COCOLabels()
			For Each obj As DetectedObject In objs
				Dim classPrediction As ClassPrediction = labels.decodePredictions(obj.getClassPredictions(), 1)(0)(0)
				log.info(obj.ToString() & " " & classPrediction)
				assertEquals("dog", classPrediction.getLabel())
			Next obj

			initializedModel.params().close()
		End Sub

	End Class

End Namespace