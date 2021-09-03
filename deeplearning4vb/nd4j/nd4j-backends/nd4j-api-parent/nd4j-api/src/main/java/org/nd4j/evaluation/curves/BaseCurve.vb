Imports System
Imports JsonMappers = org.nd4j.serde.json.JsonMappers
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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

Namespace org.nd4j.evaluation.curves


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) public abstract class BaseCurve
	Public MustInherit Class BaseCurve
		Public Const DEFAULT_FORMAT_PREC As Integer = 4

		''' <returns> The number of points in the curve </returns>
		Public MustOverride Function numPoints() As Integer

		''' <returns> X axis values </returns>
		Public MustOverride ReadOnly Property X As Double()

		''' <returns> Y-axis values </returns>
		Public MustOverride ReadOnly Property Y As Double()

		''' <returns> Title for the curve </returns>
		Public MustOverride ReadOnly Property Title As String

		''' <returns> Area under the curve </returns>
		Protected Friend Overridable Function calculateArea() As Double
			Return calculateArea(X, Y)
		End Function

		Protected Friend Overridable Function calculateArea(ByVal x() As Double, ByVal y() As Double) As Double
			Dim nPoints As Integer = x.Length
			Dim area As Double = 0.0
			Dim i As Integer = 0
			Do While i < nPoints - 1
				Dim xLeft As Double = x(i)
				Dim yLeft As Double = y(i)
				Dim xRight As Double = x(i + 1)
				Dim yRight As Double = y(i + 1)

				'y axis: TPR
				'x axis: FPR
				Dim deltaX As Double = Math.Abs(xRight - xLeft) 'Iterating in threshold order, so FPR decreases as threshold increases
				Dim avg As Double = (yRight + yLeft) / 2.0

				area += deltaX * avg
				i += 1
			Loop

			Return area
		End Function

		Protected Friend Overridable Function format(ByVal d As Double, ByVal precision As Integer) As String
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Return String.Format("%." & precision & "f", d)
		End Function

		''' <returns>  JSON representation of the curve </returns>
		Public Overridable Function toJson() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' <returns> YAML  representation of the curve </returns>
		Public Overridable Function toYaml() As String
			Try
				Return JsonMappers.YamlMapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' 
		''' <param name="json">       JSON representation </param>
		''' <param name="curveClass"> Class for the curve </param>
		''' @param <T>        Type </param>
		''' <returns>           Instance of the curve </returns>
		Public Shared Function fromJson(Of T As BaseCurve)(ByVal json As String, ByVal curveClass As Type(Of T)) As T
			Try
				Return JsonMappers.Mapper.readValue(json, curveClass)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' 
		''' <param name="yaml">       YAML representation </param>
		''' <param name="curveClass"> Class for the curve </param>
		''' @param <T>        Type </param>
		''' <returns>           Instance of the curve </returns>
		Public Shared Function fromYaml(Of T As BaseCurve)(ByVal yaml As String, ByVal curveClass As Type(Of T)) As T
			Try
				Return JsonMappers.YamlMapper.readValue(yaml, curveClass)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

	End Class

End Namespace