Imports System
Imports System.Collections.Generic
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports TypeReference = org.nd4j.shade.jackson.core.type.TypeReference
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.datavec.api.records.reader.impl.jackson


	Public Class JacksonReaderUtils

		Private Shared ReadOnly typeRef As TypeReference(Of IDictionary(Of String, Object)) = New TypeReferenceAnonymousInnerClass()

		Private Class TypeReferenceAnonymousInnerClass
			Inherits TypeReference(Of IDictionary(Of String, Object))

		End Class

		Private Sub New()
		End Sub

		Public Shared Function parseRecord(ByVal line As String, ByVal selection As FieldSelection, ByVal mapper As ObjectMapper) As IList(Of Writable)
			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			Dim paths As IList(Of String()) = selection.getFieldPaths()
			Dim valueIfMissing As IList(Of Writable) = selection.getValueIfMissing()
			Dim map As IDictionary(Of String, Object)
			Try
				map = mapper.readValue(line, typeRef)
			Catch e As IOException
				Throw New Exception("Error parsing file", e)
			End Try

			'Now, extract out values...
			For i As Integer = 0 To paths.Count - 1
				Dim currPath() As String = paths(i)
				Dim value As String = Nothing
				Dim currMap As IDictionary(Of String, Object) = map
				Dim j As Integer = 0
				Do While j < currPath.Length
					If currMap.ContainsKey(currPath(j)) Then
						Dim o As Object = currMap(currPath(j))
						If j = currPath.Length - 1 Then
							'Expect to get the final value
							If TypeOf o Is String Then
								value = DirectCast(o, String)
							ElseIf TypeOf o Is Number Then
								value = o.ToString()
							Else
								Throw New System.InvalidOperationException("Expected to find String on path " & Arrays.toString(currPath) & ", found " & o.GetType() & " with value " & o)
							End If
						Else
							'Expect to get a map...
							If TypeOf o Is System.Collections.IDictionary Then
								currMap = DirectCast(o, IDictionary(Of String, Object))
							End If
						End If
					Else
						'Not found
						value = Nothing
						Exit Do
					End If
					j += 1
				Loop

				Dim outputWritable As Writable
				If value Is Nothing Then
					outputWritable = valueIfMissing(i)
				Else
					outputWritable = New Text(value)
				End If
				[out].Add(outputWritable)
			Next i

			Return [out]
		End Function

	End Class

End Namespace