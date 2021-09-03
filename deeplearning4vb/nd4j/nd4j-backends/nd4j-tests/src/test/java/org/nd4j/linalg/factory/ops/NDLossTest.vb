Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.factory.ops

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SAMEDIFF) @NativeTag public class NDLossTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDLossTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAbsoluteDifference(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAbsoluteDifference(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)


			Dim loss As SDVariable = sd.loss().absoluteDifference("loss", labels, predictions, w, reduction)
			Dim loss2 As SDVariable = sd.loss().absoluteDifference("loss2", labels, predictions,Nothing, reduction)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().absoluteDifference(labelsArr, predictionsArr, wArr, reduction)
			Dim y2 As INDArray = Nd4j.loss().absoluteDifference(labelsArr, predictionsArr, Nothing, reduction)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCosineDistance(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCosineDistance(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			predictionsArr.diviColumnVector(predictionsArr.norm2(1))
			labelsArr.diviColumnVector(labelsArr.norm2(1))

			Dim loss As SDVariable = sd.loss().cosineDistance("loss", labels, predictions, w, reduction, 0)
			Dim loss2 As SDVariable = sd.loss().cosineDistance("loss2", labels, predictions, Nothing, reduction, 0)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().cosineDistance(labelsArr, predictionsArr, wArr, reduction, 0)
			Dim y2 As INDArray = Nd4j.loss().cosineDistance(labelsArr, predictionsArr, Nothing, reduction, 0)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHingeLoss(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHingeLoss(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().hingeLoss("loss", labels, predictions, w, reduction)
			Dim loss2 As SDVariable = sd.loss().hingeLoss("loss2", labels, predictions, Nothing, reduction)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().hingeLoss(labelsArr, predictionsArr, wArr, reduction)
			Dim y2 As INDArray = Nd4j.loss().hingeLoss(labelsArr, predictionsArr, Nothing, reduction)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testHuberLoss(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testHuberLoss(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().huberLoss("loss", labels, predictions, w, reduction, 0.02)
			Dim loss2 As SDVariable = sd.loss().huberLoss("loss2", labels, predictions, Nothing, reduction, 0.02)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().huberLoss(labelsArr, predictionsArr, wArr, reduction, 0.02)
			Dim y2 As INDArray = Nd4j.loss().huberLoss(labelsArr, predictionsArr, Nothing, reduction, 0.02)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testL2Loss(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testL2Loss(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().l2Loss("loss", predictions)
			sd.associateArrayWithVariable(predictionsArr, predictions)

			Dim y_exp As INDArray = loss.eval()

			Dim y As INDArray = Nd4j.loss().l2Loss(predictionsArr)
			assertEquals(y_exp, y)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogLoss(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogLoss(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Nd4j.Executioner.exec(New BernoulliDistribution(labelsArr, 0.5))
			predictionsArr = Nd4j.rand(predictionsArr.shape()).muli(0.8).addi(0.1)

			Dim eps As Double = 1e-7

			Dim loss As SDVariable = sd.loss().logLoss("loss", labels, predictions, w, reduction, eps)
			Dim loss2 As SDVariable = sd.loss().logLoss("loss2", labels, predictions, Nothing, reduction, eps)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			'TODO: Test fails.   "Op [log_loss] execution failed"
			Dim y As INDArray = Nd4j.loss().logLoss(labelsArr, predictionsArr, wArr, reduction, eps)
			Dim y2 As INDArray = Nd4j.loss().logLoss(labelsArr, predictionsArr, Nothing, reduction, eps)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLogPoisson(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLogPoisson(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().logPoisson("loss", labels, predictions, w, reduction, False)
			Dim loss2 As SDVariable = sd.loss().logPoisson("loss2", labels, predictions, Nothing, reduction, False)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().logPoisson(labelsArr, predictionsArr, wArr, reduction, False)
			Dim y2 As INDArray = Nd4j.loss().logPoisson(labelsArr, predictionsArr, Nothing, reduction, False)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanPairwiseSquaredError(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanPairwiseSquaredError(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().meanPairwiseSquaredError("loss", labels, predictions, w, reduction)
			Dim loss2 As SDVariable = sd.loss().meanPairwiseSquaredError("loss2", labels, predictions, Nothing, reduction)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().meanPairwiseSquaredError(labelsArr, predictionsArr, wArr, reduction)
			Dim y2 As INDArray = Nd4j.loss().meanPairwiseSquaredError(labelsArr, predictionsArr, Nothing, reduction)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMeanSquaredError(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMeanSquaredError(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)

			Dim loss As SDVariable = sd.loss().meanSquaredError("loss", labels, predictions, w, reduction)
			Dim loss2 As SDVariable = sd.loss().meanSquaredError("loss2", labels, predictions, Nothing, reduction)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().meanSquaredError(labelsArr, predictionsArr, wArr, reduction)
			Dim y2 As INDArray = Nd4j.loss().meanSquaredError(labelsArr, predictionsArr, Nothing, reduction)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSigmoidCrossEntropy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSigmoidCrossEntropy(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.create(New Double()(){
				New Double() {0, 0, 0, 0},
				New Double() {0, 0, 1, 1},
				New Double() {1, 1, 0, 0},
				New Double() {1, 1, 1, 1},
				New Double() {1, 1, 1, 1},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2},
				New Double() {2, 2, 2, 2}
			})
			Dim w As SDVariable = sd.var("weights", wArr)

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelSmoothing As Double = 0.01

			Dim loss As SDVariable = sd.loss().sigmoidCrossEntropy("loss", labels, predictions, w, reduction, labelSmoothing)
			Dim loss2 As SDVariable = sd.loss().sigmoidCrossEntropy("loss2", labels, predictions, Nothing, reduction, labelSmoothing)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().sigmoidCrossEntropy(labelsArr, predictionsArr, wArr, reduction, labelSmoothing)
			Dim y2 As INDArray = Nd4j.loss().sigmoidCrossEntropy(labelsArr, predictionsArr, Nothing, reduction, labelSmoothing)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSoftmaxCrossEntropy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSoftmaxCrossEntropy(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.DOUBLE, -1, nOut)

			Dim wArr As INDArray = Nd4j.scalar(1.0) 'TODO: This test fails with a complex weights array.
			Dim w As SDVariable = Nothing

			Dim reduction As LossReduce = LossReduce.MEAN_BY_NONZERO_WEIGHT_COUNT

			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			labelsArr.assign(0)
			Dim i As Integer = 0
			Do While i < labelsArr.size(0)
				labelsArr.putScalar(i, i Mod labelsArr.size(1), 1.0)
				i += 1
			Loop

			Dim labelSmoothing As Double = 0.0

			Dim loss As SDVariable = sd.loss().softmaxCrossEntropy("loss", labels, predictions, Nothing, reduction, labelSmoothing)
			Dim loss2 As SDVariable = sd.loss().softmaxCrossEntropy("loss2", labels, predictions, Nothing, reduction, labelSmoothing)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()
			Dim y_exp2 As INDArray = loss2.eval()

			Dim y As INDArray = Nd4j.loss().softmaxCrossEntropy(labelsArr, predictionsArr, wArr, reduction, labelSmoothing)
			Dim y2 As INDArray = Nd4j.loss().softmaxCrossEntropy(labelsArr, predictionsArr, Nothing, reduction, labelSmoothing)
			assertEquals(y_exp, y)
			assertEquals(y_exp2, y2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSparseSoftmaxCrossEntropy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSparseSoftmaxCrossEntropy(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()

			Dim nOut As Integer = 4
			Dim minibatch As Integer = 10
			Dim predictions As SDVariable = sd.var("in", DataType.DOUBLE, minibatch, nOut)
			Dim labels As SDVariable = sd.var("labels", DataType.INT32, -1)


			Dim predictionsArr As INDArray = Nd4j.randn(DataType.DOUBLE, minibatch, nOut)
			Dim labelsArr As INDArray = Nd4j.create(DataType.INT32, minibatch)
			For i As Integer = 0 To minibatch - 1
				labelsArr.putScalar(i, i Mod nOut)
			Next i

			Dim loss As SDVariable = sd.loss().sparseSoftmaxCrossEntropy("loss", predictions, labels)
			sd.associateArrayWithVariable(predictionsArr, predictions)
			sd.associateArrayWithVariable(labelsArr, labels)

			Dim y_exp As INDArray = loss.eval()

			Dim y As INDArray = Nd4j.loss().sparseSoftmaxCrossEntropy(predictionsArr, labelsArr)
			assertEquals(y_exp, y)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWeightedCrossEntropyWithLogits(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWeightedCrossEntropyWithLogits(ByVal backend As Nd4jBackend)
			' This one from SamediffTests.java
			Dim sameDiff As SameDiff = SameDiff.create()
			Dim targets As INDArray = Nd4j.create(New Long(){1, 5})
			Dim inputs As INDArray = Nd4j.create(New Long(){1, 5})
			Dim weights As INDArray = Nd4j.create(New Long(){1, 5})

			Dim sdInputs As SDVariable = sameDiff.var("inputs", inputs)
			Dim sdWeights As SDVariable = sameDiff.var("weights", weights)
			Dim sdTargets As SDVariable = sameDiff.var("targets", targets)

			Dim res As SDVariable = sameDiff.loss().weightedCrossEntropyWithLogits(sdTargets, sdInputs, sdWeights)

			Dim resultArray As INDArray = res.eval()
			assertArrayEquals(New Long(){1, 5}, resultArray.shape())

			' Make sure the INDArray interface produces the same result.
			Dim y As INDArray = Nd4j.loss().weightedCrossEntropyWithLogits(targets, inputs, weights)
			assertEquals(resultArray, y)
		End Sub
	End Class

End Namespace