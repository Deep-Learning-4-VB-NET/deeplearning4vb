Imports System
Imports System.Collections.Generic
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports ListenerResponse = org.nd4j.autodiff.listeners.ListenerResponse
Imports ListenerVariables = org.nd4j.autodiff.listeners.ListenerVariables
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports ScoreListener = org.nd4j.autodiff.listeners.impl.ScoreListener
Imports History = org.nd4j.autodiff.listeners.records.History
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports Metric = org.nd4j.evaluation.classification.Evaluation.Metric
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports IrisDataSetIterator = org.nd4j.linalg.dataset.IrisDataSetIterator
Imports SingletonDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports XavierInitScheme = org.nd4j.weightinit.impl.XavierInitScheme
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.autodiff.samediff.listeners


	Public Class ListenerTest
		Inherits BaseNd4jTestWithBackends


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void irisHistoryTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub irisHistoryTest(ByVal backend As Nd4jBackend)

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
			Dim a0 As SDVariable = sd.nn().relu(z0, 0)
			Dim z1 As SDVariable = a0.mmul(w1).add(b1)
			Dim predictions As SDVariable = sd.nn().softmax("predictions", z1, 1)

			Dim loss As SDVariable = sd.loss_Conflict.softmaxCrossEntropy("loss", label, predictions, Nothing)

			sd.setLossVariables("loss")

			Dim updater As IUpdater = New Adam(1e-2)

			Dim e As New Evaluation()

			Dim conf As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(updater).dataSetFeatureMapping("input").dataSetLabelMapping("label").trainEvaluation(predictions, 0, e).build()

			sd.TrainingConfig = conf

			sd.setListeners(New ScoreListener(1))

			Dim hist As History = sd.fit(iter, 50)
	'        Map<String, List<IEvaluation>> evalMap = new HashMap<>();
	'        evalMap.put("prediction", Collections.singletonList(e));
	'
	'        sd.evaluateMultiple(iter, evalMap);

			e = hist.finalTrainingEvaluations().evaluation(predictions)

			Console.WriteLine(e.stats())

			Dim losses() As Single = hist.lossCurve().meanLoss(loss)

			Console.WriteLine("Losses: " & Arrays.toString(losses))

			Dim acc As Double = hist.finalTrainingEvaluations().getValue(Evaluation.Metric.ACCURACY)
			assertTrue(acc >= 0.75,"Accuracy < 75%, was " & acc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testListenerCalls()
		Public Overridable Sub testListenerCalls()
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 3))
			Dim z As SDVariable = [in].mmul(w).add(b)
			Dim softmax As SDVariable = sd.nn_Conflict.softmax("softmax", z)
			Dim loss As SDVariable = sd.loss_Conflict.logLoss("loss",label, softmax)

			Dim tl As New TestListener(Operation.INFERENCE)
			sd.setListeners(tl)

			'Check listener called during inference
			Dim phMap As IDictionary(Of String, INDArray) = Collections.singletonMap("in", Nd4j.rand(1, 4))

			For i As Integer = 1 To 5
				Dim [out] As INDArray = sd.outputSingle(phMap, "softmax")

				assertEquals(0, tl.epochStartCount)
				assertEquals(0, tl.epochEndCount)
				assertEquals(0, tl.validationDoneCount)
				assertEquals(0, tl.iterationStartCount)
				assertEquals(0, tl.iterationDoneCount)
				assertEquals(Collections.singletonMap(Operation.INFERENCE, i), tl.operationStartCount)
				assertEquals(Collections.singletonMap(Operation.INFERENCE, i), tl.operationEndCount)
				assertEquals(3*i, tl.preOpExecutionCount) 'mmul, add, softmax
				assertEquals(3*i, tl.opExecutionCount)
				assertEquals(3*i, tl.activationAvailableCount) 'mmul, add, softmax outputs
				assertEquals(0, tl.preUpdateCount) 'Inference -> no updating
			Next i

			'Check listener NOT called during inference when set to Operation.TRAINING
			tl = New TestListener(Operation.TRAINING)
			sd.setListeners(tl)
			sd.outputSingle(phMap, "softmax")

			assertEquals(0, tl.epochStartCount)
			assertEquals(0, tl.epochEndCount)
			assertEquals(0, tl.validationDoneCount)
			assertEquals(0, tl.iterationStartCount)
			assertEquals(0, tl.iterationDoneCount)
			assertEquals(Collections.emptyMap(), tl.operationStartCount)
			assertEquals(Collections.emptyMap(), tl.operationEndCount)
			assertEquals(0, tl.preOpExecutionCount)
			assertEquals(0, tl.opExecutionCount)
			assertEquals(0, tl.activationAvailableCount)
			assertEquals(0, tl.preUpdateCount)

			'Check listener called during gradient calculation
			tl = New TestListener(Operation.TRAINING)
			sd.setListeners(tl)
			phMap = New Dictionary(Of String, INDArray)()
			phMap("in") = Nd4j.rand(DataType.FLOAT, 1, 4)
			phMap("label") = Nd4j.createFromArray(0f, 1f, 0f).reshape(1, 3)

			For i As Integer = 1 To 3
				sd.calculateGradients(phMap, "in", "w", "b")
				assertEquals(0, tl.epochStartCount)
				assertEquals(0, tl.epochEndCount)
				assertEquals(0, tl.validationDoneCount)
				assertEquals(0, tl.iterationStartCount)
				assertEquals(0, tl.iterationDoneCount)
				assertEquals(Collections.singletonMap(Operation.TRAINING, i), tl.operationStartCount)
				assertEquals(Collections.singletonMap(Operation.TRAINING, i), tl.operationEndCount)
				assertEquals(7*i, tl.preOpExecutionCount) 'mmul, add, softmax, loss grad, softmax backward, add backward, mmul backward
				assertEquals(7*i, tl.opExecutionCount)
				assertEquals(11*i, tl.activationAvailableCount) 'mmul, add, softmax, loss grad (weight, in, label), softmax bp, add backward (z, b), mmul (in, w)
				assertEquals(0, tl.preUpdateCount)
			Next i


			'Check listener NOT called during gradient calculation - when listener is still set to INFERENCE mode
			tl = New TestListener(Operation.INFERENCE)
			sd.setListeners(tl)
			For i As Integer = 1 To 3
				sd.calculateGradients(phMap, "in", "w", "b")
				assertEquals(0, tl.epochStartCount)
				assertEquals(0, tl.epochEndCount)
				assertEquals(0, tl.validationDoneCount)
				assertEquals(0, tl.iterationStartCount)
				assertEquals(0, tl.iterationDoneCount)
				assertEquals(Collections.emptyMap(), tl.operationStartCount)
				assertEquals(Collections.emptyMap(), tl.operationEndCount)
				assertEquals(0, tl.preOpExecutionCount)
				assertEquals(0, tl.opExecutionCount)
				assertEquals(0, tl.activationAvailableCount)
				assertEquals(0, tl.preUpdateCount)
			Next i

			'Check fit:
			tl = New TestListener(Operation.TRAINING)
			sd.setListeners(tl)
			sd.TrainingConfig = TrainingConfig.builder().dataSetFeatureMapping("in").dataSetLabelMapping("label").updater(New Adam(1e-3)).build()

			Dim dsi As New SingletonDataSetIterator(New DataSet(phMap("in"), phMap("label")))
			For i As Integer = 1 To 3
				sd.fit(dsi, 1)
				assertEquals(i, tl.epochStartCount)
				assertEquals(i, tl.epochEndCount)
				assertEquals(0, tl.validationDoneCount)
				assertEquals(i, tl.iterationStartCount)
				assertEquals(i, tl.iterationDoneCount)
				assertEquals(Collections.singletonMap(Operation.TRAINING, i), tl.operationStartCount)
				assertEquals(Collections.singletonMap(Operation.TRAINING, i), tl.operationEndCount)
				assertEquals(7*i, tl.preOpExecutionCount) 'mmul, add, softmax, loss grad, softmax backward, add backward, mmul backward
				assertEquals(7*i, tl.opExecutionCount)
				assertEquals(11*i, tl.activationAvailableCount) 'mmul, add, softmax, loss grad (weight, in, label), softmax bp, add backward (z, b), mmul (in, w)
				assertEquals(2*i, tl.preUpdateCount) 'w, b
			Next i


			'Check evaluation:
			tl = New TestListener(Operation.EVALUATION)
			sd.setListeners(tl)

			For i As Integer = 1 To 3
				sd.evaluate(dsi, "softmax", New Evaluation())
				assertEquals(0, tl.epochStartCount)
				assertEquals(0, tl.epochEndCount)
				assertEquals(0, tl.validationDoneCount)
				assertEquals(0, tl.iterationStartCount)
				assertEquals(0, tl.iterationDoneCount)
				assertEquals(Collections.singletonMap(Operation.EVALUATION, i), tl.operationStartCount)
				assertEquals(Collections.singletonMap(Operation.EVALUATION, i), tl.operationEndCount)
				assertEquals(3*i, tl.preOpExecutionCount) 'mmul, add, softmax
				assertEquals(3*i, tl.opExecutionCount)
				assertEquals(3*i, tl.activationAvailableCount) 'mmul, add, softmax
				assertEquals(0, tl.preUpdateCount) 'w, b
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCustomListener(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCustomListener(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("input", DataType.FLOAT, -1, 4)
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, -1, 3)
			Dim w As SDVariable = sd.var("w", Nd4j.rand(DataType.FLOAT, 4, 3))
			Dim b As SDVariable = sd.var("b", Nd4j.rand(DataType.FLOAT, 3))
			Dim z As SDVariable = sd.nn().linear("z", [in], w, b)
			Dim [out] As SDVariable = sd.nn().softmax("out", z, 1)
			Dim loss As SDVariable = sd.loss().softmaxCrossEntropy("loss", label, [out], Nothing)

			'Create and set the training configuration
			Dim learningRate As Double = 1e-3
			Dim config As TrainingConfig = (New TrainingConfig.Builder()).l2(1e-4).updater(New Adam(learningRate)).dataSetFeatureMapping("input").dataSetLabelMapping("label").addEvaluations(False,"out",0,New Evaluation()).build()
			sd.TrainingConfig = config

			Dim listener As New CustomListener()
			Dim m As IDictionary(Of String, INDArray) = sd.output().data(New IrisDataSetIterator(150, 150)).output("out").listeners(listener).exec()

			assertEquals(1, m.Count)
			assertTrue(m.ContainsKey("out"))
			assertNotNull(listener.z)
			assertNotNull(listener.out)

		End Sub

		Private Class TestListener
			Implements Listener

			Public Sub New(ByVal operation As Operation)
				Me.operation = operation
			End Sub

			Friend ReadOnly operation As Operation

			Friend epochStartCount As Integer = 0
			Friend epochEndCount As Integer = 0
			Friend validationDoneCount As Integer = 0
			Friend iterationStartCount As Integer = 0
			Friend iterationDoneCount As Integer = 0
			Friend operationStartCount As IDictionary(Of Operation, Integer) = New Dictionary(Of Operation, Integer)()
			Friend operationEndCount As IDictionary(Of Operation, Integer) = New Dictionary(Of Operation, Integer)()
			Friend preOpExecutionCount As Integer = 0
			Friend opExecutionCount As Integer = 0
			Friend activationAvailableCount As Integer = 0
			Friend preUpdateCount As Integer = 0


			Public Overridable Function requiredVariables(ByVal sd As SameDiff) As ListenerVariables
				Return Nothing
			End Function

			Public Overridable Function isActive(ByVal operation As Operation) As Boolean
				Return Me.operation = Nothing OrElse Me.operation = operation
			End Function

			Public Overridable Sub epochStart(ByVal sd As SameDiff, ByVal at As At)
				epochStartCount += 1
			End Sub

			Public Overridable Function epochEnd(ByVal sd As SameDiff, ByVal at As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse
				epochEndCount += 1
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Function validationDone(ByVal sd As SameDiff, ByVal at As At, ByVal validationTimeMillis As Long) As ListenerResponse
				validationDoneCount += 1
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Sub iterationStart(ByVal sd As SameDiff, ByVal at As At, ByVal data As MultiDataSet, ByVal etlTimeMs As Long)
				iterationStartCount += 1
			End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public void iterationDone(final org.nd4j.autodiff.samediff.SameDiff sd, final org.nd4j.autodiff.listeners.At at, final org.nd4j.linalg.dataset.api.MultiDataSet dataSet, final org.nd4j.autodiff.listeners.Loss loss)
			Public Overridable Sub iterationDone(ByVal sd As SameDiff, ByVal at As At, ByVal dataSet As MultiDataSet, ByVal loss As Loss)
				iterationDoneCount += 1
			End Sub

			Public Overridable Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation)
				If Not operationStartCount.ContainsKey(op) Then
					operationStartCount(op) = 1
				Else
					operationStartCount(op) = operationStartCount(op) + 1
				End If
			End Sub

			Public Overridable Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation)
				If Not operationEndCount.ContainsKey(op) Then
					operationEndCount(op) = 1
				Else
					operationEndCount(op) = operationEndCount(op) + 1
				End If
			End Sub

			Public Overridable Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal opContext As OpContext)
				preOpExecutionCount += 1
			End Sub

			Public Overridable Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
				opExecutionCount += 1
			End Sub

			Public Overridable Sub activationAvailable(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray)
				activationAvailableCount += 1
			End Sub

			Public Overridable Sub preUpdate(ByVal sd As SameDiff, ByVal at As At, ByVal v As Variable, ByVal update As INDArray)
				preUpdateCount += 1
			End Sub
		End Class

		Private Class CustomListener
			Inherits BaseListener

			Public z As INDArray
			Public [out] As INDArray

			' Specify that this listener is active during inference operations
			Public Overrides Function isActive(ByVal operation As Operation) As Boolean
				Return operation = Operation.INFERENCE
			End Function

			' Specify that this listener requires the activations of "z" and "out"
			Public Overrides Function requiredVariables(ByVal sd As SameDiff) As ListenerVariables
				Return (New ListenerVariables.Builder()).inferenceVariables("z", "out").build()
			End Function

			' Called when the activation of a variable becomes available
			Public Overrides Sub activationAvailable(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray)
				Console.WriteLine("activation:" & varName)

				' if the variable is z or out, store its activation
				If varName.Equals("z") Then
					z = activation.detach().dup()
				ElseIf varName.Equals("out") Then
					[out] = activation.detach().dup()
				End If
			End Sub

		End Class
	End Class

End Namespace