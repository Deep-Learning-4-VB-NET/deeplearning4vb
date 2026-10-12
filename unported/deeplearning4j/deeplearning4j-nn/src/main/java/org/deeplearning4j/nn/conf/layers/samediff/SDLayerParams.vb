Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.nn.conf.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"paramsList", "weightParamsList", "biasParamsList"}) @NoArgsConstructor @Data public class SDLayerParams implements java.io.Serializable
	<Serializable>
	Public Class SDLayerParams

		Private weightParams As IDictionary(Of String, Long()) = New LinkedHashMap(Of String, Long())()
		Private biasParams As IDictionary(Of String, Long()) = New LinkedHashMap(Of String, Long())()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private List<String> paramsList;
		Private paramsList As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private List<String> weightParamsList;
		Private weightParamsList As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private List<String> biasParamsList;
		Private biasParamsList As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDLayerParams(@JsonProperty("weightParams") Map<String, long[]> weightParams, @JsonProperty("biasParams") Map<String, long[]> biasParams)
		Public Sub New(ByVal weightParams As IDictionary(Of String, Long()), ByVal biasParams As IDictionary(Of String, Long()))
			Me.weightParams = weightParams
			Me.biasParams = biasParams
		End Sub

		''' <summary>
		''' Add a weight parameter to the layer, with the specified shape. For example, a standard fully connected layer
		''' could have weight parameters with shape [numInputs, layerSize]
		''' </summary>
		''' <param name="paramKey">   The parameter key (name) for the weight parameter </param>
		''' <param name="paramShape"> Shape of the weight parameter array </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addWeightParam(@NonNull String paramKey, @NonNull long... paramShape)
		Public Overridable Sub addWeightParam(ByVal paramKey As String, ParamArray ByVal paramShape() As Long)
			Preconditions.checkArgument(paramShape.Length > 0, "Provided weight parameter shape is" & " invalid: length 0 provided for shape. Parameter: " & paramKey)
			weightParams(paramKey) = paramShape
			paramsList = Nothing
			weightParamsList = Nothing
			biasParamsList = Nothing
		End Sub

		''' <summary>
		''' Add a bias parameter to the layer, with the specified shape. For example, a standard fully connected layer
		''' could have bias parameters with shape [1, layerSize]
		''' </summary>
		''' <param name="paramKey">   The parameter key (name) for the bias parameter </param>
		''' <param name="paramShape"> Shape of the bias parameter array </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addBiasParam(@NonNull String paramKey, @NonNull long... paramShape)
		Public Overridable Sub addBiasParam(ByVal paramKey As String, ParamArray ByVal paramShape() As Long)
			Preconditions.checkArgument(paramShape.Length > 0, "Provided mia- parameter shape is" & " invalid: length 0 provided for shape. Parameter: " & paramKey)
			biasParams(paramKey) = paramShape
			paramsList = Nothing
			weightParamsList = Nothing
			biasParamsList = Nothing
		End Sub

		''' <returns> Get a list of parameter names / keys (previously added via <seealso cref="addWeightParam(String, Long...)"/> and
		''' <seealso cref="addBiasParam(String, Long...)"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public List<String> getParameterKeys()
		Public Overridable ReadOnly Property ParameterKeys As IList(Of String)
			Get
				If paramsList Is Nothing Then
					Dim [out] As IList(Of String) = New List(Of String)()
					CType([out], List(Of String)).AddRange(getWeightParameterKeys())
					CType([out], List(Of String)).AddRange(getBiasParameterKeys())
					Me.paramsList = Collections.unmodifiableList([out])
				End If
				Return paramsList
			End Get
		End Property

		''' <returns> Get a list of parameter names / keys for weight parameters only, previously added via
		''' <seealso cref="addWeightParam(String, Long...)"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public List<String> getWeightParameterKeys()
		Public Overridable ReadOnly Property WeightParameterKeys As IList(Of String)
			Get
				If weightParamsList Is Nothing Then
					weightParamsList = Collections.unmodifiableList(New List(Of )(weightParams.Keys))
				End If
				Return weightParamsList
			End Get
		End Property

		''' <returns> Get a list of parameter names / keys for weight parameters only, previously added via
		''' <seealso cref="addWeightParam(String, Long...)"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public List<String> getBiasParameterKeys()
		Public Overridable ReadOnly Property BiasParameterKeys As IList(Of String)
			Get
				If biasParamsList Is Nothing Then
					biasParamsList = Collections.unmodifiableList(New List(Of )(biasParams.Keys))
				End If
				Return biasParamsList
			End Get
		End Property

		''' <summary>
		''' Get the parameter shapes for all parameters
		''' </summary>
		''' <returns> Map of parameter shapes, by parameter </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public Map<String, long[]> getParamShapes()
		Public Overridable ReadOnly Property ParamShapes As IDictionary(Of String, Long())
			Get
				Dim map As IDictionary(Of String, Long()) = New LinkedHashMap(Of String, Long())()
				map.PutAll(weightParams)
				map.PutAll(biasParams)
				Return map
			End Get
		End Property

		''' <summary>
		''' Clear any previously set weight/bias parameters (including their shapes)
		''' </summary>
		Public Overridable Sub clear()
			weightParams.Clear()
			biasParams.Clear()
			paramsList = Nothing
			weightParamsList = Nothing
			biasParamsList = Nothing
		End Sub

		Public Overridable Function isWeightParam(ByVal param As String) As Boolean
			Return weightParams.ContainsKey(param)
		End Function

		Public Overridable Function isBiasParam(ByVal param As String) As Boolean
			Return biasParams.ContainsKey(param)
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not (TypeOf o Is SDLayerParams) Then
				Return False
			End If
			Dim s As SDLayerParams = DirectCast(o, SDLayerParams)
			Return equals(weightParams, s.weightParams) AndAlso equals(biasParams, s.biasParams)
		End Function

		Private Shared Function equals(ByVal first As IDictionary(Of String, Long()), ByVal second As IDictionary(Of String, Long())) As Boolean
			'Helper method - Lombok equals method seems to have trouble with arrays...
			If Not first.Keys.Equals(second.Keys) Then
				Return False
			End If
			For Each e As KeyValuePair(Of String, Long()) In first.SetOfKeyValuePairs()
				If Not e.Value.SequenceEqual(second(e.Key)) Then
					Return False
				End If
			Next e
			Return True
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return weightParams.GetHashCode() Xor biasParams.GetHashCode()
		End Function
	End Class

End Namespace