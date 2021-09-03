Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LocalResponseNormalization = org.deeplearning4j.nn.conf.layers.LocalResponseNormalization
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
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
Namespace org.deeplearning4j.nn.layers.normalization

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Local Response Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LocalResponseTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LocalResponseTest
		Inherits BaseDL4JTest

		Private x As INDArray = Nd4j.create(New Double() { 0.88128096, -0.96666986, -0.61832994, 0.26418415, 0.05694608, 0.2950289, 0.99222249, 0.24541704, 0.4219842, 0.96430975, 0.19299535, -0.06658337, -0.27603117, 0.24216647, 0.21834095, 0.03863283, -0.82313406, -0.37236378, -0.77667993, 0.66295379, -0.34406275, -0.25924176, 0.26652309, -0.58964926, -0.46907067, 0.34666502, 0.81208313, -0.17042427, -0.22470538, 0.8348338, 0.50494033, 0.45004508, 0.58735144, -0.87217808, -0.74788797, -0.04363599, 0.72276866, 0.52476895, -0.52383977, 0.1311436, 0.2628099, 0.77274454, 0.86400729, -0.35246921, -0.03399619, -0.502312, 0.42834607, 0.85534132, 0.90083021, 0.24571614, 0.63058525, -0.82919437, 0.57236177, -0.0913529, -0.7102778, 0.81631756, -0.89004314, 0.43995622, -0.26112801, -0.76135367, 0.65180862, -0.54667377, 0.94908774, 0.59298772, 0.36457643, 0.58892179, -0.52951556, 0.31559938, -0.55268252, 0.8272332, 0.37911707, -0.96299696, -0.40717798, 0.43324658, 0.2589654, -0.15605508, 0.96334064, -0.31666604, 0.19781154, 0.09908111, 0.64796048, -0.99037546, 0.67919868, 0.43810204 }, New Integer() { 2, 7, 3, 2 })

		Private activationsExpected As INDArray = Nd4j.create(New Double() { 0.52397668, -0.57476264, -0.3676528, 0.15707894, 0.03385943, 0.17542371, 0.58992499, 0.14591768, 0.25090647, 0.57335907, 0.11475233, -0.03958985, -0.16411273, 0.14398433, 0.12981956, 0.02297027, -0.48942304, -0.22139823, -0.46177959, 0.39418164, -0.20457059, -0.15413573, 0.15846729, -0.3505919, -0.27889356, 0.20611978, 0.48284137, -0.10133155, -0.13360347, 0.49636194, 0.30022132, 0.26758799, 0.34922296, -0.51858318, -0.4446843, -0.02594452, 0.42974478, 0.31202248, -0.31146204, 0.07797609, 0.15626372, 0.4594543, 0.51370209, -0.20957276, -0.02021335, -0.29866382, 0.25469059, 0.50856382, 0.53558689, 0.14609739, 0.37491882, -0.49301448, 0.34031925, -0.05431537, -0.42228988, 0.48536259, -0.52917528, 0.26157826, -0.15526266, -0.45265958, 0.38753596, -0.32503816, 0.56427884, 0.35256693, 0.21676543, 0.35014921, -0.31483513, 0.18764766, -0.32859638, 0.49183461, 0.22540972, -0.57255536, -0.24210122, 0.25760418, 0.15397197, -0.0927838, 0.57277, -0.18827969, 0.1176173, 0.05891332, 0.38526815, -0.58884346, 0.40383074, 0.26048511 }, New Integer() { 2, 7, 3, 2 })

		Private epsilon As INDArray = Nd4j.create(New Double() { -0.13515499, 0.96470547, -0.62253004, 0.80172491, -0.97510445, -0.41198033, -0.4790071, 0.07551047, -0.01383764, -0.05797465, 0.21242172, 0.7145375, -0.17809176, -0.11465316, -0.2066526, 0.21950938, 0.4627091, 0.30275798, 0.61443841, 0.75912178, -0.132248, -0.82923287, 0.74962652, -0.88993639, 0.04406403, 0.32096064, -0.46400586, 0.1603231, 0.63007826, 0.10626783, 0.08009516, 0.88297033, 0.11441587, 0.35862735, 0.40441504, -0.60132015, 0.87743825, 0.09792926, 0.92742652, 0.6182847, -0.9602651, -0.19611064, 0.15762019, 0.00339905, -0.9238292, 0.02451134, -0.44294646, -0.5450229, 0.87502575, -0.59481794, 0.65259099, -0.77772689, 0.53300053, 0.11541174, 0.32667685, 0.99437004, -0.04084824, -0.45166185, 0.29513556, 0.53582036, 0.95541358, -0.75714606, -0.63295805, -0.70315111, -0.6553846, -0.78824568, 0.84295344, -0.38352135, -0.04541624, 0.17396702, 0.41530582, 0.11870354, 0.85787249, -0.94597596, 0.05792254, 0.04811822, 0.04847952, -0.82953823, 0.8089835, 0.50185651, -0.88619858, -0.78598201, 0.27489874, 0.63673472 }, New Integer() { 2, 7, 3, 2 })

		Private newEpsilonExpected As INDArray = Nd4j.create(New Double() { -0.08033668, 0.57355404, -0.37014094, 0.47668865, -0.57978398, -0.24495915, -0.28474802, 0.04490108, -0.00823483, -0.03448687, 0.12630466, 0.42485803, -0.10589627, -0.06816553, -0.12287001, 0.13051508, 0.27510744, 0.18001786, 0.36528736, 0.45133191, -0.07863599, -0.49303374, 0.44571424, -0.52912313, 0.02620371, 0.19082049, -0.27585581, 0.09532529, 0.3746179, 0.06316902, 0.04761803, 0.52497554, 0.06804816, 0.21323238, 0.24044329, -0.35752413, 0.52168733, 0.05821467, 0.55140609, 0.3676247, -0.57095432, -0.11660115, 0.09367896, 0.00202246, -0.54928631, 0.01455687, -0.26336867, -0.3240425, 0.52023786, -0.35366109, 0.3879728, -0.46243483, 0.31692421, 0.06862034, 0.19421607, 0.59124804, -0.0242459, -0.26852599, 0.17547797, 0.31857637, 0.56804365, -0.45020312, -0.37634474, -0.41804832, -0.38966343, -0.4686695, 0.50119156, -0.22802454, -0.02698562, 0.10343311, 0.24693431, 0.0706142, 0.5100745, -0.56245267, 0.03443092, 0.02860913, 0.02883426, -0.49320197, 0.4810102, 0.29840365, -0.5269345, -0.46732581, 0.16344811, 0.37857518 }, New Integer() { 2, 7, 3, 2 })

		Private activationsActual As INDArray

		Private layer As Layer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void doBefore()
		Friend Overridable Sub doBefore()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).seed(123).layer((New LocalResponseNormalization.Builder()).k(2).n(5).alpha(1e-4).beta(0.75).build()).build()
			layer = (New LocalResponseNormalization()).instantiate(conf, Nothing, 0, Nothing, False, Nd4j.defaultFloatingPointType())
			activationsActual = layer.activate(x, False, LayerWorkspaceMgr.noWorkspaces())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activate") void testActivate()
		Friend Overridable Sub testActivate()
			' Precision is off from the expected results because expected results generated in numpy
			assertEquals(activationsExpected, activationsActual)
			assertArrayEquals(activationsExpected.shape(), activationsActual.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Backprop Gradient") void testBackpropGradient()
		Friend Overridable Sub testBackpropGradient()
			Dim containedOutput As Pair(Of Gradient, INDArray) = layer.backpropGradient(epsilon, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(newEpsilonExpected.getDouble(8), containedOutput.Second.getDouble(8), 1e-4)
			assertEquals(newEpsilonExpected.getDouble(20), containedOutput.Second.getDouble(20), 1e-4)
			assertEquals(Nothing, containedOutput.First.getGradientFor("W"))
			assertArrayEquals(newEpsilonExpected.shape(), containedOutput.Second.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regularization") void testRegularization()
		Friend Overridable Sub testRegularization()
			' Confirm a structure with regularization true will not throw an error
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).gradientNormalization(GradientNormalization.RenormalizeL2PerLayer).l1(0.2).l2(0.1).seed(123).layer((New LocalResponseNormalization.Builder()).k(2).n(5).alpha(1e-4).beta(0.75).build()).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi CNN Layer") void testMultiCNNLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiCNNLayer()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.LINE_GRADIENT_DESCENT).seed(123).list().layer(0, (New ConvolutionLayer.Builder()).nIn(1).nOut(6).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New LocalResponseNormalization.Builder()).build()).layer(2, (New DenseLayer.Builder()).nOut(2).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(2).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			network.fit([next])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lrn Manual") void testLrnManual()
		Friend Overridable Sub testLrnManual()
			Dim wh As Integer = 5
			Dim depth As Integer = 6
			Dim minibatch As Integer = 3
			Dim n As Integer = 4
			Dim k As Double = 2.0
			Dim alpha As Double = 1e-4
			Dim beta As Double = 0.75
			Dim [in] As INDArray = Nd4j.rand(New Integer() { minibatch, depth, wh, wh })
			Dim outExp As INDArray = Nd4j.zeros(minibatch, depth, wh, wh)
			For m As Integer = 0 To minibatch - 1
				For x As Integer = 0 To wh - 1
					For y As Integer = 0 To wh - 1
						For i As Integer = 0 To depth - 1
							Dim jFrom As Integer = Math.Max(0, i - n \ 2)
							Dim jTo As Integer = Math.Min(depth - 1, i + n \ 2)
							Dim sum As Double = 0.0
							For j As Integer = jFrom To jTo
								Dim d As Double = [in].getDouble(m, j, x, y)
								sum += d * d
							Next j
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
							Dim [out] As Double = [in].getDouble(m, i, x, y) / Math.Pow(k + alpha * sum, beta)
							outExp.putScalar(m, i, x, y, [out])
						Next i
					Next y
				Next x
			Next m
			Dim lrn As LocalResponseNormalization = (New LocalResponseNormalization.Builder()).build()
			Dim nnc As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).layer(lrn).build()
			Dim layer As org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization = DirectCast(lrn.instantiate(nnc, Nothing, 0, Nothing, False, Nd4j.defaultFloatingPointType()), org.deeplearning4j.nn.layers.normalization.LocalResponseNormalization)
			Dim outAct As INDArray = layer.activate([in], True, LayerWorkspaceMgr.noWorkspaces())
			assertEquals(outExp, outAct)
		End Sub
	End Class

End Namespace