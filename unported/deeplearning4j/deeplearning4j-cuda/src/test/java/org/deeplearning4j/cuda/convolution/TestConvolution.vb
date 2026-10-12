Imports System
Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports CuDNNTestUtils = org.deeplearning4j.cuda.CuDNNTestUtils
Imports TestUtils = org.deeplearning4j.cuda.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports GaussianDistribution = org.deeplearning4j.nn.conf.distribution.GaussianDistribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Resources = org.nd4j.common.resources.Resources
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions
Imports org.junit.jupiter.api

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.convolution


	''' <summary>
	''' Created by Alex on 15/11/2016.
	''' </summary>
	Public Class TestConvolution
		Inherits BaseDL4JTest


		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 240000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSameModeActivationSizes()
		Public Overridable Sub testSameModeActivationSizes()
			Dim inH As Integer = 3
			Dim inW As Integer = 4
			Dim inDepth As Integer = 3
			Dim minibatch As Integer = 5

			Dim sH As Integer = 2
			Dim sW As Integer = 2
			Dim kH As Integer = 3
			Dim kW As Integer = 3

			Dim l(1) As org.deeplearning4j.nn.conf.layers.Layer
			l(0) = (New ConvolutionLayer.Builder()).nOut(4).kernelSize(kH, kW).stride(sH, sW).build()
			l(1) = (New SubsamplingLayer.Builder()).kernelSize(kH, kW).stride(sH, sW).build()

			For i As Integer = 0 To l.Length - 1

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).list().layer(0, l(i)).layer(1, (New OutputLayer.Builder()).nOut(3).build()).setInputType(InputType.convolutional(inH, inW, inDepth)).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim inData As INDArray = Nd4j.create(minibatch, inDepth, inH, inW)
				Dim activations As IList(Of INDArray) = net.feedForward(inData)
				Dim actL0 As INDArray = activations(1)

				Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(sH)))))
				Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(sW)))))

				Console.WriteLine(Arrays.toString(actL0.shape()))
				assertArrayEquals(New Long(){minibatch, (If(i = 0, 4, inDepth)), outH, outW}, actL0.shape())
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompareCudnnStandardOutputsVsMode() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCompareCudnnStandardOutputsVsMode()

			Dim cm() As ConvolutionMode = {ConvolutionMode.Strict, ConvolutionMode.Truncate, ConvolutionMode.Same}

			For Each c As ConvolutionMode In cm
				For Each a As ConvolutionLayer.AlgoMode In New ConvolutionLayer.AlgoMode(){ConvolutionLayer.AlgoMode.NO_WORKSPACE, ConvolutionLayer.AlgoMode.PREFER_FASTEST}
					For Each conv As Boolean In New Boolean(){True, False}
						Dim msg As String = c & " - " & a & " - " & (If(conv, "conv", "subsampling"))
						Console.WriteLine(msg)

						Dim l As org.deeplearning4j.nn.conf.layers.Layer
						If conv Then
							l = (New ConvolutionLayer.Builder()).nOut(4).kernelSize(4, 4).stride(2, 2).build()
						Else
							l = (New SubsamplingLayer.Builder()).kernelSize(4, 4).stride(2, 2).build()
						End If

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).seed(12345).l2(0.0005).updater(New Sgd(0.01)).weightInit(WeightInit.XAVIER).convolutionMode(c).cudnnAlgoMode(a).list().layer(0, l).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
						If conv Then
							assertEquals(a, DirectCast(l, ConvolutionLayer).getCudnnAlgoMode())
						End If

						Nd4j.Random.setSeed(12345)
						Dim net1 As New MultiLayerNetwork(conf)
						net1.init()
						net1.initGradientsView()

						Nd4j.Random.setSeed(12345)
						Dim net2 As New MultiLayerNetwork(conf)
						net2.init()
						net2.initGradientsView()

						Dim layerCudnn As Layer = net1.getLayer(0)
						Dim layerStandard As Layer = net2.getLayer(0)

						Dim f As System.Reflection.FieldInfo = layerStandard.GetType().getDeclaredField("helper")
						f.setAccessible(True)
						f.set(layerStandard, Nothing)

						If f.get(layerCudnn) Is Nothing Then
							Throw New Exception()
						End If
						If f.get(layerStandard) IsNot Nothing Then
							Throw New Exception()
						End If


						Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){1, 1, 20, 20}) '(20-4+0)/2 +1 = 9

						Dim outCudnn As INDArray = layerCudnn.activate([in], False, LayerWorkspaceMgr.noWorkspaces())
						Dim outStd As INDArray = layerStandard.activate([in], False, LayerWorkspaceMgr.noWorkspaces())

						assertEquals(outStd, outCudnn, msg)


						'Check backprop:
						Dim epsilon As INDArray = Nd4j.rand(DataType.DOUBLE, outStd.shape())
						Dim pCudnn As Pair(Of Gradient, INDArray) = layerCudnn.backpropGradient(epsilon.dup(), LayerWorkspaceMgr.noWorkspaces())
						Dim pStd As Pair(Of Gradient, INDArray) = layerStandard.backpropGradient(epsilon.dup(), LayerWorkspaceMgr.noWorkspaces())

	'                    System.out.println(Arrays.toString(pStd.getSecond().data().asFloat()));
	'                    System.out.println(Arrays.toString(pCudnn.getSecond().data().asFloat()));

						Dim epsOutStd As INDArray = pStd.Second
						Dim epsOutCudnn As INDArray = pCudnn.Second

						assertTrue(epsOutStd.equalsWithEps(epsOutCudnn, 1e-4), msg)

						If conv Then
							Dim gradStd As INDArray = pStd.First.gradient()
							Dim gradCudnn As INDArray = pCudnn.First.gradient()

							assertTrue(gradStd.equalsWithEps(gradCudnn, 1e-4), msg)
						End If
					Next conv
				Next a
			Next c
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Disabled due to permissions issues") public void validateXceptionImport(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub validateXceptionImport(ByVal testDir As Path)
			Dim dir As File = testDir.resolve("test-dir").toFile()
			Dim fSource As File = Resources.asFile("modelimport/keras/examples/xception/xception_tf_keras_2.h5")
			Dim fExtracted As New File(dir, "xception_tf_keras_2.h5")
			FileUtils.copyFile(fSource, fExtracted)

			Dim inSize As Integer = 256
			Dim model As ComputationGraph = KerasModelImport.importKerasModelAndWeights(fExtracted.getAbsolutePath(), New Integer(){inSize, inSize, 3}, False)
			model = model.convertDataType(DataType.DOUBLE)

			Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, New Integer(){1, inSize, inSize, 3}) 'Keras import model -> NHWC

			CuDNNTestUtils.assertHelpersPresent(model.Layers)
			Dim withCudnn As IDictionary(Of String, INDArray) = model.feedForward([in], False)

			CuDNNTestUtils.removeHelpers(model.Layers)
			CuDNNTestUtils.assertHelpersAbsent(model.Layers)
			Dim noCudnn As IDictionary(Of String, INDArray) = model.feedForward([in], False)

			assertEquals(withCudnn.Keys, noCudnn.Keys)

			For Each s As String In withCudnn.Keys
				assertEquals(withCudnn(s), noCudnn(s), s)
			Next s
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCudnnDilation()
		Public Overridable Sub testCudnnDilation()
			'Sanity check on dilated conv execution
			Dim k() As Integer = {2, 3, 4, 5}
			Dim d() As Integer = {1, 2, 3, 4}

			For Each inputSize As Integer() In New Integer()(){
				New Integer() {10, 1, 28, 28},
				New Integer() {3, 3, 224, 224}
			}
				For i As Integer = 0 To k.Length - 1
					For Each cm As ConvolutionMode In New ConvolutionMode(){ConvolutionMode.Same, ConvolutionMode.Truncate}

						Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).convolutionMode(ConvolutionMode.Same).list().layer((New ConvolutionLayer.Builder()).kernelSize(k(i), k(i)).dilation(d(i), d(i)).nOut(3).build()).layer((New SubsamplingLayer.Builder()).kernelSize(k(i), k(i)).dilation(d(i), d(i)).build()).layer((New OutputLayer.Builder()).nOut(10).build()).setInputType(InputType.convolutional(inputSize(3), inputSize(2), inputSize(1))).build()

						Dim net As New MultiLayerNetwork(conf)
						net.init()

						Dim [in] As INDArray = Nd4j.create(inputSize)
						net.output([in])
					Next cm
				Next i
			Next inputSize
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGradientNorm() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGradientNorm()

			Dim height As Integer = 100
			Dim width As Integer = 100
			Dim channels As Integer = 1
			Dim numLabels As Integer = 10

			For Each batchSize As Integer In New Integer(){1, 32}

				Dim seed As Long = 12345
				Dim nonZeroBias As Double = 1

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(seed).dataType(DataType.DOUBLE).dist(New NormalDistribution(0.0, 0.01)).activation(Activation.RELU).updater(New Adam(5e-3)).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).l2(5 * 1e-4).list().layer(convInit("cnn1", channels, 96, New Integer(){11, 11}, New Integer(){4, 4}, New Integer(){3, 3}, 0)).layer(maxPool("maxpool1", New Integer(){3, 3})).layer(conv5x5("cnn2", 256, New Integer(){1, 1}, New Integer(){2, 2}, nonZeroBias)).layer(maxPool("maxpool2", New Integer(){3, 3})).layer(conv3x3("cnn3", 384, 0)).layer(conv3x3("cnn4", 384, nonZeroBias)).layer(conv3x3("cnn5", 256, nonZeroBias)).layer(maxPool("maxpool3", New Integer(){3, 3})).layer(fullyConnected("ffn1", 4096, nonZeroBias, New GaussianDistribution(0, 0.005))).layer(fullyConnected("ffn2", 4096, nonZeroBias, New GaussianDistribution(0, 0.005))).layer((New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).name("output").nOut(numLabels).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutional(height, width, channels)).build()


				Dim netNoCudnn As New MultiLayerNetwork(conf.clone())
				netNoCudnn.init()
				Dim netWithCudnn As New MultiLayerNetwork(conf.clone())
				netWithCudnn.init()

				CuDNNTestUtils.removeHelpers(netNoCudnn.Layers)



				Nd4j.Random.setSeed(12345)
				For j As Integer = 0 To 2
	'                System.out.println("j=" + j);
					Dim f As INDArray = Nd4j.rand(New Integer(){batchSize, channels, height, width})
					Dim l As INDArray = TestUtils.randomOneHot(batchSize, numLabels)

					netNoCudnn.fit(f, l)
					netWithCudnn.fit(f, l)

					assertEquals(netNoCudnn.score(), netWithCudnn.score(), 1e-5)

					For Each e As KeyValuePair(Of String, INDArray) In netNoCudnn.paramTable().SetOfKeyValuePairs()
						Dim pEq As Boolean = e.Value.equalsWithEps(netWithCudnn.paramTable()(e.Key), 1e-4)
	'                    int idx = e.getKey().indexOf("_");
	'                    int layerNum = Integer.parseInt(e.getKey().substring(0, idx));
						'System.out.println(e.getKey() + " - " + pEq + " - " + netNoCudnn.getLayer(layerNum).getClass().getSimpleName());
						assertTrue(pEq)
					Next e

					Dim eq As Boolean = netNoCudnn.params().equalsWithEps(netWithCudnn.params(), 1e-4)
					assertTrue(eq)
				Next j
			Next batchSize
		End Sub


		Private Shared Function convInit(ByVal name As String, ByVal [in] As Integer, ByVal [out] As Integer, ByVal kernel() As Integer, ByVal stride() As Integer, ByVal pad() As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(kernel, stride, pad)).name(name).nIn([in]).nOut([out]).biasInit(bias).build()
		End Function

		Private Shared Function conv3x3(ByVal name As String, ByVal [out] As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() { 3, 3 }, New Integer() { 1, 1 }, New Integer() { 1, 1 })).name(name).nOut([out]).biasInit(bias).build()
		End Function

		Private Shared Function conv5x5(ByVal name As String, ByVal [out] As Integer, ByVal stride() As Integer, ByVal pad() As Integer, ByVal bias As Double) As ConvolutionLayer
			Return (New ConvolutionLayer.Builder(New Integer() { 5, 5 }, stride, pad)).name(name).nOut([out]).biasInit(bias).build()
		End Function

		Private Shared Function maxPool(ByVal name As String, ByVal kernel() As Integer) As SubsamplingLayer
			Return (New SubsamplingLayer.Builder(kernel, New Integer() { 2, 2 })).name(name).build()
		End Function

		Private Shared Function fullyConnected(ByVal name As String, ByVal [out] As Integer, ByVal bias As Double, ByVal dist As Distribution) As DenseLayer
			Return (New DenseLayer.Builder()).name(name).nOut([out]).biasInit(bias).dist(dist).build()
		End Function
	End Class

End Namespace