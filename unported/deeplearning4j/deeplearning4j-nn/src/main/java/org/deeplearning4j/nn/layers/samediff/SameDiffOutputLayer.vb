Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SingleThreadArrayHolder = org.nd4j.autodiff.samediff.array.SingleThreadArrayHolder
Imports InferenceSession = org.nd4j.autodiff.samediff.internal.InferenceSession
Imports SessionMemMgr = org.nd4j.autodiff.samediff.internal.SessionMemMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.layers.samediff


	<Serializable>
	Public Class SameDiffOutputLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.samediff.SameDiffOutputLayer)
		Implements IOutputLayer

		Public Const INPUT_KEY As String = "input"
		Public Const LABELS_KEY As String = "labels"

		Protected Friend sameDiff As SameDiff
		Protected Friend outputVar As SDVariable
		Protected Friend outputKey As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.api.ndarray.INDArray labels;
		Protected Friend labels As INDArray

'JAVA TO VB CONVERTER NOTE: The field params was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend params_Conflict As INDArray
		Protected Friend gradients As INDArray
'JAVA TO VB CONVERTER NOTE: The field paramTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend paramTable_Conflict As IDictionary(Of String, INDArray)
		Protected Friend gradTable As IDictionary(Of String, INDArray)


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function clone() As Layer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'TODO - properly support weight noise...
		End Sub

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activateHelper(True, workspaceMgr)
		End Function

		Private Function activateHelper(ByVal activations As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			'Check where the output occurs. If it's a simple loss layer (no params) this could
			' just be the input!
			If activations AndAlso INPUT_KEY.Equals(layerConf().activationsVertexName()) Then
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input_Conflict)
			End If

			'TODO optimize
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					doInit()
				End If
			End Using

			'Configure memory management for SameDiff instance - use DL4J workspaces
			Dim wsNameWorking As String = workspaceMgr.getWorkspaceName(ArrayType.FF_WORKING_MEM)
			Dim wsNameOutput As String = workspaceMgr.getWorkspaceName(ArrayType.ACTIVATIONS)
			Dim confWorking As WorkspaceConfiguration = workspaceMgr.getConfiguration(ArrayType.FF_WORKING_MEM)
			Dim confOutput As WorkspaceConfiguration = workspaceMgr.getConfiguration(ArrayType.ACTIVATIONS)
			Dim actScopedOut As Boolean = workspaceMgr.isScopedOut(ArrayType.ACTIVATIONS)
			Preconditions.checkState(actScopedOut OrElse wsNameOutput IsNot Nothing, "Activations must have a workspace or must be scoped out")
			Dim mmgr As SessionMemMgr = New DL4JSameDiffMemoryMgr(wsNameWorking, wsNameOutput, confWorking, confOutput)

			Dim [is] As InferenceSession = sameDiff.getSessions().get(Thread.CurrentThread.getId())
			If [is] Is Nothing Then
				[is] = New InferenceSession(sameDiff)
				sameDiff.getSessions().put(Thread.CurrentThread.getId(), [is])
			End If
			[is].setMmgr(mmgr)

			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			phMap(INPUT_KEY) = input_Conflict
			If Not activations AndAlso layerConf().labelsRequired() AndAlso labels IsNot Nothing Then
				phMap(LABELS_KEY) = labels
			End If

			Dim s As String = If(activations, layerConf().activationsVertexName(), outputVar.name())

			Dim [out] As INDArray = sameDiff.outputSingle(phMap, s)

			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()

			'Edge case: vertex is just an Identity function, for example
			'TODO there may be a cleaner way to do this...
			If Not actScopedOut AndAlso Not [out].data().ParentWorkspace.Id.Equals(wsNameOutput) Then
				[out] = workspaceMgr.dup(ArrayType.ACTIVATIONS, [out])
			ElseIf actScopedOut AndAlso [out].Attached Then
				[out] = [out].detach()
			End If

			Return [out]
		End Function


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Preconditions.checkState(Not layerConf().labelsRequired() OrElse labels IsNot Nothing, "Cannot execute backprop: Labels are not set. " & "If labels are not required for this SameDiff output layer, override SameDiffOutputLayer.labelsRequired()" & " to return false instead")
			Dim g As Gradient = New DefaultGradient()

			Dim dLdIn As INDArray
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					'Usually doInit will be called in forward pass; not necessarily the case in output layers
					' (for efficiency, we skip output layer forward pass in MultiLayerNetwork/ComputationGraph)
					doInit()
				End If
				If sameDiff.getFunction("grad") Is Nothing Then
					sameDiff.createGradFunction(INPUT_KEY)
				End If
			End Using

			'Configure memory management for SameDiff instance - use DL4J workspaces
			Dim sessionMap As IDictionary(Of Long, InferenceSession) = sameDiff.getFunction("grad").getSessions()
			If Not sessionMap.ContainsKey(Thread.CurrentThread.getId()) Then
				sessionMap(Thread.CurrentThread.getId()) = New InferenceSession(sameDiff.getFunction("grad"))
			End If
			Dim wsNameWorking As String = workspaceMgr.getWorkspaceName(ArrayType.BP_WORKING_MEM)
			Dim wsNameActGrad As String = workspaceMgr.getWorkspaceName(ArrayType.ACTIVATION_GRAD)
			Dim confWorking As WorkspaceConfiguration = workspaceMgr.getConfiguration(ArrayType.BP_WORKING_MEM)
			Dim confOutput As WorkspaceConfiguration = workspaceMgr.getConfiguration(ArrayType.ACTIVATION_GRAD)

			Dim actGradScopedOut As Boolean = workspaceMgr.isScopedOut(ArrayType.ACTIVATION_GRAD)
			Preconditions.checkState(actGradScopedOut OrElse wsNameActGrad IsNot Nothing, "Activation gradients must have a workspace or be scoped out")
			Dim mmgr As SessionMemMgr = New DL4JSameDiffMemoryMgr(wsNameWorking, wsNameActGrad, confWorking, confOutput)
			sessionMap(Thread.CurrentThread.getId()).setMmgr(mmgr)

			If Not sameDiff.hasGradientFunction() Then
				'Create when scoped out, to ensure any arrays are not in WS
				sameDiff.createGradFunction(INPUT_KEY)
			End If

			Dim gradVarNames As IList(Of String) = New List(Of String)()
			CType(gradVarNames, List(Of String)).AddRange(paramTable_Conflict.Keys)
			gradVarNames.Add(INPUT_KEY)

			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			phMap(INPUT_KEY) = input_Conflict
			phMap(LABELS_KEY) = labels

			Dim grads As IDictionary(Of String, INDArray) = sameDiff.calculateGradients(phMap, gradVarNames)
			For Each s As String In paramTable_Conflict.Keys
				Dim sdGrad As INDArray = grads(s)
				Dim dl4jGrad As INDArray = gradTable(s)
				dl4jGrad.assign(sdGrad) 'TODO OPTIMIZE THIS
				g.gradientForVariable()(s) = dl4jGrad
				If sdGrad.closeable() Then
					sdGrad.close()
				End If
			Next s

			dLdIn = grads(INPUT_KEY)

			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()

			'TODO there may be a cleaner way to do this...
			If Not actGradScopedOut AndAlso Not dLdIn.data().ParentWorkspace.Id.Equals(wsNameActGrad) Then
				dLdIn = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, dLdIn)
			ElseIf actGradScopedOut AndAlso dLdIn.Attached Then
				dLdIn = dLdIn.detach()
			End If

			Return New Pair(Of Gradient, INDArray)(g, dLdIn)
		End Function

		''' <summary>
		'''Returns the parameters of the neural network as a flattened row vector </summary>
		''' <returns> the parameters of the neural network </returns>
		Public Overrides Function params() As INDArray
			Return params_Conflict
		End Function

		Public Overrides Function getParam(ByVal param As String) As INDArray
			Return paramTable(param)
		End Function

		Public Overrides Function numParams() As Long
			Return If(params_Conflict Is Nothing, 0, CInt(params_Conflict.length()))
		End Function

		Public Overrides Sub setParam(ByVal key As String, ByVal val As INDArray)
			If Not paramTable_Conflict.ContainsKey(key) Then
				Throw New System.ArgumentException("Cannot set parameter, invalid/unknown parameter key: " & key)
			End If
			Dim current As INDArray = paramTable(key)
			If Not current.shape().SequenceEqual(val.shape()) Then
				Throw New System.ArgumentException("Cannot set parameter """ & key & """, invalid shape: parameter array has shape " & java.util.Arrays.toString(current.shape()) & ", trying to set parameter of shape " & java.util.Arrays.toString(val.shape()))
			End If
		End Sub

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				If params IsNot Nothing Then
					Throw New System.NotSupportedException("Not supported")
				End If
			End Set
		End Property

		Protected Friend Overrides Sub setParams(ByVal params As INDArray, ByVal order As Char)
			Me.Params = params
		End Sub

		Public Overrides WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				Me.params_Conflict = params
			End Set
		End Property

		Public Overrides ReadOnly Property GradientsViewArray As INDArray
			Get
				Return gradients
			End Get
		End Property

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				Me.gradients = gradients
				Me.gradTable = layerConf().initializer().getGradientsFromFlattened(conf(), gradients)
			End Set
		End Property

		Public Overridable Overloads WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				If Me.paramTable_Conflict Is Nothing Then
					Me.paramTable_Conflict = paramTable
				Else
					For Each e As KeyValuePair(Of String, INDArray) In paramTable.SetOfKeyValuePairs()
						setParam(e.Key, e.Value)
					Next e
				End If
			End Set
		End Property

		Public Overrides Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		Public Overrides Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Return paramTable_Conflict
		End Function

		Protected Friend Overridable Sub doInit()
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				Dim bl As org.deeplearning4j.nn.conf.layers.samediff.SameDiffOutputLayer = layerConf()
				sameDiff = SameDiff.create()
				'Use SingleThreadArrayHolder so we can use views (also don't nede multithreading here, DL4J is not thread safe)
				sameDiff.setArrayHolders(New SingleThreadArrayHolder(), New SingleThreadArrayHolder(), False)
				Dim p As IDictionary(Of String, INDArray) = paramTable()

				Dim inputShape() As Long = CType(input_Conflict.shape().Clone(), Long())
				inputShape(0) = -1
				Dim inputVar As SDVariable = sameDiff.placeHolder(INPUT_KEY, dataType, inputShape)
				Dim labelVar As SDVariable = Nothing
				If layerConf().labelsRequired() Then
					Dim labelShape() As Long = If(labels Is Nothing, New Long(){-1, -1}, CType(labels.shape().Clone(), Long()))
					labelShape(0) = -1
					labelVar = sameDiff.placeHolder(LABELS_KEY, dataType, labelShape)
				End If
				Dim paramShapes As IDictionary(Of String, Long()) = layerConf().getLayerParams().getParamShapes()
				Dim params As IDictionary(Of String, SDVariable) = New LinkedHashMap(Of String, SDVariable)()
				For Each s As String In paramShapes.Keys
					Dim ps As val = paramShapes(s)
					Dim v As SDVariable = sameDiff.var(s, dataType, ps)
					params(s) = v
				Next s
				Dim layerOutput As SDVariable = bl.defineLayer(sameDiff, inputVar, labelVar, params)
				Preconditions.checkNotNull(layerOutput, "Invalid output: layer output is null")
				outputVar = layerOutput

				For Each e As KeyValuePair(Of String, INDArray) In p.SetOfKeyValuePairs()
					Dim arr As INDArray = e.Value
					sameDiff.associateArrayWithVariable(arr, sameDiff.getVariable(e.Key))
				Next e

				Me.outputKey = layerOutput.name()
			End Using
		End Sub

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return layerConf().labelsRequired()
		End Function

		Public Overridable Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements IOutputLayer.computeScore
			Dim scoreArr As INDArray = activateHelper(False, workspaceMgr)
			Return (scoreArr.getDouble(0) + fullNetRegTerm) / input_Conflict.size(0)
		End Function

		Public Overridable Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IOutputLayer.computeScoreForExamples
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Function f1Score(ByVal data As DataSet) As Double
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function numLabels() As Integer
			Return 0
		End Function

		Public Overridable Overloads Sub fit(ByVal iter As DataSetIterator)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function predict(ByVal examples As INDArray) As Integer()
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function predict(ByVal dataSet As DataSet) As IList(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Throw New System.NotSupportedException("Not supported")
		End Sub
	End Class

End Namespace