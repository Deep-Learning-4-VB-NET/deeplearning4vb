Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
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


	Public Class UniqueMergeFunction
		Implements Function2(Of IDictionary(Of String, ISet(Of Writable)), IDictionary(Of String, ISet(Of Writable)), IDictionary(Of String, ISet(Of Writable)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Map<String, java.util.@Set<org.datavec.api.writable.Writable>> call(java.util.Map<String, java.util.@Set<org.datavec.api.writable.Writable>> v1, java.util.Map<String, java.util.@Set<org.datavec.api.writable.Writable>> v2) throws Exception
		Public Overrides Function [call](ByVal v1 As IDictionary(Of String, ISet(Of Writable)), ByVal v2 As IDictionary(Of String, ISet(Of Writable))) As IDictionary(Of String, ISet(Of Writable))
			If v1 Is Nothing Then
				Return v2
			End If
			If v2 Is Nothing Then
				Return v1
			End If

			For Each s As String In v1.Keys
				v1(s).addAll(v2(s))
			Next s
			Return v1
		End Function
	End Class

End Namespace