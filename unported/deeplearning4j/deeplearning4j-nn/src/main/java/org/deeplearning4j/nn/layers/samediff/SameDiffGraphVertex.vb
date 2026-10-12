Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports SDVertexParams = org.deeplearning4j.nn.conf.layers.samediff.SDVertexParams
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports BaseGraphVertex = org.deeplearning4j.nn.graph.vertex.BaseGraphVertex
Imports SameDiffParamInitializer = org.deeplearning4j.nn.params.SameDiffParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
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
	Public Class SameDiffGraphVertex
		Inherits BaseGraphVertex

'JAVA TO VB CONVERTER NOTE: The field config was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend config_Conflict As SameDiffVertex
		Protected Friend sameDiff As SameDiff
		Protected Friend outputVar As SDVariable
		Protected Friend fn As ExternalErrorsFunction
		Protected Friend outputKey As String
		Protected Friend inputVars As IDictionary(Of String, SDVariable)
		Protected Friend maskArrays() As INDArray

'JAVA TO VB CONVERTER NOTE: The field params was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend params_Conflict As INDArray
		Protected Friend gradients As INDArray
'JAVA TO VB CONVERTER NOTE: The field paramTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend paramTable_Conflict As IDictionary(Of String, INDArray)
		Protected Friend gradTable As IDictionary(Of String, INDArray)
		Private currentMaskState As MaskState
		Private minibatchSize As Integer

		Public Sub New(ByVal config As SameDiffVertex, ByVal graph As ComputationGraph, ByVal name As String, ByVal vertexIndex As Integer, ByVal paramsView As INDArray, ByVal initParams As Boolean, ByVal dataType As DataType)
			MyBase.New(graph, name, vertexIndex, Nothing, Nothing, dataType)
			Me.config_Conflict = config
			Dim vp As SDVertexParams = config.VertexParams
			paramTable_Conflict = SameDiffParamInitializer.Instance.subsetAndReshape(vp.getParameterKeys(), vp.getParamShapes(), paramsView, Nothing, config)
			If initParams Then
				config.initializeParameters(paramTable_Conflict)
			End If
			Me.params_Conflict = paramsView
		End Sub

		Public Overrides Function ToString() As String
			Return Nothing
		End Function

		Public Overrides Function hasLayer() As Boolean
			Return False
		End Function

		Public Overrides ReadOnly Property Layer As Layer
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function doForward(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					doInit()
				End If
			End Using

			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			config_Conflict.validateInput(inputs)
			For i As Integer = 0 To inputs.Length - 1
				Dim name As String = config_Conflict.VertexParams.getInputs().get(i)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String maskName = name + "_mask";
				Dim maskName As String = name & "_mask"
				phMap(name) = inputs(i)
				If maskArrays IsNot Nothing AndAlso maskArrays(i) IsNot Nothing Then
					phMap(maskName) = maskArrays(i)
				Else
					phMap(maskName) = createMask(dataType, inputs(i).shape())
				End If
			Next i


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

			Dim result As INDArray = sameDiff.outputSingle(phMap, outputKey)

			'Edge case: "vertex" is just an identity activation, for example
			'TODO there may be a cleaner way to do this...
			If Not actScopedOut AndAlso Not result.data().ParentWorkspace.Id.Equals(wsNameOutput) Then
				result = workspaceMgr.dup(ArrayType.ACTIVATIONS, result)
			ElseIf actScopedOut AndAlso result.Attached Then
				result = result.detach()
			End If

			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()
			Return workspaceMgr.dup(ArrayType.ACTIVATIONS, result)
		End Function

		Public Overrides Function doBackward(ByVal tbptt As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray())
			Dim g As Gradient = New DefaultGradient()

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				If sameDiff Is Nothing Then
					doInit()
				End If
			End Using

			Dim inputNames As IList(Of String) = config_Conflict.VertexParams.getInputs()
			If Not sameDiff.hasGradientFunction() Then
				'Create when scoped out, to ensure any arrays are not in WS
				Dim inArr() As String = CType(inputNames, List(Of String)).ToArray()
				sameDiff.createGradFunction(inArr)
			End If
			config_Conflict.validateInput(inputs)

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



			Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim inputs As IList(Of String) = config_Conflict.VertexParams.getInputs()
			Dim i As Integer=0
			For Each s As String In inputs
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: phMap.put(s, this.inputs[i++]);
				phMap(s) = Me.inputs(i)
					i += 1
			Next s
			For j As Integer = 0 To Me.inputs.Length - 1
				Dim name As String = inputs(j)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String maskName = name + "_mask";
				Dim maskName As String = name & "_mask"
				If maskArrays IsNot Nothing AndAlso maskArrays(j) IsNot Nothing Then
					phMap(maskName) = maskArrays(j)
				Else
					phMap(maskName) = createMask(dataType, Me.inputs(j).shape())
				End If
			Next j
			Dim epsName As String = fn.GradPlaceholderName
			phMap(epsName) = epsilon_Conflict

			Dim required As IList(Of String) = New List(Of String)(config_Conflict.VertexParams.getInputs()) 'Ensure that the input placeholder gradients are calculated
			CType(required, List(Of String)).AddRange(paramTable_Conflict.Keys)

			Dim gradsMap As IDictionary(Of String, INDArray) = sameDiff.calculateGradients(phMap, required)
			For Each s As String In paramTable_Conflict.Keys
				Dim sdGrad As INDArray = gradsMap(s)
				Dim dl4jGrad As INDArray = gradTable(s)
				dl4jGrad.assign(sdGrad) 'TODO OPTIMIZE THIS
				g.gradientForVariable()(s) = dl4jGrad
			Next s

			Dim dLdIns(inputs.Count - 1) As INDArray
			Dim fnName As String = fn.GradPlaceholderName
			For j As Integer = 0 To inputs.Count - 1
				Dim name As String = inputs(j)
				dLdIns(j) = sameDiff.grad(name).Arr

				Dim gradName As String = sameDiff.grad(inputNames(j)).name()
				If dLdIns(j) Is Nothing AndAlso fnName.Equals(gradName) Then
					'Edge case with lambda vertices like identity: SameDiff doesn't store the placeholders
					' So, this getArr() can be trying to get placeholder from SameDiff instance, when it's available here
					dLdIns(j) = epsilon_Conflict
				End If

				'Edge case: "vertex" is just an identity activation, for example
				'TODO there may be a cleaner way to do this...
				If Not actGradScopedOut AndAlso Not dLdIns(j).data().ParentWorkspace.Id.Equals(wsNameActGrad) Then
					dLdIns(j) = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, dLdIns(j))
				ElseIf actGradScopedOut AndAlso dLdIns(j).Attached Then
					dLdIns(j) = dLdIns(j).detach()
				End If
			Next j

			'Clear placeholders and op inputs to ensure no out-of-scope arrays are still referenced anywhere
			sameDiff.clearPlaceholders(True)
			sameDiff.clearOpInputs()
			Return New Pair(Of Gradient, INDArray())(g, dLdIns)
		End Function

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal backpropGradientsViewArray As INDArray)
				Dim vp As SDVertexParams = config_Conflict.VertexParams
				gradTable = SameDiffParamInitializer.Instance.subsetAndReshape(vp.getParameterKeys(), vp.getParamShapes(), backpropGradientsViewArray, Nothing, config_Conflict)
			End Set
		End Property

		Public Overrides Function feedForwardMaskArrays(ByVal maskArrays() As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Me.maskArrays = maskArrays
			Me.currentMaskState = currentMaskState

			Return config_Conflict.feedForwardMaskArrays(maskArrays, currentMaskState, minibatchSize)
		End Function


		Protected Friend Overridable Sub doInit()
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				sameDiff = SameDiff.create()
				'Use SingleThreadArrayHolder so we can use views (also don't nede multithreading here, DL4J is not thread safe)
				sameDiff.setArrayHolders(New SingleThreadArrayHolder(), New SingleThreadArrayHolder(), False)

				inputVars = New LinkedHashMap(Of String, SDVariable)()
				Dim maskVars As New LinkedHashMap(Of String, SDVariable)()
				Dim i As Integer=0
				For Each s As String In config_Conflict.VertexParams.getInputs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: lombok.val inputShape = inputs[i++].shape().clone();
					Dim inputShape As val = CType(inputs(i).shape().Clone(), Long())
						i += 1
					Dim maskTemp As INDArray = createMask(dataType, inputShape)
					inputShape(0) = -1
					Dim inputVar As SDVariable = sameDiff.placeHolder(s, dataType, inputShape)
					inputVars(s) = inputVar
					Dim maskShape() As Long = CType(maskTemp.shape().Clone(), Long())
					maskShape(0) = -1
					Dim maskVar As SDVariable = sameDiff.placeHolder(s & "_mask", maskTemp.dataType(), maskShape)
					maskVars.put(s, maskVar)
				Next s

				Dim paramShapes As IDictionary(Of String, Long()) = config_Conflict.VertexParams.getParamShapes()
				Dim params As IDictionary(Of String, SDVariable) = New LinkedHashMap(Of String, SDVariable)()
				For Each s As String In paramShapes.Keys
					Dim ps As val = paramShapes(s)
					Dim v As SDVariable = sameDiff.var(s, dataType, ps)
					params(s) = v
				Next s
				Dim layerOutput As SDVariable = config_Conflict.defineVertex(sameDiff, inputVars, params, maskVars)
				Preconditions.checkNotNull(layerOutput, "Invalid output: layer output is null")
				outputVar = layerOutput

				For Each e As KeyValuePair(Of String, INDArray) In paramTable_Conflict.SetOfKeyValuePairs()
					sameDiff.associateArrayWithVariable(e.Value, sameDiff.getVariable(e.Key))
				Next e

				'Define the function for external errors:
				fn = SameDiffUtils.externalErrors(sameDiff, Nothing, layerOutput)
				fn.outputVariable()

				Me.outputKey = outputVar.name()
			End Using
		End Sub

		Public Overrides Sub clearVertex()
			clear()
		End Sub

		Public Overrides Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray)
			Return paramTable_Conflict
		End Function

		Public Overrides ReadOnly Property Config As TrainingConfig
			Get
				Return config_Conflict
			End Get
		End Property

		Public Overrides Function params() As INDArray
			Return params_Conflict
		End Function

		Public Overrides ReadOnly Property GradientsViewArray As INDArray
			Get
				Return gradients
			End Get
		End Property

		'Package private
		Friend Shared Function createMask(ByVal dataType As DataType, ByVal shape() As Long) As INDArray
			Select Case shape.Length
				Case 2 ' FF-Type input
					Return Nd4j.ones(dataType,shape(0), 1)
				Case 3 ' RNN-Type input
					Return Nd4j.ones(dataType, shape(0), shape(2))
				Case 4 'CNN input
					Return Nd4j.ones(dataType, shape(0), 1, 1, 1)
				Case Else
					Preconditions.throwEx("Can not create all-ones-mask for given input shape %s.", java.util.Arrays.toString(shape))
					Return Nothing
			End Select
		End Function
	End Class



End Namespace