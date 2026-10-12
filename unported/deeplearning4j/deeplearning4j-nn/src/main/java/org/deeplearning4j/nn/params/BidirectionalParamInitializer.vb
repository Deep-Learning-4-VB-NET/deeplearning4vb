Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports BaseRecurrentLayer = org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.deeplearning4j.nn.params


	Public Class BidirectionalParamInitializer
		Implements ParamInitializer

		Public Const FORWARD_PREFIX As String = "f"
		Public Const BACKWARD_PREFIX As String = "b"

		Private ReadOnly layer As Bidirectional
'JAVA TO VB CONVERTER NOTE: The field underlying was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly underlying_Conflict As Layer

'JAVA TO VB CONVERTER NOTE: The field paramKeys was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private paramKeys_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field weightKeys was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private weightKeys_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field biasKeys was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private biasKeys_Conflict As IList(Of String)

		Public Sub New(ByVal layer As Bidirectional)
			Me.layer = layer
			Me.underlying_Conflict = underlying(layer)
		End Sub

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal layer As Layer) As Long Implements ParamInitializer.numParams
			Return 2 * underlying(layer).initializer().numParams(underlying(layer))
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.paramKeys
			If paramKeys_Conflict Is Nothing Then
				Dim u As Layer = underlying(layer)
				Dim orig As IList(Of String) = u.initializer().paramKeys(u)
				paramKeys_Conflict = withPrefixes(orig)
			End If
			Return paramKeys_Conflict
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.weightKeys
			If weightKeys_Conflict Is Nothing Then
				Dim u As Layer = underlying(layer)
				Dim orig As IList(Of String) = u.initializer().weightKeys(u)
				weightKeys_Conflict = withPrefixes(orig)
			End If
			Return weightKeys_Conflict
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.biasKeys
			If biasKeys_Conflict Is Nothing Then
				Dim u As Layer = underlying(layer)
				Dim orig As IList(Of String) = u.initializer().weightKeys(u)
				biasKeys_Conflict = withPrefixes(orig)
			End If
			Return biasKeys_Conflict
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return weightKeys(Me.layer).Contains(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return biasKeys(Me.layer).Contains(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray) Implements ParamInitializer.init
			Dim n As val = paramsView.length()\2
			Dim forwardView As INDArray = paramsView.get(interval(0,0,True), interval(0, n))
			Dim backwardView As INDArray = paramsView.get(interval(0,0,True), interval(n, 2*n))

			conf.clearVariables()

			Dim c1 As NeuralNetConfiguration = conf.clone()
			Dim c2 As NeuralNetConfiguration = conf.clone()
			c1.setLayer(underlying_Conflict)
			c2.setLayer(underlying_Conflict)
			Dim origFwd As IDictionary(Of String, INDArray) = underlying_Conflict.initializer().init(c1, forwardView, initializeParams)
			Dim origBwd As IDictionary(Of String, INDArray) = underlying_Conflict.initializer().init(c2, backwardView, initializeParams)
			Dim variables As IList(Of String) = addPrefixes(c1.getVariables(), c2.getVariables())
			conf.setVariables(variables)

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In origFwd.SetOfKeyValuePairs()
				[out](FORWARD_PREFIX & e.Key) = e.Value
			Next e
			For Each e As KeyValuePair(Of String, INDArray) In origBwd.SetOfKeyValuePairs()
				[out](BACKWARD_PREFIX & e.Key) = e.Value
			Next e

			Return [out]
		End Function

		Private Function addPrefixes(Of T)(ByVal fwd As IDictionary(Of String, T), ByVal bwd As IDictionary(Of String, T)) As IDictionary(Of String, T)
			Dim [out] As IDictionary(Of String, T) = New LinkedHashMap(Of String, T)()
			For Each e As KeyValuePair(Of String, T) In fwd.SetOfKeyValuePairs()
				[out](FORWARD_PREFIX & e.Key) = e.Value
			Next e
			For Each e As KeyValuePair(Of String, T) In bwd.SetOfKeyValuePairs()
				[out](BACKWARD_PREFIX & e.Key) = e.Value
			Next e

			Return [out]
		End Function

		Private Function addPrefixes(ByVal fwd As IList(Of String), ByVal bwd As IList(Of String)) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In fwd
				[out].Add(FORWARD_PREFIX & s)
			Next s
			For Each s As String In bwd
				[out].Add(BACKWARD_PREFIX & s)
			Next s
			Return [out]
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray) Implements ParamInitializer.getGradientsFromFlattened
			Dim n As val = gradientView.length()\2
			Dim forwardView As INDArray = gradientView.get(interval(0,0,True), interval(0, n))
			Dim backwardView As INDArray = gradientView.get(interval(0,0,True), interval(n, 2*n))

			Dim origFwd As IDictionary(Of String, INDArray) = underlying_Conflict.initializer().getGradientsFromFlattened(conf, forwardView)
			Dim origBwd As IDictionary(Of String, INDArray) = underlying_Conflict.initializer().getGradientsFromFlattened(conf, backwardView)

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In origFwd.SetOfKeyValuePairs()
				[out](FORWARD_PREFIX & e.Key) = e.Value
			Next e
			For Each e As KeyValuePair(Of String, INDArray) In origBwd.SetOfKeyValuePairs()
				[out](BACKWARD_PREFIX & e.Key) = e.Value
			Next e

			Return [out]
		End Function

		Private Function underlying(ByVal layer As Layer) As Layer
			Dim b As Bidirectional = DirectCast(layer, Bidirectional)
			Return b.getFwd()
		End Function

		Private Function withPrefixes(ByVal orig As IList(Of String)) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In orig
				[out].Add(FORWARD_PREFIX & s)
			Next s
			For Each s As String In orig
				[out].Add(BACKWARD_PREFIX & s)
			Next s
			Return [out]
		End Function
	End Class

End Namespace