Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports SimpleRnn = org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
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


	Public Class SimpleRnnParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New SimpleRnnParamInitializer()

		Public Shared ReadOnly Property Instance As SimpleRnnParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY
		Public Const RECURRENT_WEIGHT_KEY As String = "RW"
		Public Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY
		Public Const GAIN_KEY As String = DefaultParamInitializer.GAIN_KEY

		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(WEIGHT_KEY, RECURRENT_WEIGHT_KEY))
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.singletonList(BIAS_KEY)


		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal layer As Layer) As Long Implements ParamInitializer.numParams
			Dim c As SimpleRnn = DirectCast(layer, SimpleRnn)
			Dim nIn As val = c.getNIn()
			Dim nOut As val = c.getNOut()
			Return nIn * nOut + nOut * nOut + nOut + (If(hasLayerNorm(layer), 2 * nOut, 0))
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ArrayList<String> keys = new ArrayList<>(3);
			Dim keys As New List(Of String)(3)
			keys.AddRange(weightKeys(layer))
			keys.AddRange(biasKeys(layer))
			Return keys
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ArrayList<String> keys = new ArrayList<>(WEIGHT_KEYS);
			Dim keys As New List(Of String)(WEIGHT_KEYS)

			If hasLayerNorm(layer) Then
				keys.Add(GAIN_KEY)
			End If

			Return keys
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Return BIAS_KEYS
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return WEIGHT_KEY.Equals(key) OrElse RECURRENT_WEIGHT_KEY.Equals(key) OrElse GAIN_KEY.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return BIAS_KEY.Equals(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim c As SimpleRnn = CType(conf.getLayer(), SimpleRnn)
			Dim nIn As val = c.getNIn()
			Dim nOut As val = c.getNOut()

			Dim m As IDictionary(Of String, INDArray)

			If initializeParams Then
				m = getSubsets(paramsView, nIn, nOut, False, hasLayerNorm(c))
				Dim w As INDArray = c.getWeightInitFn().init(nIn, nOut, New Long(){nIn, nOut}, "f"c, m(WEIGHT_KEY))
				m(WEIGHT_KEY) = w

				Dim rwInit As IWeightInit
				If c.getWeightInitFnRecurrent() IsNot Nothing Then
					rwInit = c.getWeightInitFnRecurrent()
				Else
					rwInit = c.getWeightInitFn()
				End If

				Dim rw As INDArray = rwInit.init(nOut, nOut, New Long(){nOut, nOut}, "f"c, m(RECURRENT_WEIGHT_KEY))
				m(RECURRENT_WEIGHT_KEY) = rw

				m(BIAS_KEY).assign(c.getBiasInit())

				If hasLayerNorm(c) Then
					m(GAIN_KEY).assign(c.getGainInit())
				End If
			Else
				m = getSubsets(paramsView, nIn, nOut, True, hasLayerNorm(c))
			End If

			conf.addVariable(WEIGHT_KEY)
			conf.addVariable(RECURRENT_WEIGHT_KEY)
			conf.addVariable(BIAS_KEY)
			If hasLayerNorm(c) Then
				conf.addVariable(GAIN_KEY)
			End If

			Return m
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim c As SimpleRnn = CType(conf.getLayer(), SimpleRnn)
			Dim nIn As val = c.getNIn()
			Dim nOut As val = c.getNOut()

			Return getSubsets(gradientView, nIn, nOut, True, hasLayerNorm(c))
		End Function

		Private Shared Function getSubsets(ByVal [in] As INDArray, ByVal nIn As Long, ByVal nOut As Long, ByVal reshape As Boolean, ByVal hasLayerNorm As Boolean) As IDictionary(Of String, INDArray)
			Dim pos As Long = nIn * nOut
			Dim w As INDArray = [in].get(interval(0,0,True), interval(0, pos))
			Dim rw As INDArray = [in].get(interval(0,0,True), interval(pos, pos + nOut * nOut))
			pos += nOut * nOut
			Dim b As INDArray = [in].get(interval(0,0,True), interval(pos, pos + nOut))

			If reshape Then
				w = w.reshape("f"c, nIn, nOut)
				rw = rw.reshape("f"c, nOut, nOut)
			End If

			Dim m As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			m(WEIGHT_KEY) = w
			m(RECURRENT_WEIGHT_KEY) = rw
			m(BIAS_KEY) = b
			If hasLayerNorm Then
				pos += nOut
				Dim g As INDArray = [in].get(interval(0,0,True), interval(pos, pos + 2 * nOut))
				m(GAIN_KEY) = g
			End If
			Return m
		End Function

		Protected Friend Overridable Function hasLayerNorm(ByVal layer As Layer) As Boolean
			If TypeOf layer Is SimpleRnn Then
				Return DirectCast(layer, SimpleRnn).hasLayerNorm()
			End If
			Return False
		End Function
	End Class

End Namespace