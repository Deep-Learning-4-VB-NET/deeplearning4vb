Imports System
Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports Convolution = org.nd4j.linalg.convolution.Convolution
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.conf.layers

	''' <summary>
	''' @author Jeffrey Tang.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Layer Builder Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LayerBuilderTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LayerBuilderTest
		Inherits BaseDL4JTest

		Friend ReadOnly DELTA As Double = 1e-15

		Friend numIn As Integer = 10

		Friend numOut As Integer = 5

		Friend drop As Double = 0.3

		Friend act As IActivation = New ActivationSoftmax()

		Friend poolType As PoolingType = PoolingType.MAX

		Friend kernelSize() As Integer = { 2, 2 }

		Friend stride() As Integer = { 2, 2 }

		Friend padding() As Integer = { 1, 1 }

		Friend k As Integer = 1

		Friend convType As Convolution.Type = Convolution.Type.VALID

		Friend loss As LossFunction = LossFunction.MCXENT

		Friend weight As WeightInit = WeightInit.XAVIER

		Friend corrupt As Double = 0.4

		Friend sparsity As Double = 0.3

		Friend corruptionLevel As Double = 0.5

		Friend dropOut As Double = 0.1

		Friend updater As IUpdater = New AdaGrad()

		Friend gradNorm As GradientNormalization = GradientNormalization.ClipL2PerParamType

		Friend gradNormThreshold As Double = 8

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer") void testLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLayer()
			Dim layer As DenseLayer = (New DenseLayer.Builder()).activation(act).weightInit(weight).dropOut(dropOut).updater(updater).gradientNormalization(gradNorm).gradientNormalizationThreshold(gradNormThreshold).build()
			checkSerialization(layer)
			assertEquals(act, layer.getActivationFn())
			assertEquals(weight.getWeightInitFunction(), layer.getWeightInitFn())
			assertEquals(New Dropout(dropOut), layer.getIDropout())
			assertEquals(updater, layer.getIUpdater())
			assertEquals(gradNorm, layer.GradientNormalization)
			assertEquals(gradNormThreshold, layer.GradientNormalizationThreshold, 0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feed Forward Layer") void testFeedForwardLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testFeedForwardLayer()
			Dim ff As DenseLayer = (New DenseLayer.Builder()).nIn(numIn).nOut(numOut).build()
			checkSerialization(ff)
			assertEquals(numIn, ff.getNIn())
			assertEquals(numOut, ff.getNOut())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Convolution Layer") void testConvolutionLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConvolutionLayer()
			Dim conv As ConvolutionLayer = (New ConvolutionLayer.Builder(kernelSize, stride, padding)).build()
			checkSerialization(conv)
			' assertEquals(convType, conv.getConvolutionType());
			assertArrayEquals(kernelSize, conv.getKernelSize())
			assertArrayEquals(stride, conv.getStride())
			assertArrayEquals(padding, conv.getPadding())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Subsampling Layer") void testSubsamplingLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSubsamplingLayer()
			Dim sample As SubsamplingLayer = (New SubsamplingLayer.Builder(poolType, stride)).kernelSize(kernelSize).padding(padding).build()
			checkSerialization(sample)
			assertArrayEquals(padding, sample.getPadding())
			assertArrayEquals(kernelSize, sample.getKernelSize())
			assertEquals(poolType, sample.getPoolingType())
			assertArrayEquals(stride, sample.getStride())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Output Layer") void testOutputLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testOutputLayer()
			Dim [out] As OutputLayer = (New OutputLayer.Builder(loss)).build()
			checkSerialization([out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Rnn Output Layer") void testRnnOutputLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRnnOutputLayer()
			Dim [out] As RnnOutputLayer = (New RnnOutputLayer.Builder(loss)).build()
			checkSerialization([out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Auto Encoder") void testAutoEncoder() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testAutoEncoder()
			Dim enc As AutoEncoder = (New AutoEncoder.Builder()).corruptionLevel(corruptionLevel).sparsity(sparsity).build()
			checkSerialization(enc)
			assertEquals(corruptionLevel, enc.getCorruptionLevel(), DELTA)
			assertEquals(sparsity, enc.getSparsity(), DELTA)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Graves LSTM") void testGravesLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGravesLSTM()
			Dim glstm As GravesLSTM = (New GravesLSTM.Builder()).forgetGateBiasInit(1.5).activation(Activation.TANH).nIn(numIn).nOut(numOut).build()
			checkSerialization(glstm)
			assertEquals(glstm.getForgetGateBiasInit(), 1.5, 0.0)
			assertEquals(glstm.nIn, numIn)
			assertEquals(glstm.nOut, numOut)
			assertTrue(TypeOf glstm.getActivationFn() Is ActivationTanH)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Graves Bidirectional LSTM") void testGravesBidirectionalLSTM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGravesBidirectionalLSTM()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final GravesBidirectionalLSTM glstm = new GravesBidirectionalLSTM.Builder().forgetGateBiasInit(1.5).activation(org.nd4j.linalg.activations.Activation.TANH).nIn(numIn).nOut(numOut).build();
			Dim glstm As GravesBidirectionalLSTM = (New GravesBidirectionalLSTM.Builder()).forgetGateBiasInit(1.5).activation(Activation.TANH).nIn(numIn).nOut(numOut).build()
			checkSerialization(glstm)
			assertEquals(1.5, glstm.getForgetGateBiasInit(), 0.0)
			assertEquals(glstm.nIn, numIn)
			assertEquals(glstm.nOut, numOut)
			assertTrue(TypeOf glstm.getActivationFn() Is ActivationTanH)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer") void testEmbeddingLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEmbeddingLayer()
			Dim el As EmbeddingLayer = (New EmbeddingLayer.Builder()).nIn(10).nOut(5).build()
			checkSerialization(el)
			assertEquals(10, el.getNIn())
			assertEquals(5, el.getNOut())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Batch Norm Layer") void testBatchNormLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBatchNormLayer()
			Dim bN As BatchNormalization = (New BatchNormalization.Builder()).nIn(numIn).nOut(numOut).gamma(2).beta(1).decay(0.5).lockGammaBeta(True).build()
			checkSerialization(bN)
			assertEquals(numIn, bN.nIn)
			assertEquals(numOut, bN.nOut)
			assertEquals(True, bN.isLockGammaBeta())
			assertEquals(0.5, bN.decay, 1e-4)
			assertEquals(2, bN.gamma, 1e-4)
			assertEquals(1, bN.beta, 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activation Layer") void testActivationLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testActivationLayer()
			Dim activationLayer As ActivationLayer = (New ActivationLayer.Builder()).activation(act).build()
			checkSerialization(activationLayer)
			assertEquals(act, activationLayer.activationFn)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void checkSerialization(Layer layer) throws Exception
		Private Sub checkSerialization(ByVal layer As Layer)
			Dim confExpected As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(layer).build()
			Dim confActual As NeuralNetConfiguration
			' check Java serialization
			Dim data() As SByte
			Using bos As New MemoryStream(), [out] As ObjectOutput = New ObjectOutputStream(bos)
				[out].writeObject(confExpected)
				data = bos.toByteArray()
			End Using
			Using bis As New MemoryStream(data), [in] As ObjectInput = New ObjectInputStream(bis)
				confActual = CType([in].readObject(), NeuralNetConfiguration)
			End Using
			assertEquals(confExpected.getLayer(), confActual.getLayer(), "unequal Java serialization")
			' check JSON
			Dim json As String = confExpected.toJson()
			confActual = NeuralNetConfiguration.fromJson(json)
			assertEquals(confExpected.getLayer(), confActual.getLayer(), "unequal JSON serialization")
			' check YAML
			Dim yaml As String = confExpected.toYaml()
			confActual = NeuralNetConfiguration.fromYaml(yaml)
			assertEquals(confExpected.getLayer(), confActual.getLayer(), "unequal YAML serialization")
			' check the layer's use of callSuper on equals method
			confActual.getLayer().setIDropout(New Dropout((New Random()).NextDouble()))
			assertNotEquals(confExpected.getLayer(), confActual.getLayer(), "broken equals method (missing callSuper?)")
		End Sub
	End Class

End Namespace