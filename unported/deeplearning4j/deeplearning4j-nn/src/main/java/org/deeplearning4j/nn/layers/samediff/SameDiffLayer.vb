Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports AbstractSameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.AbstractSameDiffLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SingleThreadArrayHolder = org.nd4j.autodiff.samediff.array.SingleThreadArrayHolder
Imports InferenceSession = org.nd4j.autodiff.samediff.internal.InferenceSession
Imports SessionMemMgr = org.nd4j.autodiff.samediff.internal.SessionMemMgr
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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
	Public Class SameDiffLayer
		Inherits AbstractLayer(Of AbstractSameDiffLayer)

		Public Const INPUT_KEY As String = "input"
		Public Const MASK_KEY As String = "mask"

		Protected Friend sameDiff As SameDiff
		Protected Friend outputVar As SDVariable
		Protected Friend fn As ExternalErrorsFunction
		Protected Friend outputKey As String

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
			assertInputSet(False)

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					doInit()
				End If
			End Using

			Dim bl As org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer = DirectCast(layerConf(), org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer)
			bl.validateInput(input_Conflict)

			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			phMap(INPUT_KEY) = input_Conflict
			If maskArray_Conflict IsNot Nothing Then
				phMap(MASK_KEY) = maskArray_Conflict
			Else
				phMap(MASK_KEY) = layerConf().onesMaskForInput(input_Conflict)
			End If

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

			Dim [out] As IDictionary(Of String, INDArray) = sameDiff.output(phMap, outputKey)
			Dim result As INDArray = [out](outputKey)

			'Edge case - identity activation
			'TODO there may be a cleaner way to do this...
			If Not actScopedOut AndAlso Not result.data().ParentWorkspace.Id.Equals(wsNameOutput) Then
				result = workspaceMgr.dup(ArrayType.ACTIVATIONS, result)
			ElseIf actScopedOut AndAlso result.Attached Then
				result = result.detach()
			End If


			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()

			Return result
		End Function


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim g As Gradient = New DefaultGradient()

			Dim dLdIn As INDArray

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					doInit()
				End If
				If Not sameDiff.hasGradientFunction() Then
					'Create when scoped out, to ensure any arrays are not in WS
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


			Dim bl As org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer = DirectCast(layerConf(), org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer)
			bl.validateInput(input_Conflict)

			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			phMap(INPUT_KEY) = input_Conflict
			phMap(fn.GradPlaceholderName) = epsilon
			If maskArray_Conflict IsNot Nothing Then
				phMap(MASK_KEY) = maskArray_Conflict
			Else
				phMap(MASK_KEY) = layerConf().onesMaskForInput(input_Conflict)
			End If

			Dim requiredGrads As IList(Of String) = New List(Of String)(paramTable_Conflict.Count + 1)
			requiredGrads.Add(INPUT_KEY)
			CType(requiredGrads, List(Of String)).AddRange(paramTable_Conflict.Keys)

			Dim m As IDictionary(Of String, INDArray) = sameDiff.calculateGradients(phMap, requiredGrads)
			For Each s As String In paramTable_Conflict.Keys
				Dim sdGrad As INDArray = m(s)
				Dim dl4jGrad As INDArray = gradTable(s)
				dl4jGrad.assign(sdGrad) 'TODO OPTIMIZE THIS
				g.gradientForVariable()(s) = dl4jGrad
			Next s

			dLdIn = m(INPUT_KEY)


			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()

			Dim ret As New Pair(Of Gradient, INDArray)(g, workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, dLdIn)) 'TODO OPTIMIZE THIS
			Return ret
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
				If Me.params_Conflict Is Nothing AndAlso params Is Nothing Then
					Return
				End If
				If Me.params_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Cannot set parameters of length " & params.length() & " to a layer with no parameters")
				End If
				If params Is Nothing Then
					Throw New System.InvalidOperationException("Cannot set null parameters")
				End If
    
				Preconditions.checkState(Me.params_Conflict.length() = params.length(), "Cannot assign parameter vector of length %s to a layer with %s parameters", params.length(), Me.params_Conflict.length())
				Me.params_Conflict.assign(params)
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
				Dim bl As org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer = DirectCast(layerConf(), org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer)
				sameDiff = SameDiff.create()
				'Use SingleThreadArrayHolder so we can use views (also don't nede multithreading here, DL4J is not thread safe)
				sameDiff.setArrayHolders(New SingleThreadArrayHolder(), New SingleThreadArrayHolder(), False)
				Dim p As IDictionary(Of String, INDArray) = paramTable()

				Dim inputShape() As Long = CType(input_Conflict.shape().Clone(), Long())
				inputShape(0) = -1
				Dim inputVar As SDVariable = sameDiff.placeHolder(INPUT_KEY, dataType, inputShape)
				Dim paramShapes As IDictionary(Of String, Long()) = layerConf().getLayerParams().getParamShapes()
				Dim params As IDictionary(Of String, SDVariable) = New LinkedHashMap(Of String, SDVariable)()
				For Each s As String In paramShapes.Keys
					Dim ps As val = paramShapes(s)
					Dim v As SDVariable = sameDiff.var(s, dataType, ps)
					params(s) = v
				Next s

				Dim maskShape() As Long = ArrayUtil.nTimes(CLng(inputShape.Length), -1)
				Dim mask As SDVariable = sameDiff.placeHolder(MASK_KEY, dataType, maskShape)

				Dim layerOutput As SDVariable = bl.defineLayer(sameDiff, inputVar, params, mask)
				Preconditions.checkNotNull(layerOutput, "Invalid output: layer output is null")
				outputVar = layerOutput

				For Each e As KeyValuePair(Of String, INDArray) In p.SetOfKeyValuePairs()
					sameDiff.associateArrayWithVariable(e.Value, sameDiff.getVariable(e.Key))
				Next e

				'Define the function for external errors:
				fn = SameDiffUtils.externalErrors(sameDiff, Nothing,layerOutput)
				fn.outputVariable()

				Me.outputKey = outputVar.name()
			End Using
		End Sub

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Dim bl As org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer = DirectCast(layerConf(), org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer)

			Me.maskArray_Conflict = maskArray
			Me.maskState = currentMaskState

			Return bl.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
		End Function

	End Class

End Namespace