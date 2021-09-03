Imports System
Imports System.Collections.Generic
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports ScoreListener = org.nd4j.autodiff.listeners.impl.ScoreListener
Imports History = org.nd4j.autodiff.listeners.records.History
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports IrisDataSetIterator = org.nd4j.linalg.dataset.IrisDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports SingletonDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonDataSetIterator
Imports SingletonMultiDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonMultiDataSetIterator
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports AMSGrad = org.nd4j.linalg.learning.config.AMSGrad
Imports AdaMax = org.nd4j.linalg.learning.config.AdaMax
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports XavierInitScheme = org.nd4j.weightinit.impl.XavierInitScheme

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

Namespace org.nd4j.autodiff.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.TRAINING) @Tag(TagNames.SAMEDIFF) public class SameDiffTrainingTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SameDiffTrainingTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void irisTrainingSanityCheck(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub irisTrainingSanityCheck(ByVal backend As Nd4jBackend)

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim std As New NormalizerStandardize()
			std.fit(iter)
			iter.PreProcessor = std

			For Each u As String In New String(){"adam", "nesterov"}
				Nd4j.Random.setSeed(12345)
				log.info("Starting: " & u)
				Dim sd As SameDiff = SameDiff.create()

				Dim [in] As SDVariable = sd.placeHolder("input", DataType.FLOAT, -1, 4)
				Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)

				Dim w0 As SDVariable = sd.var("w0", New XavierInitScheme("c"c, 4, 10), DataType.FLOAT, 4, 10)
				Dim b0 As SDVariable = sd.zero("b0", DataType.FLOAT, 1, 10)

				Dim w1 As SDVariable = sd.var("w1", New XavierInitScheme("c"c, 10, 3), DataType.FLOAT, 10, 3)
				Dim b1 As SDVariable = sd.zero("b1", DataType.FLOAT, 1, 3)

				Dim z0 As SDVariable = [in].mmul(w0).add(b0)
				Dim a0 As SDVariable = sd.math().tanh(z0)
				Dim z1 As SDVariable = a0.mmul(w1).add("prediction", b1)
				Dim a1 As SDVariable = sd.nn().softmax(z1,-1)

				Dim diff As SDVariable = sd.math().squaredDifference(a1, label)
				Dim lossMse As SDVariable = diff.mul(diff).mean()

				Dim updater As IUpdater
				Select Case u
					Case "sgd"
						updater = New Sgd(3e-1)
					Case "adam"
						updater = New Adam(1e-2)
					Case "nesterov"
						updater = New Nesterovs(1e-1)
					Case Else
						Throw New Exception()
				End Select

				Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(updater).dataSetFeatureMapping("input").dataSetLabelMapping("label").build()

				sd.TrainingConfig = conf

				sd.setListeners(New ScoreListener(1))

				sd.fit(iter, 50)

				Dim e As New Evaluation()
				Dim evalMap As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
				evalMap("prediction") = Collections.singletonList(e)

				sd.evaluateMultiple(iter, evalMap)

				Console.WriteLine(e.stats())

				Dim acc As Double = e.accuracy()
				assertTrue(acc >= 0.75,u & " - " & acc)
			Next u
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void irisTrainingEvalTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub irisTrainingEvalTest(ByVal backend As Nd4jBackend)

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim std As New NormalizerStandardize()
			std.fit(iter)
			iter.PreProcessor = std

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()

			Dim [in] As SDVariable = sd.placeHolder("input", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)

			Dim w0 As SDVariable = sd.var("w0", New XavierInitScheme("c"c, 4, 10), DataType.FLOAT, 4, 10)
			Dim b0 As SDVariable = sd.zero("b0", DataType.FLOAT, 1, 10)

			Dim w1 As SDVariable = sd.var("w1", New XavierInitScheme("c"c, 10, 3), DataType.FLOAT, 10, 3)
			Dim b1 As SDVariable = sd.zero("b1", DataType.FLOAT, 1, 3)

			Dim z0 As SDVariable = [in].mmul(w0).add(b0)
			Dim a0 As SDVariable = sd.math().tanh(z0)
			Dim z1 As SDVariable = a0.mmul(w1).add("prediction", b1)
			Dim a1 As SDVariable = sd.nn().softmax(z1)

			Dim diff As SDVariable = sd.math().squaredDifference(a1, label)
			Dim lossMse As SDVariable = diff.mul(diff).mean()

			Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(New Adam(1e-2)).dataSetFeatureMapping("input").dataSetLabelMapping("label").trainEvaluation("prediction", 0, New Evaluation()).build()

			sd.TrainingConfig = conf

			Dim hist As History = sd.fit().train(iter, 50).exec()

			Dim e As Evaluation = hist.finalTrainingEvaluations().evaluation("prediction")

			Console.WriteLine(e.stats())

			Dim acc As Double = e.accuracy()

			assertTrue(acc >= 0.75,"Accuracy bad: " & acc)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void irisTrainingValidationTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub irisTrainingValidationTest(ByVal backend As Nd4jBackend)

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim std As New NormalizerStandardize()
			std.fit(iter)
			iter.PreProcessor = std

			Dim valIter As DataSetIterator = New IrisDataSetIterator(30, 60)
			Dim valStd As New NormalizerStandardize()
			valStd.fit(valIter)
			valIter.PreProcessor = std

			Nd4j.Random.setSeed(12345)
			Dim sd As SameDiff = SameDiff.create()

			Dim [in] As SDVariable = sd.placeHolder("input", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)

			Dim w0 As SDVariable = sd.var("w0", New XavierInitScheme("c"c, 4, 10), DataType.FLOAT, 4, 10)
			Dim b0 As SDVariable = sd.zero("b0", DataType.FLOAT, 1, 10)

			Dim w1 As SDVariable = sd.var("w1", New XavierInitScheme("c"c, 10, 3), DataType.FLOAT, 10, 3)
			Dim b1 As SDVariable = sd.zero("b1", DataType.FLOAT, 1, 3)

			Dim z0 As SDVariable = [in].mmul(w0).add(b0)
			Dim a0 As SDVariable = sd.math().tanh(z0)
			Dim z1 As SDVariable = a0.mmul(w1).add("prediction", b1)
			Dim a1 As SDVariable = sd.nn().softmax(z1)

			Dim diff As SDVariable = sd.math().squaredDifference(a1, label)
			Dim lossMse As SDVariable = diff.mul(diff).mean()

			Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(New Adam(1e-2)).dataSetFeatureMapping("input").dataSetLabelMapping("label").validationEvaluation("prediction", 0, New Evaluation()).build()

			sd.TrainingConfig = conf

			Dim hist As History = sd.fit().train(iter, 50).validate(valIter, 5).exec()

			Dim e As Evaluation = hist.finalValidationEvaluations().evaluation("prediction")

			Console.WriteLine(e.stats())

			Dim acc As Double = e.accuracy()

			assertTrue(acc >= 0.75,"Accuracy bad: " & acc)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrainingMixedDtypes()
		Public Overridable Sub testTrainingMixedDtypes()

			For Each u As String In New String(){"adam", "nesterov", "adamax", "amsgrad"}

				Dim sd As SameDiff = SameDiff.create()
				Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)

				Dim inHalf As SDVariable = [in].castTo(DataType.HALF)
				Dim inDouble As SDVariable = [in].castTo(DataType.DOUBLE)

				Dim wFloat As SDVariable = sd.var("wFloat", Nd4j.rand(DataType.FLOAT, 4, 3))
				Dim wDouble As SDVariable = sd.var("wDouble", Nd4j.rand(DataType.DOUBLE, 4, 3))
				Dim wHalf As SDVariable = sd.var("wHalf", Nd4j.rand(DataType.HALF, 4, 3))

				Dim outFloat As SDVariable = [in].mmul(wFloat)
				Dim outDouble As SDVariable = inDouble.mmul(wDouble)
				Dim outHalf As SDVariable = inHalf.mmul(wHalf)

				Dim sum As SDVariable = outFloat.add(outDouble.castTo(DataType.FLOAT)).add(outHalf.castTo(DataType.FLOAT))

				Dim loss As SDVariable = sum.std(True)

				Dim updater As IUpdater
				Select Case u
					Case "sgd"
						updater = New Sgd(1e-2)
					Case "adam"
						updater = New Adam(1e-2)
					Case "nesterov"
						updater = New Nesterovs(1e-2)
					Case "adamax"
						updater = New AdaMax(1e-2)
					Case "amsgrad"
						updater = New AMSGrad(1e-2)
					Case Else
						Throw New Exception()
				End Select

				Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(updater).dataSetFeatureMapping("in").markLabelsUnused().build()

				sd.TrainingConfig = conf

				Dim ds As New DataSet(Nd4j.rand(DataType.FLOAT, 3, 4), Nothing)

				For i As Integer = 0 To 9
					sd.fit(ds)
				Next i
			Next u

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void simpleClassification(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub simpleClassification(ByVal backend As Nd4jBackend)
			Dim learning_rate As Double = 0.001
			Dim seed As Integer = 7
			Dim rng As org.nd4j.linalg.api.rng.Random = Nd4j.Random
			rng.setSeed(seed)
			Dim x1_label1 As INDArray = Nd4j.randn(3.0, 1.0, New Long(){1000}, rng)
			Dim x2_label1 As INDArray = Nd4j.randn(2.0, 1.0, New Long(){1000}, rng)
			Dim x1_label2 As INDArray = Nd4j.randn(7.0, 1.0, New Long(){1000}, rng)
			Dim x2_label2 As INDArray = Nd4j.randn(6.0, 1.0, New Long(){1000}, rng)

			Dim x1s As INDArray = Nd4j.concat(0, x1_label1, x1_label2)
			Dim x2s As INDArray = Nd4j.concat(0, x2_label1, x2_label2)

			Dim sd As SameDiff = SameDiff.create()
			Dim ys As INDArray = Nd4j.scalar(0.0).mul(x1_label1.length()).add(Nd4j.scalar(1.0).mul(x1_label2.length()))

			Dim X1 As SDVariable = sd.placeHolder("x1", DataType.DOUBLE, 2000)
			Dim X2 As SDVariable = sd.placeHolder("x2", DataType.DOUBLE, 2000)
			Dim y As SDVariable = sd.placeHolder("y", DataType.DOUBLE)
			Dim w As SDVariable = sd.var("w", DataType.DOUBLE, 3)

			' TF code:
			'cost = tf.reduce_mean(-tf.log(y_model * Y + (1 — y_model) * (1 — Y)))
			Dim y_model As SDVariable = sd.nn_Conflict.sigmoid(w.get(SDIndex.point(2)).mul(X2).add(w.get(SDIndex.point(1)).mul(X1)).add(w.get(SDIndex.point(0))))
			Dim cost_fun As SDVariable = (sd.math_Conflict.neg(sd.math_Conflict.log(y_model.mul(y).add((sd.math_Conflict.log(sd.constant(1.0).minus(y_model)).mul(sd.constant(1.0).minus(y)))))))
			Dim loss As SDVariable = sd.mean("loss", cost_fun)

			Dim updater As val = New Sgd(learning_rate)

			sd.setLossVariables("loss")
			sd.createGradFunction()
			Dim conf As val = (New TrainingConfig.Builder()).updater(updater).minimize("loss").dataSetFeatureMapping("x1", "x2", "y").markLabelsUnused().build()

			Dim mds As New MultiDataSet(New INDArray(){x1s, x2s, ys},Nothing)

			sd.TrainingConfig = conf
			Dim history As History = sd.fit(New SingletonMultiDataSetIterator(mds), 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrainingEvalVarNotReqForLoss()
		Public Overridable Sub testTrainingEvalVarNotReqForLoss()
			'If a variable is not required for the loss - normally it won't be calculated
			'But we want to make sure it IS calculated here - so we can perform evaluation on it

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim z As SDVariable = [in].mmul(w)
			Dim [out] As SDVariable = sd.nn_Conflict.softmax("softmax", z)
			Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss", label, [out])
			Dim notRequiredForLoss As SDVariable = sd.nn_Conflict.softmax("notRequiredForLoss", z)

			sd.TrainingConfig = TrainingConfig.builder().updater(New Adam(0.001)).dataSetFeatureMapping("in").dataSetLabelMapping("label").trainEvaluation("notRequiredForLoss", 0, New Evaluation()).build()

	'        sd.setListeners(new ScoreListener(1));

			Dim ds As New DataSet(Nd4j.rand(DataType.FLOAT, 3, 4), Nd4j.createFromArray(New Single()(){
				New Single() {1, 0, 0},
				New Single() {0, 1, 0},
				New Single() {0, 0, 1}
			}))

			Dim h As History = sd.fit().train(New SingletonDataSetIterator(ds), 4).exec()

			Dim l As IList(Of Double) = h.trainingEval(Evaluation.Metric.ACCURACY)
			assertEquals(4, l.Count)
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace