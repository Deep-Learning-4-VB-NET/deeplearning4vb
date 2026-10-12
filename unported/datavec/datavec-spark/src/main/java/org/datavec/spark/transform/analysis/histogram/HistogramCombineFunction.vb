Imports System.Collections.Generic
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports HistogramCounter = org.datavec.api.transform.analysis.histogram.HistogramCounter

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

Namespace org.datavec.spark.transform.analysis.histogram


	Public Class HistogramCombineFunction
		Implements Function2(Of IList(Of HistogramCounter), IList(Of HistogramCounter), IList(Of HistogramCounter))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.transform.analysis.histogram.HistogramCounter> call(java.util.List<org.datavec.api.transform.analysis.histogram.HistogramCounter> l1, java.util.List<org.datavec.api.transform.analysis.histogram.HistogramCounter> l2) throws Exception
		Public Overrides Function [call](ByVal l1 As IList(Of HistogramCounter), ByVal l2 As IList(Of HistogramCounter)) As IList(Of HistogramCounter)
			If l1 Is Nothing Then
				Return l2
			End If
			If l2 Is Nothing Then
				Return l1
			End If

			Dim size As Integer = l1.Count
			If size <> l2.Count Then
				Throw New System.InvalidOperationException("List lengths differ")
			End If

			Dim [out] As IList(Of HistogramCounter) = New List(Of HistogramCounter)()
			For i As Integer = 0 To size - 1
				Dim c1 As HistogramCounter = l1(i)
				Dim c2 As HistogramCounter = l2(i)

				'Normally shouldn't get null values here - but maybe for Bytes column, etc.
				If c1 Is Nothing Then
					[out].Add(c2)
				ElseIf c2 Is Nothing Then
					[out].Add(c1)
				Else
					[out].Add(c1.merge(c2))
				End If
			Next i
			Return [out]
		End Function
	End Class

End Namespace