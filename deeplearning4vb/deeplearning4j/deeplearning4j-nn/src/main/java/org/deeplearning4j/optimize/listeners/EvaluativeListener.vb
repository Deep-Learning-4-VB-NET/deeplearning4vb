Imports System
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports InvocationType = org.deeplearning4j.optimize.api.InvocationType
Imports EvaluationCallback = org.deeplearning4j.optimize.listeners.callbacks.EvaluationCallback
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EvaluativeListener extends org.deeplearning4j.optimize.api.BaseTrainingListener
	Public Class EvaluativeListener
		Inherits BaseTrainingListener

		<NonSerialized>
		Protected Friend iterationCount As New ThreadLocal(Of AtomicLong)()
		Protected Friend frequency As Integer

		Protected Friend invocationCount As New AtomicLong(0)

		<NonSerialized>
		Protected Friend dsIterator As DataSetIterator
		<NonSerialized>
		Protected Friend mdsIterator As MultiDataSetIterator
		Protected Friend ds As DataSet
		Protected Friend mds As MultiDataSet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.evaluation.IEvaluation[] evaluations;
		Protected Friend evaluations() As IEvaluation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.optimize.api.InvocationType invocationType;
		Protected Friend invocationType As InvocationType

		''' <summary>
		''' This callback will be invoked after evaluation finished
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected transient org.deeplearning4j.optimize.listeners.callbacks.EvaluationCallback callback;
		<NonSerialized>
		Protected Friend callback As EvaluationCallback

		''' <summary>
		''' Evaluation will be launched after each *frequency* iterations, with <seealso cref="Evaluation"/> datatype </summary>
		''' <param name="iterator">  Iterator to provide data for evaluation </param>
		''' <param name="frequency"> Frequency (in number of iterations) to perform evaluation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSetIterator iterator, int frequency)
		Public Sub New(ByVal iterator As DataSetIterator, ByVal frequency As Integer)
			Me.New(iterator, frequency, InvocationType.ITERATION_END, New Evaluation())
		End Sub

		''' <param name="iterator">  Iterator to provide data for evaluation </param>
		''' <param name="frequency"> Frequency (in number of iterations/epochs according to the invocation type) to perform evaluation </param>
		''' <param name="type">      Type of value for 'frequency' - iteration end, epoch end, etc </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSetIterator iterator, int frequency, @NonNull InvocationType type)
		Public Sub New(ByVal iterator As DataSetIterator, ByVal frequency As Integer, ByVal type As InvocationType)
			Me.New(iterator, frequency, type, New Evaluation())
		End Sub

		''' <summary>
		''' Evaluation will be launched after each *frequency* iterations, with <seealso cref="Evaluation"/> datatype </summary>
		''' <param name="iterator">  Iterator to provide data for evaluation </param>
		''' <param name="frequency"> Frequency (in number of iterations) to perform evaluation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSetIterator iterator, int frequency)
		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal frequency As Integer)
			Me.New(iterator, frequency, InvocationType.ITERATION_END, New Evaluation())
		End Sub

		''' <param name="iterator">  Iterator to provide data for evaluation </param>
		''' <param name="frequency"> Frequency (in number of iterations/epochs according to the invocation type) to perform evaluation </param>
		''' <param name="type">      Type of value for 'frequency' - iteration end, epoch end, etc </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSetIterator iterator, int frequency, @NonNull InvocationType type)
		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal frequency As Integer, ByVal type As InvocationType)
			Me.New(iterator, frequency, type, New Evaluation())
		End Sub

		''' <summary>
		''' Evaluation will be launched after each *frequency* iteration
		''' </summary>
		''' <param name="iterator">    Iterator to provide data for evaluation </param>
		''' <param name="frequency">   Frequency (in number of iterations) to perform evaluation </param>
		''' <param name="evaluations"> Type of evalutions to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSetIterator iterator, int frequency, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal iterator As DataSetIterator, ByVal frequency As Integer, ParamArray ByVal evaluations() As IEvaluation)
			Me.New(iterator, frequency, InvocationType.ITERATION_END, evaluations)
		End Sub

		''' <summary>
		''' Evaluation will be launched after each *frequency* iteration
		''' </summary>
		''' <param name="iterator">    Iterator to provide data for evaluation </param>
		''' <param name="frequency">   Frequency (in number of iterations/epochs according to the invocation type) to perform evaluation </param>
		''' <param name="type">        Type of value for 'frequency' - iteration end, epoch end, etc </param>
		''' <param name="evaluations"> Type of evalutions to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSetIterator iterator, int frequency, @NonNull InvocationType type, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal iterator As DataSetIterator, ByVal frequency As Integer, ByVal type As InvocationType, ParamArray ByVal evaluations() As IEvaluation)
			Me.dsIterator = iterator
			Me.frequency = frequency
			Me.evaluations = evaluations

			Me.invocationType = type
		End Sub

		''' <summary>
		''' Evaluation will be launched after each *frequency* iteration
		''' </summary>
		''' <param name="iterator">    Iterator to provide data for evaluation </param>
		''' <param name="frequency">   Frequency (in number of iterations) to perform evaluation </param>
		''' <param name="evaluations"> Type of evalutions to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSetIterator iterator, int frequency, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal frequency As Integer, ParamArray ByVal evaluations() As IEvaluation)
			Me.New(iterator, frequency, InvocationType.ITERATION_END, evaluations)
		End Sub

		''' <summary>
		''' Evaluation will be launched after each *frequency* iteration
		''' </summary>
		''' <param name="iterator">    Iterator to provide data for evaluation </param>
		''' <param name="frequency">   Frequency (in number of iterations/epochs according to the invocation type) to perform evaluation </param>
		''' <param name="type">        Type of value for 'frequency' - iteration end, epoch end, etc </param>
		''' <param name="evaluations"> Type of evalutions to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSetIterator iterator, int frequency, @NonNull InvocationType type, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal iterator As MultiDataSetIterator, ByVal frequency As Integer, ByVal type As InvocationType, ParamArray ByVal evaluations() As IEvaluation)
			Me.mdsIterator = iterator
			Me.frequency = frequency
			Me.evaluations = evaluations

			Me.invocationType = type
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSet dataSet, int frequency, @NonNull InvocationType type)
		Public Sub New(ByVal dataSet As DataSet, ByVal frequency As Integer, ByVal type As InvocationType)
			Me.New(dataSet, frequency, type, New Evaluation())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSet multiDataSet, int frequency, @NonNull InvocationType type)
		Public Sub New(ByVal multiDataSet As MultiDataSet, ByVal frequency As Integer, ByVal type As InvocationType)
			Me.New(multiDataSet, frequency, type, New Evaluation())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull DataSet dataSet, int frequency, @NonNull InvocationType type, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal dataSet As DataSet, ByVal frequency As Integer, ByVal type As InvocationType, ParamArray ByVal evaluations() As IEvaluation)
			Me.ds = dataSet
			Me.frequency = frequency
			Me.evaluations = evaluations

			Me.invocationType = type
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EvaluativeListener(@NonNull MultiDataSet multiDataSet, int frequency, @NonNull InvocationType type, org.nd4j.evaluation.IEvaluation... evaluations)
		Public Sub New(ByVal multiDataSet As MultiDataSet, ByVal frequency As Integer, ByVal type As InvocationType, ParamArray ByVal evaluations() As IEvaluation)
			Me.mds = multiDataSet
			Me.frequency = frequency
			Me.evaluations = evaluations

			Me.invocationType = type
		End Sub

		''' <summary>
		''' Event listener for each iteration
		''' </summary>
		''' <param name="model">     the model iterating </param>
		''' <param name="iteration"> the iteration </param>
		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			If invocationType = InvocationType.ITERATION_END Then
				invokeListener(model)
			End If
		End Sub

		Public Overrides Sub onEpochStart(ByVal model As Model)
			If invocationType = InvocationType.EPOCH_START Then
				invokeListener(model)
			End If
		End Sub

		Public Overrides Sub onEpochEnd(ByVal model As Model)
			If invocationType = InvocationType.EPOCH_END Then
				invokeListener(model)
			End If
		End Sub

		Protected Friend Overridable Sub invokeListener(ByVal model As Model)
			If iterationCount.get() Is Nothing Then
				iterationCount.set(New AtomicLong(0))
			End If

			If iterationCount.get().getAndIncrement() Mod frequency <> 0 Then
				Return
			End If

			For Each evaluation As IEvaluation In evaluations
				evaluation.reset()
			Next evaluation

			If dsIterator IsNot Nothing AndAlso dsIterator.resetSupported() Then
				dsIterator.reset()
			ElseIf mdsIterator IsNot Nothing AndAlso mdsIterator.resetSupported() Then
				mdsIterator.reset()
			End If

			' FIXME: we need to save/restore inputs, if we're being invoked with iterations > 1

			log.info("Starting evaluation nr. {}", invocationCount.incrementAndGet())
			If TypeOf model Is MultiLayerNetwork Then
				If dsIterator IsNot Nothing Then
					DirectCast(model, MultiLayerNetwork).doEvaluation(dsIterator, evaluations)
				ElseIf ds IsNot Nothing Then
					For Each evaluation As IEvaluation In evaluations
						evaluation.eval(ds.Labels, DirectCast(model, MultiLayerNetwork).output(ds.Features))
					Next evaluation
				End If
			ElseIf TypeOf model Is ComputationGraph Then
				If dsIterator IsNot Nothing Then
					DirectCast(model, ComputationGraph).doEvaluation(dsIterator, evaluations)
				ElseIf mdsIterator IsNot Nothing Then
					DirectCast(model, ComputationGraph).doEvaluation(mdsIterator, evaluations)
				ElseIf ds IsNot Nothing Then
					For Each evaluation As IEvaluation In evaluations
						evalAtIndex(evaluation, New INDArray() {ds.Labels}, DirectCast(model, ComputationGraph).output(ds.Features), 0)
					Next evaluation
				ElseIf mds IsNot Nothing Then
					For Each evaluation As IEvaluation In evaluations
						evalAtIndex(evaluation, mds.Labels, DirectCast(model, ComputationGraph).output(mds.Features), 0)
					Next evaluation
				End If
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Throw New DL4JInvalidInputException("Model is unknown: " & model.GetType().FullName)
			End If

			' TODO: maybe something better should be used here?
			log.info("Reporting evaluation results:")
			For Each evaluation As IEvaluation In evaluations
				log.info("{}:" & vbLf & "{}", evaluation.GetType().Name, evaluation.stats())
			Next evaluation


			If callback IsNot Nothing Then
				callback.call(Me, model, invocationCount.get(), evaluations)
			End If
		End Sub

		Protected Friend Overridable Sub evalAtIndex(ByVal evaluation As IEvaluation, ByVal labels() As INDArray, ByVal predictions() As INDArray, ByVal index As Integer)
			evaluation.eval(labels(index), predictions(index))
		End Sub

	End Class

End Namespace