Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports RecurrentLayer = org.deeplearning4j.nn.api.layers.RecurrentLayer
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports BidirectionalParamInitializer = org.deeplearning4j.nn.params.BidirectionalParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.nn.layers.recurrent


	<Serializable>
	Public Class BidirectionalLayer
		Implements RecurrentLayer

'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private conf_Conflict As NeuralNetConfiguration
		Private fwd As Layer
		Private bwd As Layer

		Private layerConf As Bidirectional
		Private paramsView As INDArray
		Private gradientView As INDArray
		<NonSerialized>
		Private gradientViews As IDictionary(Of String, INDArray)
'JAVA TO VB CONVERTER NOTE: The field input was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private input_Conflict As INDArray

		'Next 2 variables: used *only* for MUL case (needed for backprop)
		Private outFwd As INDArray
		Private outBwd As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BidirectionalLayer(@NonNull NeuralNetConfiguration conf, @NonNull Layer fwd, @NonNull Layer bwd, @NonNull INDArray paramsView)
		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal fwd As Layer, ByVal bwd As Layer, ByVal paramsView As INDArray)
			Me.conf_Conflict = conf
			Me.fwd = fwd
			Me.bwd = bwd
			Me.layerConf = CType(conf.getLayer(), Bidirectional)
			Me.paramsView = paramsView
		End Sub

		Private ReadOnly Property RNNDataFormat As RNNFormat
			Get
				Return layerConf.RNNDataFormat
			End Get
		End Property
		Public Overridable Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements RecurrentLayer.rnnTimeStep
			Throw New System.NotSupportedException("Cannot RnnTimeStep bidirectional layers")
		End Function

		Public Overridable Function rnnGetPreviousState() As IDictionary(Of String, INDArray)
			Throw New System.NotSupportedException("Not supported: cannot RnnTimeStep bidirectional layers therefore " & "no previous state is supported")
		End Function

		Public Overridable Sub rnnSetPreviousState(ByVal stateMap As IDictionary(Of String, INDArray))
			Throw New System.NotSupportedException("Not supported: cannot RnnTimeStep bidirectional layers therefore " & "no previous state is supported")
		End Sub

		Public Overridable Sub rnnClearPreviousState() Implements RecurrentLayer.rnnClearPreviousState
			'No op
		End Sub

		Public Overridable Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements RecurrentLayer.rnnActivateUsingStoredState
			Throw New System.NotSupportedException("Not supported: cannot use this method (or truncated BPTT) with bidirectional layers")
		End Function

		Public Overridable Function rnnGetTBPTTState() As IDictionary(Of String, INDArray)
			Throw New System.NotSupportedException("Not supported: cannot use this method (or truncated BPTT) with bidirectional layers")
		End Function

		Public Overridable Sub rnnSetTBPTTState(ByVal state As IDictionary(Of String, INDArray))
			Throw New System.NotSupportedException("Not supported: cannot use this method (or truncated BPTT) with bidirectional layers")
		End Sub

		Public Overridable Function tbpttBackpropGradient(ByVal epsilon As INDArray, ByVal tbpttBackLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements RecurrentLayer.tbpttBackpropGradient
			Throw New System.NotSupportedException("Not supported: cannot use this method (or truncated BPTT) with bidirectional layers")
		End Function

		Public Overridable WriteOnly Property CacheMode As CacheMode
			Set(ByVal mode As CacheMode)
				fwd.CacheMode = mode
				bwd.CacheMode = mode
			End Set
		End Property

		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return fwd.calcRegularizationScore(backpropParamsOnly) + bwd.calcRegularizationScore(backpropParamsOnly)
		End Function

		Public Overridable Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overridable Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim eFwd As INDArray
			Dim eBwd As INDArray
			Dim permute As Boolean = RNNDataFormat = RNNFormat.NWC AndAlso epsilon.rank() = 3
			If permute Then
				epsilon = epsilon.permute(0, 2, 1)
			End If
			Dim n As val = epsilon.size(1)\2
			Select Case layerConf.getMode()
				Case ADD
					eFwd = epsilon
					eBwd = epsilon
				Case MUL
					eFwd = epsilon.dup(epsilon.ordering()).muli(outBwd)
					eBwd = epsilon.dup(epsilon.ordering()).muli(outFwd)
				Case AVERAGE
					eFwd = epsilon.dup(epsilon.ordering()).muli(0.5)
					eBwd = eFwd
				Case CONCAT
					eFwd = epsilon.get(all(), interval(0,n), all())
					eBwd = epsilon.get(all(), interval(n, 2*n), all())
				Case Else
					Throw New Exception("Unknown mode: " & layerConf.getMode())
			End Select

			eBwd = TimeSeriesUtils.reverseTimeSeries(eBwd, workspaceMgr, ArrayType.BP_WORKING_MEM)

			If permute Then
				eFwd = eFwd.permute(0, 2, 1)
				eBwd = eBwd.permute(0, 2, 1)
			End If
			Dim g1 As Pair(Of Gradient, INDArray) = fwd.backpropGradient(eFwd, workspaceMgr)
			Dim g2 As Pair(Of Gradient, INDArray) = bwd.backpropGradient(eBwd, workspaceMgr)

			Dim g As Gradient = New DefaultGradient(gradientView)
			For Each e As KeyValuePair(Of String, INDArray) In g1.First.gradientForVariable().SetOfKeyValuePairs()
				g.gradientForVariable()(BidirectionalParamInitializer.FORWARD_PREFIX & e.Key) = e.Value
			Next e
			For Each e As KeyValuePair(Of String, INDArray) In g2.First.gradientForVariable().SetOfKeyValuePairs()
				g.gradientForVariable()(BidirectionalParamInitializer.BACKWARD_PREFIX & e.Key) = e.Value
			Next e

			Dim g2Right As INDArray = If(permute, g2.Right.permute(0, 2, 1), g2.Right)
			Dim g2Reversed As INDArray = TimeSeriesUtils.reverseTimeSeries(g2Right, workspaceMgr, ArrayType.BP_WORKING_MEM)
			g2Reversed = If(permute, g2Reversed.permute(0, 2, 1), g2Reversed)
			Dim epsOut As INDArray = g1.Right.addi(g2Reversed)

			Return New Pair(Of Gradient, INDArray)(g, epsOut)
		End Function

		Public Overridable Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim out1 As INDArray = fwd.activate(training, workspaceMgr)
			Dim out2 As INDArray = bwd.activate(training, workspaceMgr)
			Dim permute As Boolean = RNNDataFormat = RNNFormat.NWC AndAlso out1.rank() = 3
			If permute Then
				out1 = out1.permute(0, 2, 1)
				out2 = out2.permute(0, 2, 1)
			End If
			'Reverse the output time series. Note: when using LastTimeStepLayer, output can be rank 2
			out2 = If(out2.rank() = 2, out2, TimeSeriesUtils.reverseTimeSeries(out2, workspaceMgr, ArrayType.FF_WORKING_MEM))
			Dim ret As INDArray
			Select Case layerConf.getMode()
				Case ADD
					ret = out1.addi(out2)
				Case MUL
					'TODO may be more efficient ways than this...
					Me.outFwd = out1.detach()
					Me.outBwd = out2.detach()
					ret = workspaceMgr.dup(ArrayType.ACTIVATIONS, out1).muli(out2)
				Case AVERAGE
					ret = out1.addi(out2).muli(0.5)
				Case CONCAT
					ret = Nd4j.concat(1, out1, out2)
					ret = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
				Case Else
					Throw New Exception("Unknown mode: " & layerConf.getMode())
			End Select
			If permute Then
				ret = ret.permute(0, 2, 1)
			End If
			Return ret
		End Function

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return activate(training, workspaceMgr)
		End Function

		Public Overridable Property Listeners As ICollection(Of TrainingListener)
			Get
				Return fwd.getListeners()
			End Get
			Set(ByVal listeners() As TrainingListener)
				fwd.setListeners(listeners)
				bwd.setListeners(listeners)
			End Set
		End Property


		Public Overridable Sub addListeners(ParamArray ByVal listener() As TrainingListener)
			fwd.addListeners(listener)
			bwd.addListeners(listener)
		End Sub

		Public Overridable Sub fit()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Sub update(ByVal gradient As Gradient)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function score() As Double
			Return fwd.score() + bwd.score()
		End Function

		Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			fwd.computeGradientAndScore(workspaceMgr)
			bwd.computeGradientAndScore(workspaceMgr)
		End Sub

		Public Overridable Function params() As INDArray
			Return paramsView
		End Function

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return conf_Conflict.getLayer()
			End Get
		End Property

		Public Overridable Function numParams() As Long
			Return fwd.numParams() + bwd.numParams()
		End Function

		Public Overridable Function numParams(ByVal backwards As Boolean) As Long
			Return fwd.numParams(backwards) + bwd.numParams(backwards)
		End Function

		Public Overridable WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				Me.paramsView.assign(params)
			End Set
		End Property

		Public Overridable WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				Me.paramsView = params
				Dim n As val = params.length()
				fwd.ParamsViewArray = params.get(interval(0, 0, True), interval(0, n))
				bwd.ParamsViewArray = params.get(interval(0, 0, True), interval(n, 2*n))
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return gradientView
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				If Me.paramsView IsNot Nothing AndAlso gradients.length() <> numParams() Then
					Throw New System.ArgumentException("Invalid input: expect gradients array of length " & numParams(True) & ", got array of length " & gradients.length())
				End If
    
				Me.gradientView = gradients
				Dim n As val = gradients.length() \ 2
				Dim g1 As INDArray = gradients.get(interval(0, 0, True), interval(0,n))
				Dim g2 As INDArray = gradients.get(interval(0, 0, True), interval(n, 2*n))
				fwd.BackpropGradientsViewArray = g1
				bwd.BackpropGradientsViewArray = g2
			End Set
		End Property

		Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function gradient() As Gradient
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function batchSize() As Integer
			Return fwd.batchSize()
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return conf_Conflict
		End Function

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				Me.conf_Conflict = conf
			End Set
		End Property

		Public Overridable Function input() As INDArray
			Return input_Conflict
		End Function

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function getParam(ByVal param As String) As INDArray
			Dim [sub] As String = param.Substring(1)
			If param.StartsWith(BidirectionalParamInitializer.FORWARD_PREFIX, StringComparison.Ordinal) Then
				Return fwd.getParam([sub])
			Else
				Return bwd.getParam([sub])
			End If
		End Function

		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Dim m As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In fwd.paramTable(backpropParamsOnly).SetOfKeyValuePairs()
				m(BidirectionalParamInitializer.FORWARD_PREFIX & e.Key) = e.Value
			Next e
			For Each e As KeyValuePair(Of String, INDArray) In bwd.paramTable(backpropParamsOnly).SetOfKeyValuePairs()
				m(BidirectionalParamInitializer.BACKWARD_PREFIX & e.Key) = e.Value
			Next e
			Return m
		End Function

		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			Dim [sub] As String = paramName.Substring(1)
			If paramName.StartsWith(BidirectionalParamInitializer.FORWARD_PREFIX, StringComparison.Ordinal) Then
				Return fwd.updaterDivideByMinibatch(paramName)
			Else
				Return bwd.updaterDivideByMinibatch(paramName)
			End If
		End Function

		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				For Each e As KeyValuePair(Of String, INDArray) In paramTable.SetOfKeyValuePairs()
					setParam(e.Key, e.Value)
				Next e
			End Set
		End Property

		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray)
			Dim [sub] As String = key.Substring(1)
			If key.StartsWith(BidirectionalParamInitializer.FORWARD_PREFIX, StringComparison.Ordinal) Then
				fwd.setParam([sub], val)
			Else
				bwd.setParam([sub], val)
			End If
		End Sub

		Public Overridable Sub clear()
			fwd.clear()
			bwd.clear()
			input_Conflict = Nothing
			outFwd = Nothing
			outBwd = Nothing
		End Sub

		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)
			fwd.applyConstraints(iteration, epoch)
			bwd.applyConstraints(iteration, epoch)
		End Sub

		Public Overridable Sub init()
			'No op
		End Sub

		Public Overridable WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				fwd.setListeners(listeners)
				bwd.setListeners(listeners)
			End Set
		End Property

		Public Overridable Property Index As Integer
			Set(ByVal index As Integer)
				fwd.Index = index
				bwd.Index = index
			End Set
			Get
				Return fwd.Index
			End Get
		End Property


		Public Overridable Property IterationCount As Integer
			Get
				Return fwd.IterationCount
			End Get
			Set(ByVal iterationCount As Integer)
				fwd.IterationCount = iterationCount
				bwd.IterationCount = iterationCount
			End Set
		End Property

		Public Overridable Property EpochCount As Integer
			Get
				Return fwd.EpochCount
			End Get
			Set(ByVal epochCount As Integer)
				fwd.EpochCount = epochCount
				bwd.EpochCount = epochCount
			End Set
		End Property



		Public Overridable Sub setInput(ByVal input As INDArray, ByVal layerWorkspaceMgr As LayerWorkspaceMgr)
			Me.input_Conflict = input
			fwd.setInput(input, layerWorkspaceMgr)
			If RNNDataFormat = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			Dim reversed As INDArray
			If Not input.Attached Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					reversed = TimeSeriesUtils.reverseTimeSeries(input)
				End Using
			Else
				Dim ws As MemoryWorkspace = input.data().ParentWorkspace
				Using ws2 As org.nd4j.linalg.api.memory.MemoryWorkspace = ws.notifyScopeBorrowed()
					'Put the reversed input into the same workspace as the original input
					reversed = TimeSeriesUtils.reverseTimeSeries(input)
				End Using
			End If
			If RNNDataFormat = RNNFormat.NWC Then
				reversed = reversed.permute(0, 2, 1)
			End If
			bwd.setInput(reversed, layerWorkspaceMgr)
		End Sub

		Public Overridable Property InputMiniBatchSize As Integer
			Set(ByVal size As Integer)
				fwd.InputMiniBatchSize = size
				bwd.InputMiniBatchSize = size
			End Set
			Get
				Return fwd.InputMiniBatchSize
			End Get
		End Property


		Public Overridable Property MaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				fwd.MaskArray = maskArray
				bwd.MaskArray = TimeSeriesUtils.reverseTimeSeriesMask(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT) 'TODO
			End Set
			Get
				Return fwd.MaskArray
			End Get
		End Property


		Public Overridable ReadOnly Property PretrainLayer As Boolean
			Get
				Return fwd.PretrainLayer
			End Get
		End Property

		Public Overridable Sub clearNoiseWeightParams()
			fwd.clearNoiseWeightParams()
			bwd.clearNoiseWeightParams()
		End Sub

		Public Overridable Sub allowInputModification(ByVal allow As Boolean)
			fwd.allowInputModification(allow)
			bwd.allowInputModification(True) 'Always allow: always safe due to reverse op
		End Sub

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			Dim ret As Pair(Of INDArray, MaskState) = fwd.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
			bwd.feedForwardMaskArray(TimeSeriesUtils.reverseTimeSeriesMask(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT), currentMaskState, minibatchSize)
			Return ret
		End Function

		Public Overridable ReadOnly Property Helper As LayerHelper
			Get
				Dim f As LayerHelper = fwd.Helper
				Dim b As LayerHelper = bwd.Helper
				If f IsNot Nothing OrElse b IsNot Nothing Then
					Return New BidirectionalHelper(f,b)
				End If
				Return Nothing
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor private static class BidirectionalHelper implements org.deeplearning4j.nn.layers.LayerHelper
		Private Class BidirectionalHelper
			Implements LayerHelper

			Friend ReadOnly helperFwd As LayerHelper
			Friend ReadOnly helperBwd As LayerHelper

			Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
				Dim fwd As IDictionary(Of String, Long) = (If(helperFwd IsNot Nothing, helperFwd.helperMemoryUse(), Nothing))
				Dim bwd As IDictionary(Of String, Long) = (If(helperBwd IsNot Nothing, helperBwd.helperMemoryUse(), Nothing))

				Dim keys As ISet(Of String) = New HashSet(Of String)()
				If fwd IsNot Nothing Then
					keys.addAll(fwd.Keys)
				End If
				If bwd IsNot Nothing Then
					keys.addAll(bwd.Keys)
				End If

				Dim ret As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
				For Each s As String In keys
					Dim sum As Long = 0
					If fwd IsNot Nothing AndAlso fwd.ContainsKey(s) Then
						sum += fwd(s)
					End If
					If bwd IsNot Nothing AndAlso bwd.ContainsKey(s) Then
						sum += bwd(s)
					End If
					ret(s) = sum
				Next s
				Return ret
			End Function

			Public Overridable Function checkSupported() As Boolean Implements LayerHelper.checkSupported
				Return True
			End Function
		End Class

		Public Overridable Sub close()
			'No-op for individual layers
		End Sub
	End Class

End Namespace