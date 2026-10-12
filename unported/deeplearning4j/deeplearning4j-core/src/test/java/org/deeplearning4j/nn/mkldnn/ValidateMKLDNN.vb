Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports LayerHelperValidationUtil = org.deeplearning4j.LayerHelperValidationUtil
Imports TestUtils = org.deeplearning4j.TestUtils
Imports SingletonDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonDataSetIterator
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports junit.framework.TestCase
import static org.junit.Assume.assumeTrue

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

Namespace org.deeplearning4j.nn.mkldnn


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class ValidateMKLDNN extends org.deeplearning4j.BaseDL4JTest
	Public Class ValidateMKLDNN
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateConvSubsampling() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateConvSubsampling()
			'Only run test if using nd4j-native backend
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assumeTrue(Nd4j.Backend.GetType().FullName.ToLower().contains("native"))
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Nd4j.Random.setSeed(12345)

			Dim inputSize() As Integer = {-1, 3, 16, 16}

			For Each minibatch As Integer In New Integer(){1, 3}
				For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Same, ConvolutionMode.Truncate}
					For Each kernel As Integer() In New Integer()(){
						New Integer() {2, 2},
						New Integer() {2, 3}
					}
						For Each stride As Integer() In New Integer()(){
							New Integer() {1, 1},
							New Integer() {2, 2}
						}
							For Each pt As PoolingType In New PoolingType(){PoolingType.MAX, PoolingType.AVG}

								inputSize(0) = minibatch
								Dim f As INDArray = Nd4j.rand(DataType.FLOAT, inputSize)
								Dim l As INDArray = TestUtils.randomOneHot(minibatch, 10).castTo(DataType.FLOAT)

								Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).convolutionMode(cm).seed(12345).list().layer((New ConvolutionLayer.Builder()).activation(Activation.TANH).kernelSize(kernel).stride(stride).padding(0, 0).nOut(3).build()).layer((New SubsamplingLayer.Builder()).poolingType(pt).kernelSize(kernel).stride(stride).padding(0, 0).build()).layer((New ConvolutionLayer.Builder()).activation(Activation.TANH).kernelSize(kernel).stride(stride).padding(0, 0).nOut(3).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.convolutional(inputSize(2), inputSize(3), inputSize(1))).build()

								Dim netWith As New MultiLayerNetwork(conf.clone())
								netWith.init()

								Dim netWithout As New MultiLayerNetwork(conf.clone())
								netWithout.init()

								Dim name As String = pt & ", mb=" & minibatch & ", cm=" & cm & ", kernel=" & Arrays.toString(kernel) & ", stride=" & Arrays.toString(stride)
								Dim tc As LayerHelperValidationUtil.TestCase = LayerHelperValidationUtil.TestCase.builder().testName(name).allowHelpersForClasses(Arrays.asList(Of Type)(GetType(org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingLayer), GetType(org.deeplearning4j.nn.layers.convolution.ConvolutionLayer))).testForward(True).testScore(True).testBackward(True).testTraining(True).features(f).labels(l).data(New SingletonDataSetIterator(New DataSet(f, l))).build()

								Console.WriteLine("Starting test: " & name)
								LayerHelperValidationUtil.validateMLN(netWith, tc)
							Next pt
						Next stride
					Next kernel
				Next cm
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void validateBatchNorm()
		Public Overridable Sub validateBatchNorm()
			'Only run test if using nd4j-native backend
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assumeTrue(Nd4j.Backend.GetType().FullName.ToLower().contains("native"))
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Nd4j.Random.setSeed(12345)

			Dim inputSize() As Integer = {-1, 3, 16, 16}
			Dim stride() As Integer = {1, 1}
			Dim kernel() As Integer = {2, 2}
			Dim cm As ConvolutionMode = ConvolutionMode.Truncate

			For Each minibatch As Integer In New Integer(){1, 3}
				For Each b As Boolean In New Boolean(){True, False}

					inputSize(0) = minibatch
					Dim f As INDArray = Nd4j.rand(Nd4j.defaultFloatingPointType(), inputSize)
					Dim l As INDArray = TestUtils.randomOneHot(minibatch, 10)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.FLOAT).updater(New Adam(0.01)).convolutionMode(cm).seed(12345).list().layer((New ConvolutionLayer.Builder()).activation(Activation.TANH).kernelSize(kernel).stride(stride).padding(0, 0).nOut(3).build()).layer((New BatchNormalization.Builder()).useLogStd(b).helperAllowFallback(False).build()).layer((New ConvolutionLayer.Builder()).activation(Activation.TANH).kernelSize(kernel).stride(stride).padding(0, 0).nOut(3).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.convolutional(inputSize(2), inputSize(3), inputSize(1))).build()

					Dim netWith As New MultiLayerNetwork(conf.clone())
					netWith.init()

					Dim netWithout As New MultiLayerNetwork(conf.clone())
					netWithout.init()

					Dim tc As LayerHelperValidationUtil.TestCase = LayerHelperValidationUtil.TestCase.builder().allowHelpersForClasses(Collections.singletonList(Of Type)(GetType(org.deeplearning4j.nn.layers.normalization.BatchNormalization))).testForward(True).testScore(True).testBackward(True).testTraining(True).features(f).labels(l).data(New SingletonDataSetIterator(New DataSet(f, l))).maxRelError(1e-4).build()

					LayerHelperValidationUtil.validateMLN(netWith, tc)
				Next b
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void validateLRN()
		Public Overridable Sub validateLRN()

			'Only run test if using nd4j-native backend
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assumeTrue(Nd4j.Backend.GetType().FullName.ToLower().contains("native"))
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Nd4j.Random.setSeed(12345)

			Dim inputSize() As Integer = {-1, 3, 16, 16}
			Dim stride() As Integer = {1, 1}
			Dim kernel() As Integer = {2, 2}
			Dim cm As ConvolutionMode = ConvolutionMode.Truncate

			Dim a() As Double = {1e-4, 1e-4, 1e-3, 1e-3}
			Dim b() As Double = {0.75, 0.9, 0.75, 0.75}
			Dim n() As Double = {5, 3, 3, 4}
			Dim k() As Double = {2, 2.5, 2.75, 2}

			For Each minibatch As Integer In New Integer(){1, 3}
				For i As Integer = 0 To a.Length - 1
					Console.WriteLine("+++++ MINIBATCH = " & minibatch & ", TEST=" & i & " +++++")


					inputSize(0) = minibatch
					Dim f As INDArray = Nd4j.rand(Nd4j.defaultFloatingPointType(), inputSize)
					Dim l As INDArray = TestUtils.randomOneHot(minibatch, 10).castTo(DataType.FLOAT)

					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).convolutionMode(cm).weightInit(New NormalDistribution(0,1)).seed(12345).list().layer((New ConvolutionLayer.Builder()).activation(Activation.TANH).kernelSize(kernel).stride(stride).padding(0, 0).nOut(3).build()).layer((New LocalResponseNormalization.Builder()).alpha(a(i)).beta(b(i)).n(n(i)).k(k(i)).cudnnAllowFallback(False).build()).layer((New OutputLayer.Builder()).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.convolutional(inputSize(2), inputSize(3), inputSize(1))).build()

					Dim netWith As New MultiLayerNetwork(conf.clone())
					netWith.init()

					Dim netWithout As New MultiLayerNetwork(conf.clone())
					netWithout.init()

					Dim tc As LayerHelperValidationUtil.TestCase = LayerHelperValidationUtil.TestCase.builder().allowHelpersForClasses(Collections.singletonList(Of Type)(GetType(org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization))).testForward(True).testScore(True).testBackward(True).testTraining(True).features(f).labels(l).data(New SingletonDataSetIterator(New DataSet(f, l))).minAbsError(1e-3).maxRelError(1e-2).build()

					LayerHelperValidationUtil.validateMLN(netWith, tc)

					Console.WriteLine("/////////////////////////////////////////////////////////////////////////////")
				Next i
			Next minibatch
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void compareBatchNormBackward() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub compareBatchNormBackward()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assumeTrue(Nd4j.Backend.GetType().FullName.ToLower().contains("native"))

			Nd4j.Random.setSeed(12345)
			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 1, 3, 15, 15)
			Dim mean As INDArray = [in].mean(0, 2, 3).reshape(1,3)
			Dim var As INDArray = [in].var(0, 2, 3).reshape(1,3)
			Dim eps As INDArray = Nd4j.rand(DataType.FLOAT, [in].shape())
			Dim gamma As INDArray = Nd4j.rand(DataType.FLOAT, 1,3)
			Dim beta As INDArray = Nd4j.rand(DataType.FLOAT, 1,3)
			Dim e As Double = 1e-3

			Dim dLdIn As INDArray = [in].ulike()
			Dim dLdm As INDArray = mean.ulike()
			Dim dLdv As INDArray = var.ulike()
			Dim dLdg As INDArray = gamma.ulike()
			Dim dLdb As INDArray = beta.ulike()


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).inferenceWorkspaceMode(WorkspaceMode.NONE).trainingWorkspaceMode(WorkspaceMode.NONE).list().layer((New BatchNormalization.Builder()).nIn(3).nOut(3).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim bn As org.deeplearning4j.nn.layers.normalization.BatchNormalization = DirectCast(net.getLayer(0), org.deeplearning4j.nn.layers.normalization.BatchNormalization)
			assertNotNull(bn.Helper)
			Console.WriteLine(bn.Helper)

			net.output([in], True)
			bn.setInput([in], LayerWorkspaceMgr.noWorkspaces())
			Dim pcudnn As Pair(Of Gradient, INDArray) = net.backpropGradient(eps, LayerWorkspaceMgr.noWorkspaces())

			Dim f As System.Reflection.FieldInfo = bn.GetType().getDeclaredField("helper")
			f.setAccessible(True)
			f.set(bn, Nothing)
			assertNull(bn.Helper)

			net.output([in], True)
			bn.setInput([in], LayerWorkspaceMgr.noWorkspaces())
			Dim p As Pair(Of Gradient, INDArray) = net.backpropGradient(eps, LayerWorkspaceMgr.noWorkspaces())

			Dim dldin_dl4j As INDArray = p.Second
			Dim dldin_helper As INDArray = pcudnn.Second

			assertTrue(dldin_dl4j.equalsWithEps(dldin_helper, 1e-5))
		End Sub
	End Class

End Namespace