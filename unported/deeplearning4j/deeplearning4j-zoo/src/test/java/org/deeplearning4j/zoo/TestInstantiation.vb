Imports System
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports AsyncDataSetIterator = org.deeplearning4j.datasets.iterator.AsyncDataSetIterator
Imports BenchmarkDataSetIterator = org.deeplearning4j.datasets.iterator.impl.BenchmarkDataSetIterator
Imports Model = org.deeplearning4j.nn.api.Model
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LossLayer = org.deeplearning4j.nn.conf.layers.LossLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports TransferLearningHelper = org.deeplearning4j.nn.transferlearning.TransferLearningHelper
Imports org.deeplearning4j.zoo.model
Imports DarknetHelper = org.deeplearning4j.zoo.model.helper.DarknetHelper
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assumptions.assumeTrue

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
'ORIGINAL LINE: @Slf4j @Disabled("Times out too often") @Tag(TagNames.FILE_IO) @Tag(TagNames.DL4J_OLD_API) @NativeTag @Tag(TagNames.LONG_TEST) public class TestInstantiation extends org.deeplearning4j.BaseDL4JTest
	Public Class TestInstantiation
		Inherits BaseDL4JTest

		Protected Friend Shared Sub ignoreIfCuda()
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			If "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase) Then
				log.warn("IGNORING TEST ON CUDA DUE TO CI CRASHES - SEE ISSUE #7657")
				assumeTrue(False)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub after()
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()
			Thread.Sleep(1000)
			System.GC.Collect()
		End Sub

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnTrainingDarknet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnTrainingDarknet()
			runTest(Darknet19.builder().numClasses(10).build(), "Darknet19", 10)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCnnTrainingTinyYOLO() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnTrainingTinyYOLO()
			runTest(TinyYOLO.builder().numClasses(10).build(), "TinyYOLO", 10)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/28 - Crashing on CI linux-x86-64 CPU only - Issue #7657") public void testCnnTrainingYOLO2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCnnTrainingYOLO2()
			runTest(YOLO2.builder().numClasses(10).build(), "YOLO2", 10)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void runTest(ZooModel model, String modelName, int numClasses) throws Exception
		Public Shared Sub runTest(ByVal model As ZooModel, ByVal modelName As String, ByVal numClasses As Integer)
			ignoreIfCuda()
			Dim gridWidth As Integer = -1
			Dim gridHeight As Integer = -1
			If modelName.Equals("TinyYOLO") OrElse modelName.Equals("YOLO2") Then
				Dim inputShapes() As Integer = model.metaData().getInputShape()(0)
				gridWidth = DarknetHelper.getGridWidth(inputShapes)
				gridHeight = DarknetHelper.getGridHeight(inputShapes)
				numClasses += 4
			End If

			' set up data iterator
			Dim inputShape() As Integer = model.metaData().getInputShape()(0)
			Dim iter As DataSetIterator = New BenchmarkDataSetIterator(New Integer(){8, inputShape(0), inputShape(1), inputShape(2)}, numClasses, 1, gridWidth, gridHeight)

			Dim initializedModel As Model = model.init()
			Dim async As New AsyncDataSetIterator(iter)
			If TypeOf initializedModel Is MultiLayerNetwork Then
				DirectCast(initializedModel, MultiLayerNetwork).fit(async)
			Else
				DirectCast(initializedModel, ComputationGraph).fit(async)
			End If
			async.shutdown()

			' clean up for current model
			model = Nothing
			initializedModel = Nothing
			async = Nothing
			iter = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()
			Thread.Sleep(1000)
			System.GC.Collect()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrained() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrained()
			ignoreIfCuda()
			Dim model As ZooModel = ResNet50.builder().numClasses(0).build() 'num labels doesn't matter since we're getting pretrained imagenet
			assertTrue(model.pretrainedAvailable(PretrainedType.IMAGENET))

			Dim initializedModel As ComputationGraph = DirectCast(model.initPretrained(), ComputationGraph)
			Dim f As INDArray = Nd4j.rand(New Integer(){1, 3, 224, 224})
			Dim result() As INDArray = initializedModel.output(f)
			assertArrayEquals(result(0).shape(), New Long(){1, 1000})

			'Test fitting. Not ewe need to use transfer learning, as ResNet50 has a dense layer, not an OutputLayer
			initializedModel = (New TransferLearning.GraphBuilder(initializedModel)).removeVertexAndConnections("fc1000").addLayer("fc1000", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(2048).nOut(1000).activation(Activation.SOFTMAX).build(), "flatten_1").setOutputs("fc1000").build()
			initializedModel.fit(New org.nd4j.linalg.dataset.DataSet(f, TestUtils.randomOneHot(1, 1000, 12345)))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedVGG16() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedVGG16()
			testInitPretrained(VGG16.builder().numClasses(0).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedVGG19() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedVGG19()
			testInitPretrained(VGG19.builder().numClasses(0).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/28 - JVM crash on linux CUDA CI machines - Issue 7657") public void testInitPretrainedDarknet19() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedDarknet19()
			testInitPretrained(Darknet19.builder().numClasses(0).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/28 - JVM crash on linux CUDA CI machines - Issue 7657") public void testInitPretrainedDarknet19S2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedDarknet19S2()
			testInitPretrained(Darknet19.builder().numClasses(0).inputShape(New Integer(){3, 448, 448}).build(), New Long(){1, 3, 448, 448}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedTinyYOLO() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedTinyYOLO()
			testInitPretrained(TinyYOLO.builder().numClasses(0).build(), New Long(){1, 3, 416, 416}, New Long(){1, 125, 13, 13})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedYOLO2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedYOLO2()
			testInitPretrained(YOLO2.builder().numClasses(0).build(), New Long(){1, 3, 608, 608}, New Long(){1, 425, 19, 19})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedXception() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedXception()
			testInitPretrained(Xception.builder().numClasses(0).build(), New Long(){1, 3, 299, 299}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitPretrainedSqueezenet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitPretrainedSqueezenet()
			testInitPretrained(SqueezeNet.builder().numClasses(0).build(), New Long(){1, 3, 227, 227}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void testInitPretrained(ZooModel model, long[] inShape, long[] outShape) throws Exception
		Public Overridable Sub testInitPretrained(ByVal model As ZooModel, ByVal inShape() As Long, ByVal outShape() As Long)
			ignoreIfCuda()
			assertTrue(model.pretrainedAvailable(PretrainedType.IMAGENET))

			Dim initializedModel As ComputationGraph = DirectCast(model.initPretrained(), ComputationGraph)
			Dim result() As INDArray = initializedModel.output(Nd4j.rand(inShape))
			assertArrayEquals(result(0).shape(),outShape)

			' clean up for current model
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			initializedModel.params().close()
			For Each arr As INDArray In result
				arr.close()
			Next arr
			System.GC.Collect()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelResNet50() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelResNet50()
			testInitRandomModel(ResNet50.builder().numClasses(1000).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelVGG16() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelVGG16()
			testInitRandomModel(VGG16.builder().numClasses(1000).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelVGG19() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelVGG19()
			testInitRandomModel(VGG19.builder().numClasses(1000).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelDarknet19() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelDarknet19()
			testInitRandomModel(Darknet19.builder().numClasses(1000).build(), New Long(){1, 3, 224, 224}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelDarknet19_2() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelDarknet19_2()
			testInitRandomModel(Darknet19.builder().inputShape(New Integer(){3, 448, 448}).numClasses(1000).build(), New Long(){1, 3, 448, 448}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelXception() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelXception()
			testInitRandomModel(Xception.builder().numClasses(1000).build(), New Long(){1, 3, 299, 299}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB - 2019/05/28 - JVM crash on CI - intermittent? Issue 7657") public void testInitRandomModelSqueezenet() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelSqueezenet()
			testInitRandomModel(SqueezeNet.builder().numClasses(1000).build(), New Long(){1, 3, 227, 227}, New Long(){1, 1000})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInitRandomModelFaceNetNN4Small2() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelFaceNetNN4Small2()
			testInitRandomModel(FaceNetNN4Small2.builder().embeddingSize(100).numClasses(10).build(), New Long(){1, 3, 64, 64}, New Long(){1, 10})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/29 - Crashing on CI linux-x86-64 CPU only - Issue #7657") public void testInitRandomModelUNet() throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInitRandomModelUNet()
			testInitRandomModel(UNet.builder().build(), New Long(){1, 3, 512, 512}, New Long(){1, 1, 512, 512})
		End Sub


		Public Overridable Sub testInitRandomModel(ByVal model As ZooModel, ByVal inShape() As Long, ByVal outShape() As Long)
			ignoreIfCuda()
			'Test initialization of NON-PRETRAINED models

			log.info("Testing {}", model.GetType().Name)
			Dim initializedModel As ComputationGraph = model.init()
			Dim f As INDArray = Nd4j.rand(DataType.FLOAT, inShape)
			Dim result() As INDArray = initializedModel.output(f)
			assertArrayEquals(result(0).shape(), outShape)
			Dim l As INDArray = If(outShape.Length = 2, TestUtils.randomOneHot(1, CInt(outShape(1)), 12345), Nd4j.rand(DataType.FLOAT, outShape))
			initializedModel.fit(New org.nd4j.linalg.dataset.DataSet(f, l))

			' clean up for current model
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			f.close()
			l.close()
			initializedModel.params().close()
			initializedModel.getFlattenedGradients().close()
			System.GC.Collect()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testYolo4635() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testYolo4635()
			ignoreIfCuda()
			'https://github.com/eclipse/deeplearning4j/issues/4635

			Dim nClasses As Integer = 10
			Dim model As TinyYOLO = TinyYOLO.builder().numClasses(nClasses).build()
			Dim computationGraph As ComputationGraph = CType(model.initPretrained(), ComputationGraph)
			Dim transferLearningHelper As New TransferLearningHelper(computationGraph, "conv2d_9")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransferLearning() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTransferLearning()
			ignoreIfCuda()
			'https://github.com/eclipse/deeplearning4j/issues/7193

			Dim cg As ComputationGraph = CType(ResNet50.builder().build().initPretrained(), ComputationGraph)

			cg = (New TransferLearning.GraphBuilder(cg)).addLayer("out", (New LossLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.IDENTITY).build(), "fc1000").setInputTypes(InputType.convolutional(224, 224, 3)).setOutputs("out").build()

		End Sub
	End Class

End Namespace