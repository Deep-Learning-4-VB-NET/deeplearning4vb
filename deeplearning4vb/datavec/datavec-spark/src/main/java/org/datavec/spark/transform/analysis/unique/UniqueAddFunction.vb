Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.spark.transform.analysis.unique


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class UniqueAddFunction implements org.apache.spark.api.java.function.Function2<Map<String,@Set<org.datavec.api.writable.Writable>>, List<org.datavec.api.writable.Writable>, Map<String,@Set<org.datavec.api.writable.Writable>>>
	Public Class UniqueAddFunction
		Implements Function2(Of IDictionary(Of String, ISet(Of Writable)), IList(Of Writable), IDictionary(Of String, ISet(Of Writable)))

		Private ReadOnly columns As IList(Of String)
		Private ReadOnly schema As Schema

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Map<String, @Set<org.datavec.api.writable.Writable>> call(Map<String, @Set<org.datavec.api.writable.Writable>> v1, List<org.datavec.api.writable.Writable> v2) throws Exception
		Public Overrides Function [call](ByVal v1 As IDictionary(Of String, ISet(Of Writable)), ByVal v2 As IList(Of Writable)) As IDictionary(Of String, ISet(Of Writable))
			If v2 Is Nothing Then
				Return v1
			End If

			If v1 Is Nothing Then
				v1 = New Dictionary(Of String, ISet(Of Writable))()
				For Each s As String In columns
					v1(s) = New HashSet(Of Writable)()
				Next s
			End If
			For Each s As String In columns
				Dim idx As Integer = schema.getIndexOfColumn(s)
				Dim value As Writable = v2(idx)
				v1(s).Add(value)
			Next s
			Return v1
		End Function
	End Class

End Namespace