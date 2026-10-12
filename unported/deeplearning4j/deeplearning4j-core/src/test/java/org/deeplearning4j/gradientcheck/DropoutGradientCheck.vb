Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports org.deeplearning4j.nn.conf.dropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @NativeTag public class DropoutGradientCheck extends org.deeplearning4j.BaseDL4JTest
	Public Class DropoutGradientCheck
		Inherits BaseDL4JTest

		Private Const PRINT_RESULTS As Boolean = True
		Private Const RETURN_ON_FIRST_FAILURE As Boolean = False
		Private Const DEFAULT_EPS As Double = 1e-6
		Private Const DEFAULT_MAX_REL_ERROR As Double = 1e-3
		Private Const DEFAULT_MIN_ABS_ERROR As Double = 1e-8

		Shared Sub New()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropoutGradient()
		Public Overridable Sub testDropoutGradient()
			Dim minibatch As Integer = 3

			For Each cnn As Boolean In New Boolean(){False, True}
				For i As Integer = 0 To 4

'JAVA TO VB CONVERTER NOTE: The variable dropout was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
					Dim dropout_Conflict As IDropout
					Select Case i
						Case 0
							dropout_Conflict = New Dropout(0.6)
						Case 1
							dropout_Conflict = New AlphaDropout(0.6)
						Case 2
							dropout_Conflict = New GaussianDropout(0.1) '0.01 rate -> stdev 0.1; 0.1 rate -> stdev 0.333
						Case 3
							dropout_Conflict = New GaussianNoise(0.3)
						Case 4
							dropout_Conflict = New SpatialDropout(0.6)
						Case Else
							Throw New Exception()
					End Select

					If Not cnn AndAlso i = 4 Then
						'Skip spatial dropout for dense layer (not applicable)
						Continue For
					End If

					Dim builder As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0,1)).convolutionMode(ConvolutionMode.Same).dropOut(dropout_Conflict).activation(Activation.TANH).updater(New NoOp()).list()

					If cnn Then
						builder.layer((New ConvolutionLayer.Builder()).kernelSize(3,3).stride(2,2).nOut(2).build())
						builder.layer((New ConvolutionLayer.Builder()).kernelSize(3,3).stride(2,2).nOut(2).build())
						builder.InputType = InputType.convolutional(6,6,2)
					Else
						builder.layer((New DenseLayer.Builder()).nOut(3).build())
						builder.layer((New DenseLayer.Builder()).nOut(3).build())
						builder.InputType = InputType.feedForward(6)
					End If
					builder.layer((New OutputLayer.Builder()).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunction.MCXENT).build())

					Dim conf As MultiLayerConfiguration = builder.build()
					'Remove spatial dropout from output layer - can't be used for 2d input
					If i = 4 Then
					   conf.getConf(2).getLayer().setIDropout(Nothing)
					End If

					Dim mln As New MultiLayerNetwork(conf)
					mln.init()

					Dim msg As String = (If(cnn, "CNN", "Dense")) & ": " & dropout_Conflict.GetType().Name

					Dim f As INDArray
					If cnn Then
						f = Nd4j.rand(New Integer(){minibatch, 2, 6, 6}).muli(10).subi(5)
					Else
						f = Nd4j.rand(minibatch, 6).muli(10).subi(5)
					End If
					Dim l As INDArray = TestUtils.randomOneHot(minibatch, 3)

					log.info("*** Starting test: " & msg & " ***")
					Dim gradOK As Boolean = GradientCheckUtil.checkGradients(mln, DEFAULT_EPS, DEFAULT_MAX_REL_ERROR, DEFAULT_MIN_ABS_ERROR, PRINT_RESULTS, RETURN_ON_FIRST_FAILURE, f, l, Nothing, Nothing, False, -1, Nothing, 12345) 'Last arg: ensures RNG is reset at each iter... otherwise will fail due to randomness!

					assertTrue(gradOK, msg)
					TestUtils.testModelSerialization(mln)
				Next i
			Next cnn
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCompGraphMultiInput()
		Public Overridable Sub testCompGraphMultiInput()
			'Validate nets where the one output array is used as the input to multiple layers...
			Nd4j.Random.setSeed(12345)
			Dim mb As Integer = 3

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).dist(New NormalDistribution(0,1)).convolutionMode(ConvolutionMode.Same).dropOut(New GaussianDropout(0.1)).activation(Activation.TANH).updater(New NoOp()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "0").addLayer("2", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "0").addLayer("3", (New DenseLayer.Builder()).nIn(5).nOut(5).build(), "0").addLayer("out", (New OutputLayer.Builder()).nIn(15).nOut(5).activation(Activation.SOFTMAX).lossFunction(LossFunction.MCXENT).build(), "1", "2", "3").setOutputs("out").build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			Dim [in]() As INDArray = {Nd4j.rand(mb, 5)}
			Dim l() As INDArray = {TestUtils.randomOneHot(mb, 5)}

			Dim gradOK As Boolean = GradientCheckUtil.checkGradients((New GradientCheckUtil.GraphConfig()).net(cg).inputs([in]).labels(l).callEachIter(New ConsumerAnonymousInnerClass(Me)))

			assertTrue(gradOK)
		End Sub

		Private Class ConsumerAnonymousInnerClass
			Implements Consumer(Of ComputationGraph)

			Private ReadOnly outerInstance As DropoutGradientCheck

			Public Sub New(ByVal outerInstance As DropoutGradientCheck)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub accept(ByVal net As ComputationGraph)
				Nd4j.Random.setSeed(12345)
			End Sub
		End Class

	End Class

End Namespace