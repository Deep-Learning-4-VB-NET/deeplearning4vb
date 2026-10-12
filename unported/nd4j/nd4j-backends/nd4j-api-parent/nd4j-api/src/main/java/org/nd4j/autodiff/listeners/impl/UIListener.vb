Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Table = com.google.flatbuffers.Table
Imports NonNull = lombok.NonNull
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports ListenerResponse = org.nd4j.autodiff.listeners.ListenerResponse
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports UIGraphStructure = org.nd4j.graph.UIGraphStructure
Imports UIInfoType = org.nd4j.graph.UIInfoType
Imports UIStaticInfoRecord = org.nd4j.graph.UIStaticInfoRecord
Imports LogFileWriter = org.nd4j.graph.ui.LogFileWriter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.autodiff.listeners.impl


	Public Class UIListener
		Inherits BaseListener

		''' <summary>
		''' Default: FileMode.CREATE_OR_APPEND<br>
		''' The mode for handling behaviour when an existing UI file already exists<br>
		''' CREATE: Only allow new file creation. An exception will be thrown if the log file already exists.<br>
		''' APPEND: Only allow appending to an existing file. An exception will be thrown if: (a) no file exists, or (b) the
		''' network configuration in the existing log file does not match the current log file.<br>
		''' CREATE_OR_APPEND: As per APPEND, but create a new file if none already exists.<br>
		''' CREATE_APPEND_NOCHECK: As per CREATE_OR_APPEND, but no exception will be thrown if the existing model does not
		''' match the current model structure. This mode is not recommended.<br>
		''' </summary>
		Public Enum FileMode
			CREATE
			APPEND
			CREATE_OR_APPEND
			CREATE_APPEND_NOCHECK

		''' <summary>
		''' Used to specify how the Update:Parameter ratios are computed. Only relevant when the update ratio calculation is
		''' enabled via <seealso cref="Builder.updateRatios(Integer, UpdateRatio)"/>; update ratio collection is disabled by default<br>
		''' L2: l2Norm(updates)/l2Norm(parameters) is used<br>
		''' MEAN_MAGNITUDE: mean(abs(updates))/mean(abs(parameters)) is used<br>
		''' </summary>
		End Enum
		Public Enum UpdateRatio
			L2
			MEAN_MAGNITUDE

		''' <summary>
		''' Used to specify which histograms should be collected. Histogram collection is disabled by default, but can be
		''' enabled via <seealso cref="Builder.histograms(Integer, HistogramType...)"/>. Note that multiple histogram types may be collected simultaneously.<br>
		''' Histograms may be collected for:<br>
		''' PARAMETERS: All trainable parameters<br>
		''' PARAMETER_GRADIENTS: Gradients corresponding to the trainable parameters<br>
		''' PARAMETER_UPDATES: All trainable parameter updates, before they are applied during training (updates are gradients after applying updater and learning rate etc)<br>
		''' ACTIVATIONS: Activations - ARRAY type SDVariables - those that are not constants, variables or placeholders<br>
		''' ACTIVATION_GRADIENTS: Activation gradients
		''' </summary>
		End Enum
		Public Enum HistogramType
			PARAMETERS
			PARAMETER_GRADIENTS
			PARAMETER_UPDATES
			ACTIVATIONS
			ACTIVATION_GRADIENTS


		End Enum
		Private fileMode As FileMode
		Private logFile As File
		Private lossPlotFreq As Integer
		Private performanceStatsFrequency As Integer
		Private updateRatioFrequency As Integer
		Private updateRatioType As UpdateRatio
		Private histogramFrequency As Integer
		Private histogramTypes() As HistogramType
		Private opProfileFrequency As Integer
		Private trainEvalMetrics As IDictionary(Of Pair(Of String, Integer), IList(Of Evaluation.Metric))
		Private trainEvalFrequency As Integer
		Private testEvaluation As TestEvaluation
		Private learningRateFrequency As Integer

		Private currentIterDataSet As MultiDataSet

		Private writer As LogFileWriter
		Private wroteLossNames As Boolean
		Private wroteLearningRateName As Boolean

		Private relevantOpsForEval As ISet(Of String)
		Private epochTrainEval As IDictionary(Of Pair(Of String, Integer), Evaluation)
		Private wroteEvalNames As Boolean
		Private wroteEvalNamesIter As Boolean

		Private firstUpdateRatioIter As Integer = -1

'JAVA TO VB CONVERTER NOTE: The field checkStructureForRestore was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private checkStructureForRestore_Conflict As Boolean

		Private Sub New(ByVal b As Builder)
			fileMode = b.fileMode_Conflict
			logFile = b.logFile
			lossPlotFreq = b.lossPlotFreq
			performanceStatsFrequency = b.performanceStatsFrequency
			updateRatioFrequency = b.updateRatioFrequency
			updateRatioType = b.updateRatioType
			histogramFrequency = b.histogramFrequency
			histogramTypes = b.histogramTypes
			opProfileFrequency = b.opProfileFrequency
			trainEvalMetrics = b.trainEvalMetrics
			trainEvalFrequency = b.trainEvalFrequency_Conflict
			testEvaluation = b.testEvaluation_Conflict
			learningRateFrequency = b.learningRateFrequency

			Select Case fileMode
				Case org.nd4j.autodiff.listeners.impl.UIListener.FileMode.CREATE
					Preconditions.checkState(Not logFile.exists(), "Log file already exists and fileMode is set to CREATE: %s" & vbLf & "Either delete the existing file, specify a path that doesn't exist, or set the UIListener to another mode " & "such as CREATE_OR_APPEND", logFile)
				Case org.nd4j.autodiff.listeners.impl.UIListener.FileMode.APPEND
					Preconditions.checkState(logFile.exists(), "Log file does not exist and fileMode is set to APPEND: %s" & vbLf & "Either specify a path to an existing log file for this model, or set the UIListener to another mode " & "such as CREATE_OR_APPEND", logFile)
			End Select

			If logFile.exists() Then
				restoreLogFile()
			End If

		End Sub

		Protected Friend Overridable Sub restoreLogFile()
			If logFile.length() = 0 AndAlso fileMode = FileMode.CREATE_OR_APPEND OrElse fileMode = FileMode.APPEND Then
				logFile.delete()
				Return
			End If

			Try
				writer = New LogFileWriter(logFile)
			Catch e As IOException
				Throw New Exception("Error restoring existing log file at path: " & logFile.getAbsolutePath(), e)
			End Try

			If fileMode = FileMode.APPEND OrElse fileMode = FileMode.CREATE_OR_APPEND Then
				'Check the graph structure, if it exists.
				'This is to avoid users creating UI log file with one network configuration, then unintentionally appending data
				' for a completely different network configuration

				Dim si As LogFileWriter.StaticInfo
				Try
					si = writer.readStatic()
				Catch e As IOException
					Throw New Exception("Error restoring existing log file, static info at path: " & logFile.getAbsolutePath(), e)
				End Try

				Dim staticList As IList(Of Pair(Of UIStaticInfoRecord, Table)) = si.getData()
				If si IsNot Nothing Then
					For i As Integer = 0 To staticList.Count - 1
						Dim r As UIStaticInfoRecord = staticList(i).getFirst()
						If r.infoType() = UIInfoType.GRAPH_STRUCTURE Then
							'We can't check structure now (we haven't got SameDiff instance yet) but we can flag it to check on first iteration
							checkStructureForRestore_Conflict = True
						End If
					Next i
				End If

			End If
		End Sub

		Protected Friend Overridable Sub checkStructureForRestore(ByVal sd As SameDiff)
			Dim si As LogFileWriter.StaticInfo
			Try
				si = writer.readStatic()
			Catch e As IOException
				Throw New Exception("Error restoring existing log file, static info at path: " & logFile.getAbsolutePath(), e)
			End Try

			Dim staticList As IList(Of Pair(Of UIStaticInfoRecord, Table)) = si.getData()
			If si IsNot Nothing Then
				Dim [structure] As UIGraphStructure = Nothing
				For i As Integer = 0 To staticList.Count - 1
					Dim r As UIStaticInfoRecord = staticList(i).getFirst()
					If r.infoType() = UIInfoType.GRAPH_STRUCTURE Then
						[structure] = CType(staticList(i).getSecond(), UIGraphStructure)
						Exit For
					End If
				Next i

				If [structure] IsNot Nothing Then
					Dim nInFile As Integer = [structure].inputsLength()
					Dim phs As IList(Of String) = New List(Of String)(nInFile)
					For i As Integer = 0 To nInFile - 1
						phs.Add([structure].inputs(i))
					Next i

					Dim actPhs As IList(Of String) = sd.inputs()
					If actPhs.Count <> phs.Count OrElse Not actPhs.ContainsAll(phs) Then
						Throw New System.InvalidOperationException("Error continuing collection of UI stats in existing model file " & logFile.getAbsolutePath() & ": Model structure differs. Existing (file) model placeholders: " & phs & " vs. current model placeholders: " & actPhs & ". To disable this check, use FileMode.CREATE_APPEND_NOCHECK though this may result issues when rendering data via UI")
					End If

					'Check variables:
					Dim nVarsFile As Integer = [structure].variablesLength()
					Dim vars As IList(Of String) = New List(Of String)(nVarsFile)
					For i As Integer = 0 To nVarsFile - 1
						vars.Add([structure].variables(i).name())
					Next i
					Dim sdVars As IList(Of SDVariable) = sd.variables()
					Dim varNames As IList(Of String) = New List(Of String)(sdVars.Count)
					For Each v As SDVariable In sdVars
						varNames.Add(v.name())
					Next v

					If varNames.Count <> vars.Count OrElse Not varNames.ContainsAll(vars) Then
						Dim countDifferent As Integer = 0
						Dim different As IList(Of String) = New List(Of String)()
						For Each s As String In varNames
							If Not vars.Contains(s) Then
								countDifferent += 1
								If different.Count < 10 Then
									different.Add(s)
								End If
							End If
						Next s
						Dim msg As New StringBuilder()
						msg.Append("Error continuing collection of UI stats in existing model file ").Append(logFile.getAbsolutePath()).Append(": Current model structure differs vs. model structure in file - ").Append(countDifferent).Append(" variable names differ.")
						If different.Count = countDifferent Then
							msg.Append(vbLf & "Variables in new model not present in existing (file) model: ").Append(different)
						Else
							msg.Append(vbLf & "First 10 variables in new model not present in existing (file) model: ").Append(different)
						End If
						msg.Append(vbLf & "To disable this check, use FileMode.CREATE_APPEND_NOCHECK though this may result issues when rendering data via UI")

						Throw New System.InvalidOperationException(msg.ToString())
					End If
				End If
			End If

			checkStructureForRestore_Conflict = False
		End Sub



		Protected Friend Overridable Sub initalizeWriter(ByVal sd As SameDiff)
			Try
				initializeHelper(sd)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void initializeHelper(org.nd4j.autodiff.samediff.SameDiff sd) throws java.io.IOException
		Protected Friend Overridable Sub initializeHelper(ByVal sd As SameDiff)
			writer = New LogFileWriter(logFile)

			'Write graph structure:
			writer.writeGraphStructure(sd)

			'Write system info:
			'TODO

			'All static info completed
			writer.writeFinishStaticMarker()
		End Sub

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return operation = Operation.TRAINING
		End Function

		Public Overrides Sub epochStart(ByVal sd As SameDiff, ByVal at As At)
			epochTrainEval = Nothing
		End Sub

		Public Overrides Function epochEnd(ByVal sd As SameDiff, ByVal at As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse

			'If any training evaluation, report it here:
			If epochTrainEval IsNot Nothing Then
				Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
				For Each e As KeyValuePair(Of Pair(Of String, Integer), Evaluation) In epochTrainEval.SetOfKeyValuePairs()
					Dim n As String = "evaluation/" & e.Key.getFirst() 'TODO what if user does same eval with multiple labels? Doesn't make sense... add validation to ensure this?

					Dim l As IList(Of Evaluation.Metric) = trainEvalMetrics(e.Key)
					For Each m As Evaluation.Metric In l
						Dim mName As String = n & "/train/" & m.ToString().ToLower()
						If Not wroteEvalNames Then
							If Not writer.registeredEventName(mName) Then 'Might have been registered if continuing training
								writer.registerEventNameQuiet(mName)
							End If
						End If

						Dim score As Double = e.Value.scoreForMetric(m)
						Try
							writer.writeScalarEvent(mName, LogFileWriter.EventSubtype.EVALUATION, time, at.iteration(), at.epoch(), score)
						Catch ex As IOException
							Throw New Exception("Error writing to log file", ex)
						End Try
					Next m

					wroteEvalNames = True
				Next e
			End If

			epochTrainEval = Nothing
			Return ListenerResponse.CONTINUE
		End Function

		Public Overrides Sub iterationStart(ByVal sd As SameDiff, ByVal at As At, ByVal data As MultiDataSet, ByVal etlMs As Long)
			If writer Is Nothing Then
				initalizeWriter(sd)
			End If
			If checkStructureForRestore_Conflict Then
				checkStructureForRestore(sd)
			End If

			'If there's any evaluation to do in opExecution method, we'll need this there
			currentIterDataSet = data
		End Sub

		Public Overrides Sub iterationDone(ByVal sd As SameDiff, ByVal at As At, ByVal dataSet As MultiDataSet, ByVal loss As Loss)
			Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()

			'iterationDone method - just writes loss values (so far)

			If Not wroteLossNames Then
				For Each s As String In loss.getLossNames()
					Dim n As String = "losses/" & s
					If Not writer.registeredEventName(n) Then 'Might have been registered if continuing training
						writer.registerEventNameQuiet(n)
					End If
				Next s

				If loss.numLosses() > 1 Then
					Dim n As String = "losses/totalLoss"
					If Not writer.registeredEventName(n) Then 'Might have been registered if continuing training
						writer.registerEventNameQuiet(n)
					End If
				End If
				wroteLossNames = True
			End If

			Dim lossNames As IList(Of String) = loss.getLossNames()
			Dim lossVals() As Double = loss.getLosses()
			For i As Integer = 0 To lossVals.Length - 1
				Try
					Dim eventName As String = "losses/" & lossNames(i)
					writer.writeScalarEvent(eventName, LogFileWriter.EventSubtype.LOSS, time, at.iteration(), at.epoch(), lossVals(i))
				Catch e As IOException
					Throw New Exception("Error writing to log file", e)
				End Try
			Next i

			If lossVals.Length > 1 Then
				Dim total As Double = loss.totalLoss()
				Try
					Dim eventName As String = "losses/totalLoss"
					writer.writeScalarEvent(eventName, LogFileWriter.EventSubtype.LOSS, time, at.iteration(), at.epoch(), total)
				Catch e As IOException
					Throw New Exception("Error writing to log file", e)
				End Try
			End If

			currentIterDataSet = Nothing

			If learningRateFrequency > 0 Then
				'Collect + report learning rate
				If Not wroteLearningRateName Then
					Dim name As String = "learningRate"
					If Not writer.registeredEventName(name) Then
						writer.registerEventNameQuiet(name)
					End If
					wroteLearningRateName = True
				End If

				If at.iteration() Mod learningRateFrequency = 0 Then
					Dim u As IUpdater = sd.getTrainingConfig().getUpdater()
					If u.hasLearningRate() Then
						Dim lr As Double = u.getLearningRate(at.iteration(), at.epoch())
						Try
							writer.writeScalarEvent("learningRate", LogFileWriter.EventSubtype.LEARNING_RATE, time, at.iteration(), at.epoch(), lr)
						Catch e As IOException
							Throw New Exception("Error writing to log file")
						End Try
					End If
				End If
			End If
		End Sub



		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)


			'Do training set evaluation, if required
			'Note we'll do it in opExecution not iterationDone because we can't be sure arrays will be stil be around in the future
			'i.e., we'll eventually add workspaces and clear activation arrays once they have been consumed
			If at.operation() = Operation.TRAINING AndAlso trainEvalMetrics IsNot Nothing AndAlso trainEvalMetrics.Count > 0 Then
				Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()

				'First: check if this op is relevant at all to evaluation...
				If relevantOpsForEval Is Nothing Then
					'Build list for quick lookups to know if we should do anything for this op
					relevantOpsForEval = New HashSet(Of String)()
					For Each p As Pair(Of String, Integer) In trainEvalMetrics.Keys
						Dim v As Variable = sd.getVariables().get(p.First)
						Dim opName As String = v.getOutputOfOp()
						Preconditions.checkState(opName IsNot Nothing, "Cannot evaluate on variable of type %s - variable name: ""%s""", v.getVariable().getVariableType(), opName)
						relevantOpsForEval.Add(v.getOutputOfOp())
					Next p
				End If

				If Not relevantOpsForEval.Contains(op.Name) Then
					'Op outputs are not required for eval
					Return
				End If

				If epochTrainEval Is Nothing Then
					epochTrainEval = New Dictionary(Of Pair(Of String, Integer), Evaluation)()

					For Each p As Pair(Of String, Integer) In trainEvalMetrics.Keys
						epochTrainEval(p) = New Evaluation()
					Next p
				End If

				'Perform evaluation:
				Dim wrote As Boolean = False
				For Each p As Pair(Of String, Integer) In trainEvalMetrics.Keys
					Dim idx As Integer = op.getOutputsOfOp().IndexOf(p.First)
					Dim [out] As INDArray = outputs(idx)
					Dim label As INDArray = currentIterDataSet.getLabels(p.Second)
					Dim mask As INDArray = currentIterDataSet.getLabelsMaskArray(p.Second)

					epochTrainEval(p).eval(label, [out], mask)

					If trainEvalFrequency > 0 AndAlso at.iteration() > 0 AndAlso at.iteration() Mod trainEvalFrequency = 0 Then
						For Each m As Evaluation.Metric In trainEvalMetrics(p)
							Dim n As String = "evaluation/train_iter/" & p.getKey() & "/" & m.ToString().ToLower()
							If Not wroteEvalNamesIter Then
								If Not writer.registeredEventName(n) Then 'Might have been written previously if continuing training
									writer.registerEventNameQuiet(n)
								End If
								wrote = True
							End If

							Dim score As Double = epochTrainEval(p).scoreForMetric(m)

							Try
								writer.writeScalarEvent(n, LogFileWriter.EventSubtype.EVALUATION, time, at.iteration(), at.epoch(), score)
							Catch e As IOException
								Throw New Exception("Error writing to log file")
							End Try
						Next m
					End If
				Next p
				wroteEvalNamesIter = wrote
			End If
		End Sub

		Public Overrides Sub preUpdate(ByVal sd As SameDiff, ByVal at As At, ByVal v As Variable, ByVal update As INDArray)
			If writer Is Nothing Then
				initalizeWriter(sd)
			End If

			If updateRatioFrequency > 0 AndAlso at.iteration() Mod updateRatioFrequency = 0 Then
				If firstUpdateRatioIter < 0 Then
					firstUpdateRatioIter = at.iteration()
				End If

				If firstUpdateRatioIter = at.iteration() Then
					'Register name
					Dim name As String = "logUpdateRatio/" & v.getName()
					If Not writer.registeredEventName(name) Then 'Might have already been registered if continuing
						writer.registerEventNameQuiet(name)
					End If
				End If

				Dim params As Double
				Dim updates As Double
				If updateRatioType = UpdateRatio.L2 Then
					params = v.getVariable().getArr().norm2Number().doubleValue()
					updates = update.norm2Number().doubleValue()
				Else
					'Mean magnitude - L1 norm divided by N. But in the ratio later, N cancels out...
					params = v.getVariable().getArr().norm1Number().doubleValue()
					updates = update.norm1Number().doubleValue()
				End If

				Dim ratio As Double = updates / params
				If params = 0.0 Then
					ratio = 0.0
				Else
					ratio = Math.Max(-10, Math.Log10(ratio)) 'Clip to -10, when updates are too small
				End If


				Try
					Dim name As String = "logUpdateRatio/" & v.getName()
					writer.writeScalarEvent(name, LogFileWriter.EventSubtype.LOSS, DateTimeHelper.CurrentUnixTimeMillis(), at.iteration(), at.epoch(), ratio)
				Catch e As IOException
					Throw New Exception("Error writing to log file", e)
				End Try
			End If
		End Sub





		Public Shared Function builder(ByVal logFile As File) As Builder
			Return New Builder(logFile)
		End Function

		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field fileMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend fileMode_Conflict As FileMode = FileMode.CREATE_OR_APPEND
			Friend logFile As File

			Friend lossPlotFreq As Integer = 1
			Friend performanceStatsFrequency As Integer = -1 'Disabled by default

			Friend updateRatioFrequency As Integer = -1 'Disabled by default
			Friend updateRatioType As UpdateRatio = UpdateRatio.MEAN_MAGNITUDE

			Friend histogramFrequency As Integer = -1 'Disabled by default
			Friend histogramTypes() As HistogramType

			Friend opProfileFrequency As Integer = -1 'Disabled by default

			Friend trainEvalMetrics As IDictionary(Of Pair(Of String, Integer), IList(Of Evaluation.Metric))
'JAVA TO VB CONVERTER NOTE: The field trainEvalFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend trainEvalFrequency_Conflict As Integer = 10 'Report evaluation metrics every 10 iterations by default

'JAVA TO VB CONVERTER NOTE: The field testEvaluation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend testEvaluation_Conflict As TestEvaluation = Nothing

			Friend learningRateFrequency As Integer = 10 'Whether to plot learning rate or not

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull File logFile)
			Public Sub New(ByVal logFile As File)
				Me.logFile = logFile
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter fileMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function fileMode(ByVal fileMode_Conflict As FileMode) As Builder
				Me.fileMode_Conflict = fileMode_Conflict
				Return Me
			End Function

			Public Overridable Function plotLosses(ByVal frequency As Integer) As Builder
				Me.lossPlotFreq = frequency
				Return Me
			End Function

			Public Overridable Function performanceStats(ByVal frequency As Integer) As Builder
				Me.performanceStatsFrequency = frequency
				Return Me
			End Function

			Public Overridable Function trainEvaluationMetrics(ByVal name As String, ByVal labelIdx As Integer, ParamArray ByVal metrics() As Evaluation.Metric) As Builder
				If trainEvalMetrics Is Nothing Then
					trainEvalMetrics = New LinkedHashMap(Of Pair(Of String, Integer), IList(Of Evaluation.Metric))()
				End If
				Dim p As New Pair(Of String, Integer)(name, labelIdx)
				If Not trainEvalMetrics.ContainsKey(p) Then
					trainEvalMetrics(p) = New List(Of Evaluation.Metric)()
				End If
				Dim l As IList(Of Evaluation.Metric) = trainEvalMetrics(p)
				For Each m As Evaluation.Metric In metrics
					If Not l.Contains(m) Then
						l.Add(m)
					End If
				Next m
				Return Me
			End Function

			Public Overridable Function trainAccuracy(ByVal name As String, ByVal labelIdx As Integer) As Builder
				Return trainEvaluationMetrics(name, labelIdx, Evaluation.Metric.ACCURACY)
			End Function

			Public Overridable Function trainF1(ByVal name As String, ByVal labelIdx As Integer) As Builder
				Return trainEvaluationMetrics(name, labelIdx, Evaluation.Metric.F1)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter trainEvalFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function trainEvalFrequency(ByVal trainEvalFrequency_Conflict As Integer) As Builder
				Me.trainEvalFrequency_Conflict = trainEvalFrequency_Conflict
				Return Me
			End Function

			Public Overridable Function updateRatios(ByVal frequency As Integer) As Builder
				Return updateRatios(frequency, UpdateRatio.MEAN_MAGNITUDE)
			End Function

			Public Overridable Function updateRatios(ByVal frequency As Integer, ByVal ratioType As UpdateRatio) As Builder
				Me.updateRatioFrequency = frequency
				Me.updateRatioType = ratioType
				Return Me
			End Function

			Public Overridable Function histograms(ByVal frequency As Integer, ParamArray ByVal types() As HistogramType) As Builder
				Me.histogramFrequency = frequency
				Me.histogramTypes = types
				Return Me
			End Function

			Public Overridable Function profileOps(ByVal frequency As Integer) As Builder
				Me.opProfileFrequency = frequency
				Return Me
			End Function

			Public Overridable Function testEvaluation(ByVal testEvalConfig As TestEvaluation) As Builder
				Me.testEvaluation_Conflict = testEvalConfig
				Return Me
			End Function

			Public Overridable Function learningRate(ByVal frequency As Integer) As Builder
				Me.learningRateFrequency = frequency
				Return Me
			End Function

			Public Overridable Function build() As UIListener
				Return New UIListener(Me)
			End Function
		End Class

		Public Class TestEvaluation
			'TODO
		End Class
	End Class

End Namespace