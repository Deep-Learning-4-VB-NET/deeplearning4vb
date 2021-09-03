Imports System
Imports System.Collections.Generic
Imports lombok
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.conf.constraint



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @EqualsAndHashCode @Data public abstract class BaseConstraint implements org.deeplearning4j.nn.api.layers.LayerConstraint
	<Serializable>
	Public MustInherit Class BaseConstraint
		Implements LayerConstraint

		Public MustOverride Property Params As ISet(Of String) Implements LayerConstraint.getParams
		Public Const DEFAULT_EPSILON As Double = 1e-6
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected java.util.@Set<String> params = new java.util.HashSet<>();
		Protected Friend params As ISet(Of String) = New HashSet(Of String)()
		Protected Friend epsilon As Double = 1e-6
		Protected Friend dimensions() As Integer

		Protected Friend Sub New()
			'No arg for json ser/de
		End Sub

		Protected Friend Sub New(ByVal paramNames As ISet(Of String), ParamArray ByVal dimensions() As Integer)
			Me.New(paramNames, DEFAULT_EPSILON, dimensions)
		End Sub

		Public Overridable Sub applyConstraint(ByVal layer As Layer, ByVal iteration As Integer, ByVal epoch As Integer) Implements LayerConstraint.applyConstraint
			Dim paramTable As IDictionary(Of String, INDArray) = layer.paramTable()
			If paramTable Is Nothing OrElse paramTable.Count = 0 Then
				Return
			End If

			Dim i As ParamInitializer = layer.conf().getLayer().initializer()
			For Each e As KeyValuePair(Of String, INDArray) In paramTable.SetOfKeyValuePairs()
				If params.Contains(e.Key) Then
					apply(e.Value)
				End If
				If params IsNot Nothing AndAlso params.Contains(e.Key) Then
					apply(e.Value)
				End If
			Next e
		End Sub

		Public MustOverride Sub apply(ByVal param As INDArray)

		Public MustOverride Function clone() As BaseConstraint

		Public Shared Function getBroadcastDims(ByVal reduceDimensions() As Integer, ByVal rank As Integer) As Integer()
			Dim [out]((rank-reduceDimensions.Length) - 1) As Integer
			Dim outPos As Integer = 0
			For i As Integer = 0 To rank - 1
				If Not ArrayUtils.contains(reduceDimensions, i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outPos++] = i;
					[out](outPos) = i
						outPos += 1
				End If
			Next i
			Return [out]
		End Function
	End Class

End Namespace