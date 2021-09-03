Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports AbstractSameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.AbstractSameDiffLayer
Imports SameDiffVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffVertex
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SameDiffParamInitializer implements org.deeplearning4j.nn.api.ParamInitializer
	Public Class SameDiffParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New SameDiffParamInitializer()

		Public Shared ReadOnly Property Instance As SameDiffParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal layer As Layer) As Long Implements ParamInitializer.numParams
			Dim sd As AbstractSameDiffLayer = DirectCast(layer, AbstractSameDiffLayer)
			Dim m As IDictionary(Of String, Long()) = sd.LayerParams.getParamShapes()
			Dim n As Integer = 0
			For Each arr As val In m.Values
				n += ArrayUtil.prod(arr)
			Next arr
			Return n
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.paramKeys
			Dim sd As AbstractSameDiffLayer = DirectCast(layer, AbstractSameDiffLayer)
			Return sd.LayerParams.getParameterKeys()
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.weightKeys
			Dim sd As AbstractSameDiffLayer = DirectCast(layer, AbstractSameDiffLayer)
			Return sd.LayerParams.getWeightParameterKeys()
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.biasKeys
			Dim sd As AbstractSameDiffLayer = DirectCast(layer, AbstractSameDiffLayer)
			Return sd.LayerParams.getBiasParameterKeys()
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return weightKeys(layer).Contains(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return biasKeys(layer).Contains(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray) Implements ParamInitializer.init
			Dim sd As AbstractSameDiffLayer = CType(conf.getLayer(), AbstractSameDiffLayer)
			Dim [out] As IDictionary(Of String, INDArray) = subsetAndReshape(sd.LayerParams.getParameterKeys(), sd.LayerParams.getParamShapes(), paramsView, sd)
			If initializeParams Then
				sd.initializeParameters([out])
			End If

			For Each s As String In sd.LayerParams.getParameterKeys()
				conf.addVariable(s)
			Next s

			Return [out]
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray) Implements ParamInitializer.getGradientsFromFlattened
			Dim sd As AbstractSameDiffLayer = CType(conf.getLayer(), AbstractSameDiffLayer)
			Return subsetAndReshape(sd.LayerParams.getParameterKeys(), sd.LayerParams.getParamShapes(), gradientView, sd)
		End Function

		Private Function subsetAndReshape(ByVal params As IList(Of String), ByVal paramShapes As IDictionary(Of String, Long()), ByVal view As INDArray, ByVal sdl As AbstractSameDiffLayer) As IDictionary(Of String, INDArray)
			Return subsetAndReshape(params, paramShapes, view, sdl, Nothing)
		End Function

		Public Overridable Function subsetAndReshape(ByVal params As IList(Of String), ByVal paramShapes As IDictionary(Of String, Long()), ByVal view As INDArray, ByVal sdl As AbstractSameDiffLayer, ByVal sdv As SameDiffVertex) As IDictionary(Of String, INDArray)
			Dim clazz As Type = (If(sdl IsNot Nothing, sdl.GetType(), sdv.GetType()))
			Dim layerName As String = (If(sdl IsNot Nothing, sdl.LayerName, "")) 'TODO

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			Dim soFar As Integer = 0
			For Each s As String In params
				Dim sh As val = paramShapes(s)
				Dim length As val = ArrayUtil.prodLong(sh)
				If length <= 0 Then
					Throw New System.InvalidOperationException("Invalid array state for parameter """ & s & """ in layer " & layerName & " of type " & clazz.Name & ": parameter length (" & length & ") must be > 0 - parameter array shape: " & Arrays.toString(sh))
				End If
				Dim [sub] As INDArray = view.get(interval(0,0,True), interval(soFar, soFar + length))

				If Not [sub].shape().SequenceEqual(sh) Then
					Dim order As Char = (If(sdl IsNot Nothing, sdl.paramReshapeOrder(s), sdv.paramReshapeOrder(s)))
					[sub] = [sub].reshape(order, sh)
				End If
				[out](s) = [sub]

				soFar += length
			Next s
			Return [out]
		End Function
	End Class

End Namespace