Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports org.datavec.api.transform.ops
Imports AggregableColumnReduction = org.datavec.api.transform.reduce.AggregableColumnReduction
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.reduce.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class GeographicMidpointReduction implements org.datavec.api.transform.reduce.AggregableColumnReduction
	<Serializable>
	Public Class GeographicMidpointReduction
		Implements AggregableColumnReduction

		Public Const EDGE_CASE_EPS As Double = 1e-9

		Private delim As String
		Private newColumnName As String

		''' <param name="delim"> Delimiter for the coordinates in text format. For example, if format is "lat,long" use "," </param>
		Public Sub New(ByVal delim As String)
			Me.New(delim, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public GeographicMidpointReduction(@JsonProperty("delim") String delim, @JsonProperty("newColumnName") String newColumnName)
		Public Sub New(ByVal delim As String, ByVal newColumnName As String)
			Me.delim = delim
			Me.newColumnName = newColumnName
		End Sub

		Public Overridable Function reduceOp() As IAggregableReduceOp(Of Writable, IList(Of Writable)) Implements AggregableColumnReduction.reduceOp
			Return New AverageCoordinateReduceOp(delim)
		End Function

		Public Overridable Function getColumnsOutputName(ByVal columnInputName As String) As IList(Of String) Implements AggregableColumnReduction.getColumnsOutputName
			If newColumnName IsNot Nothing Then
				Return Collections.singletonList(newColumnName)
			End If
			Return Collections.singletonList("midpoint(" & columnInputName & ")")
		End Function

		Public Overridable Function getColumnOutputMetaData(ByVal newColumnName As IList(Of String), ByVal columnInputMeta As ColumnMetaData) As IList(Of ColumnMetaData) Implements AggregableColumnReduction.getColumnOutputMetaData
			Return Collections.singletonList(Of ColumnMetaData)(New StringMetaData(newColumnName(0)))
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			'No change
			Return inputSchema
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				'No op
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function outputColumnName() As String
			Return Nothing
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return New String(){}
		End Function

		Public Overridable Function columnNames() As String()
			Return New String(){}
		End Function

		Public Overridable Function columnName() As String
			Return Nothing
		End Function

		<Serializable>
		Public Class AverageCoordinateReduceOp
			Implements IAggregableReduceOp(Of Writable, IList(Of Writable))

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Friend Shared ReadOnly PI_180 As Double = Math.PI / 180.0

			Friend delim As String

			Friend sumx As Double
			Friend sumy As Double
			Friend sumz As Double
			Friend count As Integer

			Public Sub New(ByVal delim As String)
				Me.delim = delim
			End Sub

			Public Overridable Sub combine(Of W As IAggregableReduceOp(Of Writable, IList(Of Writable)))(ByVal accu As W) Implements IAggregableReduceOp(Of Writable, IList(Of Writable)).combine
				If TypeOf accu Is AverageCoordinateReduceOp Then
					Dim r As AverageCoordinateReduceOp = CType(accu, AverageCoordinateReduceOp)
					sumx += r.sumx
					sumy += r.sumy
					sumz += r.sumz
					count += r.count
				Else
					Throw New System.InvalidOperationException("Cannot combine type of class: " & accu.GetType())
				End If
			End Sub

			Public Overridable Sub accept(ByVal writable As Writable)
				Dim str As String = writable.ToString()
				Dim split() As String = str.Split(delim, True)
				If split.Length <> 2 Then
					Throw New System.InvalidOperationException("Could not parse lat/long string: """ & str & """")
				End If
				Dim latDeg As Double = Double.Parse(split(0))
				Dim longDeg As Double = Double.Parse(split(1))

				Preconditions.checkState(latDeg >= -90.0 AndAlso latDeg <= 90.0, "Invalid latitude: must be -90 to -90. Got: %s", latDeg)
				Preconditions.checkState(latDeg >= -180.0 AndAlso latDeg <= 180.0, "Invalid longitude: must be -180 to -180. Got: %s", longDeg)

				Dim lat As Double = latDeg * PI_180
				Dim lng As Double = longDeg * PI_180

				Dim x As Double = Math.Cos(lat) * Math.Cos(lng)
				Dim y As Double = Math.Cos(lat) * Math.Sin(lng)
				Dim z As Double = Math.Sin(lat)

				sumx += x
				sumy += y
				sumz += z
				count += 1
			End Sub

			Public Overridable Function get() As IList(Of Writable)
				Dim x As Double = sumx / count
				Dim y As Double = sumy / count
				Dim z As Double = sumz / count

				If count = 0 Then
					Throw New System.InvalidOperationException("Cannot calculate geographic midpoint: no datapoints were added to be reduced")
				End If

				If Math.Abs(x) < EDGE_CASE_EPS AndAlso Math.Abs(y) < EDGE_CASE_EPS AndAlso Math.Abs(z) < EDGE_CASE_EPS Then
					Throw New System.InvalidOperationException("No Geographic midpoint exists: midpoint is center of the earth")
				End If

				Dim longRad As Double = Math.Atan2(y,x)
				Dim hyp As Double = Math.Sqrt(x*x + y*y)
				Dim latRad As Double = Math.Atan2(z, hyp)

				Dim latDeg As Double = latRad / PI_180
				Dim longDeg As Double = longRad / PI_180

				Preconditions.checkState(Not Double.IsNaN(latDeg), "Final latitude is NaN")
				Preconditions.checkState(Not Double.IsNaN(longDeg), "Final longitude is NaN")

				Dim str As String = latDeg & delim & longDeg
				Return Collections.singletonList(Of Writable)(New Text(str))
			End Function
		End Class
	End Class

End Namespace