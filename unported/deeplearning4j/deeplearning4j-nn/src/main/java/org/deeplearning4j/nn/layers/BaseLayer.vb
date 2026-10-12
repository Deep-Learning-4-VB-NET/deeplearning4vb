Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Solver = org.deeplearning4j.optimize.Solver
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LayerNorm = org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm
Imports LayerNormBp = org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNormBp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
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

Namespace org.deeplearning4j.nn.layers


	''' <summary>
	''' A layer with parameters
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseLayer<LayerConfT extends org.deeplearning4j.nn.conf.layers.BaseLayer> extends AbstractLayer<LayerConfT>
	<Serializable>
	Public MustInherit Class BaseLayer(Of LayerConfT As org.deeplearning4j.nn.conf.layers.BaseLayer)
		Inherits AbstractLayer(Of LayerConfT)

		Protected Friend paramsFlattened As INDArray
		Protected Friend gradientsFlattened As INDArray
'JAVA TO VB CONVERTER NOTE: The field params was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend params_Conflict As IDictionary(Of String, INDArray)
		<NonSerialized>
		Protected Friend gradientViews As IDictionary(Of String, INDArray)
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend score_Conflict As Double = 0.0
'JAVA TO VB CONVERTER NOTE: The field optimizer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend optimizer_Conflict As ConvexOptimizer
'JAVA TO VB CONVERTER NOTE: The field gradient was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradient_Conflict As Gradient
		Protected Friend solver As Solver

		Protected Friend weightNoiseParams As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function layerConf() As LayerConfT
			Return CType(Me.conf_Conflict.getLayer(), LayerConfT)
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			'If this layer is layer L, then epsilon is (w^(L+1)*(d^(L+1))^T) (or equivalent)
			Dim zAndPreNorm As Pair(Of INDArray, INDArray) = preOutputWithPreNorm(True, True, workspaceMgr)
			Dim z As INDArray = zAndPreNorm.First 'Note: using preOutput(INDArray) can't be used as this does a setInput(input) and resets the 'appliedDropout' flag
			Dim preNorm As INDArray = zAndPreNorm.Second
			Dim delta As INDArray = layerConf().getActivationFn().backprop(z, epsilon).getFirst() 'TODO handle activation function params

			If maskArray_Conflict IsNot Nothing Then
				applyMask(delta)
			End If

			Dim ret As Gradient = New DefaultGradient()

			If hasBias() Then
				Dim biasGrad As INDArray = gradientViews(DefaultParamInitializer.BIAS_KEY)
				delta.sum(biasGrad, 0) 'biasGrad is initialized/zeroed first
				ret.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = biasGrad
			End If

			Dim W As INDArray = getParamWithNoise(DefaultParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim epsilonNext As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, delta.dataType(), New Long(){W.size(0), delta.size(0)}, "f"c)
			If hasLayerNorm() Then
				Dim g As INDArray = getParam(DefaultParamInitializer.GAIN_KEY)

				Dim dldg As INDArray = gradientViews(DefaultParamInitializer.GAIN_KEY)
				Nd4j.Executioner.exec(New LayerNormBp(preNorm, g, delta, delta, dldg, True, 1))
				ret.gradientForVariable()(DefaultParamInitializer.GAIN_KEY) = dldg

			End If

			epsilonNext = W.mmuli(delta.transpose(),epsilonNext).transpose() 'W.mmul(delta.transpose()).transpose();

			Dim weightGrad As INDArray = gradientViews(DefaultParamInitializer.WEIGHT_KEY) 'f order
			Nd4j.gemm(input_Conflict.castTo(weightGrad.dataType()), delta, weightGrad, True, False, 1.0, 0.0) 'TODO avoid castTo?
			ret.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = weightGrad

			weightNoiseParams.Clear()

			epsilonNext = backpropDropOutIfPresent(epsilonNext)
			Return New Pair(Of Gradient, INDArray)(ret, epsilonNext)
		End Function

		Public Overrides Sub fit()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			If Me.input_Conflict Is Nothing Then
				Return
			End If

			Dim output As INDArray = activate(True, workspaceMgr)
			ScoreWithZ = output
		End Sub


		Protected Friend Overridable WriteOnly Property ScoreWithZ As INDArray
			Set(ByVal z As INDArray)
			End Set
		End Property

		''' <summary>
		''' Objective function:  the specified objective </summary>
		''' <returns> the score for the objective </returns>

		Public Overrides Function score() As Double
			Return score_Conflict
		End Function

		Public Overrides Function gradient() As Gradient
			Return gradient_Conflict
		End Function

		Public Overrides Sub update(ByVal gradient As Gradient)
			For Each paramType As String In gradient.gradientForVariable().Keys
				update(gradient.getGradientFor(paramType), paramType)
			Next paramType
		End Sub

		Public Overrides Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			setParam(paramType, getParam(paramType).addi(gradient))
		End Sub


		Public Overrides ReadOnly Property Optimizer As ConvexOptimizer
			Get
				If optimizer_Conflict Is Nothing Then
					Dim solver As Solver = (New Solver.Builder()).model(Me).configure(conf()).build()
					Me.optimizer_Conflict = solver.Optimizer
				End If
				Return optimizer_Conflict
			End Get
		End Property

		''' <summary>
		'''Returns the parameters of the neural network as a flattened row vector </summary>
		''' <returns> the parameters of the neural network </returns>
		Public Overrides Function params() As INDArray
			Return paramsFlattened
		End Function

		Public Overrides Function getParam(ByVal param As String) As INDArray
			Return params(param)
		End Function

		Public Overrides Sub setParam(ByVal key As String, ByVal val As INDArray)
			If params_Conflict.ContainsKey(key) Then
				params(key).assign(val)
			Else
				params(key) = val
			End If
		End Sub

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				If params Is paramsFlattened Then
					Return 'no op
				End If
				setParams(params, "f"c)
			End Set
		End Property

		Protected Friend Overrides Sub setParams(ByVal params As INDArray, ByVal order As Char)
			Dim parameterList As IList(Of String) = conf_Conflict.variables()
			Dim length As Integer = 0
			For Each s As String In parameterList
				length += getParam(s).length()
			Next s
			If params.length() <> length Then
				Throw New System.ArgumentException("Unable to set parameters: must be of length " & length & ", got params of length " & params.length() & " - " & layerId())
			End If
			Dim idx As Integer = 0
			Dim paramKeySet As ISet(Of String) = Me.params_Conflict.Keys
			For Each s As String In paramKeySet
				Dim param As INDArray = getParam(s)
				Dim get As INDArray = params.get(NDArrayIndex.point(0), NDArrayIndex.interval(idx, idx + param.length()))
				If param.length() <> get.length() Then
					Throw New System.InvalidOperationException("Parameter " & s & " should have been of length " & param.length() & " but was " & get.length() & " - " & layerId())
				End If
				param.assign(get.reshape(order, param.shape())) 'Use assign due to backprop params being a view of a larger array
				idx += param.length()
			Next s
		End Sub

		Public Overrides WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				If Me.params_Conflict IsNot Nothing AndAlso params.length() <> numParams() Then
					Throw New System.ArgumentException("Invalid input: expect params of length " & numParams() & ", got params of length " & params.length() & " - " & layerId())
				End If
    
				Me.paramsFlattened = params
			End Set
		End Property

		Public Overrides ReadOnly Property GradientsViewArray As INDArray
			Get
				Return gradientsFlattened
			End Get
		End Property

		Public Overrides WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				If Me.params_Conflict IsNot Nothing AndAlso gradients.length() <> numParams() Then
					Throw New System.ArgumentException("Invalid input: expect gradients array of length " & numParams(True) & ", got array of length " & gradients.length() & " - " & layerId())
				End If
    
				Me.gradientsFlattened = gradients
				Me.gradientViews = conf_Conflict.getLayer().initializer().getGradientsFromFlattened(conf_Conflict, gradients)
			End Set
		End Property

		Public Overridable Overloads WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				Me.params_Conflict = paramTable
			End Set
		End Property

		Public Overrides Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		Public Overrides Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Return params_Conflict
		End Function

		''' <summary>
		''' Get the parameter, after applying any weight noise (such as DropConnect) if necessary.
		''' Note that during training, this will store the post-noise parameters, as these should be used
		''' for both forward pass and backprop, for a single iteration.
		''' Consequently, the parameters (post noise) should be cleared after each training iteration
		''' </summary>
		''' <param name="param">    Parameter key </param>
		''' <param name="training"> If true: during training </param>
		''' <returns> The parameter, after applying any noise </returns>
		Protected Friend Overridable Function getParamWithNoise(ByVal param As String, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim p As INDArray
			If layerConf().getWeightNoise() IsNot Nothing Then
				If training AndAlso weightNoiseParams.Count > 0 AndAlso weightNoiseParams.ContainsKey(param) Then
					'Re-use these weights for both forward pass and backprop - don't want to use 2 different params here
					'These should be cleared during  backprop
					Return weightNoiseParams(param)
				Else
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						p = layerConf().getWeightNoise().getParameter(Me, param, IterationCount, EpochCount, training, workspaceMgr)
					End Using
				End If

				If training Then
					'Store for re-use in backprop
					weightNoiseParams(param) = p
				End If
			Else
				Return getParam(param)
			End If

			Return p
		End Function

		Protected Friend Overridable Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return preOutputWithPreNorm(training, False, workspaceMgr).First
		End Function

		Protected Friend Overridable Function preOutputWithPreNorm(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			assertInputSet(forBackprop)
			applyDropOutIfNecessary(training, workspaceMgr)
			Dim W As INDArray = getParamWithNoise(DefaultParamInitializer.WEIGHT_KEY, training, workspaceMgr)
			Dim b As INDArray = getParamWithNoise(DefaultParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim g As INDArray = (If(hasLayerNorm(), getParam(DefaultParamInitializer.GAIN_KEY), Nothing))

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			'Input validation:
			If input.rank() <> 2 OrElse input.columns() <> W.rows() Then
				If input.rank() <> 2 Then
					Throw New DL4JInvalidInputException("Input that is not a matrix; expected matrix (rank 2), got rank " & input.rank() & " array with shape " & java.util.Arrays.toString(input.shape()) & ". Missing preprocessor or wrong input type? " & layerId())
				End If
				Throw New DL4JInvalidInputException("Input size (" & input.columns() & " columns; shape = " & java.util.Arrays.toString(input.shape()) & ") is invalid: does not match layer input size (layer # inputs = " & W.size(0) & ") " & layerId())
			End If


			Dim ret As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, W.dataType(), input.size(0), W.size(1))
			input.castTo(ret.dataType()).mmuli(W, ret) 'TODO Can we avoid this cast? (It sohuld be a no op if not required, however)

			Dim preNorm As INDArray = ret
			If hasLayerNorm() Then
				preNorm = (If(forBackprop, ret.dup(ret.ordering()), ret))
				Nd4j.Executioner.exec(New LayerNorm(preNorm, g, ret, True, 1))
			End If

			If hasBias() Then
				ret.addiRowVector(b)
			End If

			If maskArray_Conflict IsNot Nothing Then
				applyMask(ret)
			End If

			Return New Pair(Of INDArray, INDArray)(ret, preNorm)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim z As INDArray = preOutput(training, workspaceMgr)
			Dim ret As INDArray = layerConf().getActivationFn().getActivation(z, training)

			If maskArray_Conflict IsNot Nothing Then
				applyMask(ret)
			End If

			Return ret
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Dim scoreSum As Double = 0.0
			For Each e As KeyValuePair(Of String, INDArray) In paramTable().SetOfKeyValuePairs()
				Dim l As IList(Of Regularization) = layerConf().getRegularizationByParam(e.Key)
				If l Is Nothing OrElse l.Count = 0 Then
					Continue For
				End If
				For Each r As Regularization In l
					scoreSum += r.score(e.Value, IterationCount, EpochCount)
				Next r
			Next e
			Return scoreSum
		End Function

		Public Overrides Function clone() As Layer
			Dim layer As Layer = Nothing
			Try
				Dim c As System.Reflection.ConstructorInfo = Me.GetType().GetConstructor(GetType(NeuralNetConfiguration))
				layer = DirectCast(c.Invoke(conf_Conflict), Layer)
				Dim linkedTable As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
				For Each entry As KeyValuePair(Of String, INDArray) In params_Conflict.SetOfKeyValuePairs()
					linkedTable(entry.Key) = entry.Value.dup()
				Next entry
				layer.ParamTable = linkedTable
			Catch e As Exception
				log.error("",e)
			End Try

			Return layer

		End Function

		''' <summary>
		''' The number of parameters for the model
		''' </summary>
		''' <returns> the number of parameters for the model </returns>
		Public Overrides Function numParams() As Long
			Dim ret As Integer = 0
			For Each val As INDArray In params_Conflict.Values
				ret += val.length()
			Next val
			Return ret
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			If input IsNot Nothing Then
				setInput(input, workspaceMgr)
				applyDropOutIfNecessary(True, workspaceMgr)
			End If
			If solver Is Nothing Then
				solver = (New Solver.Builder()).model(Me).configure(conf()).listeners(getListeners()).build()
			End If
			Me.optimizer_Conflict = solver.Optimizer
			solver.optimize(workspaceMgr)
		End Sub

		Public Overrides Function ToString() As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return Me.GetType().FullName & "{" & "conf=" & conf_Conflict & ", score=" & score_Conflict & ", optimizer=" & optimizer_Conflict & ", listeners=" & trainingListeners + AscW("}"c)
		End Function

		Public Overrides Sub clear()
			MyBase.clear()
			weightNoiseParams.Clear()
		End Sub

		Public Overrides Sub clearNoiseWeightParams()
			weightNoiseParams.Clear()
		End Sub

		''' <summary>
		''' Does this layer have no bias term? Many layers (dense, convolutional, output, embedding) have biases by
		''' default, but no-bias versions are possible via configuration
		''' </summary>
		''' <returns> True if a bias term is present, false otherwise </returns>
		Public Overridable Function hasBias() As Boolean
			'Overridden by layers supporting no bias mode: dense, output, convolutional, embedding
			Return True
		End Function

		''' <summary>
		''' Does this layer support and is it enabled layer normalization? Only Dense and SimpleRNN Layers support
		''' layer normalization.
		''' </summary>
		''' <returns> True if layer normalization is enabled on this layer, false otherwise </returns>
		Public Overridable Function hasLayerNorm() As Boolean
			' Overridden by layers supporting layer normalization.
			Return False
		End Function
	End Class

End Namespace