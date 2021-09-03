Imports System
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.datavec.api.transform.metadata


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @JsonIgnoreProperties("allowVarLength") public class NDArrayMetaData extends BaseColumnMetaData
	<Serializable>
	Public Class NDArrayMetaData
		Inherits BaseColumnMetaData

		Private shape() As Long
		Private allowVarLength As Boolean


		''' <param name="name">  Name of the NDArray column </param>
		''' <param name="shape"> shape of the NDArray column. Use -1 in entries to specify as "variable length" in that dimension </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayMetaData(@JsonProperty("name") String name, @JsonProperty("shape") long[] shape)
		Public Sub New(ByVal name As String, ByVal shape() As Long)
			MyBase.New(name)
			Me.shape = shape
			For Each i As Long In shape
				If i < 0 Then
					allowVarLength = True
					Exit For
				End If
			Next i
		End Sub

		Public Overrides ReadOnly Property ColumnType As ColumnType
			Get
				Return ColumnType.NDArray
			End Get
		End Property

		Public Overrides Function isValid(ByVal writable As Writable) As Boolean
			If Not (TypeOf writable Is NDArrayWritable) Then
				Return False
			End If
			Dim arr As INDArray = DirectCast(writable, NDArrayWritable).get()
			If arr Is Nothing Then
				Return False
			End If
			If allowVarLength Then
				For i As Integer = 0 To shape.Length - 1
					If shape(i) < 0 Then
						Continue For
					End If
					If shape(i) <> arr.size(i) Then
						Return False
					End If
				Next i
				Return True
			Else
				Return shape.SequenceEqual(arr.shape())
			End If
		End Function

		Public Overrides Function isValid(ByVal input As Object) As Boolean
			If input Is Nothing Then
				Return False
			ElseIf TypeOf input Is Writable Then
				Return isValid(DirectCast(input, Writable))
			ElseIf TypeOf input Is INDArray Then
				Return isValid(New NDArrayWritable(DirectCast(input, INDArray)))
			Else
				Throw New System.NotSupportedException("Unknown object type: " & input.GetType())
			End If
		End Function

		Public Overrides Function clone() As NDArrayMetaData
			Return New NDArrayMetaData(name_Conflict, CType(shape.Clone(), Long()))
		End Function

	End Class

End Namespace